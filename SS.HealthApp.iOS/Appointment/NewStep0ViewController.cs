using System;
using CoreGraphics;
using UIKit;
using ToastIOS;
using System.Collections.Generic;
using Plugin.Connectivity;
using System.Linq;

namespace SS.HealthApp.iOS.Appointment {

    public class NewStep0ViewController : UIViewController {

        const int PICKER_HEIGHT = 80;
        const Model.Enum.AppointmentType INITIAL_TYPE = Model.Enum.AppointmentType.C;

        enum PickerType { Doctor, Specialty, Service, Facility, Payor }

        PCL.Services.AppointmentService aService;
        Model.AppointmentModels.AppointmentData aptData;
        List<Model.PickerItem> payors; //Payor changes

        Model.AppointmentModels.AppointmentBook aptBook;
        Components.PickerList picker = new Components.PickerList();
        string facilityID;

        UIScrollView scrollView;
        UISegmentedControl sgmAppointmentType;
        UITextField txtDoctor, txtSpecialty, txtService, txtFacility, txtPayor; //Payor changes
        UIView vwDoctor;

        public NewStep0ViewController() { }

        public NewStep0ViewController(string ID) { facilityID = ID; }

        public override void ViewDidLoad() {
            try {
                base.ViewDidLoad();

                if (!CrossConnectivity.Current.IsConnected)
                    Utils.ShowToast(Utils.LocalizedString("ErrConnLight"), ToastType.Error, 3000);

                Title = Utils.LocalizedString("AppointmentNew");

                NavigationItem.BackBarButtonItem = new UIBarButtonItem(Utils.LocalizedString("Back"), UIBarButtonItemStyle.Plain, null);

                //ATENTION: If created after the async call the top needs to be set with navigation bar height (crazy bug!!!)
                scrollView = new UIScrollView(new CGRect(0, PICKER_HEIGHT, View.Frame.Width, View.Frame.Height));
                View.AddSubview(scrollView);

                SetupAsync();
            }
            catch (Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void SetupAsync() {

            Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

            aService = new PCL.Services.AppointmentService();
            aptData = await aService.GetAllDataAsync();
            payors = await aService.GetPayorsAsync(); //Payor changes

            //The data should be initially filtered accordingly to appointment type and facility (optional)
            aptBook = new Model.AppointmentModels.AppointmentBook() {
                Type = INITIAL_TYPE,
                Facility = string.IsNullOrEmpty(facilityID) ? null : aptData.Facilities.Find(f => f.ID.Equals(facilityID))
            };
            aptData = aService.GetFilteredData(aptBook);

            float top = 0, width = (float)(View.Frame.Width), height = (float)(View.Frame.Height);

            //-- Appointment Type Field ---------------------
            CreateTypeField(PICKER_HEIGHT, width);

            //-- Doc Field ---------------------
            vwDoctor = new UIView(new CGRect(0, (top += 55), width, 70));
            CreatePickerField(vwDoctor, ref txtDoctor, 0, Utils.LocalizedString("AppointmentDoctor"), PickerType.Doctor, true);
            scrollView.AddSubview(vwDoctor);

            //-- Specialty Field ---------------------
            CreatePickerField(scrollView, ref txtSpecialty, (top += PICKER_HEIGHT), Utils.LocalizedString("AppointmentSpecialty"), PickerType.Specialty, true);

            //-- Service Field ---------------------
            CreatePickerField(scrollView, ref txtService, (top += PICKER_HEIGHT), Utils.LocalizedString("AppointmentService"), PickerType.Service, true);

            //-- Facility Field ---------------------
            CreatePickerField(scrollView, ref txtFacility, (top += PICKER_HEIGHT), Utils.LocalizedString("AppointmentFacility"), PickerType.Facility, false);
            if (aptBook.Facility != null) { Utils.SetPickerValue(txtFacility, aptBook.Facility); }

            //-- Payor changes ---------------------
            CreatePickerField(scrollView, ref txtPayor, (top += PICKER_HEIGHT), Utils.LocalizedString("AppointmentPayor"), PickerType.Payor, false);
            if (aptBook.Payor != null) { Utils.SetPickerValue(txtPayor, aptBook.Payor); }
            //-- Date and time Field | Suggests next day ---------------------
            //Utils.NewDateAndLabelField(scrollView, ref txtDate, (top += PICKER_HEIGHT), Utils.LocalizedString("AppointmentFrom"), DateTime.Now.AddDays(1));

            Utils.HideKeyboardOnTapping(View, scrollView);

            scrollView.ContentSize = new CGSize(width, (top += (PICKER_HEIGHT * 2))); 

            if (aptData.Relations.Count > 0) {
                Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 1);
                CreateControlButtons();
            }
            else
                Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);

        }

