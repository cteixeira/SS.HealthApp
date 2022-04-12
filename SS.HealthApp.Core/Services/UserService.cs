using System;
using SS.HealthApp.Model.UserModels;
using SS.HealthApp.ClientConnector.Interfaces;

namespace SS.HealthApp.Core.Services
{
    public class UserService
    {

        public Models.AuthenticatedUser LoginUser(Model.UserModels.Login loginData) {

            ILoginClientConnector loginService = 
                ClientConnector.ClientConnectorManager.Instance.Create<ILoginClientConnector>(loginData.CompanyId);

            if (String.IsNullOrEmpty(loginData.CompanyId))
                throw new ArgumentNullException("LoginData.CompanyId");

            if (String.IsNullOrEmpty(loginData.Username))
                throw new ArgumentNullException("LoginData.Username");

            if (String.IsNullOrEmpty(loginData.Password))
                throw new ArgumentNullException("LoginData.Password");

            ClientConnector.Models.AuthenticatedUser authUser = loginService.Login(loginData);
            if (authUser != null)
            {
                Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null, loginData.CompanyId, loginData.Username);
                return Models.AuthenticatedUser.ConvertFromClientConnectorModel(authUser, loginData.CompanyId);
            }

            

            return null;

        }

        public PersonalData GetPersonalData() {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            IUserClientConnector service = 
                ClientConnector.ClientConnectorManager.Instance.Create<IUserClientConnector>(CoreContext.CurrentUser.CompanyId);

            return service.GetPersonalData(CoreContext.CurrentUser.ConvertToClientConnectorModel());
        }

        public bool SavePersonalData(PersonalData pData) {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            IUserClientConnector service = 
                ClientConnector.ClientConnectorManager.Instance.Create<IUserClientConnector>(CoreContext.CurrentUser.CompanyId);

            return service.SavePersonalData(CoreContext.CurrentUser.ConvertToClientConnectorModel(), pData);
        }

        public bool ChangePassword(ChangePassword pData) {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            IUserClientConnector service =
                ClientConnector.ClientConnectorManager.Instance.Create<IUserClientConnector>(CoreContext.CurrentUser.CompanyId);

            return service.ChangePassword(CoreContext.CurrentUser.ConvertToClientConnectorModel(), pData);
        }

    }
}
