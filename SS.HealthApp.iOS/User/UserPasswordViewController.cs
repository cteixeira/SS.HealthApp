using System;
using CoreGraphics;
using ToastIOS;
using UIKit;
using Plugin.Connectivity;
using SS.HealthApp.Model.UserModels;
using System.Collections.Generic;

namespace SS.HealthApp.iOS.User {
    public class UserPasswordViewController : UIViewController {

        UITextField txtOldPassword, txtNewPassword, txtConfirmPassword;

        public override void DidReceiveMemoryWarning() {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad() {

            try {
                base.ViewDidLoad();

                Title = Utils.LocalizedString("UserTitlePassword");

                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    return;
                }

                float width = (float)(View.Bounds.Width);
                float height = (float)(View.Frame.Height);

                UIScrollView scrollView = new UIScrollView(new CGRect(0, 20, width, height));

                NewPasswordField(scrollView, ref txtOldPassword, 0, Utils.LocalizedString("UserPasswordOld"));
                NewPasswordField(scrollView, ref txtNewPassword, txtOldPassword.Frame.Top + 60, Utils.LocalizedString("UserPasswordNew"));
                NewPasswordField(scrollView, ref txtConfirmPassword, txtNewPassword.Frame.Top + 60, Utils.LocalizedString("UserPasswordConfirm"));

                CreateControlButtons();

                Utils.HideKeyboardOnTapping(View, scrollView);

                scrollView.ContentSize = new CGSize(width, height);
                View.AddSubview(scrollView);
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private void NewPasswordField(UIView view, ref UITextField txt, nfloat top, string text) {
            txt = Utils.NewTextField(new CGRect(10, top, View.Bounds.Width - 20, 40), text, UIKeyboardType.NumberPad);
            txt.SecureTextEntry = true;
            view.AddSubview(txt);
        }

        private void CreateControlButtons() {

            List<UIBarButtonItem> barButtons = new List<UIBarButtonItem>();

            barButtons.Add(new UIBarButtonItem(
                UIImage.FromBundle("Icons/ic_check_circle_white_36pt").Scale(new CGSize(32, 32), 0),
                UIBarButtonItemStyle.Plain,
                (s, a) => { Save(); }));

            NavigationItem.SetRightBarButtonItems(barButtons.ToArray(), false);
        }
        
        private async void Save() {

            try {

                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    return;
                }

                bool validForm = true;

                txtOldPassword.Layer.BorderWidth = txtNewPassword.Layer.BorderWidth = txtConfirmPassword.Layer.BorderWidth = 0;
                View.EndEditing(true);

                //Check required fields
                validForm = Utils.CheckRequiredField(txtOldPassword);
                validForm = Utils.CheckRequiredField(txtNewPassword) && validForm;
                validForm = Utils.CheckRequiredField(txtConfirmPassword) && validForm;

                if (!validForm) {
                    Utils.ShowToast(Utils.LocalizedString("RequiredField"), ToastType.Error, 3000);
                    return;
                }

                if (txtNewPassword.Text != txtConfirmPassword.Text) {
                    Utils.ShowToast(Utils.LocalizedString("UserPasswordMismatch"), ToastType.Error, 3000);
                    txtNewPassword.Text = txtConfirmPassword.Text = string.Empty;
                    return;
                }

                //start Login but prepare the view first
                foreach (var item in NavigationItem.RightBarButtonItems)
                    item.Enabled = false;

                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

                PCL.Services.UserService uService = new PCL.Services.UserService();
                validForm = await uService.ChangePassword(new ChangePassword() { oldPassword = txtOldPassword.Text, newPassword = txtNewPassword.Text });

                if (validForm) {
                    Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 3000);
                    txtOldPassword.Text = txtNewPassword.Text = txtConfirmPassword.Text = string.Empty;
                }
                else
                    Utils.ShowToast(Utils.LocalizedString("ErrChangePassword"), ToastType.Error, 3000);

                foreach (var item in NavigationItem.RightBarButtonItems)
                    item.Enabled = true;
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

    }
}