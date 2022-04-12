using System.Collections.Generic;
using SS.HealthApp.Model.FacilityModels;

namespace SS.HealthApp.ClientConnector.Interfaces
{
    public interface IFacilityClientConnector {

        List<Facility> GetFacilities();

    }
}
