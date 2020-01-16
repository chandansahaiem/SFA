using SFA.Extensions;
using SFA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.Filters
{
    public class CheckAccessAttribute : TypeFilterAttribute
    {
        public CheckAccessAttribute() : base(typeof(ResourceAccessAsyncActionFilter))
        {
        }

        private class ResourceAccessAsyncActionFilter : IAsyncActionFilter
        {
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var currentUser = context.HttpContext.Session.Get<User>("SESSIONSFAUSER");
                if (currentUser == null)
                {
                    context.Result = new ForbidResult();
                    return;
                }
                //if (currentUser.BranchId == null || currentUser.FinYearId == null || currentUser.WorkingDate == null)
                //{
                //    context.Result = new ForbidResult();
                //    return;
                //}
                var requestPath = context.HttpContext.Request.Path;
                var targetAccess = currentUser.Permissions.FirstOrDefault(m => requestPath.Value.StartsWith(m.MenuTarget));
                if (targetAccess == null)
                {
                    context.Result = new ForbidResult();
                    return;
                }
                var controller = context.Controller as Controller;
                if (controller == null)
                {
                    return;
                }
                controller.ViewBag.Access = targetAccess.HasFullAccess ? 7 : (targetAccess.HasWriteAccess ? 3 : (targetAccess.HasReadAccess ? 1 : 0));
                controller.ViewBag.IsAdmin = currentUser.IsSuperAdmin;
                await next();
            }
        }
    }
}