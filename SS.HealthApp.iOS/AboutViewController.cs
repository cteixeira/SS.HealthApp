using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace SS.HealthApp.iOS {
    public partial class AboutViewController : UIViewController {

        private const float PADDING_IMAGE = 34;
        private const float PADDING_TEXT = 16;

        UIScrollView scrollView;

        public AboutViewController() : base(null, null) {}

        public override void DidReceiveMemoryWarning() {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad() {
            base.ViewDidLoad();
            Title = Utils.LocalizedString("AboutTitle");

            //ATENTION: If created after the async call the top needs to be set with navigation bar height (crazy bug!!!)
            scrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));
            View.AddSubview(scrollView);

            Setup();
        }

        private void Setup() {

            nfloat top = PADDING_IMAGE;
            nfloat width = (float)View.Bounds.Width;

            //Client logo
            top += AddImage(top, width, "Logo", Settings.COMPANY_URL);

            top += AddLabel(top, width, Settings.COMPANY_ABOUT);

            //developer info
            //top += AddLabel(top, width, Utils.LocalizedString("AboutDevelopedBy"), Settings.DEVELOPER_URL);

            scrollView.ContentSize = new CGSize(View.Frame.Width, top);
        }

        private nfloat AddImage(nfloat top, nfloat width, string bundleName, string url) {
            UIImage img = UIImage.FromBundle(bundleName);
            UIImageView imgView = new UIImageView(new CGRect((width - img.Size.Width) / 2, top, img.Size.Width, img.Size.Height)) { Image = img };
            TouchUrl(imgView, url);
            scrollView.AddSubview(imgView);
            return (imgView.Frame.Height + PADDING_IMAGE);
        }

        private nfloat AddLabel(nfloat top, nfloat width, string text, string url = null) {
            UILabel lbl = Utils.NewLabel(new CGRect(8, top, width - 16, 20), text, UIColor.Gray, UIFont.PreferredSubheadline);
            lbl.Lines = 0;
            lbl.SizeToFit();
            if (!string.IsNullOrEmpty(url)) { TouchUrl(lbl, url); }
            scrollView.AddSubview(lbl);
            return (lbl.Frame.Height + PADDING_TEXT);
        }

        private void TouchUrl(UIView view, string url) {
            UITapGestureRecognizer tap = new UITapGestureRecognizer(() => {
                UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
            });
            view.UserInteractionEnabled = true;
            view.AddGestureRecognizer(tap);
        }
    }
}