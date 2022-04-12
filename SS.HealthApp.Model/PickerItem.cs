namespace SS.HealthApp.Model {
    public class PickerItem : _BaseModel {
        public string Title { get; set; }
        public PickerItem(string id, string title) {
            this.ID = id;
            this.Title = title;
        }

        public override string ToString()
        {
            return Title;
        }

    }

}
