using Api.Data;
using Api.Utility;
using BLL.Authorities;
using BLL.Managers;
using BOL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Api.Filters
{
    public class TopicActionFilter : BaseActionFilter
    {
        private readonly UserManager userManager;
        private readonly TopicManager topicManager;
        
        public TopicActionFilter(ApplicationDbContext dbContext)
        {
            userManager  = new UserManager(dbContext);
            topicManager = new TopicManager(dbContext);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(IsModelStateValid(context))
            {
                // Current Controller Action
                var action = GetCurrentAction(context);

                // Current Topic
                var topic = GetTopicFromRouteData(context.RouteData);

                // Set 404 header if no topic is found against given Id in url
                if(topic == null && action != Operation.ReadList && action != Operation.Create)
                {
                    context.Result = new NotFoundResult();
                    return;
                }

                // Current User
                var user = userManager.GetUserByIdAsync(context.HttpContext.User.Identity.GetUserId()).Result;
                
                var topicAuthority = new TopicAuthority(user, topic);

                // Set 403 header if user is not allowed to perform given action
                if(!topicAuthority.IsUserAllowed(action))
                    context.Result = new ForbidResult();
            }
        }

        private Topic GetTopicFromRouteData(RouteData routeData)
        {
            if(routeData.Values.ContainsKey("id"))
            {
                int topicId = int.Parse(routeData.Values["id"].ToString());

                return topicManager.GetTopicByIdAsync(topicId).Result;
            }

            return null;
        }
    }
}