using SS.HealthApp.iOS.Components;
using System.Collections.Generic;
using SS.HealthApp.PCL.Services;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using System;
using CoreGraphics;
using Plugin.Connectivity;
using ToastIOS;
using QuickLook;

namespace SS.HealthApp.iOS.Account {
    public class AccountListViewController : BaseType.ListViewController<BaseType.DetailViewController> {

        private bool OperationInProgress = false;

        public AccountListViewController() : base(Utils.LocalizedString("AccountTitle")) { }

        public override void ViewDidLoad() {
            base.ViewDidLoad();

            if (!CrossConnectivity.Current.IsConnected)
                Utils.ShowToast(Utils.LocalizedString("ErrConnLight"), ToastType.Error, 3000);
        }

        protected override async Task<List<Cell>> LoadList() {

            Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

            ListCell = new List<Cell>();

            try {
                List<Model.AccountModels.AccountStatement> items = await new AccountService().GetItemsAsync();

                decimal value = 0;

                foreach (var item in items) {

                    if (!item.Payed)
                        value += item.Value;

                    ListCell.Add(new AccountCell() {
                        ID = item.ID,
                        Title = item.Description,
                        //Title = string.Format("{0} | {1}", item.Facility, item.Date.ToShortDateString()),
                        Detail = item.Date.ToShortDateString(),
                        Value = item.Value,
                        Payed = item.Payed
                    });
                }

                //AccountViewController.PendingValue = value;

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

            AccountUITableViewCell cell = (AccountUITableViewCell)tableView.DequeueReusableCell(Components.Cell.CELL_KEY);

            try {

                if (cell == null)
                    cell = new AccountUITableViewCell(UITableViewCellStyle.Subtitle, Components.Cell.CELL_KEY, View.Frame.Width);

                AccountCell option = (AccountCell)ListCell[indexPath.Row];

                cell.LeftLabel.Text = option.Title;

                cell.RightLabel.Text = String.Format("{0}{1}", option.Value, PCL.Settings.CurrencySymbol);
                //cell.RightLabel.TextColor = option.Payed ? Settings.FIRST_COLOR : UIColor.Orange;
                cell.RightLabel.TextAlignment = UITextAlignment.Right;

                cell.DetailLabel.Text = option.Detail;
                cell.DetailLabel.Font = UIFont.PreferredCaption1;
                cell.DetailLabel.TextColor = UIColor.Gray;
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }

            return cell;
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
                        Utils.LocalizedString("AccountSendDocument"), null, UIAlertControllerStyle.Alert);
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

                string filePath = await new AccountService().DownloadDocumentAsync(id);
                if (filePath != null) {
                    Utils.ShowToast(Utils.LocalizedString("Success"), ToastType.Info, 1);
                    QuickLookItem prevItem = new QuickLookItem(Utils.LocalizedString("AccountFileTitle"), filePath);
                    QLPreviewController prevController = new QLPreviewController();
                    prevController.DataSource = new QuickLookDataSource(prevItem);
                    AccountViewController.PushView(prevController);
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

                if (await new AccountService().SendDocumentAsync(id)) {
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

        private class AccountCell : Cell {
            public decimal Value { get; set; }
            public bool Payed { get; set; }
        }

        private class AccountUITableViewCell : UITableViewCell {

            private const float FIRST_ROW_HEIGHT = 60;
            private const float SECOND_ROW_HEIGHT = 25;
            private const float RIGHT_COLUMN_WIDTH = 50;

            public UILabel LeftLabel { get; set; }
            public UILabel RightLabel { get; set; }
            public UILabel DetailLabel { get; set; }

            public AccountUITableViewCell(UITableViewCellStyle style, string reuseIdentifier, nfloat width) : base(style, reuseIdentifier){

                LeftLabel = new UILabel(new CGRect(20, 0, width - RIGHT_COLUMN_WIDTH, FIRST_ROW_HEIGHT));
                AddSubview(LeftLabel);

                RightLabel = new UILabel(new CGRect(width - RIGHT_COLUMN_WIDTH, 0, RIGHT_COLUMN_WIDTH, FIRST_ROW_HEIGHT));
                AddSubview(RightLabel);

                DetailLabel = new UILabel(new CGRect(20, FIRST_ROW_HEIGHT - SECOND_ROW_HEIGHT, width, SECOND_ROW_HEIGHT));
                AddSubview(DetailLabel);
            }

        }
    }
}