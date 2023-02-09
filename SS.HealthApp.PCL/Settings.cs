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
        internal const string TokenEndpointUrl = "http://10.0.2.2:8080/api/token";
        internal const string ServicesBaseUrl = "http://10.0.2.2:8080/api/";

        // with Conveyor
        //internal const string TokenEndpointUrl = "http://192.168.1.93:45457/api/token";
        //internal const string ServicesBaseUrl = "http://192.168.1.93:45457/api/";

        //SAMS
        //internal const string CompanyID = "SAMS";
        //internal const string CompanySecret = "xxx";
        //internal const string TokenEndpointUrl = "http://ss.healthapp.services.simplesolutions.pt/api/token";
        //internal const string ServicesBaseUrl = "http://ss.healthapp.services.simplesolutions.pt/api/";
    }
}
