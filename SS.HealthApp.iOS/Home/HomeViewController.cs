using CoreGraphics;
using UIKit;
using ToastIOS;
using Plugin.Connectivity;
using SS.HealthApp.PCL.Services;

namespace SS.HealthApp.iOS.Home {
    public partial class HomeViewController : UIViewController {

        private const float BANNER_CONTROL_HEIGHT = 170;
        private const float EMERGENCY_TICKET_CONTROL_HEIGHT = 104;

        private static UINavigationController Navigation { get; set; }

        UIScrollView scrollView;

        public override void ViewDidAppear(bool animated) {
            base.ViewDidAppear(animated);
            Navigation = NavigationController; //HACKING: Navigation Controller looses reference after selecting option from SlideOutViewController

            //Emergency or Ticket
            SetupAsync();
        }

        public override void ViewDidLoad() {

            try {
                base.ViewDidLoad();

                Title = Settings.APP_NAME;

                if (!CrossConnectivity.Current.IsConnected)
                    Utils.ShowToast(Utils.LocalizedString("ErrConnLight"), ToastType.Error, 3000);

                NavigationItem.BackBarButtonItem = new UIBarButtonItem(Utils.LocalizedString("Back"), UIBarButtonItemStyle.Plain, null);

                //Put user button in navigation bar
                UIBarButtonItem navItem = new UIBarButtonItem(
                    UIImage.FromBundle("Icons/ic_person_white_36pt").Scale(new CGSize(32, 32), 0),
                    UIBarButtonItemStyle.Plain,
                    (s, a) => { PushView(new User.UserViewController()); });
                NavigationItem.SetRightBarButtonItem(navItem, true);

                scrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));

                //Banners
                var banner = new BannerViewController(0);
                scrollView.AddSubview(banner.View);

                //Main Menu Options
                var menu = new MenuViewController(BANNER_CONTROL_HEIGHT + EMERGENCY_TICKET_CONTROL_HEIGHT);
                scrollView.AddSubview(menu.View);

                scrollView.ContentSize = new CGSize(View.Frame.Width, BANNER_CONTROL_HEIGHT + EMERGENCY_TICKET_CONTROL_HEIGHT + menu.Height);
                View.AddSubview(scrollView);
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void SetupAsync() {

            var ticket = await new AttendanceService().GetNextTicketAsync();

            if (ticket != null) { //Next Ticket Information
                var ticketViewController = new TicketViewController(BANNER_CONTROL_HEIGHT, EMERGENCY_TICKET_CONTROL_HEIGHT, ticket.Local, ticket.Number);
                scrollView.AddSubview(ticketViewController.View);
            }
            else { //Emergency Wait Time
                var emergency = new EmergencyViewController(BANNER_CONTROL_HEIGHT);
                scrollView.AddSubview(emergency.View);
            }

        }

        internal static void PushView(UIViewController controller) {
            Navigation.PushViewController(controller, false);
        }

    }
}