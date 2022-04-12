using SS.HealthApp.iOS.Components;
using System.Collections.Generic;
using SS.HealthApp.PCL.Services;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using ToastIOS;
using Plugin.Connectivity;
using QuickLook;

namespace SS.HealthApp.iOS.Declaration {
    public class DeclarationListViewController : BaseType.ListViewController<BaseType.DetailViewController> {

        private bool OperationInProgress = false;

        public DeclarationListViewController() : base(Utils.LocalizedString("DeclarationTitle")) {
            ROW_HEIGHT = 84;
        }

        public override void ViewDidLoad() {
            base.ViewDidLoad();

            TableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);

            if (!CrossConnectivity.Current.IsConnected)
                Utils.ShowToast(Utils.LocalizedString("ErrConnLight"), ToastType.Error, 3000);
        }

        protected override async Task<List<Cell>> LoadList() {

            Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

            ListCell = new List<Cell>();

            try {
                List<Model.DeclarationModels.PresenceDeclaration> items = await new DeclarationService().GetItemsAsync();

                foreach (var item in items) {
                    ListCell.Add(new Cell() {
                        ID = item.ID,
                        Title = item.Appointment,
                        Detail = string.Format("{0} | {1} | {2}", item.Facility, item.Moment.ToShortDateString(), item.Moment.ToShortTimeString())
                    });
                }

                if (items.Count > 0)
                    Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 1);
                else
                    Utils.ShowToast(Utils.LocalizedString("NoRecords"), ToastType.Info, 3000);
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }

            return ListCell;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath) {
            UITableViewCell swCell = base.GetCell(tableView, indexPath);
            swCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            return swCell;
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

                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                    OpenDocument(ListCell[indexPath.Row].ID);
                else {
                    UIAlertController confirm = UIAlertController.Create(
                        Utils.LocalizedString("DeclarationSendDocument"), null, UIAlertControllerStyle.Alert);
                    confirm.AddAction(UIAlertAction.Create(Utils.LocalizedString("No"), UIAlertActionStyle.Cancel, null));
                    confirm.AddAction(UIAlertAction.Create(Utils.LocalizedString("Yes"), UIAlertActionStyle.Default,
                        (action) => { SendDocument(ListCell[indexPath.Row].ID); }));
                    PresentViewController(confirm, true, null);
                }

                tableView.DeselectRow(indexPath, true);
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }
        
        private async void OpenDocument(string id) {

            try {

                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    return;
                }

                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);
                OperationInProgress = true;

                string filePath = await new DeclarationService().DownloadPresenceDeclarationAsync(id);
                if (filePath != null) {
                    Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 1);

                    if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0)) {
                        QuickLookItem prevItem = new QuickLookItem(Utils.LocalizedString("DeclarationFileTitle"), filePath);
                        QLPreviewController prevController = new QLPreviewController();
                        prevController.DataSource = new QuickLookDataSource(prevItem);
                        DeclarationViewController.PushView(prevController);
                    }
                }
                else
                    Utils.ShowToast(Utils.LocalizedString("ErrOpenDeclaration"), ToastType.Error, 3000);

                OperationInProgress = false;
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void SendDocument(string id) {

            try {
                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    return;
                }

                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);
                OperationInProgress = true;

                if (await new DeclarationService().SendPresenceDeclarationAsync(id)) {
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

    }
}