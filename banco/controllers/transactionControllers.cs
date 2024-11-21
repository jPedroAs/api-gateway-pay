using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using banco.viewModels;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Components.Web;
using System.IdentityModel.Tokens.Jwt;
using Sprache;


[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly BlueBankContext _context;

    public TransactionController(BlueBankContext context)
    {
        _context = context;
    }

    [HttpPost("deposito")]
    public async Task<IActionResult> Deposit([FromBody] TransactionView model)
    {
        var transaction = new Transaction();
        var account = await _context.AccountBaks.FindAsync(model.Cod);
        if (account == null) 
            return BadRequest("account not found");

        account.Balance += model.Amount;
        transaction.Amount = model.Amount;
        transaction.Date = DateTime.Now;
        transaction.Type = TransactionType.Deposit;
        transaction.AccountId = account.Id;
        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync();
        return Ok(account.Balance);
    }

    [HttpPost("transferencia")]
    public async Task<IActionResult> Transaction([FromBody] TransactionView model)
    {
        try{
            var transaction = new Transaction();

            if(model.Type == TransactionType.CashOut)
            {
                if (model.Cod == null)
                    return BadRequest("É necessario informar o número da conta.");
                
                var account = await _context.AccountBaks.FindAsync(model.Cod);
                if (account == null || account.Balance < model.Amount) 
                    return BadRequest("not enough money");

                account.Balance -= model.Amount;
                transaction.Amount = model.Amount;
                transaction.Date = DateTime.Now;
                transaction.Type = TransactionType.CashOut;
                transaction.AccountId = account.Id;
            }
            else if (model.Type == TransactionType.Savings)
            {
                
                var token = Request.Headers["Authorization"].ToString();
                var accountValue = GetToken(token);

                var account = await _context.AccountBaks.FindAsync(accountValue);
                if (account == null || account.Balance < model.Amount) 
                    return BadRequest("Insufficient funds");

                account.Balance -= model.Amount;
                transaction.Amount = model.Amount;
                transaction.Date = DateTime.Now;
                transaction.Type = TransactionType.Savings;
                transaction.AccountId = account.Id;
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return Ok("Transferência realizada");
        }
        catch
        {
            return BadRequest("Acorreu um erro");
        }
    }

    [HttpPost("transferToSavings")]
    public async Task<IActionResult> TransferToSavings([FromBody] TransactionView model)
    {
        var transaction = new Transaction();
        var account = await _context.AccountBaks.FindAsync(model.Cod);
        if (account == null || account.Balance < model.Amount) 
            return BadRequest("account not found");

        account.Balance -= model.Amount;
        account.SavingsBalance += model.Amount;

        transaction.Amount = model.Amount;
        transaction.Date = DateTime.Now;
        transaction.Type = TransactionType.Savings;
        transaction.AccountId = account.Id;
        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync();
        return Ok(account.SavingsBalance);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetTransactionHistory()
    {
        var token = Request.Headers["Authorization"].ToString();
        var accountValue = GetToken(token);
        var account = int.Parse(accountValue);

        var transactions = await _context.Transactions.Where(x => x.AccountId == account).ToListAsync();
        return Ok(transactions);
    }


    public string GetToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (handler.CanReadToken(token))
        {
            var jwtToken = handler.ReadJwtToken(token);
            
            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Account")?.Value;
            
            return claim ?? "Conta não encontrado.";
        }
        
        return "Token inválido.";
    }
}
