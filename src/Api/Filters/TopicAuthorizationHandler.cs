using BOL.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

public class TopicAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Topic>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Topic resource)
    {
        var user = context.User;

        if(user.IsInRole(Role.Supervisor))
        {
            if(requirement.Name.CompareTo(Operations.Create.Name) == 0 ||
                requirement.Name.CompareTo(Operations.Read.Name) == 0)
            {

            }

        }

        context.Succeed(requirement);

        return Task.CompletedTask;
    }
}