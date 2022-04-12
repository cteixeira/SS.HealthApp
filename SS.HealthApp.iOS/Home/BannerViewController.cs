using System;
using UIKit;
using Carousels;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;
using System.Timers;
using SS.HealthApp.PCL.Services;

namespace SS.HealthApp.iOS.Home {
    public class BannerViewController : UIViewController {
        
        public float Top { get; set; }
        public float Height { get; set; }
        public float Width { get { return (float)View.Bounds.Width; } }
        public List<Model.HomeModels.Banner> Items { get; set; }

        private iCarousel carousel;
        private Timer timer = new Timer();
        private const float IMAGE_HEIGHT = 150;
        private const float PAGE_CONTROL_HEIGHT = 20;

        public BannerViewController(float top) {
            Top = top;
            Height = IMAGE_HEIGHT + PAGE_CONTROL_HEIGHT;
        }
        
        public override void ViewDidLoad() {
            try {
                base.ViewDidLoad();
                View.Frame = new CGRect(0, Top, Width, Height);
                SetupAsync();
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }
        }


        private async void SetupAsync() {

            Items = await new HomeService().GetBannersAsync();

            if (Items.Count == 0)
                return;

            // create the carousel
            carousel = new iCarousel(new CGRect(0, 0, Width, IMAGE_HEIGHT));
            carousel.DataSource = new CarouselDataSource(Items, Width);
            carousel.Type = iCarouselType.Linear;
            View.AddSubview(carousel);

            UIPageControl pc = new UIPageControl(new CGRect(0, IMAGE_HEIGHT, Width, PAGE_CONTROL_HEIGHT));
            pc.Pages = carousel.NumberOfItems;
            pc.CurrentPage = carousel.CurrentItemIndex;
            // For some reason I needed to set the back ground color for ValueChanged event to fire if you want pages to change when the UIPageControl is tapped.
            pc.BackgroundColor = Settings.BACKUP_COLOR;
            View.AddSubview(pc);

            carousel.GetValue = (sender, option, value) => {
                if (option == iCarouselOption.Wrap)
                    return 1f;
                return value; // use the defaults for everything else
            };

            carousel.ScrollEnd += (sender, e) => {
                pc.CurrentPage = carousel.CurrentItemIndex;
                timer.Enabled = false; //if manually changed should stop autoscroll
            };

            pc.ValueChanged += (sender, e) => {
                carousel.CurrentItemIndex = pc.CurrentPage;
                timer.Enabled = false; //if manually changed should stop autoscroll
            };

            // handle item selections
            carousel.ItemSelected += (sender, args) => {
                timer.Enabled = false; //if manually changed should stop autoscroll
                UIApplication.SharedApplication.OpenUrl(new NSUrl(Items[(int)args.Index].Link));
            };

            timer = new Timer();
            timer.Interval = PCL.Settings.BannerTimer;
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

            private List<Model.HomeModels.Banner> Banners;
            private List<UIImageView> Images;

            public CarouselDataSource(List<Model.HomeModels.Banner> banners, float width) {
                Banners = banners;
                Images = new List<UIImageView>();

                try {
                    foreach (var banner in banners) {
                        var imageView = new UIImageView(new CGRect(0, 0, width, IMAGE_HEIGHT));
                        imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                        Images.Add(imageView);

                        PCL.Utils.DownloadManager.StartDownloadAsync(
                            banner.Image,
                            (s) => {
                                InvokeOnMainThread(() => {
                                    imageView.Image = UIImage.FromFile(s);
                                });
                            });
                    }
                }
                catch (System.Exception ex) {
                    Utils.ErrorHandling(ex, true);
                }

            }
            
            public override nint GetNumberOfItems(iCarousel carousel) {
                return Banners.Count;
            }

            public override UIView GetViewForItem(iCarousel carousel, nint index, UIView view) {
                return Images[(int)index];
            }
        }

    }
}