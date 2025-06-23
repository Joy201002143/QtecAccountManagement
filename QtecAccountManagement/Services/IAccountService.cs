using System.Data;

namespace QtecAccountManagement.Services
{
    public interface IAccountService
    {
        Task<List<ChartOfAccountModel>> GetAccounts();
        Task<ChartOfAccountModel> GetAccountById(int id);
        Task<int> CreateAccount(ChartOfAccountModel account);
        Task UpdateAccount(ChartOfAccountModel account);
        Task DeleteAccount(int id);
        Task<List<ChartOfAccountModel>> GetAccountTree();
    }

    // AccountService.cs
    public class AccountService : IAccountService
    {
        private readonly IDbConnection _db;

        public AccountService(IDbConnection db)
        {
            _db = db;
        }

        public async Task<List<ChartOfAccountModel>> GetAccounts()
        {
            using var connection = _db;
            var result = await connection.QueryAsync<ChartOfAccountModel>("sp_ManageChartOfAccounts",
                new { Action = "SELECT" },
                commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<ChartOfAccountModel> GetAccountById(int id)
        {
            using var connection = _db;
            var result = await connection.QueryFirstOrDefaultAsync<ChartOfAccountModel>("sp_ManageChartOfAccounts",
                new { Action = "SELECTBYID", AccountId = id },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<int> CreateAccount(ChartOfAccountModel account)
        {
            using var connection = _db;
            var result = await connection.QueryFirstOrDefaultAsync<int>("sp_ManageChartOfAccounts",
                new
                {
                    Action = "INSERT",
                    AccountCode = account.AccountCode,
                    AccountName = account.AccountName,
                    ParentAccountId = account.ParentAccountId,
                    AccountType = account.AccountType,
                    IsActive = account.IsActive
                },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task UpdateAccount(ChartOfAccountModel account)
        {
            using var connection = _db;
            await connection.ExecuteAsync("sp_ManageChartOfAccounts",
                new
                {
                    Action = "UPDATE",
                    AccountId = account.AccountId,
                    AccountCode = account.AccountCode,
                    AccountName = account.AccountName,
                    ParentAccountId = account.ParentAccountId,
                    AccountType = account.AccountType,
                    IsActive = account.IsActive
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAccount(int id)
        {
            using var connection = _db;
            await connection.ExecuteAsync("sp_ManageChartOfAccounts",
                new { Action = "DELETE", AccountId = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<ChartOfAccountModel>> GetAccountTree()
        {
            var accounts = await GetAccounts();
            var accountTree = new List<ChartOfAccountModel>();
            var accountLookup = accounts.ToDictionary(a => a.AccountId);

            foreach (var account in accounts)
            {
                if (account.ParentAccountId.HasValue && accountLookup.ContainsKey(account.ParentAccountId.Value))
                {
                    accountLookup[account.ParentAccountId.Value].Children.Add(account);
                }
                else if (!account.ParentAccountId.HasValue)
                {
                    accountTree.Add(account);
                }
            }

            return accountTree;
        }
    }
}
