using System.Collections.Generic;
using SS.HealthApp.Model.HomeModels;

namespace SS.HealthApp.ClientConnector.Interfaces
{
    public interface IHomeClientConnector
    {

        List<Banner> GetBanners();

        List<EmergencyDelay> GetEmergencyWaitTime();

    }
}
