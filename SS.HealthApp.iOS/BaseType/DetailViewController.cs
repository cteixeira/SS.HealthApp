using System;

using UIKit;

namespace SS.HealthApp.iOS.BaseType {
    public class DetailViewController : UIViewController {

        public string ID { get; set; }

        public DetailViewController() { }

        public DetailViewController(string ID) {
            this.ID = ID;
        }
    }
}