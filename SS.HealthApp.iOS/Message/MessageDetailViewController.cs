using UIKit;
using SS.HealthApp.PCL.Services;
using CoreGraphics;
using System;
using Plugin.Connectivity;
using ToastIOS;

namespace SS.HealthApp.iOS.Message {

    public class MessageDetailViewController : BaseType.DetailViewController {

        private const float PADDING_TOP = 16;
        private const float PADDING_LEFT = 60;
        private const float PADDING_TEXT = 6;

        private string Subject { get; set; }
        private string Sender { get; set; }

        UIScrollView scrollView;

        public MessageDetailViewController() { }

        public MessageDetailViewController(string ID, string subject, string sender) : base(ID) {
            Subject = subject;
            Sender = sender;
        }

        public override void ViewDidLoad() {

            try {
                base.ViewDidLoad();

                Title = Utils.LocalizedString("MessageTitle");

                NavigationItem.BackBarButtonItem = new UIBarButtonItem(Utils.LocalizedString("Back"), UIBarButtonItemStyle.Plain, null);

                //Navigation bar buttons
                UIBarButtonItem navItem = new UIBarButtonItem(
                    UIImage.FromBundle("Icons/ic_reply_white_36pt").Scale(new CGSize(32, 32), 0),
                    UIBarButtonItemStyle.Plain,
                    (s, a) => {

                        if (!CrossConnectivity.Current.IsConnected) {
                            Utils.ShowToast(Utils.LocalizedString("ErrConnSevere"), ToastType.Error, 3000);
                            return;
                        }

                        NavigationController.PushViewController(new Message.NewMessageViewController(ID), false);
                    });
                NavigationItem.SetRightBarButtonItem(navItem, true);

                //ATENTION: If created after the async call the top needs to be set with navigation bar height (crazy bug!!!)
                scrollView = new UIScrollView(new CGRect(0, 0, View.Bounds.Width, View.Frame.Height));
                View.AddSubview(scrollView);

                SetupAsync();
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void SetupAsync() {

            Utils.ShowToast(Utils.LocalizedString("Waiting"), ToastType.Info);

            var headerView = BuildHeader();
            scrollView.AddSubview(headerView);

            var conversation = await new MessageService().OpenItemAsync(ID);

            float width = (float)View.Bounds.Width - (PADDING_TEXT * 2) - PADDING_LEFT;
            nfloat top = headerView.Frame.Height + PADDING_TOP + PADDING_TOP;

            foreach (var msg in conversation) {

                nfloat left = msg.Received ? PADDING_LEFT + 6 : 6;
                nfloat innerTop = PADDING_TEXT;

                //detail
                UILabel lblDetail = Utils.NewLabel(new CGRect(PADDING_TEXT, innerTop, width, 20), msg.Detail, UIColor.Black, UIFont.PreferredBody);
                lblDetail.Lines = 0;
                lblDetail.SizeToFit();

                innerTop += (lblDetail.Frame.Height + PADDING_TEXT);

                //date
                UILabel lblDate = Utils.NewLabel(new CGRect(PADDING_TEXT, innerTop, width, 20),
                string.Format("{0} {1}", msg.Moment.ToShortDateString(), msg.Moment.ToShortTimeString()),
                UIColor.Gray, UIFont.PreferredCaption1);

                innerTop += (lblDate.Frame.Height + PADDING_TEXT);

                UIView boxView = new UIView(new CGRect(left, top, width, innerTop)) { BackgroundColor = Settings.TABLE_HEADER_COLOR };
                boxView.AddSubview(lblDetail);
                boxView.AddSubview(lblDate);
                scrollView.AddSubview(boxView);

                top += (innerTop + PADDING_TOP + PADDING_TOP);
            }

            scrollView.ContentSize = new CGSize(View.Frame.Width, top);

            Utils.ShowToast(string.Empty, ToastType.Info, 1);
        }

        private UIView BuildHeader() {

            float width = (float)View.Bounds.Width - (PADDING_TEXT * 2);
            nfloat headerTop = (PADDING_TEXT * 2);

            UILabel lblSubject = Utils.NewLabel(new CGRect(PADDING_TEXT, headerTop, width, 20), Subject, Settings.FIRST_COLOR, UIFont.PreferredTitle3);
            lblSubject.Lines = 0;
            lblSubject.SizeToFit();
            headerTop += (lblSubject.Frame.Height + PADDING_TEXT);

            UILabel lblSender = Utils.NewLabel(new CGRect(PADDING_TEXT, headerTop, width, 20), Sender, UIColor.Gray, UIFont.PreferredCaption1);
            headerTop += (lblSender.Frame.Height + PADDING_TEXT);

            UIView headerView = new UIView(new CGRect(0, 0, View.Frame.Width, headerTop)) { BackgroundColor = Settings.TABLE_HEADER_COLOR };
            headerView.AddSubview(lblSubject);
            headerView.AddSubview(lblSender);

            return headerView;
        }
    }
}