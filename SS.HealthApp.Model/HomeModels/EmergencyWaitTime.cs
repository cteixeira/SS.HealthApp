namespace SS.HealthApp.Model.HomeModels {
    public class EmergencyDelay : _BaseModel {

        public string Facility { get; set; }

        public int AdultDelay { get; set; }

        public int ChildrenDelay { get; set; }
        
    }
}
