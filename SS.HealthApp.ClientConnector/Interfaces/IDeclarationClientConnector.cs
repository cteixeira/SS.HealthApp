using System.Collections.Generic;
using SS.HealthApp.Model.DeclarationModels;

namespace SS.HealthApp.ClientConnector.Interfaces
{
   public  interface IDeclarationClientConnector
    {

        List<PresenceDeclaration> GetPresenceDeclaration(Models.AuthenticatedUser User);

        byte[] GetPresenceDeclarationFile(ClientConnector.Models.AuthenticatedUser User, string declarationID);

        string GetPresenceDeclarationIdByAppointmentID(ClientConnector.Models.AuthenticatedUser User, string appointmentID);

    }
}
