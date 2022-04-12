using CoreGraphics;
using UIKit;
using ToastIOS;
using Plugin.Connectivity;
using System.Collections.Generic;

namespace SS.HealthApp.iOS.User {
    public class UserDetailViewController : UIViewController {

        UITextField txtName, txtEmail, txtTaxNumber, txtMobile, txtPhone;
        UITextView txtAddress;

        public override void DidReceiveMemoryWarning() {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad() {
            base.ViewDidLoad();

            try {
                Title = Utils.LocalizedString("UserTitle");

                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    return;
                }

                //ATENTION: If created after the async call the top needs to be set with navigation bar height (crazy bug!!!)
                UIScrollView scrollView = new UIScrollView(new CGRect(0, 20, View.Frame.Width, View.Frame.Height));

                SetupAsync(scrollView);

                View.AddSubview(scrollView);
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        
        private async void SetupAsync(UIScrollView scrollView) {

            Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

            PCL.Services.UserService uService = new PCL.Services.UserService();
            Model.UserModels.PersonalData user = await uService.GetPersonalData();

            float width = (float)(View.Frame.Width);
            float height = (float)(View.Frame.Height);
            float ctrlWidth = width - 20;

            Utils.NewTextAndLabelField(scrollView, ref txtName, 0, Utils.LocalizedString("UserName"), user.Name);
            txtName.Enabled = false;

            Utils.NewTextAndLabelField(scrollView, ref txtEmail, 80, Utils.LocalizedString("UserEmail"), user.Email, UIKeyboardType.EmailAddress);

            Utils.NewTextAndLabelField(scrollView, ref txtTaxNumber, 160, Utils.LocalizedString("UserTaxNumber"), user.TaxNumber, UIKeyboardType.NumberPad);
            txtTaxNumber.Enabled = false;

            Utils.NewTextAndLabelField(scrollView, ref txtPhone, 240, Utils.LocalizedString("UserPhone"), user.PhoneNumber, UIKeyboardType.PhonePad);

            Utils.NewTextAndLabelField(scrollView, ref txtMobile, 320, Utils.LocalizedString("UserMobile"), user.Mobile, UIKeyboardType.PhonePad);

            Utils.NewTextViewAndLabelField(scrollView, ref txtAddress, 400, 100, Utils.LocalizedString("UserAddress"), user.Address);
            txtAddress.Editable = false;

            Utils.HideKeyboardOnTapping(View, scrollView);

            scrollView.ContentSize = new CGSize(width, 760); //maxsize + keyboard size to edit the last textfield

            if (!string.IsNullOrEmpty(user.Name)) {
                Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 1);
                CreateControlButtons();
            }
            else
                Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);

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

                txtName.Layer.BorderWidth = txtEmail.Layer.BorderWidth = txtTaxNumber.Layer.BorderWidth =
                    txtMobile.Layer.BorderWidth = txtPhone.Layer.BorderWidth = txtAddress.Layer.BorderWidth = 0;
                View.EndEditing(true);

                //Check required fields
                //validForm = Utils.CheckRequiredField(txtName);
                validForm = Utils.CheckRequiredField(txtEmail) && validForm;
                //validForm = Utils.CheckRequiredField(txtTaxNumber) && validForm;
                validForm = Utils.CheckRequiredField(txtMobile) && validForm;
                validForm = Utils.CheckRequiredField(txtPhone) && validForm;
                //validForm = Utils.CheckRequiredField(txtAddress) && validForm;

                if (!validForm) {
                    Utils.ShowToast(Utils.LocalizedString("RequiredField"), ToastType.Error, 3000);
                    return;
                }

                //start Login but prepare the view first
                foreach (var item in NavigationItem.RightBarButtonItems)
                    item.Enabled = false;

                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

                string error = string.Empty;
                PCL.Services.UserService uService = new PCL.Services.UserService();

                validForm = await uService.SavePersonalData(new Model.UserModels.PersonalData() {
                    Name = txtName.Text,
                    Email = txtEmail.Text,
                    Mobile = txtMobile.Text,
                    PhoneNumber = txtPhone.Text,
                    TaxNumber = txtTaxNumber.Text,
                    Address = txtAddress.Text
                });

                if (validForm)
                    Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 3000);
                else
                    Utils.ShowToast(error, ToastType.Error, 3000);

                foreach (var item in NavigationItem.RightBarButtonItems)
                    item.Enabled = true;
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }
    }
}