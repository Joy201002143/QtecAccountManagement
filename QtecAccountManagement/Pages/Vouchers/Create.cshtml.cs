using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QtecAccountManagement.Services;
using System.Security.Claims;
using static QtecAccountManagement.Models.VoucherModels;

namespace QtecAccountManagement.Pages.Vouchers
{
    public class CreateModel : PageModel
    {
        private readonly IVoucherService _voucherService;
        private readonly IAccountService _accountService;
        private readonly IAuthService _authService;

        [BindProperty]
        public VoucherModel Voucher { get; set; }

        public SelectList Accounts { get; set; }

        public CreateModel(IVoucherService voucherService, IAccountService accountService, IAuthService authService)
        {
            _voucherService = voucherService;
            _accountService = accountService;
            _authService = authService;
        }

        public async Task OnGetAsync()
        {
            Voucher = new VoucherModel
            {
                VoucherDate = DateTime.Today,
                Details = new List<VoucherDetailModel> { new VoucherDetailModel() }
            };

            await LoadAccounts();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAccounts();
                return Page();
            }

            // Remove empty details
            Voucher.Details = Voucher.Details.Where(d => d.AccountId > 0).ToList();

            // Check debit/credit balance
            var totalDebit = Voucher.Details.Where(d => d.IsDebit).Sum(d => d.Amount);
            var totalCredit = Voucher.Details.Where(d => !d.IsDebit).Sum(d => d.Amount);

            if (totalDebit != totalCredit)
            {
                ModelState.AddModelError(string.Empty, "Total debit and credit amounts must be equal.");
                await LoadAccounts();
                return Page();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _voucherService.SaveVoucher(Voucher, userId);

            return RedirectToPage("./Index");
        }

        private async Task LoadAccounts()
        {
            var accounts = await _accountService.GetAccounts();
            Accounts = new SelectList(accounts, "AccountId", "AccountName");
        }
    }
}
