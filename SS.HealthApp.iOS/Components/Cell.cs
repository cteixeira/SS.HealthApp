using System.Collections.Generic;
using UIKit;

namespace SS.HealthApp.iOS.Components {
    public class Cell {

        internal const string CELL_KEY = "iOS.Components.Cell";

        public string ID { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string Detail { get; set; }

        public Dictionary<string, object> Properties { get; set;  }

        public List<UIButton> Buttons { get; set; }

    }
}
