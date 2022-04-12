using Foundation;
using UIKit;
using MonoTouch.SlideoutNavigation;
using HockeyApp.iOS;
using System;

namespace SS.HealthApp.iOS {

    public class Application {
        public static void Main(string[] args) {
            try {
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch {}
        }
    }

    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate {

        public override UIWindow Window { get; set; }

        public SlideoutNavigationController Menu { get; private set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions) {

            try {
                //HockeyApp Logger
                /*var manager = BITHockeyManager.SharedHockeyManager;
                manager.Configure("59bab9aa6d724a2fbb99618385b21069");
                manager.StartManager();
                manager.Authenticator.AuthenticateInstallation();*/

                /*AppDomain.CurrentDomain.UnhandledException += (sender, ex) => {
                    Utils.ErrorHandling((Exception)ex.ExceptionObject);
                };*/

                Window = new UIWindow(UIScreen.MainScreen.Bounds);
                SetupAsync();
                Window.MakeKeyAndVisible(); // make the window visible
                return true;
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
                return false;
            }
        }

        private async void SetupAsync() {

            //Navigation bar configuration
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes() {
                TextColor = Settings.FIRST_COLOR_TEXT,
                Font = UIFont.BoldSystemFontOfSize(20)
            });
            UINavigationBar.Appearance.BarTintColor = Settings.FIRST_COLOR;
            UINavigationBar.Appearance.TintColor = Settings.FIRST_COLOR_TEXT;

            //temporary screen while async method doesnt return
            Window.RootViewController = new UINavigationController(new UIViewController());
            Window.BackgroundColor = UIColor.White;

            //-- TESTING PURPOSE -------------------------------
            //Window.RootViewController = new UINavigationController(new Message.MessageViewController());
            //return;
            //--------------------------------------------------

            //check for user login
            bool userloggedIn = await new PCL.Services.UserService().IsLoggedIn();
            if (userloggedIn)
                StartHome();
            else
                StartLogin();
        }

        internal void StartHome() {
            //Main Navigation Controller + Slideout configuration
            Menu = new SlideoutNavigationController();
            Menu.MainViewController = new MainNavigationController(new Home.HomeViewController(), Menu);
            Menu.MenuViewController = new MenuNavigationController(new SlideoutViewController(), Menu) {
                NavigationBarHidden = false,
                HidesBarsOnSwipe = true
            };
            Window.RootViewController = Menu;
        }

        internal void StartLogin() {
            Window.RootViewController = new UINavigationController(new User.LoginViewController());
        }

    }
    
}


