using System;
using CoreGraphics;
using UIKit;
using ToastIOS;
using System.Collections.Generic;
using Plugin.Connectivity;

namespace SS.HealthApp.iOS.Appointment {
    public class NewStep1ViewController : UIViewController {

        const int PICKER_HEIGHT = 80;
        enum PickerType { Date, Time, Payor }

        Model.AppointmentModels.AppointmentBook AptBook;
        PCL.Services.AppointmentService aService;
        List<Model.PickerItem> availDates;
        List<Model.PickerItem> availTimes;
        //List<Model.PickerItem> payors;  //Payor changes

        Components.PickerList picker = new Components.PickerList();

        UIScrollView scrollView;
        UITextField txtDate, txtTime; //, txtPayor;

        public NewStep1ViewController(Model.AppointmentModels.AppointmentBook aptBook) {
            AptBook = aptBook;
        }

        public override void ViewDidLoad() {
            try {
                base.ViewDidLoad();

                Title = Utils.LocalizedString("AppointmentNew");

                if (!CrossConnectivity.Current.IsConnected)
                    Utils.ShowToast(Utils.LocalizedString("ErrConnLight"), ToastType.Error, 3000);

                NavigationItem.BackBarButtonItem = new UIBarButtonItem(Utils.LocalizedString("Back"), UIBarButtonItemStyle.Plain, null);

                //ATENTION: If created after the async call the top needs to be set with navigation bar height (crazy bug!!!)
                scrollView = new UIScrollView(new CGRect(0, 20, View.Frame.Width, View.Frame.Height));
                View.AddSubview(scrollView);

                SetupAsync();
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void SetupAsync() {

            Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

            aService = new PCL.Services.AppointmentService();
            availDates = await aService.GetAvailableDatesAsync(AptBook);
            //payors = await aService.GetPayorsAsync(); //Payor changes

            float width = (float)(View.Frame.Width);
            float height = (float)(View.Frame.Height);
            float ctrlWidth = width - 20;
            float top = 10;

            //-- Date Field ---------------------
            CreatePickerField(scrollView, ref txtDate, top, Utils.LocalizedString("AppointmentDate"), PickerType.Date, false);

            //-- Time Field -> Enable time options only after choosing date
            CreatePickerField(scrollView, ref txtTime, (top += PICKER_HEIGHT), Utils.LocalizedString("AppointmentTime"), PickerType.Time, false);
            txtTime.Enabled = false;

            //-- Payor Field ---------------------
            /*CreatePickerField(scrollView, ref txtPayor, (top += PICKER_HEIGHT), Utils.LocalizedString("AppointmentPayor"), PickerType.Payor, false);
            if (payors.Count == 1) Utils.SetPickerValue(txtPayor, (AptBook.Payor = payors[0]));*/

            Utils.HideKeyboardOnTapping(View, scrollView);

            scrollView.ContentSize = new CGSize(width, height); //maxsize + keyboard size to edit the last textfield

            if (availDates.Count > 1) {
                Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 1);
                CreateControlButtons();
            }
            else
                Utils.ShowToast(Utils.LocalizedString("ErrNoAppointmentDates"), ToastType.Error, 3000);
        }

        private void CreatePickerField(UIView view, ref UITextField txt, float top, string caption, PickerType pType, bool grouped) {

            Utils.NewPickerField(view, ref txt, top, caption,

                (sender) => { //Event BeginEditing

                    //enumeration is required because lambda expressions don't work with ref vars
                    List<Model.PickerItem> items = null;
                    if (pType == PickerType.Date) items = availDates;
                    else if (pType == PickerType.Time) items = availTimes;
                    //else if (pType == PickerType.Payor) items = payors; //Payor changes

                    picker.Show(NavigationController, caption, items, grouped, async (item) => {

                        try {
                            //enumeration is required because lambda expressions don't work with ref vars
                            if (pType == PickerType.Date) {

                                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

                                Utils.SetPickerValue(txtDate, item);
                                AptBook.Moment = Convert.ToDateTime(item.ID);

                                availTimes = await aService.GetAvailableTimeAsync(AptBook);
                                txtTime.Enabled = true;
                                if (availTimes.Count == 1) {
                                    Utils.SetPickerValue(txtTime, availTimes[0]);
                                    AptBook.SlotID = availTimes[0].ID;
                                }
                                else {
                                    Utils.SetPickerValue(txtTime, null);
                                    AptBook.SlotID = string.Empty;
                                }
                                Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 1);

                            }
                            else if (pType == PickerType.Time) {
                                Utils.SetPickerValue(txtTime, item);
                                AptBook.SlotID = item.ID;
                                AptBook.Moment = AptBook.Moment.Add(Convert.ToDateTime(item.Title).TimeOfDay);
                            }
                            //Payor changes
                            /*else if (pType == PickerType.Payor) {
                                Utils.SetPickerValue(txtPayor, (AptBook.Payor = item));
                            }*/

                            NavigationController.PopViewController(false);
                        }
                        catch (Exception ex) {
                            Utils.ErrorHandling(ex, true);
                        }
                    });

                    return false;
                },

                (sender, args) => { //Event Clear
                    try {
                        if (pType == PickerType.Date) {
                            Utils.SetPickerValue(txtDate, null);
                            Utils.SetPickerValue(txtTime, null);
                            txtTime.Enabled = false;
                            AptBook.Moment = DateTime.MinValue;
                            AptBook.SlotID = string.Empty;
                        }
                        else if (pType == PickerType.Time) {
                            Utils.SetPickerValue(txtTime, null);
                            AptBook.Moment = AptBook.Moment.Date;
                            AptBook.SlotID = string.Empty;
                        }
                        //Payor changes
                        /*else if (pType == PickerType.Payor) {
                            Utils.SetPickerValue(txtPayor, (AptBook.Payor = null));
                        }*/
                    }
                    catch (Exception ex) {
                        Utils.ErrorHandling(ex, true);
                    }
                });
        }

