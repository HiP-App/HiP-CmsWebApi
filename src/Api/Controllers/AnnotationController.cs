using Api.Data;
using Microsoft.Extensions.Logging;
using Api.Managers;
using Api.Permission;

namespace Api.Controllers
{
    public partial class AnnotationController : ApiController
    {
        private readonly AnnotationTagManager _tagManager;
        private readonly AnnotationPermissions _annotationPermissions;

        public AnnotationController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            _tagManager = new AnnotationTagManager(dbContext);
            _annotationPermissions = new AnnotationPermissions(dbContext);
        }
    }
}
