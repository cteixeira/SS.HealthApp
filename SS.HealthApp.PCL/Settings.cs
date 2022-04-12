
namespace SS.HealthApp.PCL {
    public class Settings {

        //******************************************************************
        //** This settings should change accordingly to the company build **
        //******************************************************************

        //Local 
        //internal const string CompanyID = "Local";
        //internal const string CompanySecret = "password";

        //SAMS
        internal const string CompanyID = "SAMS";
        internal const string CompanySecret = "w^$SC=j2Xpy7%K#!M7m%#QwSTYeY^^Qj";

        //internal const string TokenEndpointUrl = "http://localhost/SS.HealthApp.Services/api/token";
        //internal const string ServicesBaseUrl = "http://localhost/SS.HealthApp.Services/api/";
        internal const string TokenEndpointUrl = "http://ss.healthapp.services.simplesolutions.pt/api/token";
        internal const string ServicesBaseUrl = "http://ss.healthapp.services.simplesolutions.pt/api/";


        //*********************
        //** Common settings **
        //*********************
        public const int BannerTimer = 5000;
        public const int DelayTimer = 3000;
        public const string CurrencySymbol = "€";
    }
}
