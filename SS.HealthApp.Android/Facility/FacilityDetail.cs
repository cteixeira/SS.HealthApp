using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;

using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Widget;
using Square.Picasso;
using Android.Content;
using Plugin.Connectivity;

namespace SS.HealthApp.Android.Facility
{
    [Activity(Label = "FacilityDetail", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class FacilityDetail : BaseType.BaseActivity
    {

        PCL.Services.FacilityService fService = new PCL.Services.FacilityService();
        Model.FacilityModels.Facility facility = null;

        #region contrlos

        RelativeLayout rlTravelTime;
        TextView tvFacilityTitle;
        TextView tvFacilityAddressPhone;
        ImageView ivFacilityImage;
        TextView tvFacilityDescription;
        TextView tvNavigationTime;
        TextView tvNavigatioMessage;

        #endregion

        #region activity lifecycle methods

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.FacilityDetail);

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                string selectedItemId = extras.GetString("selected_item_id");
                LoadViewDetail(selectedItemId);
                LoadNavigationTimeAsync();
            }

            base.SetupToolbar(Resources.GetString(Resource.String.menu_Facility));

        }

        #endregion

        #region toolbar methods

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.TopToolbarMenuFacility, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_call)
            {
                Intent intent = new Intent(Intent.ActionCall, global::Android.Net.Uri.Parse("tel:" + facility.Phone));
                StartActivity(intent);
            }
            else if (item.ItemId == Resource.Id.action_schedule)
            {
                var intent = new Intent(this, typeof(Appointment.AppointmentNew));
                intent.PutExtra("facilityId", facility.ID);
                StartActivity(intent);
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        private void LoadViewDetail(string selectedItemId)
        {
            facility = fService.GetItem(selectedItemId);

            tvFacilityTitle = FindViewById<TextView>(Resource.Id.tvFacilityTitle);
            tvFacilityAddressPhone = FindViewById<TextView>(Resource.Id.tvFacilityAddressPhone);
            ivFacilityImage = FindViewById<ImageView>(Resource.Id.ivFacilityImage);
            tvFacilityDescription = FindViewById<TextView>(Resource.Id.tvFacilityDescription);

            rlTravelTime = FindViewById<RelativeLayout>(Resource.Id.rlTravelTime);
            rlTravelTime.Click += RlTravelTime_Click;


            tvFacilityTitle.Text = facility.Name;
            tvFacilityAddressPhone.Text = string.Format("{0} | {1}", facility.Address, facility.Phone);
            if (!string.IsNullOrEmpty(facility.Image))
            {
                Picasso.With(this.ApplicationContext).Load(facility.Image).Fit().Into(ivFacilityImage);
            }
            else
            {
                ivFacilityImage.Visibility = ViewStates.Gone;
            }
 
            tvFacilityDescription.Text = facility.Detail;

        }

        private async void LoadNavigationTimeAsync()
        {
            tvNavigationTime = FindViewById<TextView>(Resource.Id.tvNavigationTime);
            tvNavigatioMessage = FindViewById<TextView>(Resource.Id.tvNavigatioMessage);

            int timeDistance = await fService.GetTimeDistanceAsync(facility.Coordinates);
            if (timeDistance >= 0)
            {
                tvNavigationTime.Text = String.Format(Resources.GetString(Resource.String.facility_travel_time_min), (timeDistance / 60).ToString());
                tvNavigatioMessage.Text = Resources.GetString(Resource.String.facility_travel_time_BasedInTraffic);
            }
            else
            {
                tvNavigationTime.Text = "...";
                tvNavigatioMessage.Text = Resources.GetString(Resource.String.ErrAccessGPS);
            }
            rlTravelTime.Visibility = ViewStates.Visible;
        }

        private void RlTravelTime_Click(object sender, System.EventArgs e)
        {
            //lauch directly google maps
            //string mapsAddress = String.Format("http://maps.google.com/maps?daddr={0}", facility.Address);

            //give the option to user select the that want to open to navigate
            string mapsAddress = String.Format("geo:0,0?q={0}", facility.Address);

            Intent intent = new Intent(global::Android.Content.Intent.ActionView,
               global::Android.Net.Uri.Parse(mapsAddress));

            StartActivity(intent);
        }
    }
}