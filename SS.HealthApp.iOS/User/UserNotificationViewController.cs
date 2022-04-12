using System;
using CoreGraphics;
using UIKit;

namespace SS.HealthApp.iOS.User {
    public class UserNotificationViewController : UIViewController {

        public override void DidReceiveMemoryWarning() {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad() {
            base.ViewDidLoad();
            Title = Utils.LocalizedString("UserTitleNotification");
        }
        
        
    }
}