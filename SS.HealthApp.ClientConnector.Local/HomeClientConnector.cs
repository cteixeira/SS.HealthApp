using System;
using System.Collections.Generic;
using System.Linq;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.Model.HomeModels;

namespace SS.HealthApp.ClientConnector.Local
{
    public class HomeClientConnector : IHomeClientConnector
    {

        static List<Local.Models.Banner> bannersDataSource = FillBannersDataSource();

        public List<Banner> GetBanners()
        {
            return bannersDataSource.Where(b => String.IsNullOrEmpty(b.HealthCareFacilityId)).Select(b => b.ToConnectorModel()).ToList();
        }

        public List<EmergencyDelay> GetEmergencyWaitTime() {
            return new List<EmergencyDelay>
            {
                new EmergencyDelay {
                    Facility = "All Saints Hospital",
                    AdultDelay = 7,
                    ChildrenDelay = 3
                },
                new EmergencyDelay {
                    Facility = "Surgical Hospital",
                    AdultDelay = 13,
                    ChildrenDelay = -1
                },
                new EmergencyDelay {
                    Facility = "Main Clinic",
                    AdultDelay = 2,
                    ChildrenDelay = 0
                },
            };
        }

        private static List<Local.Models.Banner> FillBannersDataSource()
        {

            List<ClientConnector.Local.Models.Banner> ret = new List<Models.Banner>();

            ret.Add(new Models.Banner {
                Id = "1",
                ImageUrl = string.Concat(Settings.ResourcesUrl, "Promo1.jpg"),
                Link = "http://www.simplesolutions.pt"
            });
            ret.Add(new Models.Banner {
                Id = "3",
                ImageUrl = string.Concat(Settings.ResourcesUrl, "Promo3.jpg"),
                Link = "http://www.simplesolutions.pt"
            });
            ret.Add(new Models.Banner {
                Id = "2",
                ImageUrl = string.Concat(Settings.ResourcesUrl, "Promo2.jpg"),
                Link = "http://www.simplesolutions.pt"
            });
            ret.Add(new Models.Banner {
                Id = "4",
                ImageUrl = string.Concat(Settings.ResourcesUrl, "Promo4.jpg"),
                Link = "http://www.simplesolutions.pt"
            });

            return ret;
        }
        

    }
}
