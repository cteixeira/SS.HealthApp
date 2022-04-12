using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Fragment = Android.Support.V4.App.Fragment;
using static Android.Views.Animations.Animation;
using Android.Views.Animations;
using SS.HealthApp.Android.ViewModel;
using SS.HealthApp.Model.HomeModels;
using Plugin.Connectivity;
using SS.HealthApp.Model.AttendanceModels;

namespace SS.HealthApp.Android.Home
{
    public class Home : BaseType.BaseFragment
    {

        //banner slider fields
        private ViewFlipper viewFlipperBanners;
        private ViewFlipper viewFlipperWaitingTime;
        private RelativeLayout panelWaitingTime;
        private LinearLayout panelNextTicket;
        private Context context;

        #region activity lifecycle methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.Home, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {

            base.OnViewCreated(view, savedInstanceState);

            SetupToolbarTitle(Resources.GetString(Resource.String.menu_Home));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this.Context, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            context = this.Context;
            viewFlipperBanners = this.Activity.FindViewById<ViewFlipper>(Resource.Id.view_flipper_banners);
            viewFlipperWaitingTime = this.Activity.FindViewById<ViewFlipper>(Resource.Id.view_flipper_waiting_time);
            panelWaitingTime = this.Activity.FindViewById<RelativeLayout>(Resource.Id.panel_waiting_time);
            panelNextTicket = this.Activity.FindViewById<LinearLayout>(Resource.Id.panel_next_ticket);

            SetupBannerSliderAsync();
            SetupNextTicketOrWaitingTimeSliderAsync();
            SetupMenuListView();
        }

        #endregion

