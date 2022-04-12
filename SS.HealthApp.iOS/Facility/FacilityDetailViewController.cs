using UIKit;
using SS.HealthApp.PCL.Services;
using CoreGraphics;
using System;
using System.Collections.Generic;
using Foundation;
using System.Threading.Tasks;

namespace SS.HealthApp.iOS.Facility {

    public class FacilityDetailViewController : BaseType.DetailViewController {

        private const float IMAGE_HEIGHT = 150;
        private const float PADDING_TOP = 8;
        private const float NAVIGATION_CONTROL_HEIGHT = 70;

        UIScrollView scrollView;

        public FacilityDetailViewController() { }

        public FacilityDetailViewController(string ID) : base(ID) { }

        public override void ViewDidLoad() {

            try {
                base.ViewDidLoad();

                NavigationItem.BackBarButtonItem = new UIBarButtonItem(Utils.LocalizedString("Back"), UIBarButtonItemStyle.Plain, null);

                //ATENTION: If created after the async call the top needs to be set with navigation bar height (crazy bug!!!)
                scrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
                View.AddSubview(scrollView);

                SetupAsync(ID);
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void SetupAsync(string ID) {

            Model.FacilityModels.Facility item = new FacilityService().GetItem(ID);

            //With two bar buttons, title must be hiden
            //Title = Utils.LocalizedString("FacilityTitle");

            SetupNavigationButtons(item);

            float width = (float)View.Bounds.Width;
            nfloat top = PADDING_TOP;

            UILabel lblName = Utils.NewLabel(new CGRect(6, top, width - 12, 24), item.Name, UIColor.Black, UIFont.PreferredTitle3);
            lblName.Lines = 0;
            lblName.SizeToFit();
            scrollView.AddSubview(lblName);
            top += (lblName.Frame.Height);

            UILabel lblAddress = Utils.NewLabel(new CGRect(8, top, width - 16, 20), item.Address, UIColor.Gray, UIFont.PreferredCaption1);
            scrollView.AddSubview(lblAddress);
            top += (lblAddress.Frame.Height + PADDING_TOP);

            if (!string.IsNullOrEmpty(item.Image)) {

                UIImageView img = new UIImageView(new CGRect(0, top, width, IMAGE_HEIGHT));

                top += (img.Frame.Height + PADDING_TOP);

                img.ContentMode = UIViewContentMode.ScaleAspectFit;
                PCL.Utils.DownloadManager.StartDownloadAsync(item.Image, (s) => { InvokeOnMainThread(() => { img.Image = UIImage.FromFile(s); });});

                scrollView.AddSubview(img);
            }

            //Just changed the order between label and distance info (top + NAVIGATION_CONTROL_HEIGHT + PADDING_TOP) to render the screen with more info before staling
            UILabel lblDetail = Utils.NewLabel(new CGRect(6, top + NAVIGATION_CONTROL_HEIGHT + PADDING_TOP, width - 12, 20), item.Detail, UIColor.Black, UIFont.PreferredBody);
            lblDetail.Lines = 0;
            lblDetail.SizeToFit();
            scrollView.AddSubview(lblDetail);

            scrollView.ContentSize = new CGSize(View.Frame.Width, top + NAVIGATION_CONTROL_HEIGHT + PADDING_TOP + lblDetail.Frame.Height);

            UIView toolBarView = await SetupNavigationAsync(top, item);
            scrollView.AddSubview(toolBarView);
            
            View.AddSubview(scrollView);
        }

        private void SetupNavigationButtons(Model.FacilityModels.Facility item) {

            List<UIBarButtonItem> barButtons = new List<UIBarButtonItem>();

            barButtons.Add(new UIBarButtonItem(
                UIImage.FromBundle("Icons/ic_phone_white_36pt").Scale(new CGSize(32, 32), 0),
                UIBarButtonItemStyle.Plain,
                (s, a) => {
                    NSUrl url = new NSUrl(string.Format("tel:{0}", item.Phone));
                    if (UIApplication.SharedApplication.CanOpenUrl(url))
                        UIApplication.SharedApplication.OpenUrl(url);
                    else
                        new UIAlertView("Error", "Phone is not supported on this device", null, "Ok").Show();
                }) 
            );

            barButtons.Add(new UIBarButtonItem(
                UIImage.FromBundle("Icons/ic_event_white_36pt").Scale(new CGSize(32, 32), 0),
                UIBarButtonItemStyle.Plain,
                (s, a) => {
                    NavigationController.PushViewController(new Appointment.NewStep0ViewController(item.ID), false);
                }) 
            );

            NavigationItem.SetRightBarButtonItems(barButtons.ToArray(), false);
        }

        private async Task<UIView> SetupNavigationAsync(nfloat top, Model.FacilityModels.Facility item) {

            UIColor pedColor = UIColor.FromRGB(212, 97, 121);
            UIView toolBarView = new UIView(new CGRect(0, top, View.Frame.Width, NAVIGATION_CONTROL_HEIGHT));
            toolBarView.BackgroundColor = Settings.TABLE_HEADER_COLOR;

            toolBarView.AddSubview(
                new UIImageView(new CGRect(4, 12, 36, 36)) {
                    Image = UIImage.FromBundle("Icons/ic_directions_car_white_36pt").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate),
                    TintColor = Settings.FIRST_COLOR
            });

            toolBarView.AddSubview(Utils.NewLabel(new CGRect(44, 10, 120, 24), Utils.LocalizedString("FacilityDrivingTitle"), Settings.FIRST_COLOR, UIFont.PreferredTitle2));

            toolBarView.AddSubview(
                new UIImageView(new CGRect(164, 12, 22, 22)) {
                    Image = UIImage.FromBundle("Icons/ic_access_time_white_36pt").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate),
                    TintColor = pedColor
                });

            int timeDistance = await new FacilityService().GetTimeDistanceAsync(item.Coordinates);
            string origin = await new FacilityService().GetOriginCoordinates();

            toolBarView.AddSubview(Utils.NewLabel(new CGRect(188, 12, 82, 20),
                    string.Format(Utils.LocalizedString("FacilityDrivingTime"), timeDistance > -1 ? (timeDistance / 60).ToString() : "..."), //return distance in seconds
                    pedColor, UIFont.PreferredTitle2));

            toolBarView.AddSubview(
                Utils.NewLabel(new CGRect(44, 40, 240, 16),
                (timeDistance > -1 ? Utils.LocalizedString("FacilityDrivingCalculation") : Utils.LocalizedString("ErrAccessGPS")),
                Settings.BACKUP_COLOR, UIFont.PreferredSubheadline));

            //toolBarView.AddSubview(Utils.NewLabel(new CGRect(44, 40, 240, 16), "O:" + origin, Settings.BACKUP_COLOR, UIFont.PreferredSubheadline));
            //toolBarView.AddSubview(Utils.NewLabel(new CGRect(44, 56, 240, 16),"D:" + item.Coordinates, Settings.BACKUP_COLOR, UIFont.PreferredSubheadline));

            var btnMap = NewButton(new CGRect(270, 12, 40, 40), "Icons/ic_navigate_next_white_36pt.png");
            btnMap.BackgroundColor = Settings.SECOND_COLOR;
            btnMap.ImageView.TintColor = UIColor.White;
            btnMap.TouchUpInside += (s, e) => {
                NSUrl url = new NSUrl(string.Format("http://maps.apple.com/?daddr={0}", item.Coordinates)); //Utils.CleanURL(item.Address)
                if (UIApplication.SharedApplication.CanOpenUrl(url)) 
                    UIApplication.SharedApplication.OpenUrl(url);
                else 
                    new UIAlertView("Error", "Maps is not supported on this device", null, "Ok").Show();
            };
            toolBarView.AddSubview(btnMap);

            return toolBarView;
        }

        private UIButton NewButton(CGRect frame, string image) {
            UIButton btn = new UIButton(frame);
            btn.SetImage(UIImage.FromBundle(image).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            btn.ImageView.TintColor = Settings.FIRST_COLOR;
            return btn;
        }

    }
}