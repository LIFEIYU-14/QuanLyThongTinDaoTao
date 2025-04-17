using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

public class RoleAuthorizeAttribute : AuthorizeAttribute
{
    private readonly string[] allowedRoles;
    public RoleAuthorizeAttribute(params string[] roles)
    {
        this.allowedRoles = roles;
    }

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        var userRole = httpContext.Session["TenQuyen"]?.ToString();
        return userRole != null && allowedRoles.Contains(userRole);
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        filterContext.Result = new RedirectResult("~/Admin/Login/DangNhap");
    }
}
