using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.CmsApi
{
    /// <summary>
    /// This is a modified version of <see cref="SwaggerOperationFilter"/> which behaves as follows:
    /// - if authorization is required, a required access token field is inserted
    /// - if authorization is optional (i.e. [AllowAnonymous] attribute is present), an optional access token field
    ///   is inserted (THIS is the modification. In the original version, no access token field is inserted)
    /// - if authorization is not required at all, no access token field is inserted
    /// 
    /// This class should be removed once the modification is approved for the original class.
    /// </summary>
    public class CustomSwaggerOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
            var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

            if (!isAuthorized)
                return;

            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "Authorization",
                In = "header",
                Description = "access token",
                Required = !allowAnonymous,
                Type = "string"
            });

            if (!allowAnonymous && !operation.Responses.ContainsKey("401"))
                operation.Responses.Add("401", new Response { Description = "Unauthorized" });
        }

    }
}
