using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.HealthApp.ClientConnector
{
    public static class Config
    {

        private static SimpleSolutions.wcSecure.Decifra wcSec = new SimpleSolutions.wcSecure.Decifra();

        public static string MySAMSApiWS_Username { get { return wcSec.Decifrar(ConfigurationManager.AppSettings["MySAMSApiWS_Username"]); }  }
        public static string MySAMSApiWS_Password { get { return wcSec.Decifrar(ConfigurationManager.AppSettings["MySAMSApiWS_Password"]); } }


    }
}