        private void CreateControlButtons() {

            List<UIBarButtonItem> barButtons = new List<UIBarButtonItem>();

            barButtons.Add(new UIBarButtonItem(
                UIImage.FromBundle("Icons/ic_check_circle_white_36pt").Scale(new CGSize(32, 32), 0),
                UIBarButtonItemStyle.Plain,
                (s, a) => { BookAsync(); }));

            NavigationItem.SetRightBarButtonItems(barButtons.ToArray(), false);
        }

        private async void BookAsync() {

            if (!CrossConnectivity.Current.IsConnected) {
                Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                return;
            }

            bool validForm = true;

            txtDate.Layer.BorderWidth = txtTime.Layer.BorderWidth = 0; // = txtPayor.Layer.BorderWidth = 0;
            View.EndEditing(true);

            //Check required fields
            validForm = Utils.CheckRequiredField(txtDate);
            validForm = Utils.CheckRequiredField(txtTime) && validForm;
            //validForm = Utils.CheckRequiredField(txtPayor) && validForm; //Payor Changes

            if (!validForm) {
                Utils.ShowToast(Utils.LocalizedString("RequiredField"), ToastType.Error, 3000);
                return;
            }

            //Confirm all the data before making new appointment
            UIAlertController confirm = UIAlertController.Create(
                Utils.LocalizedString("AppointmentConfirmation"),
                string.Format("{0}\n{1}\n{2}\n{3}\n{4} {5}\n{6}",
                                AptBook.Doctor != null ? AptBook.Doctor.Title : string.Empty,
                                AptBook.Specialty.Title, AptBook.Service.Title,
                                AptBook.Facility.Title, AptBook.Moment.ToShortDateString(),
                                AptBook.Moment.ToShortTimeString(), AptBook.Payor.Title),
                UIAlertControllerStyle.Alert);

            confirm.AddAction(UIAlertAction.Create(
                Utils.LocalizedString("No"),
                UIAlertActionStyle.Cancel,
                null));

            confirm.AddAction(UIAlertAction.Create(
                Utils.LocalizedString("Yes"),
                UIAlertActionStyle.Default,
                async (action) => {

                    try {
                        
                        //start booking but prepare the view first
                        foreach (var item in NavigationItem.RightBarButtonItems)
                            item.Enabled = false;

                        Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

                        PCL.Services.AppointmentService aService = new PCL.Services.AppointmentService();
                        validForm = await aService.BookNewAppointmentAsync(AptBook);

                        if (validForm) {
                            Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 1);

                            try { //Create calendar reminder
                                Utils.CreateICS(AptBook.Service.Title, AptBook.Facility.Title, AptBook.Moment);
                            }
                            catch (Exception ex) {
                                Utils.ErrorHandling(ex, false);
                                InvokeOnMainThread(() => {
                                    Utils.ShowToast(ex.ToString(), ToastType.Error, 3000);
                                });
                            }

                            //use Slideout to erase trace to these screen
                            SlideoutViewController.PushView(new AppointmentViewController());
                        }
                        else 
                            Utils.ShowToast(Utils.LocalizedString("ErrCreateAppointment"), ToastType.Error, 3000);

                        foreach (var item in NavigationItem.RightBarButtonItems)
                            item.Enabled = true;
                    }
                    catch (Exception ex) {
                        Utils.ErrorHandling(ex, true);
                    }
                }));

            PresentViewController(confirm, true, null);
        }
        
    }
}