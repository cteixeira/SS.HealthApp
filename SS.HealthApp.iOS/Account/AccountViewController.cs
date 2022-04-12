using CoreGraphics;
using System;
using UIKit;
using ToastIOS;
using Plugin.Connectivity;

namespace SS.HealthApp.iOS.Account {
    public partial class AccountViewController : UIViewController {

        /*private static UILabel lblPending;

        internal static decimal PendingValue {
            set {
                lblPending.Text = string.Format("{0}: {1}{2}", Utils.LocalizedString("AccountPending"), value, PCL.Settings.CurrencySymbol);
                if (value > 0)
                    lblPending.TextColor = UIColor.Orange;
            }
        }*/

        private static UINavigationController Navigation { get; set; }

        public override void ViewDidAppear(bool animated) {
            base.ViewDidAppear(animated);
            Navigation = NavigationController; //HACKING: Navigation Controller looses reference after selecting option from SlideOutViewController
        }

        public override void ViewDidLoad() {

            try {
                base.ViewDidLoad();

                Title = Utils.LocalizedString("AccountTitle");

                if (!CrossConnectivity.Current.IsConnected)
                    Utils.ShowToast(Utils.LocalizedString("ErrConnLight"), ToastType.Error, 3000);

                NavigationItem.BackBarButtonItem = new UIBarButtonItem(Utils.LocalizedString("Back"), UIBarButtonItemStyle.Plain, null);

                nfloat NavBarHeight = NavigationController.NavigationBar.Frame.Top + NavigationController.NavigationBar.Frame.Height;

                /*UIView headerView = new UIView(new CGRect(0, NavBarHeight, View.Frame.Width, 50));
                headerView.BackgroundColor = Settings.TABLE_HEADER_COLOR;
                UILabel lblTitle = Utils.NewLabel(new CGRect(0, 15, View.Frame.Width, 20),
                    Utils.LocalizedString("AccountTableTitle"), Settings.FIRST_COLOR, UIFont.PreferredSubheadline, UITextAlignment.Center);
                headerView.AddSubview(lblTitle);

                View.AddSubview(headerView);*/

                /*lblPending = Utils.NewLabel(new CGRect(10, 10, View.Frame.Width - 20, 30), 
                    string.Empty, Settings.FIRST_COLOR, UIFont.PreferredTitle2, UITextAlignment.Right);
                headerView.AddSubview(lblPending);*/

                var list = new AccountListViewController();
                //list.View.Frame = new CGRect(0, headerView.Frame.Top + headerView.Frame.Height, View.Frame.Width, View.Frame.Height - headerView.Frame.Top - headerView.Frame.Height);
                list.View.Frame = new CGRect(0, NavBarHeight, View.Frame.Width, View.Frame.Height - NavBarHeight);
                View.AddSubview(list.View);
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        internal static void PushView(UIViewController controller) {
            Navigation.PushViewController(controller, false);
        }

    }
}