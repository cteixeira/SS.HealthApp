using CoreGraphics;
using System;
using ToastIOS;
using UIKit;
using Plugin.Connectivity;

namespace SS.HealthApp.iOS.User {
    public class LoginViewController : UIViewController {

        UITextField txtUserName, txtPassword;
        UIButton btnSubmit;

        public LoginViewController() : base(null, null) {
            Title = Settings.APP_NAME;
        }

        public override void DidReceiveMemoryWarning() {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad() {

            try {
                base.ViewDidLoad();

                NavigationController.NavigationBarHidden = true;

                float width = (float)(View.Frame.Width);
                float height = (float)(View.Frame.Height);
                float ctrlWidth = width - 20;

                UIScrollView scrollView = new UIScrollView(new CGRect(0, 20, width, height));

                UIImage img = UIImage.FromBundle("Logo");
                UIImageView imgView = new UIImageView(new CGRect((width - img.Size.Width) / 2, 0, img.Size.Width, img.Size.Height));
                imgView.Image = img;
                scrollView.AddSubview(imgView);

                txtUserName = Utils.NewTextField(new CGRect(10, 70, ctrlWidth, 40), Utils.LocalizedString("LoginUserName"), Settings.LOGIN_KEYBOARD_TYPE);
                txtUserName.BecomeFirstResponder();
                scrollView.AddSubview(txtUserName);

                txtPassword = Utils.NewTextField(new CGRect(10, 120, ctrlWidth, 40), Utils.LocalizedString("LoginPassword"), UIKeyboardType.NumberPad);
                txtPassword.SecureTextEntry = true;
                scrollView.AddSubview(txtPassword);

                btnSubmit = Utils.NewButton(new CGRect(10, 170, ctrlWidth, 40), Utils.LocalizedString("LoginSubmit"));
                btnSubmit.TouchUpInside += btnSubmit_TouchUpInside;
                scrollView.AddSubview(btnSubmit);

                Utils.HideKeyboardOnTapping(View, scrollView);

                //there is no need to set content size because all the controls fit on the screen, even with the keyboard
                //scrollView.ContentSize = new CGSize(width, height);
                View.AddSubview(scrollView);
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        protected void btnSubmit_TouchUpInside(object sender, EventArgs e) {
            Login();
        }

        private async void Login() {

            try {
                bool validForm = true;

                txtUserName.Layer.BorderWidth = txtPassword.Layer.BorderWidth = 0;
                View.EndEditing(true);

                //Check required fields
                validForm = Utils.CheckRequiredField(txtUserName);
                validForm = Utils.CheckRequiredField(txtPassword) && validForm;

                if (!validForm) {
                    Utils.ShowToast(Utils.LocalizedString("RequiredField"), ToastType.Error, 3000);
                    return;
                }

                //start Login but prepare the view first
                btnSubmit.Enabled = false;
                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

                PCL.Services.UserService uService = new PCL.Services.UserService();

                if (CrossConnectivity.Current.IsConnected) {
                    validForm = await uService.LoginAsync(txtUserName.Text, txtPassword.Text);
                    if (validForm) {
                        AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
                        app.StartHome();
                    }
                    else {
                        //set error
                        Utils.ShowToast(Utils.LocalizedString("LoginInvalid"), ToastType.Error, 3000);
                        txtPassword.Text = string.Empty;
                        btnSubmit.Enabled = true;
                    }
                }
                else {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    btnSubmit.Enabled = true;
                }
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }

        }
    }
}