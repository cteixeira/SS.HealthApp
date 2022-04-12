using SS.HealthApp.ClientConnector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using SS.HealthApp.Model.HomeModels;

namespace SS.HealthApp.ClientConnector.SAMS
{
    public class HomeClientConnector : IHomeClientConnector
    {
        public List<Banner> GetBanners()
        {
            return GetBannersV2("");
        }

        public List<Banner> GetBannersV2(string userid)
        {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {

                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.HomepageBanner[] banners = serviceProxy.ObterBannersHomepageV2(userid);

                if(banners != null)
                {
                    return banners.Select(b => new Banner {
                        ID = b.ID, 
                        Image = b.Imagem,
                        Link = b.Link
                    }).ToList();
                }

                return null;
            }
        }
        
        public List<EmergencyDelay> GetEmergencyWaitTime() {

            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {
                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.APTempoEspera[] emergencyWaitTimes = serviceProxy.ObterTempoAP();

                if (emergencyWaitTimes != null) {

                    List<EmergencyDelay> ret = new List<EmergencyDelay>();

                    //Centro clinico
                    var centroClinicoAdultWaitingTime = emergencyWaitTimes.FirstOrDefault(wt => wt.TipoAtendimentoPermanente == MySAMSApiWS.EnumsTipoAtendimentoPermanente.CentroClinicoAdultos);
                    var centroClinicoChildrenWaitingTime = emergencyWaitTimes.FirstOrDefault(wt => wt.TipoAtendimentoPermanente == MySAMSApiWS.EnumsTipoAtendimentoPermanente.CentroClinicoPediatria);

                    ret.Add(new EmergencyDelay {
                        Facility = "Centro Clínico de Lisboa",
                        AdultDelay = (centroClinicoAdultWaitingTime.Encerrado ? -1 : centroClinicoAdultWaitingTime.MinutosEspera),
                        ChildrenDelay = (centroClinicoChildrenWaitingTime.Encerrado ? -1 : centroClinicoChildrenWaitingTime.MinutosEspera)
                    });

                    //Hospital
                    var hospitalAdultWaitingTime = emergencyWaitTimes.FirstOrDefault(wt => wt.TipoAtendimentoPermanente == MySAMSApiWS.EnumsTipoAtendimentoPermanente.HospitalAdultos);
                    var hospitaChildrenlWaitingTime = emergencyWaitTimes.FirstOrDefault(wt => wt.TipoAtendimentoPermanente == MySAMSApiWS.EnumsTipoAtendimentoPermanente.HospitalPediatria);

                    ret.Add(new EmergencyDelay {
                        Facility = "Hospital SAMS",
                        AdultDelay = (hospitalAdultWaitingTime.Encerrado ? -1 : hospitalAdultWaitingTime.MinutosEspera),
                        ChildrenDelay = -2 //(hospitaChildrenlWaitingTime.Encerrado ? -1 : hospitaChildrenlWaitingTime.MinutosEspera)
                    });

                    return ret;
                }

                return null;

            }
        }

    }
}
