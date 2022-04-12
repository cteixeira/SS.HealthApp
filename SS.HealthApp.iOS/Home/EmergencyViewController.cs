using System.Collections.Generic;
using System.Drawing;
using UIKit;
using SS.HealthApp.PCL.Services;
using Carousels;
using System.Timers;
using System;

namespace SS.HealthApp.iOS.Home {
    public partial class EmergencyViewController : UIViewController {

        public float Top { get; set; }
        public float Height { get; set; }
        public float Width { get { return (float)View.Bounds.Width; } }
        public List<Model.HomeModels.EmergencyDelay> Items { get; set; }

        private iCarousel carousel;
        private Timer timer = new Timer();
        private const float CONTROL_HEIGHT = 90;
        private const float PAGE_CONTROL_HEIGHT = 14;

        public EmergencyViewController(float top) {
            this.Top = top;
            Height = CONTROL_HEIGHT + PAGE_CONTROL_HEIGHT;
        }

        public override void ViewDidLoad() {
            try {
                base.ViewDidLoad();
                View.Frame = new RectangleF(0, Top, Width, Height);
                View.BackgroundColor = Settings.TABLE_HEADER_COLOR;
                SetupAsync();
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }

        private async void SetupAsync() {
            Items = await new HomeService().GetEmergencyDelayAsync();

            if (Items.Count == 0)
                return;

            carousel = new iCarousel(new RectangleF(0, 0, Width, CONTROL_HEIGHT));
            carousel.DataSource = new CarouselDataSource(Items, new RectangleF(0, 0, Width, CONTROL_HEIGHT));
            carousel.Type = iCarouselType.Linear;
            View.AddSubview(carousel);

            carousel.GetValue = (sender, option, value) => {
                if (option == iCarouselOption.Wrap)
                    return 1f;
                return value; // use the defaults for everything else
            };

            UIPageControl pc = new UIPageControl(new RectangleF(0, CONTROL_HEIGHT, Width, PAGE_CONTROL_HEIGHT));
            pc.Pages = carousel.NumberOfItems;
            pc.CurrentPage = carousel.CurrentItemIndex;
            // For some reason I needed to set the back ground color for ValueChanged event to fire if you want pages to change when the UIPageControl is tapped.
            pc.BackgroundColor = UIColor.FromRGB(243, 243, 243);
            pc.PageIndicatorTintColor = Settings.BACKUP_COLOR;
            View.AddSubview(pc);

            carousel.ScrollEnd += (sender, e) => {
                pc.CurrentPage = carousel.CurrentItemIndex;
                timer.Enabled = false; //if manually changed should stop autoscroll
            };

            pc.ValueChanged += (sender, e) => {
                carousel.CurrentItemIndex = pc.CurrentPage;
                timer.Enabled = false; //if manually changed should stop autoscroll
            };

            timer = new Timer();
            timer.Interval = PCL.Settings.DelayTimer;
            timer.Elapsed += (sender, e) => {
                InvokeOnMainThread(() => {
                    nint index = carousel.CurrentItemIndex;
                    index = (index == carousel.NumberOfItems - 1 ? 0 : index + 1);
                    carousel.CurrentItemIndex = index;
                    pc.CurrentPage = index;
                    timer.Enabled = true;
                });
            };
            timer.Enabled = true;
        }

        private class CarouselDataSource : iCarouselDataSource {

            private List<Model.HomeModels.EmergencyDelay> Items;
            private RectangleF Frame;


            public CarouselDataSource(List<Model.HomeModels.EmergencyDelay> items, RectangleF frame) {
                this.Items = items;
                this.Frame = frame;
            }

            public override nint GetNumberOfItems(iCarousel carousel) {
                return Items.Count;
            }

            public override UIView GetViewForItem(iCarousel carousel, nint index, UIView view) {

                UIView dataView = null;

                try {

                    if (view == null) {
                        // create new view if no view is available for recycling
                        dataView = new UIView(Frame);

                        var delayItem = Items[(int)index];

                        dataView.AddSubview(
                            Utils.NewLabel(new RectangleF(0, 8, Frame.Width, 24),
                                    Utils.LocalizedString("EmergencyTitle"),
                                    Settings.FIRST_COLOR,
                                    UIFont.PreferredTitle2,
                                    UITextAlignment.Center));

                        dataView.AddSubview(
                            Utils.NewLabel(new RectangleF(0, 32, Frame.Width, 32),
                                    string.Format(Utils.LocalizedString("WaitTime"), delayItem.Facility),
                                    Settings.BACKUP_COLOR,
                                    UIFont.PreferredSubheadline,
                                    UITextAlignment.Center));

                        //Adult Section
                        if (delayItem.AdultDelay >= 0) {
                            UIColor adultColor = UIColor.FromRGB(11, 83, 148);
                            dataView.AddSubview(GetImage(new RectangleF((delayItem.ChildrenDelay >= 0 ? 7 : 87), 64, 26, 26), adultColor));
                            dataView.AddSubview(
                                Utils.NewLabel(new RectangleF((delayItem.ChildrenDelay >= 0 ? 33 : 113), 66, 120, 20),
                                        string.Format(Utils.LocalizedString("WaitTimeAdult"), delayItem.AdultDelay),
                                        adultColor,
                                        UIFont.PreferredSubheadline,
                                        UITextAlignment.Center));
                        }

                        //Pediatric Section
                        if (delayItem.ChildrenDelay >= 0) {
                            UIColor pedColor = UIColor.FromRGB(212, 97, 121);
                            dataView.AddSubview(GetImage(new RectangleF((delayItem.AdultDelay >= 0 ? 160 : 87), 64, 26, 26), pedColor));
                            dataView.AddSubview(
                                Utils.NewLabel(new RectangleF((delayItem.AdultDelay >= 0 ? 186 : 113), 66, 120, 20),
                                        string.Format(Utils.LocalizedString("WaitTimeChildren"), delayItem.ChildrenDelay),
                                        pedColor,
                                        UIFont.PreferredSubheadline,
                                        UITextAlignment.Center));
                        }

                    }
                    else {
                        // get a reference to the label in the recycled view
                        dataView = (UIView)view;
                    }
                }
                catch (System.Exception ex) {
                    Utils.ErrorHandling(ex, true);
                }

                return dataView;
            }
            
            private UIImageView GetImage(RectangleF frame, UIColor color) {
                return new UIImageView(frame) {
                    Image = UIImage.FromBundle("Icons/ic_access_time_white_36pt")
                            .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate),
                    TintColor = color
                };
            }

        }
    }
}