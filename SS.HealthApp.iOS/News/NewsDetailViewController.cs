using UIKit;
using SS.HealthApp.PCL.Services;
using CoreGraphics;
using System;

namespace SS.HealthApp.iOS.News {

    public class NewsDetailViewController : BaseType.DetailViewController {

        private const float IMAGE_HEIGHT = 150;

        private const float PADDING_TOP = 8;

        public NewsDetailViewController() { }

        public NewsDetailViewController(string ID) : base(ID) { }

        public override void ViewDidLoad() {
            try {
                base.ViewDidLoad();
                Setup(ID);
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private void Setup(string ID) {

            Model.NewsModels.News item = new NewsService().GetItem(ID);

            Title = Utils.LocalizedString("NewsTitle");

            float width = (float)View.Bounds.Width;
            nfloat top = PADDING_TOP;

            UIScrollView scrollView = new UIScrollView(new CGRect(0, 0, width, View.Frame.Height));

            UILabel lblTopic = Utils.NewLabel(new CGRect(6, top, width - 12, 24), item.Name, UIColor.Black, UIFont.PreferredTitle3);
            lblTopic.Lines = 0;
            lblTopic.SizeToFit();
            scrollView.AddSubview(lblTopic);

            top += (lblTopic.Frame.Height);

            UILabel lblDate = Utils.NewLabel(new CGRect(8, top, width - 16, 20), item.Date.ToShortDateString(), UIColor.Gray, UIFont.PreferredCaption1);
            scrollView.AddSubview(lblDate);

            top += (lblDate.Frame.Height + PADDING_TOP + PADDING_TOP);

            if (!string.IsNullOrEmpty(item.Image)) {

                UIImageView img = new UIImageView(new CGRect(0, top, width, IMAGE_HEIGHT));

                top += (img.Frame.Height + PADDING_TOP + PADDING_TOP);

                img.ContentMode = UIViewContentMode.ScaleAspectFit;
                PCL.Utils.DownloadManager.StartDownloadAsync(item.Image, (s) => {
                    InvokeOnMainThread(() => {
                        img.Image = UIImage.FromFile(s);
                    });
                });

                scrollView.AddSubview(img);
            }

            UILabel lblDetail = Utils.NewLabel(new CGRect(6, top, width - 12, 20), item.Detail, UIColor.Black, UIFont.PreferredBody);
            lblDetail.Lines = 0;
            lblDetail.SizeToFit();
            scrollView.AddSubview(lblDetail);

            scrollView.ContentSize = new CGSize(View.Frame.Width, top + lblDetail.Frame.Height);
            View.AddSubview(scrollView);
        }
    }
}