        #region top toolbar methdos

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            //do not let the base class clear the top toolbar manu
        }

        #endregion

        #region banner slider

        private async void SetupBannerSliderAsync()
        {

            try
            {
                PCL.Services.HomeService hService = new PCL.Services.HomeService();
                List<Banner> banners = await hService.GetBannersAsync();

                foreach (Banner item in banners)
                {
                    AddImageToBannerSlider(item);
                }

                GestureDetector detector = new GestureDetector(new Util.ViewFlipper.ImageLinkViewFlipperGestureDetector(context, viewFlipperBanners));
                viewFlipperBanners.Touch += (sender, args) => {
                    detector.OnTouchEvent(args.Event);
                };

                //auto play
                viewFlipperBanners.AutoStart = true;
                viewFlipperBanners.SetFlipInterval(PCL.Settings.BannerTimer);
                viewFlipperBanners.StartFlipping();
                viewFlipperBanners.InAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.left_in);
                viewFlipperBanners.OutAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.left_out);
            }
            catch (Exception)
            {
                //sometimes throwing one exception because async pattern.
                //Ignore it
            }
        }

        private void AddImageToBannerSlider(Banner Banner)
        {
            RelativeLayout rLayout = new RelativeLayout(context);
            rLayout.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            ImageView ivBanner = new ImageView(context);
            ivBanner.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            Square.Picasso.Picasso.With(context).Load(Banner.Image).Fit().Into(ivBanner);

            ivBanner.Click += (s, e) => {
                Intent intent = new Intent();
                intent.SetAction(Intent.ActionView);
                intent.AddCategory(Intent.CategoryBrowsable);
                intent.SetData(global::Android.Net.Uri.Parse(Banner.Link));
                StartActivity(intent);
            };
            ivBanner.Clickable = false;

            rLayout.AddView(ivBanner);

            if (this.IsVisible)
                viewFlipperBanners.AddView(rLayout);
        }


        #endregion

        #region Emergency time/Next Ticket


        private async void SetupNextTicketOrWaitingTimeSliderAsync()
        {

            //check if there's a next ticket to display
            PCL.Services.AttendanceService aService = new PCL.Services.AttendanceService();
            Ticket nextTicket = await aService.GetNextTicketAsync();


            if (nextTicket != null)
            {
                SetupNextTicket(nextTicket);
            }
            else
            {
                SetupWaitingTimeSliderAsync();
            }
        }

        private void SetupNextTicket(Ticket nextTicket)
        {
            try
            {
                if (!string.IsNullOrEmpty(nextTicket.Number))
                {
                    //Next ticket has number
                    this.Activity.FindViewById<TextView>(Resource.Id.tvTicket).Text = Resources.GetString(Resource.String.Ticket).ToLower();
                    this.Activity.FindViewById<TextView>(Resource.Id.tvTicketNumber).Text = nextTicket.Number;
                }
                else
                {
                    //Next ticket does not have number
                    this.Activity.FindViewById<TextView>(Resource.Id.tvTicket).Text = "";
                    this.Activity.FindViewById<TextView>(Resource.Id.tvTicketNumber).Text = "";
                }

                this.Activity.FindViewById<TextView>(Resource.Id.tvGoTo).Text = String.Format("{0} {1}", Resources.GetString(Resource.String.home_ticket_goto), nextTicket.Local);
                panelNextTicket.Visibility = ViewStates.Visible;
                panelWaitingTime.Visibility = ViewStates.Gone;
                panelNextTicket.Clickable = true;

                panelNextTicket.Click += (s,e) => {

                    //change to appointments, the same way as if we clicked on the menu
                    int navDrawerItemPosition = (this.Activity as MainActivity).NavDrawerMenuItems.FindIndex(i => i.Title == Resources.GetString(Resource.String.menu_Appointment));

                    var drawerList = Activity.FindViewById<ListView>(Resource.Id.drawer_menu);
                    drawerList.PerformItemClick(drawerList.Adapter.GetView(navDrawerItemPosition, null, null), navDrawerItemPosition, drawerList.Adapter.GetItemId(navDrawerItemPosition));
                };

            }
            catch (Exception)
            {
                //sometimes throws one exception
                //Ignore it
            }

        }


        private async void SetupWaitingTimeSliderAsync()
        {

            try
            {

                panelWaitingTime.Visibility = ViewStates.Visible;
                panelNextTicket.Visibility = ViewStates.Gone;

                PCL.Services.HomeService hService = new PCL.Services.HomeService();
                List<EmergencyDelay> emergencyDelays = await hService.GetEmergencyDelayAsync();

                //fill with waiting times
                foreach (EmergencyDelay item in emergencyDelays)
                {
                    AddFacilityToWaitingTimeSlider(item);
                }

                GestureDetector detector = new GestureDetector(new Util.ViewFlipper.ViewFlipperSwipeGestureDetector(context, viewFlipperWaitingTime));
                viewFlipperWaitingTime.Touch += (sender, args) => {
                    detector.OnTouchEvent(args.Event);
                };

                //auto play
                viewFlipperWaitingTime.AutoStart = true;
                viewFlipperWaitingTime.SetFlipInterval(PCL.Settings.DelayTimer);
                viewFlipperWaitingTime.StartFlipping();
                viewFlipperWaitingTime.InAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.left_in);
                viewFlipperWaitingTime.OutAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.left_out);
            }
            catch (Exception)
            {
                //sometimes throws one exception because async pattern.
                //Ignore it
            }
        }

        private void AddFacilityToWaitingTimeSlider(EmergencyDelay EmergencyDelay)
        {
            View item = Activity.LayoutInflater.Inflate(Resource.Layout.EmergencyTimeItem, viewFlipperWaitingTime, false);
            item.FindViewById<TextView>(Resource.Id.tvFacilityName).Text = EmergencyDelay.Facility;

            if (EmergencyDelay.AdultDelay >= 0)
            {
                item.FindViewById<TextView>(Resource.Id.tvTimeAdult).Text = String.Format("{0} {1}", EmergencyDelay.AdultDelay.ToString(), Resources.GetString(Resource.String.emergency_time_minutes));
            }
            else
            {
                item.FindViewById<LinearLayout>(Resource.Id.panelAdult).Visibility = ViewStates.Gone;
                LinearLayout panelChildren = item.FindViewById<LinearLayout>(Resource.Id.panelChildren);
                LinearLayout.LayoutParams panelChildrenLayoutParams = panelChildren.LayoutParameters as LinearLayout.LayoutParams;
                panelChildrenLayoutParams.Weight = 1f;
            }

            if (EmergencyDelay.ChildrenDelay >= 0)
            {
                item.FindViewById<TextView>(Resource.Id.tvTimePediatric).Text = String.Format("{0} {1}", EmergencyDelay.ChildrenDelay.ToString(), Resources.GetString(Resource.String.emergency_time_minutes));

            }
            else
            {
                item.FindViewById<LinearLayout>(Resource.Id.panelChildren).Visibility = ViewStates.Gone;

                LinearLayout panelAdult = item.FindViewById<LinearLayout>(Resource.Id.panelAdult);
                LinearLayout.LayoutParams panelAdultLayoutParams = panelAdult.LayoutParameters as LinearLayout.LayoutParams;
                panelAdultLayoutParams.Weight = 1f;
            }

            if (this.IsVisible)
                viewFlipperWaitingTime.AddView(item);
        }

        #endregion

        #region menu

        private void SetupMenuListView()
        {
            ListView menuListView = this.Activity.FindViewById<ListView>(Resource.Id.lvHomeMenu);
            menuListView.Adapter = new HomeScreenMenuAdapter(this.Activity, (this.Activity as MainActivity).HomeMenuItems.ToArray());
            menuListView.ItemClick += MenuListView_Click;
            menuListView.Focusable = false;
        }

        private void MenuListView_Click(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView lv = sender as ListView;
            HomeScreenMenuAdapter lvAdapter = lv.Adapter as HomeScreenMenuAdapter;
            ViewModel.ListViewMenuItem selectedItem = lvAdapter[e.Position];
            int navDrawerItemPosition = (this.Activity as MainActivity).NavDrawerMenuItems.FindIndex(i => i.Title == selectedItem.Title);

            var drawerList = Activity.FindViewById<ListView>(Resource.Id.drawer_menu);
            drawerList.PerformItemClick(drawerList.Adapter.GetView(navDrawerItemPosition, null, null), navDrawerItemPosition, drawerList.Adapter.GetItemId(navDrawerItemPosition));

        }

        #endregion

        public class HomeScreenMenuAdapter : BaseAdapter<ListViewMenuItem>
        {
            ListViewMenuItem[] items;
            Activity context;
            public HomeScreenMenuAdapter(Activity context, ListViewMenuItem[] items) : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override ListViewMenuItem this[int position]
            {
                get { return items[position]; }
            }
            public override int Count
            {
                get { return items.Length; }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView; // re-use an existing view, if one is available
                if (view == null) // otherwise create a new one
                    view = context.LayoutInflater.Inflate(Resource.Layout.HomeMenuItem, null);
                view.FindViewById<TextView>(Resource.Id.tvTitle).Text = items[position].Title;
                view.FindViewById<TextView>(Resource.Id.tvDescription).Text = items[position].Description;
                view.FindViewById<ImageView>(Resource.Id.ivIcon).SetImageResource(items[position].Icon);
                return view;
            }

        }

    }

}