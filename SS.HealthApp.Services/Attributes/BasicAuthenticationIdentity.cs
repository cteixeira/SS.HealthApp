using System.Security.Principal;

namespace SS.HealthApp.Services.Attributes {
    public class BasicAuthenticationIdentity : GenericIdentity {
        public string Password { get; set; }

        public BasicAuthenticationIdentity(string username, string password)
            : base(username, "Basic") {
            this.Password = password;
        }
    }
}