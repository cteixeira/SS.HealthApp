using System;

namespace SS.HealthApp.Model.DeclarationModels {

    public class PresenceDeclaration : _BaseModel {

        public string Appointment { get; set; }

        public string Facility { get; set;  }

        public DateTime Moment { get; set;  }
        
    }

}
