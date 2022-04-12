using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SS.HealthApp.Services.NLog
{

    [LayoutRenderer("user-id")]
    public class UserIdLayoutRender : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (Core.CoreContext.CurrentUser != null)
            {
                builder.Append(Core.CoreContext.CurrentUser.Id);
            }
        }
    }

    [LayoutRenderer("user-username")]
    public class UserUsernameLayoutRender : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (Core.CoreContext.CurrentUser != null)
            {
                builder.Append(Core.CoreContext.CurrentUser.Username);
            }
        }

    }
}