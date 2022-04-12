using System;

namespace SS.HealthApp.Model.AccountModels {

    public class AccountStatement : _BaseModel {

        public string Description { get; set; }

        public string Facility { get; set; }

        public DateTime Date { get; set; }

        public decimal Value { get; set; }

        public bool Payed { get; set; }

    }
}
