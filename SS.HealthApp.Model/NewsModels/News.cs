using System;

namespace SS.HealthApp.Model.NewsModels {
    public class News : _BaseModel {

        public string Name { get; set; }

        public string Image { get; set; }

        public string Detail { get; set; }

        public DateTime Date { get; set; }
        
    }
}
