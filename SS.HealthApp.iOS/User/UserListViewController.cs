using SS.HealthApp.iOS.Components;
using System.Collections.Generic;
using SS.HealthApp.PCL.Services;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using ToastIOS;
using Plugin.Connectivity;

namespace SS.HealthApp.iOS.User {
    public class UserListViewController : BaseType.ListViewController<BaseType.DetailViewController> {

        public UserListViewController() : base(Utils.LocalizedString("UserTitle")) { }

        private enum MenuOption {
            UserDetail,
            ChangePassword,
            Notifications
        }

        public override void ViewDidLoad() {
            base.ViewDidLoad();
            TableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
        }

        protected override async Task<List<Cell>> LoadList() {

            ListCell = new List<Cell>();

            ListCell.Add(new Cell() {
                ID = MenuOption.UserDetail.ToString(),
                Title = Utils.LocalizedString("UserTitleDetail"),
                Detail = Utils.LocalizedString("UserSubTitleDetail") });

            ListCell.Add(new Cell() {
                ID = MenuOption.ChangePassword.ToString(),
                Title = Utils.LocalizedString("UserTitlePassword"),
                Detail = Utils.LocalizedString("UserSubTitlePassword") });

            /*ListCell.Add(new Cell() {
                ID = MenuOption.Notifications.ToString(),
                Name = Utils.LocalizedString("UserTitleNotification"),
                Detail = Utils.LocalizedString("UserSubTitleNotification") });*/

            return ListCell;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath) {
            UITableViewCell swCell = base.GetCell(tableView, indexPath);
            swCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            return swCell;
        }
        

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath) {

            tableView.DeselectRow(indexPath, true);

            if (!CrossConnectivity.Current.IsConnected) {
                Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                return;
            }

            if (ListCell[indexPath.Row].ID == MenuOption.UserDetail.ToString())
                UserViewController.PushView(new UserDetailViewController());
            else if (ListCell[indexPath.Row].ID == MenuOption.ChangePassword.ToString())
                UserViewController.PushView(new UserPasswordViewController());
            else if (ListCell[indexPath.Row].ID == MenuOption.Notifications.ToString())
                UserViewController.PushView(new UserNotificationViewController());
        }

    }
}