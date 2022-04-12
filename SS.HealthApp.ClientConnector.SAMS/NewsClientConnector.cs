using SS.HealthApp.ClientConnector.Interfaces;
using System.Collections.Generic;
using System.Linq;
using SS.HealthApp.Model.NewsModels;

namespace SS.HealthApp.ClientConnector.SAMS
{
    public class NewsClientConnector : INewsClientConnector
    {

        public List<News> GetNews()
        {
            using (MySAMSApiWS.MySAMSApiWSClient serviceProxy = new MySAMSApiWS.MySAMSApiWSClient()) {

                serviceProxy.ClientCredentials.UserName.UserName = Config.MySAMSApiWS_Username;
                serviceProxy.ClientCredentials.UserName.Password = Config.MySAMSApiWS_Password;

                MySAMSApiWS.Noticia[] news = serviceProxy.ObterListaNoticias();

                if(news != null)
                {
                    return news.Select(b => new News {
                        ID = b.IdNoticia, 
                        Name = b.Titulo,
                        Image = b.ImagemMobile,
                        Detail = b.TextoDescritivo,
                        Date = b.Data
                    }).OrderByDescending(b => b.Date).ToList();
                }

                return null;
            }
        }
    }
}
