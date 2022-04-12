namespace SS.HealthApp.Core.Utils {
    internal class Resources
    {
        public static string GetResource(string resourceName)
        {
            return Core.Resources.Strings.ResourceManager.GetString(string.Format("{0}_{1}", CoreContext.CurrentUser.CompanyId, resourceName));
        }

    }
}
