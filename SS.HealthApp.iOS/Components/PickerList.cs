using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using System.Linq;

namespace SS.HealthApp.iOS.Components {
    public class PickerList : UITableViewController {

        protected const float ROW_HEIGHT = 70;
        private const string PADDING = "    "; //fix align padding in table headers

        private bool Grouped = true;
        protected Dictionary<string, List<Cell>> GroupedListCell { get; set; }
        private List<Model.PickerItem> Items = new List<Model.PickerItem>();
        private Action<Model.PickerItem> OnSelected;
        

        public void Show(UINavigationController navigation, string title, List<Model.PickerItem> items, bool grouped, Action<Model.PickerItem> onSelected) {
            Title = title; Items = items; OnSelected = onSelected; Grouped = grouped;
            Setup();
            navigation.PushViewController(this, false);
            TableView.SetContentOffset(new CoreGraphics.CGPoint(0,0), true); //scroll top
        }

        public override void ViewDidLoad() {
            base.ViewDidLoad();
            TableView.RowHeight = ROW_HEIGHT;
            TableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
            NavigationItem.BackBarButtonItem = new UIBarButtonItem(Utils.LocalizedString("Back"), UIBarButtonItemStyle.Plain, null);
            SetupRefreshControl();
        }

        private void Setup() {

            string[] s = { "" };

            GroupedListCell = new Dictionary<string, List<Cell>>();

            var groupedItems = Grouped ?
                Items.Select(i => i.Title.Substring(0, 1).ToUpper()).Distinct().OrderBy(i => i) :
                (IEnumerable<string>)new string[] { string.Empty };

            foreach (var item in groupedItems)
                GroupedListCell[item] = new List<Cell>();

            foreach (var item in Items) {
                if (Grouped)
                    GroupedListCell[item.Title.Substring(0, 1).ToUpper()].Add(new Cell() { ID = item.ID, Title = item.Title });
                else
                    GroupedListCell[string.Empty].Add(new Cell() { ID = item.ID, Title = item.Title });
            }

            TableView.ReloadData();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath) {

            UITableViewCell swCell = tableView.DequeueReusableCell(Cell.CELL_KEY);

            if (swCell == null)
                swCell = new UITableViewCell(UITableViewCellStyle.Subtitle, Cell.CELL_KEY);

            Cell option = GroupedListCell[GroupedListCell.Keys.ElementAt(indexPath.Section)][indexPath.Row];
            swCell.TextLabel.Text = option.Title;
            swCell.TextLabel.Lines = 0;
            swCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

            return swCell;
        }
        
        public override nint NumberOfSections(UITableView tableView) {
            return GroupedListCell.Count();
        }

        public override nint RowsInSection(UITableView tableView, nint section) {
            return GroupedListCell[GroupedListCell.Keys.ElementAt((int)section)].Count();
        }

        public override string[] SectionIndexTitles(UITableView tableView) {
            return GroupedListCell.Keys.ToArray();
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section) {
            UILabel lblHeader = new UILabel();
            if (Grouped) {
                lblHeader.TextColor = Settings.FIRST_COLOR;
                lblHeader.BackgroundColor = Settings.TABLE_HEADER_COLOR;
                lblHeader.Text = PADDING + Utils.LocalizedString(GroupedListCell.Keys.ElementAt((int)section));
            }
            return lblHeader;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section) {
            return Grouped ? 50 : 0;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath) {
            Cell cell = GroupedListCell[GroupedListCell.Keys.ElementAt(indexPath.Section)][indexPath.Row];
            var item = Items.Find(i => i.ID == cell.ID);
            OnSelected(item);
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