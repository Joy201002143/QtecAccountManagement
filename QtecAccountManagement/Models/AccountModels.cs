using System.ComponentModel.DataAnnotations;

namespace QtecAccountManagement.Models
{
    public class AccountModels
    {
        public class ChartOfAccountModel
        {
            public int AccountId { get; set; }

            [Required]
            [Display(Name = "Account Code")]
            public string AccountCode { get; set; }

            [Required]
            [Display(Name = "Account Name")]
            public string AccountName { get; set; }

            [Display(Name = "Parent Account")]
            public int? ParentAccountId { get; set; }

            [Display(Name = "Parent Account")]
            public string ParentAccountName { get; set; }

            [Required]
            [Display(Name = "Account Type")]
            public string AccountType { get; set; }

            [Display(Name = "Active")]
            public bool IsActive { get; set; } = true;

            public List<ChartOfAccountModel> Children { get; set; } = new List<ChartOfAccountModel>();
        }
    }
}
