using System.ComponentModel.DataAnnotations;

namespace QtecAccountManagement.Models
{
    public class VoucherModels
    {
        public class VoucherModel
        {
            public int VoucherId { get; set; }

            [Required]
            [Display(Name = "Voucher Type")]
            public string VoucherType { get; set; }

            [Required]
            [Display(Name = "Voucher Number")]
            public string VoucherNumber { get; set; }

            [Required]
            [Display(Name = "Voucher Date")]
            [DataType(DataType.Date)]
            public DateTime VoucherDate { get; set; } = DateTime.Today;

            [Display(Name = "Reference")]
            public string Reference { get; set; }

            [Display(Name = "Notes")]
            public string Notes { get; set; }

            public List<VoucherDetailModel> Details { get; set; } = new List<VoucherDetailModel>();
        }

        public class VoucherDetailModel
        {
            public int AccountId { get; set; }

            [Display(Name = "Account")]
            public string AccountName { get; set; }

            [Required]
            [Range(0.01, double.MaxValue)]
            public decimal Amount { get; set; }

            [Required]
            [Display(Name = "Debit/Credit")]
            public bool IsDebit { get; set; }

            [Display(Name = "Description")]
            public string Description { get; set; }
        }
    }
}
