using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class ActiveRequirement : IAuthorizationRequirement { }

public class ActiveHandler : AuthorizationHandler<ActiveRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveRequirement requirement)
    {
        var activeClaim = context.User.Claims.FirstOrDefault(c => c.Type == "active")?.Value;

        if (activeClaim == "true")
        {
            context.Succeed(requirement); 
        }

        return Task.CompletedTask;
    }
}