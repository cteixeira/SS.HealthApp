using SS.HealthApp.ClientConnector.Interfaces;
using System.Collections.Generic;
using System.Linq;
using SS.HealthApp.Model.FacilityModels;

namespace SS.HealthApp.ClientConnector.SAMS
{
    public class FacilityClientConnector : IFacilityClientConnector
    {

        public List<Facility> GetFacilities()
        {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {

                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.UnidadeSaude[] facilities = serviceProxy.ObterListaUnidades();

                if(facilities != null)
                {
                    return facilities.Select(b => new Facility {
                        ID = b.CodigoUnidadeSaude, 
                        Name = b.Titulo,
                        Image = b.ImagemMobile,
                        Detail = b.Descricao,
                        Address = b.Morada,
                        Phone = b.NumeroTelefone,
                        Coordinates = b.CoordenadasGPS
                    }).ToList();
                }

                return null;
            }
        }
    }
}
