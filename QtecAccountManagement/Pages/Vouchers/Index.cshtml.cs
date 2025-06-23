using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QtecAccountManagement.Services;
using static QtecAccountManagement.Models.VoucherModels;

namespace QtecAccountManagement.Pages.Vouchers
{
    public class IndexModel : PageModel
    {
        private readonly IVoucherService _voucherService;

        public List<VoucherModel> Vouchers { get; set; } = new List<VoucherModel>();

        public IndexModel(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        public async Task OnGetAsync()
        {
            Vouchers = await _voucherService.GetVouchers();
        }

        public async Task<IActionResult> OnGetExportAsync()
        {
            var stream = await _voucherService.ExportVouchersToExcel();
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Vouchers.xlsx");
        }
    }
}
