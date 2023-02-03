namespace SS.HealthApp.PCL {
    public class Settings {

        //*********************
        //** Common settings **
        //*********************
        public const int BannerTimer = 5000;
        public const int DelayTimer = 3000;
        public const string CurrencySymbol = "€";

        //******************************************************************
        //** This settings should change accordingly to the company build **
        //******************************************************************

        //Local 
        internal const string CompanyID = "Local";
        internal const string CompanySecret = "password";
        internal const string TokenEndpointUrl = "http://localhost:53582/api/token";
        internal const string ServicesBaseUrl = "http://localhost:53582/api/";

        //SAMS
        //internal const string CompanyID = "SAMS";
        //internal const string CompanySecret = "xxx";
        //internal const string TokenEndpointUrl = "http://ss.healthapp.services.simplesolutions.pt/api/token";
        //internal const string ServicesBaseUrl = "http://ss.healthapp.services.simplesolutions.pt/api/";
    }
}
