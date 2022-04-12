using System;
using SS.HealthApp.iOS.Components;
using System.Collections.Generic;
using SS.HealthApp.PCL.Services;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using Plugin.Connectivity;
using ToastIOS;
using CoreGraphics;
using System.Linq;
using QuickLook;

namespace SS.HealthApp.iOS.Appointment {
    public class AppointmentViewController : UITableViewController {

        protected const float ROW_HEIGHT = 84;
        private const string PADDING = "    "; //fix align padding in table headers
        private const string CELL_KEY_BOOKED = "iOS.Components.Cell.Booked";

        private static UINavigationController Navigation { get; set; }
        private bool OperationInProgress = false;
        private string ticketNumber = string.Empty;
        private List<Model.AppointmentModels.Appointment> items;

        protected Dictionary<string, List<Cell>> GroupedListCell { get; set; }

        public override void ViewDidAppear(bool animated) {
            base.ViewDidAppear(animated);
            Navigation = NavigationController; //HACKING: Navigation Controller looses reference after selecting option from SlideOutViewController
        }

        public override void ViewDidLoad() {

            try {
                base.ViewDidLoad();

                if (!CrossConnectivity.Current.IsConnected)
                    Utils.ShowToast(Utils.LocalizedString("ErrConnLight"), ToastType.Error, 3000);

                SetupAsync(true);

                Title = Utils.LocalizedString("AppointmentTitle");
                TableView.RowHeight = ROW_HEIGHT;
                TableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);

                NavigationItem.BackBarButtonItem = new UIBarButtonItem(Utils.LocalizedString("Back"), UIBarButtonItemStyle.Plain, null);

                //Navigation bar buttons
                UIBarButtonItem navItem = new UIBarButtonItem(
                    UIImage.FromBundle("Icons/ic_add_circle_white_36pt").Scale(new CGSize(32, 32), 0),
                    UIBarButtonItemStyle.Plain,
                    (s, a) => {

                        if (!CrossConnectivity.Current.IsConnected) {
                            Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                            return;
                        }
                        PushView(new NewStep0ViewController());
                    });
                NavigationItem.SetRightBarButtonItem(navItem, true);

                SetupRefreshControl();
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void SetupAsync(bool showToast) {
            await LoadList(showToast);
            TableView.ReloadData();
        }

        protected async Task LoadList(bool showToast) {

            if(showToast)
                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

            GroupedListCell = new Dictionary<string, List<Cell>>();

            items = await new AppointmentService().GetItemsAsync();

            var ticketItem = items.FirstOrDefault(i => i.Status == Model.Enum.AppointmentStatus.Arrived || i.Status == Model.Enum.AppointmentStatus.Called);
            if(ticketItem != null) {
                GroupedListCell["AppointmentTicketTableTitle"] = new List<Cell>();
                ticketNumber = ticketItem.TicketNumber;
            }

            if (items.Exists(i => i.Status == Model.Enum.AppointmentStatus.Booked))
                GroupedListCell["AppointmentBookedTableTitle"] = new List<Cell>();

            if (items.Exists(i => i.Status == Model.Enum.AppointmentStatus.Closed))
                GroupedListCell["AppointmentClosedTableTitle"] = new List<Cell>();


            foreach (var item in items) {
                if (item.Status == Model.Enum.AppointmentStatus.Arrived || item.Status == Model.Enum.AppointmentStatus.Called)
                    GroupedListCell["AppointmentTicketTableTitle"].Add(NewCell(item));
                else if (item.Status == Model.Enum.AppointmentStatus.Booked)
                    GroupedListCell["AppointmentBookedTableTitle"].Add(NewCell(item));
                else if (item.Status == Model.Enum.AppointmentStatus.Closed)
                    GroupedListCell["AppointmentClosedTableTitle"].Add(NewCell(item));
            }

            if (showToast) {
                if (items.Count > 0)
                    Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 1);
                else
                    Utils.ShowToast(Utils.LocalizedString("NoRecords"), ToastType.Info, 3000);
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath) {

            bool hasActionsAllowed = (bool)GroupedListCell[GroupedListCell.Keys.ElementAt(indexPath.Section)][indexPath.Row].Properties["HasActionsAllowed"];

            UITableViewCell swCell = hasActionsAllowed ? 
                tableView.DequeueReusableCell(CELL_KEY_BOOKED) : 
                tableView.DequeueReusableCell(Cell.CELL_KEY);

            try {
                if (swCell == null) {
                    swCell = hasActionsAllowed ?
                        new UITableViewCell(UITableViewCellStyle.Subtitle, CELL_KEY_BOOKED) :
                        new UITableViewCell(UITableViewCellStyle.Subtitle, Cell.CELL_KEY);
                }

                Cell option = GroupedListCell[GroupedListCell.Keys.ElementAt(indexPath.Section)][indexPath.Row];
                swCell.TextLabel.Text = option.Title;
                swCell.TextLabel.Lines = 0;
                swCell.DetailTextLabel.Text = option.Detail;
                swCell.DetailTextLabel.TextColor = UIColor.Gray;

                if (hasActionsAllowed)
                    swCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }

            return swCell;
        }
        
        public override nint NumberOfSections(UITableView tableView) {
            return GroupedListCell.Count();
        }

        public override nint RowsInSection(UITableView tableView, nint section) {
            return GroupedListCell[GroupedListCell.Keys.ElementAt((int)section)].Count();
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section) {

            UILabel lblHeader = new UILabel();

            string tableTitle = GroupedListCell.Keys.ElementAt((int)section);

            if (tableTitle == "AppointmentTicketTableTitle") {

                lblHeader.TextColor = UIColor.White;
                lblHeader.BackgroundColor = UIColor.DarkGray;

                if (string.IsNullOrEmpty(ticketNumber))
                    lblHeader.Text = PADDING + Utils.LocalizedString("AppointmentArrivedWithoutNumberTableTitle");
                else
                    lblHeader.Text = PADDING + String.Format("{0} {1}", Utils.LocalizedString("AppointmentArrivedWithNumberTableTitle"), ticketNumber);
            }
            else {
                lblHeader.TextColor = Settings.FIRST_COLOR;
                lblHeader.BackgroundColor = Settings.TABLE_HEADER_COLOR;
                lblHeader.Text = PADDING + Utils.LocalizedString(tableTitle);
            }

            return lblHeader;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section) {
            return 50;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath) {

            try {
                if (OperationInProgress) {
                    tableView.DeselectRow(indexPath, true);
                    return;
                }

                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    tableView.DeselectRow(indexPath, true);
                    return;
                }

                bool hasActionsAllowed = (bool)GroupedListCell[GroupedListCell.Keys.ElementAt(indexPath.Section)][indexPath.Row].Properties["HasActionsAllowed"];

                if (hasActionsAllowed) {

                    Model.AppointmentModels.Appointment appt = items
                        .First(a => a.ID == GroupedListCell[GroupedListCell.Keys.ElementAt(indexPath.Section)][indexPath.Row].ID);

                    UIAlertController actionSheetAlert = UIAlertController.Create(string.Empty, Utils.LocalizedString("SelectOption"), UIAlertControllerStyle.ActionSheet);

                    if (appt.AllowCheckIn) 
                        actionSheetAlert.AddAction(CheckInAction(appt.ID));

                    if (appt.AllowParkingQRCode) 
                        actionSheetAlert.AddAction(ParkingQRCodeAction(appt.ID));

                    if (appt.AllowPresenceDeclaration) 
                        actionSheetAlert.AddAction(PresenceDeclarationAction(appt.ID));

                    if (appt.AllowRateService) 
                        actionSheetAlert.AddAction(RateServiceAction(appt));

                    if (appt.AllowCancel) 
                        actionSheetAlert.AddAction(CancelAction(appt.ID));

                    if (appt.AllowAddToCalendar) 
                        actionSheetAlert.AddAction(CreateICSAction(appt));

                    actionSheetAlert.AddAction(UIAlertAction.Create(Utils.LocalizedString("Close"), UIAlertActionStyle.Cancel, null));

                    PresentViewController(actionSheetAlert, true, null); //Display the alert
                }

                tableView.DeselectRow(indexPath, true);
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private UIAlertAction CheckInAction(string apptID) {
            return UIAlertAction.Create(
                        Utils.LocalizedString("AppointmentCheckIn"),
                        UIAlertActionStyle.Default,
                        (sAction) => {

                            UIAlertController confirm = UIAlertController.Create(
                                Utils.LocalizedString("AppointmentCheckIn"),
                                Utils.LocalizedString("AreYouSure"),
                                UIAlertControllerStyle.Alert);

                            confirm.AddAction(
                                UIAlertAction.Create(
                                    Utils.LocalizedString("No"),
                                    UIAlertActionStyle.Cancel,
                                    null));

                            confirm.AddAction(
                                UIAlertAction.Create(
                                    Utils.LocalizedString("Yes"),
                                    UIAlertActionStyle.Default,
                                    (cAction) => { CheckIn(apptID); }));

                            PresentViewController(confirm, true, null);
                        });
        }

        private UIAlertAction ParkingQRCodeAction(string apptID) {

            return UIAlertAction.Create(
                        Utils.LocalizedString("AppointmentQRCode"),
                        UIAlertActionStyle.Default,
                        (sAction) => { OpenParkingQRCode(apptID); });
        }

        private UIAlertAction PresenceDeclarationAction(string apptID) {

            return UIAlertAction.Create(
                        Utils.LocalizedString("AppointmentPresenceDeclaration"),
                        UIAlertActionStyle.Default,
                        (sAction) => {

                            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                                OpenDocument(apptID);
                            else {

                                UIAlertController confirm = UIAlertController.Create(
                                    Utils.LocalizedString("DeclarationSendDocument"),
                                    Utils.LocalizedString("AreYouSure"),
                                    UIAlertControllerStyle.Alert);

                                confirm.AddAction(
                                    UIAlertAction.Create(
                                        Utils.LocalizedString("No"),
                                        UIAlertActionStyle.Cancel,
                                        null));

                                confirm.AddAction(
                                    UIAlertAction.Create(
                                        Utils.LocalizedString("Yes"),
                                        UIAlertActionStyle.Default,
                                        (cAction) => { SendDocument(apptID); }));

                                PresentViewController(confirm, true, null);
                            }
                        });
        }

        
        private UIAlertAction RateServiceAction(Model.AppointmentModels.Appointment appt) {
            
            //TODO: IMPLEMENTATION

            return UIAlertAction.Create(
                        Utils.LocalizedString("AppointmentRating"),
                        UIAlertActionStyle.Default,
                        (sAction) => {
                            PushView(new RateServiceViewController(appt));
                        });
        }

        private UIAlertAction CancelAction(string apptID) {

            return UIAlertAction.Create(
                        Utils.LocalizedString("AppointmentCancelAppointment"), 
                        UIAlertActionStyle.Destructive,
                        (sAction) => {

                            UIAlertController confirm = UIAlertController.Create(
                                Utils.LocalizedString("AppointmentCancelAppointment"),
                                Utils.LocalizedString("AreYouSure"), 
                                UIAlertControllerStyle.Alert);

                            confirm.AddAction(
                                UIAlertAction.Create(
                                    Utils.LocalizedString("No"), 
                                    UIAlertActionStyle.Cancel, 
                                    null));

                            confirm.AddAction(
                                UIAlertAction.Create(
                                    Utils.LocalizedString("Yes"), 
                                    UIAlertActionStyle.Default,
                                    (cAction) => { CancelAppointment(apptID); }));

                            PresentViewController(confirm, true, null);
                        });
        }

        private UIAlertAction CreateICSAction(Model.AppointmentModels.Appointment appt) {

            return UIAlertAction.Create(
                        Utils.LocalizedString("AppointmentDownloadICS"),
                        UIAlertActionStyle.Default,
                        (sAction) => {

                            if (OperationInProgress)
                                return;

                            try {
                                OperationInProgress = true;
                                Utils.CreateICS(appt.Description, appt.Facility, appt.Moment);
                                Utils.ShowToast(Utils.LocalizedString("AppointmentEventCreationSuccess"), ToastType.Info, 3000);
                            }
                            catch (Exception ex) {
                                Utils.ErrorHandling(ex, false);
                                InvokeOnMainThread(() => {
                                    Utils.ShowToast(ex.ToString(), ToastType.Error, 3000);
                                });
                            }
                            finally {
                                OperationInProgress = false;
                            }

                        });
        }

        private async void CheckIn(string apptID) {

            try {
                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    return;
                }

                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);
                OperationInProgress = true;

                var result = await new AttendanceService().CheckInAsync(apptID);

                if (result != null && result.Sucess) {
                    SetupAsync(false); //Refresh the list
                    Utils.ShowToast(string.Format("{0} {1}", Utils.LocalizedString("AppointmentGoTo"), result.Message), ToastType.Info, 3000);
                }
                else if (result != null && !string.IsNullOrEmpty(result.Message))
                    Utils.ShowToast(result.Message, ToastType.Error, 3000);
                else
                    Utils.ShowToast(Utils.LocalizedString("ErrCheckinAppointment"), ToastType.Error, 3000);

                OperationInProgress = false;
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void OpenDocument(string apptID) {

            try {

                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    return;
                }

                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);
                OperationInProgress = true;

                string filePath = await new DeclarationService().DownloadPresenceDeclarationByAppointmentIdAsync(apptID);
                if (filePath != null) {
                    Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 1);
                    QuickLookItem prevItem = new QuickLookItem(Utils.LocalizedString("DeclarationFileTitle"), filePath);
                    QLPreviewController prevController = new QLPreviewController();
                    prevController.DataSource = new QuickLookDataSource(prevItem);
                    PushView(prevController);
                }
                else
                    Utils.ShowToast(Utils.LocalizedString("ErrOpenDeclaration"), ToastType.Error, 3000);

                OperationInProgress = false;
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void SendDocument(string apptID) {

            try {
                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    return;
                }

                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);
                OperationInProgress = true;

                if (await new DeclarationService().SendPresenceDeclarationByAppointmentIdAsync(apptID)) {
                    Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 3000);
                }
                else
                    Utils.ShowToast(Utils.LocalizedString("ErrSendDeclaration"), ToastType.Error, 3000);

