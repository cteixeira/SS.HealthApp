using System.Collections.Generic;

namespace SS.HealthApp.ClientConnector.Models
{
    public class AuthenticatedUser
    {

        public string Id { get; set; }

        public string Username { get; set; }

        public Dictionary<string, string> Properties { get; set; }


        public AuthenticatedUser()
        {
            Properties = new Dictionary<string, string>();
        }

    }

}
