using System.Drawing;
using UIKit;
using CoreGraphics;
using Foundation;

namespace SS.HealthApp.iOS.Home {
    public partial class TicketViewController : UIViewController {

        float Top { get; set; }
        float Height { get; set; }
        float Width { get { return (float)View.Bounds.Width; } }
        private string GoTo { get; set; }
        private string TicketNumber { get; set; }

        public TicketViewController(float top, float height, string goTo, string ticketNumber) {
            Top = top;
            Height = height;
            GoTo = goTo;
            TicketNumber = ticketNumber;
        }

        public override void ViewDidLoad() {
            try {
                base.ViewDidLoad();
                View.Frame = new RectangleF(0, Top, Width, Height);
                View.BackgroundColor = Settings.TABLE_HEADER_COLOR;
                Setup();
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private void Setup() {
            
            bool hasNumber = !string.IsNullOrEmpty(TicketNumber);
            int TicketWidth = 60;
            int margin = 12;

            UILabel lblGoTo = Utils.NewLabel(
                new CGRect(12, 0, (hasNumber ? Width - TicketWidth - (margin * 2) : Width - (margin * 2)), Height), 
                string.Format("{0} {1}", Utils.LocalizedString("AppointmentGoTo"), GoTo), 
                Settings.FIRST_COLOR, 
                UIFont.PreferredTitle3);
            lblGoTo.Lines = 0;
            View.AddSubview(lblGoTo);

            if (hasNumber) {
                UILabel lblTicketLabel = Utils.NewLabel(
                    new CGRect(Width - TicketWidth - margin, 0, TicketWidth, 40),
                    Utils.LocalizedString("AppointmentTicket"),
                    Settings.SECOND_COLOR,
                    UIFont.PreferredCaption1,
                    UITextAlignment.Center);
                View.AddSubview(lblTicketLabel);

                UILabel lblTicketNumber = Utils.NewLabel(
                    new CGRect(Width - TicketWidth - margin, 0, TicketWidth, Height),
                    TicketNumber,
                    Settings.SECOND_COLOR,
                    UIFont.PreferredTitle1,
                    UITextAlignment.Center);
                View.AddSubview(lblTicketNumber);
            }

            SetTouchEvent();
        }

        private void SetTouchEvent() {
            UITapGestureRecognizer tap = new UITapGestureRecognizer(() => {
                HomeViewController.PushView(new Appointment.AppointmentViewController());
            });
            View.UserInteractionEnabled = true;
            View.AddGestureRecognizer(tap);
        }

    }
}