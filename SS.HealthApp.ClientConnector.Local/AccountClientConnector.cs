using System;
using System.Collections.Generic;
using SS.HealthApp.ClientConnector.Interfaces;
using SS.HealthApp.ClientConnector.Models;
using SS.HealthApp.Model.AccountModels;

namespace SS.HealthApp.ClientConnector.Local
{
    public class AccountClientConnector : IAccountClientConnector
    {

        public List<AccountStatement> GetAccountStatement(ClientConnector.Models.AuthenticatedUser User)
        {
            var random = new Random();

            return new List<AccountStatement>
            {
                new AccountStatement {
                    ID = "1",
                    Facility = "All Saints Hospital",
                    Description = String.Format("ASH{0}", random.Next(100000, int.MaxValue)),
                    Date = new System.DateTime(2016, 5, 19, 11, 30, 0),
                    Value = 54,
                    Payed = true
                },
                new AccountStatement() {
                    ID = "2",
                    Facility = "Main Clinic",
                    Description = String.Format("MC{0}", random.Next(100000, int.MaxValue)),
                    Date = new System.DateTime(2016, 4, 19, 11, 30, 0),
                    Value = 37.5m,
                    Payed = false
                },
                new AccountStatement() {
                    ID = "3",
                    Facility = "Surgical Hospital",
                    Description = String.Format("SH{0}", random.Next(100000, int.MaxValue)),
                    Date = new System.DateTime(2015, 8, 5, 16, 45, 0),
                    Value = 125,
                    Payed = true
                },
                new AccountStatement() {
                    ID = "4",
                    Facility = "Main Clinic",
                    Description = String.Format("MC{0}", random.Next(100000, int.MaxValue)),
                    Date = new System.DateTime(2011, 8, 8, 12, 40, 0),
                    Value = 32.5m,
                    Payed = true
                },
                new AccountStatement() {
                    ID = "5",
                    Facility = "All Saints Hospital",
                    Description = String.Format("ASH{0}", random.Next(100000, int.MaxValue)),
                    Date = new System.DateTime(2011, 8, 5, 12, 40, 0),
                    Value = 32.5m,
                    Payed = true
                },
                new AccountStatement() {
                    ID = "6",
                    Facility = "All Saints Hospital",
                    Description = String.Format("ASH{0}", random.Next(100000, int.MaxValue)),
                    Date = new System.DateTime(2011, 8, 5, 12, 40, 0),
                    Value = 25,
                    Payed = true
                },
                new AccountStatement() {
                    ID = "7",
                    Facility = "Surgical Hospital",
                    Description = String.Format("SH{0}", random.Next(100000, int.MaxValue)),
                    Date = new System.DateTime(2011, 1, 12, 11, 25, 0),
                    Value = 12,
                    Payed = true
                }
            };
        }

        public byte[] GetAccountStatementFile(AuthenticatedUser User, string documentID)
        {
            return System.IO.File.ReadAllBytes(string.Concat(Settings.ResourcesDir, "testdoc.pdf"));
        }

    }
}
