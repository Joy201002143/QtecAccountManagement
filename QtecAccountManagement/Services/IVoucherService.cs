using System.Data;

namespace QtecAccountManagement.Services
{
    public interface IVoucherService
    {
        Task<int> SaveVoucher(VoucherModel voucher, int userId);
        Task<List<VoucherModel>> GetVouchers();
        Task<VoucherModel> GetVoucherById(int id);
        Task<MemoryStream> ExportVouchersToExcel();
    }

    // VoucherService.cs
    public class VoucherService : IVoucherService
    {
        private readonly IDbConnection _db;

        public VoucherService(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> SaveVoucher(VoucherModel voucher, int userId)
        {
            using var connection = _db;
            var parameters = new DynamicParameters();
            parameters.Add("@VoucherType", voucher.VoucherType);
            parameters.Add("@VoucherNumber", voucher.VoucherNumber);
            parameters.Add("@VoucherDate", voucher.VoucherDate);
            parameters.Add("@Reference", voucher.Reference);
            parameters.Add("@Notes", voucher.Notes);
            parameters.Add("@CreatedBy", userId);

            // Create DataTable for voucher details
            var detailsTable = new DataTable();
            detailsTable.Columns.Add("AccountId", typeof(int));
            detailsTable.Columns.Add("Amount", typeof(decimal));
            detailsTable.Columns.Add("IsDebit", typeof(bool));
            detailsTable.Columns.Add("Description", typeof(string));

            foreach (var detail in voucher.Details)
            {
                detailsTable.Rows.Add(detail.AccountId, detail.Amount, detail.IsDebit, detail.Description);
            }

            parameters.Add("@Details", detailsTable.AsTableValuedParameter("dbo.VoucherDetailType"));

            var result = await connection.QueryFirstOrDefaultAsync<int>("sp_SaveVoucher",
                parameters,
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<List<VoucherModel>> GetVouchers()
        {
            using var connection = _db;
            var vouchers = await connection.QueryAsync<VoucherModel>(
                "SELECT * FROM Vouchers ORDER BY VoucherDate DESC");

            var voucherList = vouchers.ToList();

            foreach (var voucher in voucherList)
            {
                var details = await connection.QueryAsync<VoucherDetailModel>(
                    @"SELECT vd.*, coa.AccountName 
                  FROM VoucherDetails vd 
                  JOIN ChartOfAccounts coa ON vd.AccountId = coa.AccountId
                  WHERE vd.VoucherId = @VoucherId",
                    new { VoucherId = voucher.VoucherId });

                voucher.Details = details.ToList();
            }

            return voucherList;
        }

        public async Task<VoucherModel> GetVoucherById(int id)
        {
            using var connection = _db;
            var voucher = await connection.QueryFirstOrDefaultAsync<VoucherModel>(
                "SELECT * FROM Vouchers WHERE VoucherId = @VoucherId",
                new { VoucherId = id });

            if (voucher != null)
            {
                var details = await connection.QueryAsync<VoucherDetailModel>(
                    @"SELECT vd.*, coa.AccountName 
                  FROM VoucherDetails vd 
                  JOIN ChartOfAccounts coa ON vd.AccountId = coa.AccountId
                  WHERE vd.VoucherId = @VoucherId",
                    new { VoucherId = id });

                voucher.Details = details.ToList();
            }

            return voucher;
        }

        public async Task<MemoryStream> ExportVouchersToExcel()
        {
            var vouchers = await GetVouchers();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Vouchers");

            // Add headers
            worksheet.Cells[1, 1].Value = "Voucher Number";
            worksheet.Cells[1, 2].Value = "Date";
            worksheet.Cells[1, 3].Value = "Type";
            worksheet.Cells[1, 4].Value = "Reference";
            worksheet.Cells[1, 5].Value = "Notes";

            // Add data
            int row = 2;
            foreach (var voucher in vouchers)
            {
                worksheet.Cells[row, 1].Value = voucher.VoucherNumber;
                worksheet.Cells[row, 2].Value = voucher.VoucherDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 3].Value = voucher.VoucherType;
                worksheet.Cells[row, 4].Value = voucher.Reference;
                worksheet.Cells[row, 5].Value = voucher.Notes;

                // Add details
                foreach (var detail in voucher.Details)
                {
                    row++;
                    worksheet.Cells[row, 2].Value = $"{detail.AccountName} - {(detail.IsDebit ? "Debit" : "Credit")}";
                    worksheet.Cells[row, 3].Value = detail.Amount;
                    worksheet.Cells[row, 4].Value = detail.Description;
                }

                row++;
            }

            // Auto fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return stream;
        }
    }
}
