using SS.HealthApp.ClientConnector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using SS.HealthApp.Model.UserModels;

namespace SS.HealthApp.ClientConnector.Local
{
    public class UserClientConnector : IUserClientConnector
    {

        internal static List<Models.User> dataSource = FillDataSource();

        public PersonalData GetPersonalData(ClientConnector.Models.AuthenticatedUser User)
        {
            Models.User user = UserClientConnector.dataSource.FirstOrDefault(u => u.Id == User.Id);
            if (user != null)
            {
                return user.ToConnectorModel();
            }
            return null;
        }

        public bool SavePersonalData(ClientConnector.Models.AuthenticatedUser User, PersonalData PersonalData)
        {
            Models.User user = UserClientConnector.dataSource.FirstOrDefault(u => u.Id == User.Id);
            user.Name = PersonalData.Name;
            user.Email = PersonalData.Email;
            user.Address = PersonalData.Address;
            user.PhoneNumber = PersonalData.PhoneNumber;
            user.TaxNumber = PersonalData.TaxNumber;
            return true;
        }

        public bool ChangePassword(ClientConnector.Models.AuthenticatedUser User, ChangePassword pData)
        {
            Models.User user = UserClientConnector.dataSource.FirstOrDefault(u => u.Id == User.Id);
            if (user.Password == pData.oldPassword)
            {
                user.Password = pData.newPassword;
                return true;
            }
            return false;
        }

        private static List<Models.User> FillDataSource()
        {
            return new List<Models.User>{
                new Models.User {
                    Id= "1", Name = "Simple Solutions", Username = "me", Password = "1234", Email = "digitalcustomer@simplesolutions.pt",
                    TaxNumber = "505045923",Mobile = "", PhoneNumber = "214676118", Address = "Taguspark - Núcleo Central 205 | 2740-122 Porto-Salvo"
                },
                new Models.User {
                    Id= "2", Name = "InterSystems", Username = "intersystems", Password = "0000", Email = "mike.fuller@intersystems.com",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "USA"
                },
                new Models.User {
                    Id= "3", Name = "BMAC", Username = "bmac", Password = "0000", Email = "manuel.magalhaes@bmac.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "4", Name = "SPARK2D", Username = "spark2d", Password = "0000", Email = "andre.carvalho@spark2d.com",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "5", Name = "Everedge", Username = "everedge", Password = "0000", Email = "bruno.costa@everedge.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "6", Name = "InovaPrime", Username = "inovaprime", Password = "0000", Email = "duarte.abrantes@inovaprime.com",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "7", Name = "Bizdirect", Username = "bizdirect", Password = "0000", Email = "eduardo.oliveira@bizdirect.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "8", Name = "IDEALGLOBALTEK", Username = "idealglobaltek", Password = "0000", Email = "nuno.gouveia@idealglobaltek.com",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "9", Name = "GFI", Username = "gfi", Password = "0000", Email = "ricardo.nunes@gfi.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "10", Name = "Decsis", Username = "decsis", Password = "0000", Email = "artur.romao@decsis.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "11", Name = "Microsoft", Username = "microsoft", Password = "0000", Email = "geral@microsoft.com",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "12", Name = "Lusíadas", Username = "lusiadas", Password = "0000", Email = "geral@lusiadas.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "13", Name = "SCML", Username = "scml", Password = "0000", Email = "geral@scml.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "14", Name = "TrofaSaude", Username = "trofasaude", Password = "0000", Email = "geral@trofasaude.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "15", Name = "HTQ", Username = "htq", Password = "0000", Email = "geral@htq.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "16", Name = "Luz Saúde", Username = "luzsaude", Password = "0000", Email = "geral@luzsaude.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "17", Name = "HPA", Username = "hpa", Password = "0000", Email = "geral@hpa.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "18", Name = "JCS", Username = "jcs", Password = "0000", Email = "geral@jcs.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "19", Name = "Soerad", Username = "soerad", Password = "0000", Email = "geral@soerad.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "20", Name = "HSMP", Username = "hsmporto", Password = "0000", Email = "geral@hsmporto.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "21", Name = "ISJD", Username = "isjd", Password = "0000", Email = "geral@isjd.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                },
                new Models.User {
                    Id= "22", Name = "UCS", Username = "ucs", Password = "0000", Email = "geral@ucs.pt",
                    TaxNumber = "", Mobile = "", PhoneNumber = "", Address = "PT"
                }
            };
        }

    }
}
