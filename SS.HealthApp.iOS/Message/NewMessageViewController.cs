using CoreGraphics;
using Plugin.Connectivity;
using System.Collections.Generic;
using ToastIOS;
using UIKit;

namespace SS.HealthApp.iOS.Message {

    public class NewMessageViewController : BaseType.DetailViewController {

        private const int PICKER_HEIGHT = 80;
        enum PickerType { Recipient, Subject }
        
        List<Model.PickerItem> Recipients;
        List<Model.PickerItem> Subjects;
        UITextField txtRecipient, txtSubject;
        UITextView txtMessage;
        UIScrollView scrollView;

        Model.PickerItem SelectedRecipient;
        Model.PickerItem SelectedSubject;
        Components.PickerList picker = new Components.PickerList();

        public NewMessageViewController() { }

        public NewMessageViewController(string ID) : base(ID) { }

        public override void ViewDidLoad() {

            try {
                base.ViewDidLoad();

                Title = Utils.LocalizedString("NewMessageTitle");

                if (!CrossConnectivity.Current.IsConnected)
                    Utils.ShowToast(Utils.LocalizedString("ErrConnLight"), ToastType.Error, 3000);

                NavigationItem.BackBarButtonItem = new UIBarButtonItem(Utils.LocalizedString("Back"), UIBarButtonItemStyle.Plain, null);

                //Navigation bar buttons
                UIBarButtonItem navItem = new UIBarButtonItem(
                    UIImage.FromBundle("Icons/ic_check_circle_white_36pt").Scale(new CGSize(32, 32), 0),
                    UIBarButtonItemStyle.Plain, (s, a) => { SendAsync(); });
                NavigationItem.SetRightBarButtonItem(navItem, true);

                //ATENTION: If created after the async call the top needs to be set with navigation bar height (crazy bug!!!)
                scrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
                View.AddSubview(scrollView);

                SetupAsync();
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void SetupAsync() {

            float top = 10;

            if (string.IsNullOrEmpty(ID)) {
                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

                var mService = new PCL.Services.MessageService();
                Subjects = await mService.GetSubjectsAsync();
                Recipients = await mService.GetRecipientsAsync();

                //-- Recipient Field ---------------------
                CreatePickerField(scrollView, ref txtRecipient, top, Utils.LocalizedString("MessageRecipient"), PickerType.Recipient, false);
                if (Recipients.Count == 1)
                    Utils.SetPickerValue(txtRecipient, (SelectedRecipient = Recipients[0]));

                //-- Subjects Field ---------------------
                CreatePickerField(scrollView, ref txtSubject, (top += PICKER_HEIGHT), Utils.LocalizedString("MessageSubject"), PickerType.Subject, false);
                if (Subjects.Count == 1)
                    Utils.SetPickerValue(txtSubject, (SelectedSubject = Subjects[0]));

                top += PICKER_HEIGHT;

                Utils.ShowToast(string.Empty, ToastType.Info, 1);
            }

            //-- Message Field ---------------------
            txtMessage = Utils.NewTextViewField(new CGRect(10, top + 10, (View.Frame.Width - 20), (View.Frame.Height - top - 80)));
            if (!string.IsNullOrEmpty(ID)) { txtMessage.BecomeFirstResponder(); }
            scrollView.AddSubview(txtMessage);

            Utils.HideKeyboardOnTapping(View, scrollView);

            //maxsize + keyboard size to edit the last textfield
            scrollView.ContentSize = new CGSize(View.Frame.Width, (top + txtMessage.Frame.Height + 300));
        }

        private void CreatePickerField(UIView view, ref UITextField txt, float top, string caption, PickerType pType, bool grouped) {

            Utils.NewPickerField(view, ref txt, top, caption,

                (sender) => { //Event BeginEditing

                    //enumeration is required because lambda expressions don't work with ref vars
                    List<Model.PickerItem> items = (pType == PickerType.Recipient ? Recipients : Subjects);

                    picker.Show(NavigationController, caption, items, grouped, (item) => {
                        //enumeration is required because lambda expressions don't work with ref vars
                        if (pType == PickerType.Recipient) 
                            Utils.SetPickerValue(txtRecipient, (SelectedRecipient = item));
                        else if (pType == PickerType.Subject) 
                            Utils.SetPickerValue(txtSubject, (SelectedSubject = item));
                        NavigationController.PopViewController(false);
                    });

                    return false;
                },

                (sender, args) => { //Event Clear
                    if (pType == PickerType.Recipient)
                        Utils.SetPickerValue(txtRecipient, (SelectedRecipient = null));
                    else if (pType == PickerType.Subject)
                        Utils.SetPickerValue(txtSubject, (SelectedRecipient = null));
                });
        }

        private async void SendAsync() {

            try {
                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    return;
                }

                bool validForm = true;

                if (string.IsNullOrEmpty(ID))
                    txtRecipient.Layer.BorderWidth = txtSubject.Layer.BorderWidth = 0;
                txtMessage.Layer.BorderWidth = 0;

                View.EndEditing(true);

                //Check required fields
                if (string.IsNullOrEmpty(ID)) {
                    validForm = Utils.CheckRequiredField(txtRecipient) && validForm;
                    validForm = Utils.CheckRequiredField(txtSubject) && validForm;
                }
                validForm = Utils.CheckRequiredField(txtMessage) && validForm;

                if (!validForm) {
                    Utils.ShowToast(Utils.LocalizedString("RequiredField"), ToastType.Error, 3000);
                    return;
                }

                //start sending but prepare the view first
                foreach (var item in NavigationItem.RightBarButtonItems)
                    item.Enabled = false;

                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

                var mService = new PCL.Services.MessageService();

                if (string.IsNullOrEmpty(ID))
                    validForm = (await mService.CreateMessageAsync(SelectedRecipient.ID, SelectedSubject.ID, txtMessage.Text) != null);
                else
                    validForm = (await mService.ReplyMessageAsync(ID, txtMessage.Text) != null);

                if (validForm) {
                    Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 1);
                    //use Slideout to erase trace to these screen
                    SlideoutViewController.PushView(new MessageViewController());
                }
                else
                    Utils.ShowToast(Utils.LocalizedString("ErrSendingMessage"), ToastType.Error, 3000);

                foreach (var item in NavigationItem.RightBarButtonItems)
                    item.Enabled = true;

            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

    }
}