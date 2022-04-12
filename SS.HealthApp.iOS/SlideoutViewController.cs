using CoreGraphics;
using MonoTouch.Dialog;
using UIKit;

namespace SS.HealthApp.iOS {
    public class SlideoutViewController : DialogViewController {

        private static UINavigationController Navigation { get; set; }

        public SlideoutViewController() : base(UITableViewStyle.Grouped, new RootElement("")) {}

        public override void ViewDidLoad() {
            base.ViewDidLoad();

            Navigation = NavigationController;

            View.BackgroundColor = Settings.FIRST_COLOR;

            Root.Add(new Section() {
                StyledElement(Utils.LocalizedString("HomeTitle"), () => PushView(new Home.HomeViewController())),
                StyledElement(Utils.LocalizedString("AppointmentTitle"), () => PushView(new Appointment.AppointmentViewController())),
                StyledElement(Utils.LocalizedString("MessageTitle"), () => PushView(new Message.MessageViewController())),
                StyledElement(Utils.LocalizedString("AccountTitle"), () => PushView(new Account.AccountViewController())),
                StyledElement(Utils.LocalizedString("NewsTitle"), () => PushView(new News.NewsViewController())),
                StyledElement(Utils.LocalizedString("FacilityTitle"), () => PushView(new Facility.FacilityViewController())),
                StyledElement(Utils.LocalizedString("DeclarationTitle"), () => PushView(new Declaration.DeclarationViewController())),
                StyledElement(Utils.LocalizedString("UserTitle"), () => PushView(new User.UserViewController())),
                StyledElement(Utils.LocalizedString("AboutTitle"), () => PushView(new AboutViewController())),
                StyledElement(Utils.LocalizedString("Logout"), () => Logout())
            });

            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        private StyledStringElement StyledElement(string name, System.Action action) {
            return new StyledStringElement( name, () => action()) { Font = UIFont.PreferredTitle2, TextColor = Settings.FIRST_COLOR_TEXT, BackgroundColor = Settings.FIRST_COLOR };
        }

        internal static void PushView(UIViewController controller) {
            Navigation.PushViewController(controller, false);
        }

        private async void Logout() {
            try {
                if (await new PCL.Services.UserService().Logout() == true) {
                    var app = (AppDelegate)UIApplication.SharedApplication.Delegate;
                    app.StartLogin();
                }
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }
    }
}
