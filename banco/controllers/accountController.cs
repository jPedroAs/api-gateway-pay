using System.Text.RegularExpressions;
using banco.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using banco.TokenServices;

[ApiController]
// [Route("api/[controller]")]
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

    [HttpPost("api/register")]
    public async Task<IActionResult> Register([FromBody] User model)
    {
       var transaction = _context.Database.BeginTransaction();
        try{
            var verify = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (verify != null)
                return BadRequest("Usuário já registrado");  
                 
            var pass = _hash.HashPassword(model.Password);
            var user = new User{
                Username = model.Username,
                Email = model.Email,
                Password = pass,
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var generateGuid = Guid.NewGuid().ToString();
            var numberBank  = Regex.Replace(generateGuid, @"\D", "").Substring(0, 6);
            var account = new Account
            {
                AccountNumber = numberBank,
                Balance = 0,
                SavingsBalance = 0,
                UserId = user.Id,
                Active = true
            };
            await _context.AccountBaks.AddAsync(account);
            await _context.SaveChangesAsync();

            transaction.Commit();
            return Ok("User registered");
        }
        catch{
            return BadRequest("Não foi possível registra, Tente novamente mais tarde");
        }
    }

    [HttpPost("api/login")]
    public async Task<IActionResult> Login([FromBody] User model)
    {
         try{
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

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