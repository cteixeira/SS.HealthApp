namespace SS.HealthApp.Model.FacilityModels {
    public class Facility : _BaseModel {

        public string Name { get; set; }

        public string Image { get; set; }

        public string Detail { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Coordinates { get; set; }

        public int TimeDistance { get; set; }
        
    }
}
