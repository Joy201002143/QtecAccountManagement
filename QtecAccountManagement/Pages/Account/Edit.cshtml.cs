using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QtecAccountManagement.Services;
using static QtecAccountManagement.Models.AccountModels;

namespace QtecAccountManagement.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly IAccountService _accountService;

        [BindProperty]
        public ChartOfAccountModel Account { get; set; }

        public SelectList ParentAccounts { get; set; }

        public bool IsNew => Account.AccountId == 0;

        public EditModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                Account = new ChartOfAccountModel();
                return Page();
            }

            Account = await _accountService.GetAccountById(id.Value);
            if (Account == null)
            {
                return NotFound();
            }

            await LoadParentAccounts();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadParentAccounts();
                return Page();
            }

            if (IsNew)
            {
                await _accountService.CreateAccount(Account);
            }
            else
            {
                await _accountService.UpdateAccount(Account);
            }

            return RedirectToPage("./Index");
        }

        private async Task LoadParentAccounts()
        {
            var accounts = await _accountService.GetAccounts();
            ParentAccounts = new SelectList(accounts, "AccountId", "AccountName");
        }
    }
}
