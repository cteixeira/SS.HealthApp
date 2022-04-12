using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
using System.Text;

namespace SS.HealthApp.Services.NLog
{
    [LayoutRenderer("company-id")]
    public class CompanyLayoutRender : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if(Core.CoreContext.CurrentUser != null)
            {
                builder.Append(Core.CoreContext.CurrentUser.CompanyId);
            }
        }

    }
}