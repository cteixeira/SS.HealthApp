using Foundation;
using SS.HealthApp.iOS.Components;
using System.Collections.Generic;
using SS.HealthApp.PCL.Services;
using System.Threading.Tasks;
using UIKit;
using Plugin.Connectivity;
using ToastIOS;
using CoreGraphics;

namespace SS.HealthApp.iOS.Message {
    public class MessageViewController : BaseType.ListViewController<MessageDetailViewController> {

        public MessageViewController() : base(Utils.LocalizedString("MessageTitle")) { }

        public override void ViewDidLoad() {

            try {
                base.ViewDidLoad();

                if (!CrossConnectivity.Current.IsConnected)
                    Utils.ShowToast(Utils.LocalizedString("ErrConnLight"), ToastType.Error, 3000);

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

                        NavigationController.PushViewController(new Message.NewMessageViewController(), false);
                    });
                NavigationItem.SetRightBarButtonItem(navItem, true);
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        protected override async Task<List<Cell>> LoadList() {

            ListCell = new List<Cell>();

            try {
                List<Model.MessageModels.Message> items = await new MessageService().GetItemsAsync();

                foreach (var item in items) {
                    ListCell.Add(new Cell() {
                        ID = item.ID,
                        Title = item.Subject,
                        Detail = string.Format("{0} | {1} {2}", item.Name, item.Moment.ToShortDateString(), item.Moment.ToShortTimeString())
                    });
                }
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
            var cell = ListCell[indexPath.Row];
            NavigationController.PushViewController(new MessageDetailViewController(cell.ID, cell.Title, cell.Detail), false);
            tableView.DeselectRow(indexPath, true);
        }

    }
}