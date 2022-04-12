using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SS.HealthApp.Core.Models
{

    public class AuthenticatedUser : ClaimsIdentity
    {

        public AuthenticatedUser() : base("bearer") { }

        public AuthenticatedUser(System.Security.Principal.IIdentity Identity) : base(Identity)
        {

        }

        public string Id
        {
            get
            {
                Claim IdClaim = Claims.FirstOrDefault(c => c.Type == "userid");
                return IdClaim != null ? IdClaim.Value : null;
            }
            set
            {
                AddClaim(new Claim("userid", value));
            }
        }

        public string Username
        {
            get
            {
                Claim usernameClaim = Claims.FirstOrDefault(c => c.Type == "username");
                return usernameClaim != null ? usernameClaim.Value : null;
            }
            set
            {
                AddClaim(new Claim("username", value));
            }
        }

        public string CompanyId
        {
            get
            {
                Claim companyIdClaim = Claims.FirstOrDefault(c => c.Type == "companyId");
                return companyIdClaim != null ? companyIdClaim.Value : null;
            }
            set
            {
                AddClaim(new Claim("companyId", value.ToString()));
            }
        }

        public Dictionary<string, string> Properties
        {
            get
            {
                Claim propertiesClaim = Claims.FirstOrDefault(c => c.Type == "customProperties");
                return propertiesClaim != null ? JsonConvert.DeserializeObject<Dictionary<string,string>>(propertiesClaim.Value) : null;
            }
            set
            {
                AddClaim(new Claim("customProperties", JsonConvert.SerializeObject(value)));
            }
        }

        public ClientConnector.Models.AuthenticatedUser ConvertToClientConnectorModel()
        {
            return new ClientConnector.Models.AuthenticatedUser
            {
                Id = Id,
                Username = Username,
                Properties = Properties
            };
        }

        public static AuthenticatedUser ConvertFromClientConnectorModel(ClientConnector.Models.AuthenticatedUser AuthenticatedUser, string CompanyId)
        {
            return new AuthenticatedUser
            {
                Id = AuthenticatedUser.Id,
                Username = AuthenticatedUser.Username,
                CompanyId = CompanyId,
                Properties = AuthenticatedUser.Properties
            };
        }

    }

}