                OperationInProgress = false;
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void OpenParkingQRCode(string apptID) {

            try {

                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    return;
                }

                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);
                OperationInProgress = true;

                string filePath = await new AttendanceService().DownloadParkingQRCodeAsync(apptID);
                if (filePath != null) {
                    Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 1);
                    QuickLookItem prevItem = new QuickLookItem(Utils.LocalizedString("AppointmentQRCode"), filePath);
                    QLPreviewController prevController = new QLPreviewController();
                    prevController.DataSource = new QuickLookDataSource(prevItem);
                    PushView(prevController);
                }
                else
                    Utils.ShowToast(Utils.LocalizedString("ErrOpenQrCode"), ToastType.Error, 3000);

                OperationInProgress = false;
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void CancelAppointment(string apptID) {

            try {
                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    return;
                }

                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);
                OperationInProgress = true;

                if (await new AppointmentService().CancelAppointmentAsync(apptID)) 
                    SetupAsync(true); //Refresh the list
                else
                    Utils.ShowToast(Utils.LocalizedString("ErrCancelAppointment"), ToastType.Error, 3000);

                OperationInProgress = false;
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }
        

        private Cell NewCell(Model.AppointmentModels.Appointment item) {
            return new Cell() {
                ID = item.ID,
                Title = item.Description,
                Detail = item.Moment.ToShortDateString().Equals(DateTime.Now.Date.ToShortDateString()) ?
                            string.Format("{0} | {1}", item.Facility, item.Moment.ToShortTimeString()) : 
                            string.Format("{0} | {1} | {2}", item.Facility, item.Moment.ToShortDateString(), item.Moment.ToShortTimeString()),
                Properties = new Dictionary<string, object>() {
                            { "HasActionsAllowed", item.HasActionsAllowed } }
            };
        }

        private void SetupRefreshControl() {
            // Setup refresh control
            UIRefreshControl refreshControl = new UIRefreshControl();
            refreshControl.ValueChanged += (sender, args) => {
                refreshControl.BeginRefreshing();
                RefreshControl.TintColor = Settings.FIRST_COLOR;
                TableView.ReloadData();
                refreshControl.EndRefreshing();
            };
            refreshControl.TintColor = Settings.FIRST_COLOR;
            TableView.AddSubview(refreshControl);
            RefreshControl = refreshControl;
        }

        internal static void PushView(UIViewController controller) {
            Navigation.PushViewController(controller, false);
        }
    }
}