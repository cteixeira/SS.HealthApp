namespace SS.HealthApp.Model {
    public class ServiceItem : PickerItem {
        public Enum.AppointmentType Type { get; set; }
        public ServiceItem(string id, string title, Enum.AppointmentType type) : base(id, title)  {
            Type = type;
        }
    }

}
