using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QtecAccountManagement.Services;
using static QtecAccountManagement.Models.AccountModels;

namespace QtecAccountManagement.Pages.Account
{
    public class IndexModel : PageModel
    {
        private readonly IAccountService _accountService;

        public List<ChartOfAccountModel> Accounts { get; set; } = new List<ChartOfAccountModel>();

        public IndexModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task OnGetAsync()
        {
            Accounts = await _accountService.GetAccounts();
        }
    }
}
