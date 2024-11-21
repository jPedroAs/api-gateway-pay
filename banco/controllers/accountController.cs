using System.Text.RegularExpressions;
using banco.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using banco.TokenServices;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly BlueBankContext _context;
    private readonly TokenService _tokenService;
    private readonly PasswordHash _hash;

    public AccountController(TokenService tokenService, PasswordHash hash,BlueBankContext context)
    {
        _context = context;
        _tokenService = tokenService;
        _hash = hash;

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User model)
    {
       var transaction = _context.Database.BeginTransaction();
        try{
            var pass = _hash.HashPassword(model.Password);
            var user = new User{
                Username = model.Username,
                Password = pass
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var generateGuid = Guid.NewGuid().ToString();
            var numberBank  = Regex.Replace(generateGuid, @"\D", "").Substring(0, 6);
            var account = new Account
            {
                AccountNumber = numberBank,
                Balance = null,
                SavingsBalance = null,
                UserId = user.Id
            };
            _context.AccountBaks.Add(account);
            await _context.SaveChangesAsync();

            transaction.Commit();
            return Ok("User registered");
        }
        catch{
            return BadRequest("Não foi possível registra, Tente novamente mais tarde");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User model)
    {
         try{
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            if(user is null) return BadRequest("Usuário Não existe");
            
            var verif = _hash.VerifyPassword(model.Password, user.Password);
            if(verif == false) return BadRequest("Senha Inválida");

            var account =  await _context.AccountBaks.FirstOrDefaultAsync(x => x.UserId == user.Id);

            var token =  _tokenService.CreateToken(account, user.Username);

             return Ok(new { Token = token });
        }
        catch(Exception ex){
            return BadRequest(ex);
        }
    }
}