using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using Microsoft.Extensions.Logging;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using PaderbornUniversity.SILab.Hip.CmsApi.Permission;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Controllers
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
