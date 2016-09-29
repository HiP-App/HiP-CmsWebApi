using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters
{
    public class BaseActionFilter : ActionFilterAttribute
    {
        protected bool IsModelStateValid(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
                return true;

            // Set 400 header response because paramerters are invalid
            context.Result = new BadRequestObjectResult(context.ModelState);

            return false;
        }
        

        protected string GetCurrentAction(ActionExecutingContext context)
        {
            if(context.HttpContext.Request.Method == "POST")
                return Operation.Create;

            if(context.HttpContext.Request.Method == "GET")
            {
                if(context.RouteData.Values["id"] == null)
                    return Operation.ReadList;
                else
                    return Operation.ReadDetail;
            }

            if(context.HttpContext.Request.Method == "PUT")
                return Operation.Update;

            else if(context.HttpContext.Request.Method == "DELETE")
                return Operation.Delete;

            else
                return Operation.Invalid;
        }
    }
}