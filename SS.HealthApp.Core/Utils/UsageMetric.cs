using System;

namespace SS.HealthApp.Core.Utils {
    public class UsageMetric {

        public static void Add(string className, string methodName, string info) {
            Add(className, methodName, info, CoreContext.CurrentUser.CompanyId, CoreContext.CurrentUser.Id);
        }

        public static void Add(string className, string methodName, string info, string companyID, string userID) {
            try {
                using (Models.Context ctx = new Models.Context()) {
                    ctx.UsageMetric.Add(new Models.UsageMetric() {
                        Class = className,
                        Method = methodName,
                        Info = info,
                        Moment = DateTime.Now,
                        CompanyID = companyID, 
                        UserID = userID
                    });
                    ctx.SaveChanges();
                }
            }
            catch { }

        }

    }
}
