using System.Web;

namespace SS.HealthApp.Core
{
    public static class CoreContext
    {
        public static Models.AuthenticatedUser CurrentUser
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.User != null)
                {
                    return new Models.AuthenticatedUser(HttpContext.Current.User.Identity);
                }
                return null;
            }
        }
    }
}
