using SS.HealthApp.iOS.Components;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using SWTableViewCells;
using System.Threading.Tasks;
using System.Drawing;

namespace SS.HealthApp.iOS.Home {
    public partial class MenuViewController : BaseType.ListViewController<BaseType.DetailViewController> {

        internal float Top { get; set; }
        internal float Height { get { return ROW_HEIGHT * 6;  } }
        public float Width { get { return (float)View.Bounds.Width; } }

        public MenuViewController(float top) : base(null) {
            this.Top = top;
        }

        public override void ViewDidLoad() {
            base.ViewDidLoad();
            View.Frame = new RectangleF(0, Top, Width, Height);
        }

        protected override async Task<List<Cell>> LoadList() {
            ListCell = new List<Cell>();
            try {
                ListCell.Add(AppointmentOption());
                ListCell.Add(MessageOption());
                ListCell.Add(AccountOption());
                ListCell.Add(NewsOption());
                ListCell.Add(FacilityOption());
                ListCell.Add(DeclarationOption());
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
            return ListCell;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath) {
            UITableViewCell swCell = base.GetCell(tableView, indexPath);
            try {
                Cell option = ListCell[indexPath.Row];

                swCell.ImageView.Image = UIImage.FromBundle(option.Image).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                swCell.ImageView.TintColor = Settings.FIRST_COLOR;

                swCell.TextLabel.TextColor = Settings.SECOND_COLOR;
                swCell.TextLabel.Font = UIFont.PreferredTitle1;
                swCell.DetailTextLabel.TextColor = Settings.FIRST_COLOR;
                swCell.DetailTextLabel.Font = UIFont.PreferredSubheadline;

                //swCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                ((SWTableViewCell)swCell).RightUtilityButtons = ListCell[indexPath.Row].Buttons.ToArray();
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }

            return swCell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath) {
            ListCell[indexPath.Row].Buttons.Last().SendActionForControlEvents(UIControlEvent.TouchUpInside);
            if (!tableView.Editing) 
                tableView.DeselectRow(indexPath, true);
        }

        private Cell AppointmentOption() {

            NSMutableArray buttons = new NSMutableArray();
            buttons.AddUtilityButton(Settings.BACKUP_COLOR, UIImage.FromBundle("Icons/ic_add_circle_white_36pt"));
            buttons.AddUtilityButton(Settings.FIRST_COLOR, UIImage.FromBundle("Icons/ic_check_circle_white_36pt"));
            buttons.AddUtilityButton(Settings.SECOND_COLOR, UIImage.FromBundle("Icons/ic_navigate_next_white_36pt"));
            List<UIButton> btnList = NSArray.FromArray<UIButton>(buttons).ToList();

            btnList[0].TouchUpInside += (sender, ea) => {
                HomeViewController.PushView(new Appointment.NewStep0ViewController());
            };

            btnList[1].TouchUpInside += (sender, ea) => {
                HomeViewController.PushView(new Appointment.AppointmentViewController());
            };

            btnList[2].TouchUpInside += (s, a) => {
                HomeViewController.PushView(new Appointment.AppointmentViewController());
            };

            return new Cell() {
                Title = Utils.LocalizedString("AppointmentTitle"),
                Image = "Icons/ic_event_white_36pt",
                Detail = Utils.LocalizedString("AppointmentSubTitle"),
                Buttons = btnList
            };
        }

        private Cell MessageOption() {

            NSMutableArray buttons = new NSMutableArray();
            buttons.AddUtilityButton(Settings.FIRST_COLOR, UIImage.FromBundle("Icons/ic_add_circle_white_36pt"));
            buttons.AddUtilityButton(Settings.SECOND_COLOR, UIImage.FromBundle("Icons/ic_navigate_next_white_36pt"));
            List<UIButton> btnList = NSArray.FromArray<UIButton>(buttons).ToList();

            btnList[0].TouchUpInside += (sender, ea) => {
                HomeViewController.PushView(new Message.MessageViewController());
            };

            btnList[1].TouchUpInside += (sender, ea) => {
                HomeViewController.PushView(new Message.MessageViewController());
            };

            return new Cell() {
                Title = Utils.LocalizedString("MessageTitle"),
                Image = "Icons/ic_question_answer_white_36pt",
                Detail = Utils.LocalizedString("MessageSubTitle"),
                Buttons = btnList
            };
        }

        private Cell AccountOption() {

            NSMutableArray buttons = new NSMutableArray();
            buttons.AddUtilityButton(Settings.SECOND_COLOR, UIImage.FromBundle("Icons/ic_navigate_next_white_36pt"));
            List<UIButton> btnList = NSArray.FromArray<UIButton>(buttons).ToList();

            btnList[0].TouchUpInside += (sender, ea) => {
                HomeViewController.PushView(new Account.AccountViewController());
            };

            return new Cell() {
                Title = Utils.LocalizedString("AccountTitle"),
                Image = "Icons/ic_euro_symbol_white_36pt",
                Detail = Utils.LocalizedString("AccountSubTitle"),
                Buttons = btnList
            };
        }

        private Cell NewsOption() {

            NSMutableArray buttons = new NSMutableArray();
            buttons.AddUtilityButton(Settings.SECOND_COLOR, UIImage.FromBundle("Icons/ic_navigate_next_white_36pt"));
            List<UIButton> btnList = NSArray.FromArray<UIButton>(buttons).ToList();

            btnList[0].TouchUpInside += (sender, ea) => {
                HomeViewController.PushView(new News.NewsViewController());
            };

            return new Cell() {
                Title = Utils.LocalizedString("NewsTitle"),
                Image = "Icons/ic_import_contacts_white_36pt",
                Detail = Utils.LocalizedString("NewsSubTitle"),
                Buttons = btnList
            };
        }

        private Cell FacilityOption() {

            NSMutableArray buttons = new NSMutableArray();
            buttons.AddUtilityButton(Settings.SECOND_COLOR, UIImage.FromBundle("Icons/ic_navigate_next_white_36pt"));
            List<UIButton> btnList = NSArray.FromArray<UIButton>(buttons).ToList();

            btnList[0].TouchUpInside += (sender, ea) => {
                HomeViewController.PushView(new Facility.FacilityViewController());
            };

            return new Cell() {
                Title = Utils.LocalizedString("FacilityTitle"),
                Image = "Icons/ic_pin_drop_white_36pt",
                Detail = Utils.LocalizedString("FacilitySubTitle"),
                Buttons = btnList
            };
        }

        private Cell DeclarationOption() {

            NSMutableArray buttons = new NSMutableArray();
            buttons.AddUtilityButton(Settings.SECOND_COLOR, UIImage.FromBundle("Icons/ic_navigate_next_white_36pt"));
            List<UIButton> btnList = NSArray.FromArray<UIButton>(buttons).ToList();

            btnList[0].TouchUpInside += (sender, ea) => {
                HomeViewController.PushView(new Declaration.DeclarationViewController());
            };

            return new Cell() {
                Title = Utils.LocalizedString("DeclarationTitle"),
                Image = "Icons/ic_insert_drive_file_white_36pt",
                Detail = Utils.LocalizedString("DeclarationSubTitle"),
                Buttons = btnList
            };
        }
        
    }
}