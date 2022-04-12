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
using Plugin.Connectivity;

namespace SS.HealthApp.Android.Appointment
{
    [Activity(Label = "NewsDetail", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class RateService : BaseType.BaseActivity
    {

        string appointmentId = null;
        string appointmentTitle = null;
        string appointmentDescription = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.RateService);
            base.SetupToolbar(Resources.GetString(Resource.String.appointment_feedback_rating));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                appointmentId = extras.GetString("appointmentId");
                appointmentTitle = extras.GetString("appointmentTitle");
                appointmentDescription = extras.GetString("appointmentDescription");

                this.FindViewById<TextView>(Resource.Id.tvAppontmentTitle).Text = appointmentTitle;
                this.FindViewById<TextView>(Resource.Id.tvAppontmentDescription).Text = appointmentDescription;

            }


            this.FindViewById<ImageButton>(Resource.Id.ivGood).Click += (e, s) => {
                RateServiceAsync(2);
            };

            this.FindViewById<ImageButton>(Resource.Id.ivNeutral).Click += (e, s) => {
                RateServiceAsync(1);
            };

            this.FindViewById<ImageButton>(Resource.Id.ivBad).Click += (e, s) => {
                RateServiceAsync(0);
            };

        }

        private async void RateServiceAsync(int rate)
        {

            ProgressDialog progressDialog = new ProgressDialog(this);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.info_wait));
            progressDialog.SetCancelable(false);
            progressDialog.SetCanceledOnTouchOutside(false);
            progressDialog.Show();

            PCL.Services.AttendanceService aService = new PCL.Services.AttendanceService();
            bool sucess = await aService.RateServiceAsync(appointmentId, rate);

            if (sucess)
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.info_success), ToastLength.Long).Show();
                this.Finish();
            }
            else
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.info_error), ToastLength.Long).Show();
            }

            progressDialog.Dismiss();

            
        }
    }


}