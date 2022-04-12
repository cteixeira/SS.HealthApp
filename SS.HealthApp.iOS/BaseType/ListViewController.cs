using System;
using SS.HealthApp.iOS.Components;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using SWTableViewCells;
using System.Threading.Tasks;

namespace SS.HealthApp.iOS.BaseType {

    public abstract partial class ListViewController<D> : UITableViewController where D : DetailViewController, new() {

        protected float ROW_HEIGHT = 70;

        protected List<Cell> ListCell { get; set; }
        private CellDelegate cellDelegate;

        public ListViewController(string title) : base(null, null) {
            this.Title = title;
        }

        public override void ViewDidLoad() {

            try {
                base.ViewDidLoad();
                SetupAsync();

                TableView.RowHeight = ROW_HEIGHT;
                //TableView.EstimatedRowHeight = 10;
                //TableView.RowHeight = UITableView.AutomaticDimension;

                TableView.ContentInset = new UIEdgeInsets(0, -10, 0, 0);

                NavigationItem.BackBarButtonItem = new UIBarButtonItem(Utils.LocalizedString("Back"), UIBarButtonItemStyle.Plain, null);

                SetupRefreshControl();
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void SetupAsync() {
            try {
                List<Cell> list = await LoadList();
                cellDelegate = new CellDelegate(list, TableView);
                TableView.ReloadData();
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        //Async Method to get cell list
        protected abstract Task<List<Cell>> LoadList();

        public override nint RowsInSection(UITableView tableView, nint section) {
            return ListCell.Count();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath) {

            SWTableViewCell swCell = (SWTableViewCell)tableView.DequeueReusableCell(Components.Cell.CELL_KEY);
            
            try {
                if (swCell == null)
                    swCell = new SWTableViewCell(UITableViewCellStyle.Subtitle, Components.Cell.CELL_KEY);

                swCell.Delegate = cellDelegate;
                Cell option = ListCell[indexPath.Row];
                swCell.TextLabel.Text = option.Title;
                swCell.TextLabel.Lines = 0;
                swCell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
                swCell.DetailTextLabel.Text = option.Detail;
                swCell.DetailTextLabel.TextColor = UIColor.Gray;
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }

            //REMARK: If cell has image should be load in overriden method using local or web image (async)

            return swCell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath) {

            NavigationController.PushViewController(new D() { ID = ListCell[indexPath.Row].ID }, false);

            if (!tableView.Editing)
                tableView.DeselectRow(indexPath, true);
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

    }
}