using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using banco.viewModels;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Components.Web;
using System.IdentityModel.Tokens.Jwt;
using Sprache;
using Microsoft.AspNetCore.Authorization;


[ApiController]
// [Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly BlueBankContext _context;

    public TransactionController(BlueBankContext context)
    {
        _context = context;
    }

    [Authorize(Policy = "ActiveUser")]
    [HttpPost("api/deposito")]
    public async Task<IActionResult> Deposit([FromBody] TransactionView model)
    {
        try{
            var transaction = new Transaction();
            var account = await _context.AccountBaks.FirstOrDefaultAsync(x => x.AccountNumber == model.Cod);
            if (account == null) 
                return BadRequest("account not found");

            var valor =  account.Balance + model.Amount;
            account.Balance = valor;

            transaction.Amount = model.Amount;
            transaction.Date = DateTime.UtcNow;
            transaction.Type = TransactionType.Deposit;
            transaction.AccountId = account.Id;
            await _context.Transactions.AddAsync(transaction);
            _context.AccountBaks.Update(account);

            await _context.SaveChangesAsync();
            return Ok(account.Balance);
        }catch(Exception e){
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "ActiveUser")]
    [HttpPost("api/transferencia")]
    public async Task<IActionResult> Transaction([FromBody] TransactionView model)
    {
        try{
            if (model.Cod == null)
                return BadRequest("É necessario informar o número da conta.");

            var transaction = new Transaction();
            var account = new Account();
               
            if(model.Type == TransactionType.CashOut)
            {
                
                account = await _context.AccountBaks.FirstOrDefaultAsync(x=> x.AccountNumber == model.Cod);
                if (account == null || account.Balance < model.Amount) 
                    return BadRequest("Saldo insuficiente");

                account.Balance -= model.Amount;
                transaction.Amount = model.Amount;
                transaction.Date = DateTime.UtcNow;
                transaction.Type = TransactionType.CashOut;
                transaction.AccountId = account.Id;
            }
            else if (model.Type == TransactionType.Transaction)
            {
                
                var token = Request.Headers["Authorization"].ToString();
                var replace = token.Replace("Bearer", "").Trim();
                var accountValue = GetToken(replace);
                var ac = int.Parse(accountValue);

                account = await _context.AccountBaks.FindAsync(ac);
                if(account.AccountNumber == model.Cod)
                    return BadRequest("Não é possível transferir para sua própria conta");
                if (account == null || account.Balance < model.Amount) 
                    return BadRequest("Saldo insuficiente");

                var account_transaction = await _context.AccountBaks.FirstOrDefaultAsync(x=> x.AccountNumber == model.Cod);
                if(account_transaction == null )
                    return NotFound("Conta não encontrada");

                account.Balance -= model.Amount;
                account_transaction.Balance += model.Amount;

                transaction.Amount = model.Amount;
                transaction.Date = DateTime.UtcNow;
                transaction.Type = TransactionType.Transaction;
                transaction.AccountId = account.Id;
                transaction.Account_transferred = account_transaction.Id;
            }
            else
            {
                return BadRequest("Apenas Type CashOut e Transaction ");
            }

            await _context.Transactions.AddAsync(transaction);
            _context.AccountBaks.Update(account);

            await _context.SaveChangesAsync();
            return Ok(new { Balance = account.Balance });
        }
        catch(Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "ActiveUser")]
    [HttpPost("api/transferToSavings")]
    public async Task<IActionResult> TransferToSavings([FromBody] TransactionView model)
    {
        try{
            var transaction = new Transaction();
            var account = await _context.AccountBaks.FirstOrDefaultAsync(x=> x.AccountNumber == model.Cod);
            if (account == null || account.Balance < model.Amount) 
                return BadRequest("Saldo insuficiente");

            account.Balance -= model.Amount;
            account.SavingsBalance += model.Amount;

            transaction.Amount = model.Amount;
            transaction.Date = DateTime.UtcNow;
            transaction.Type = TransactionType.Savings;
            transaction.AccountId = account.Id;
            await _context.Transactions.AddAsync(transaction);
            _context.AccountBaks.Update(account);

            await _context.SaveChangesAsync();
            return Ok(account.SavingsBalance);
        }catch(Exception e){
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "ActiveUser")]
    [HttpGet("api/balance")]
    public async Task<IActionResult> GetBalance()
    {
        var token = Request.Headers["Authorization"].ToString();
        var replace = token.Replace("Bearer", "").Trim();
        var accountValue = GetToken(replace);
        var account = int.Parse(accountValue);

        var balance = await _context.AccountBaks.FirstOrDefaultAsync(x => x.Id == account);
        return Ok(balance.Balance);
    }

    [Authorize(Policy = "ActiveUser")]
    [HttpGet("api/history")]
    public async Task<IActionResult> GetTransactionHistory()
    {
        var token = Request.Headers["Authorization"].ToString();
        var replace = token.Replace("Bearer", "").Trim();
        var accountValue = GetToken(replace);
        var account = int.Parse(accountValue);

        var transactions = await _context.Transactions.Where(x => x.AccountId == account).ToListAsync();
        return Ok(transactions);
    }


    public static string GetToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (handler.CanReadToken(token))
        {
            var jwtToken = handler.ReadJwtToken(token);
            
            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            
            return claim ?? "Conta não encontrado.";
        }
        
        return "Token inválido.";
    }
}
