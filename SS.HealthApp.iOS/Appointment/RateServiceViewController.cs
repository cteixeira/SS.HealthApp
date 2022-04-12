using CoreGraphics;
using Foundation;
using Plugin.Connectivity;
using SS.HealthApp.PCL.Services;
using System;
using ToastIOS;
using UIKit;

namespace SS.HealthApp.iOS.Appointment {
    public partial class RateServiceViewController : UIViewController {

        private const float IMAGE_HEIGHT = 150;
        private const float PADDING_TOP = 10;

        UIScrollView scrollView;
        UIButton btnNeutral, btnBad, btnGood;

        Model.AppointmentModels.Appointment Appt { get; set; }

        public RateServiceViewController(Model.AppointmentModels.Appointment appt) : base(null, null) {
            Appt = appt;
        }

        public override void ViewDidLoad() {
            base.ViewDidLoad();

            //ATENTION: If created after the async call the top needs to be set with navigation bar height (crazy bug!!!)
            scrollView = new UIScrollView(new CGRect(0, 6, View.Frame.Width, View.Frame.Height));
            View.AddSubview(scrollView);

            Setup();
        }

        private void Setup() {

            Title = Utils.LocalizedString("AppointmentRating");

            float width = (float)View.Bounds.Width;
            nfloat top = PADDING_TOP;

            UILabel lblDescription = Utils.NewLabel(new CGRect(12, top, width - 24, 24), Appt.Description, UIColor.Black, UIFont.PreferredTitle3);
            lblDescription.Lines = 0;
            lblDescription.SizeToFit();
            scrollView.AddSubview(lblDescription);

            top += (lblDescription.Frame.Height);

            UILabel lblDetail = Utils.NewLabel(
                new CGRect(12, top, width - 24, 20), 
                String.Format("{0} | {1} | {2}", Appt.Facility, Appt.Moment.ToShortDateString(), Appt.Moment.ToShortTimeString()), 
                UIColor.Gray, 
                UIFont.PreferredCaption1);

            scrollView.AddSubview(lblDetail);

            top += (lblDetail.Frame.Height + (PADDING_TOP * 4));

            btnBad = NewButton(
                new CGRect(30, top, 60, 60),
                UIColor.FromRGB(255, 0, 0),
                "Icons/ic_sentiment_very_dissatisfied_white_36pt.png",
                (s, e) => { RateService(0); });

            scrollView.Add(btnBad);

            btnNeutral = NewButton(
                new CGRect(130, top, 60, 60),
                UIColor.Gray,
                "Icons/ic_sentiment_neutral_white_36pt.png",
                (s, e) => { RateService(1); });

            scrollView.Add(btnNeutral);

            btnGood = NewButton(
                new CGRect(230, top, 60, 60),
                UIColor.FromRGB(0, 128, 0),
                "Icons/ic_sentiment_very_satisfied_white_36pt.png",
                (s, e) => { RateService(2); });

            scrollView.Add(btnGood);

        }

        private UIButton NewButton(CGRect frame, UIColor tintColor, string image, EventHandler action) {
            UIButton btn = new UIButton(frame);
            btn.SetImage(UIImage.FromBundle(image).Scale(new CGSize(60, 60), 0).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            btn.ImageView.TintColor = tintColor;
            btn.ImageView.BackgroundColor = UIColor.White;
            btn.TouchUpInside += action;
            return btn;
        }
        private async void RateService(int rate) {

            try {
                if (!CrossConnectivity.Current.IsConnected) {
                    Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                    return;
                }

                Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

                if (await new AttendanceService().RateServiceAsync(Appt.ID, rate)) {
                    Utils.ShowToast(Utils.LocalizedString("AppointmentRatingSuccess"), ToastType.Info, 3000);
                    btnBad.Hidden = btnNeutral.Hidden = btnGood.Hidden = true;
                }
                else
                    Utils.ShowToast(Utils.LocalizedString("ErrRatingService"), ToastType.Error, 3000);
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

    }
}