        private void CreateTypeField(float top, float width) {

            UIView containerView = new UIView(new CGRect(0, top, width, 60)) { BackgroundColor = UIColor.White };
            View.Add(containerView);

            sgmAppointmentType = new UISegmentedControl(new CGRect(10, 0, width - 20, 40)) { TintColor = Settings.BACKUP_COLOR };
            sgmAppointmentType.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.PreferredTitle3 }, UIControlState.Normal);

            foreach (var item in Enum.GetValues(typeof(Model.Enum.AppointmentType))) 
                sgmAppointmentType.InsertSegment(Utils.LocalizedString(String.Format("AppointmentType{0}", item)), (int)item, false);

            sgmAppointmentType.SelectedSegment = (int)Model.Enum.AppointmentType.C;
            sgmAppointmentType.ValueChanged += OnTypeSelection;
            containerView.Add(sgmAppointmentType);
        }

        private void OnTypeSelection(object sender, EventArgs e) {

            try {
                View.EndEditing(true);
                ClearSelection();

                //show and hide fields accordingly
                var type = (sender as UISegmentedControl).SelectedSegment;
                if (type == (int)Model.Enum.AppointmentType.C) {
                    vwDoctor.Hidden = false;
                    OffsetTop(scrollView.Subviews, PICKER_HEIGHT);
                }
                else if ((type == (int)Model.Enum.AppointmentType.E || type == (int)Model.Enum.AppointmentType.O) && vwDoctor.Hidden == false) {
                    vwDoctor.Hidden = true;
                    OffsetTop(scrollView.Subviews, -PICKER_HEIGHT);
                }
            }
            catch (Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private void ClearSelection() {
            try {
                ClearPickerItem(txtDoctor); ClearPickerItem(txtSpecialty); ClearPickerItem(txtService); ClearPickerItem(txtFacility);
                //Payor changes
                aptBook.Doctor = aptBook.Specialty = aptBook.Service = aptBook.Facility = null; //aptBook = new Model.AppointmentModels.AppointmentBook(); 
                aptBook.Type = (Model.Enum.AppointmentType)(int)sgmAppointmentType.SelectedSegment;
                aptData = aService.GetFilteredData(aptBook);
            }
            catch (Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private void ClearPickerItem(UITextField txt) {
            txt.Text = string.Empty;
            txt.Layer.BorderWidth = 0;
            txt.RightViewMode = UITextFieldViewMode.Never;
        }

        private void CreatePickerField(UIView view, ref UITextField txt, float top, string caption, PickerType pType, bool grouped) {

            Utils.NewPickerField(view, ref txt, top, caption,

                (sender) => {

                    //enumeration is required because lambda expressions don't work with ref vars
                    List<Model.PickerItem> items = null;
                    if (pType == PickerType.Doctor) items = aptData.Doctors;
                    else if (pType == PickerType.Specialty) items = aptData.Specialties;
                    else if (pType == PickerType.Service) items = aptData.Services.Cast<Model.PickerItem>().ToList();
                    else if (pType == PickerType.Facility) items = aptData.Facilities;
                    else if (pType == PickerType.Payor) items = payors; //Payor changes

                    picker.Show(NavigationController, caption, items, grouped, (item) => {

                        try {
                            //enumeration is required because lambda expressions don't work with ref vars
                            if (pType == PickerType.Doctor) Utils.SetPickerValue(txtDoctor, (aptBook.Doctor = item));
                            else if (pType == PickerType.Specialty) Utils.SetPickerValue(txtSpecialty, (aptBook.Specialty = item));
                            else if (pType == PickerType.Service) Utils.SetPickerValue(txtService, (aptBook.Service = item));
                            else if (pType == PickerType.Facility) Utils.SetPickerValue(txtFacility, (aptBook.Facility = item));
                            else if (pType == PickerType.Payor) Utils.SetPickerValue(txtPayor, (aptBook.Payor = item)); //Payor changes

                            aptData = aService.GetFilteredData(aptBook); //refreshes all options based on selected items

                            //Auto selection if relation has only one element
                            if (aptBook.Doctor == null && aptData.Doctors.Count == 1)
                                Utils.SetPickerValue(txtDoctor, (aptBook.Doctor = aptData.Doctors.First()));
                            if (aptBook.Specialty == null && aptData.Specialties.Count == 1)
                                Utils.SetPickerValue(txtSpecialty, (aptBook.Specialty = aptData.Specialties.First()));
                            if (aptBook.Service == null && aptData.Services.Count == 1)
                                Utils.SetPickerValue(txtService, (aptBook.Service = aptData.Services.First()));
                            if (aptBook.Facility == null && aptData.Facilities.Count == 1)
                                Utils.SetPickerValue(txtFacility, (aptBook.Facility = aptData.Facilities.First()));

                            NavigationController.PopViewController(false);
                        }
                        catch (Exception ex) {
                            Utils.ErrorHandling(ex, true);
                        }
                    });

                    return false;
                },
                
                (sender, args) => {
                    try {
                        //Clear the picker on btn click
                        if (pType == PickerType.Doctor) Utils.SetPickerValue(txtDoctor, (aptBook.Doctor = null));
                        else if (pType == PickerType.Specialty) Utils.SetPickerValue(txtSpecialty, (aptBook.Specialty = null));
                        else if (pType == PickerType.Service) Utils.SetPickerValue(txtService, (aptBook.Service = null));
                        else if (pType == PickerType.Facility) Utils.SetPickerValue(txtFacility, (aptBook.Facility = null));
                        else if (pType == PickerType.Payor) Utils.SetPickerValue(txtPayor, (aptBook.Payor = null)); //Payor changes
                        aptData = aService.GetFilteredData(aptBook); //refreshes all options based on selected items
                    }
                    catch (System.Exception ex) {
                        Utils.ErrorHandling(ex, true);
                    }
                });
        }

        

        private void CreateControlButtons() {

            List<UIBarButtonItem> barButtons = new List<UIBarButtonItem>();

            barButtons.Add(new UIBarButtonItem(
                UIImage.FromBundle("Icons/ic_check_circle_white_36pt").Scale(new CGSize(32, 32), 0),
                UIBarButtonItemStyle.Plain,
                (s, a) => {

                    try {
                        bool validForm = true;
                        int apptType = (int)sgmAppointmentType.SelectedSegment;

                        View.EndEditing(true);
                        txtDoctor.Layer.BorderWidth = txtSpecialty.Layer.BorderWidth = txtService.Layer.BorderWidth =
                            txtFacility.Layer.BorderWidth = txtPayor.Layer.BorderWidth = 0; //Payor changes

                        //Check required fields
                        if (apptType == 0)
                            validForm = Utils.CheckRequiredField(txtDoctor);
                        validForm = Utils.CheckRequiredField(txtSpecialty) && validForm;
                        validForm = Utils.CheckRequiredField(txtService) && validForm;
                        validForm = Utils.CheckRequiredField(txtFacility) && validForm;
                        validForm = Utils.CheckRequiredField(txtPayor) && validForm; //Payor changes

                        if (!validForm) {
                            Utils.ShowToast(Utils.LocalizedString("RequiredField"), ToastType.Error, 3000);
                            return;
                        }

                        //Payor changes
                        aptBook.Moment = DateTime.Now.Date.AddDays(1); //Convert.ToDateTime(txtDate.Text);

                        //go to next view 
                        NavigationController.PushViewController(new NewStep1ViewController(aptBook), false);
                    }
                    catch (System.Exception ex) {
                        Utils.ErrorHandling(ex, true);
                    }

                }));

            NavigationItem.SetRightBarButtonItems(barButtons.ToArray(), false);
        }
        
        private void OffsetTop(UIView[] views, float top) {
            foreach (var view in views) {
                var frame = view.Frame;
                frame.Y += top;
                view.Frame = frame;
            }
        }
    }
}