using System;

namespace SS.HealthApp.Model.UserModels {
    public class AuthenticationData
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string AccessToken { get; set; }

        public DateTime TokenIssuedDate { get; set; }

        public TimeSpan TokenExpiresIn { get; set; }

        public bool TokenIsEspired()
        {
            //token is considered expired after the date issues + exires in with a margin of error of 2 minutes
            return DateTime.Now > TokenIssuedDate.Add(TokenExpiresIn).AddMinutes(-2);
        }

    }
}
