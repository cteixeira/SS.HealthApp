using System;
using CoreGraphics;
using UIKit;

namespace SS.HealthApp.iOS.User {
    public class UserViewController : UIViewController {

        private static UINavigationController Navigation { get; set; }

        public override void ViewDidAppear(bool animated) {
            base.ViewDidAppear(animated);
            Navigation = NavigationController; //HACKING: Navigation Controller looses reference after selecting option from SlideOutViewController
        }

        public override void ViewDidLoad() {
            base.ViewDidLoad();

            Title = Utils.LocalizedString("UserTitle");

            NavigationItem.BackBarButtonItem = new UIBarButtonItem(Utils.LocalizedString("Back"), UIBarButtonItemStyle.Plain, null);

            nfloat NavBarHeight = NavigationController.NavigationBar.Frame.Top + NavigationController.NavigationBar.Frame.Height;

            UIView headerView = new UIView(new CGRect(0, NavBarHeight, View.Frame.Width, 50));
            headerView.BackgroundColor = Settings.TABLE_HEADER_COLOR;

            UILabel lblTitle = Utils.NewLabel(new CGRect(0, 15, View.Frame.Width, 20),
                Utils.LocalizedString("UserTableTitle"), Settings.FIRST_COLOR, UIFont.PreferredSubheadline, UITextAlignment.Center);
            headerView.AddSubview(lblTitle);

            View.AddSubview(headerView);

            var list = new UserListViewController();
            list.View.Frame = new CGRect(0, headerView.Frame.Top + headerView.Frame.Height,
                View.Frame.Width, View.Frame.Height - headerView.Frame.Top - headerView.Frame.Height);
            View.AddSubview(list.View);
        }

        internal static void PushView(UIViewController controller) {
            Navigation.PushViewController(controller, false);
        }
    }
}