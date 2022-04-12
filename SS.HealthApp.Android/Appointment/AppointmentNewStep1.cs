using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Spinner = FR.Ganfra.Materialspinner.MaterialSpinner;
using MvvmValidation;
using SS.HealthApp.Android.Util;
using Java.Text;
using Plugin.Connectivity;

namespace SS.HealthApp.Android.Appointment
{
    [Activity(Label = "AppointmentSelectSlot", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class AppointmentNewStep1 : BaseType.BaseActivity
    {

        EditText etMoment;
        ImageView ivClearMoment;
        EditText etSlot;
        ImageView ivClearSlot;
        /*EditText etPayor;
        ImageView ivClearPayor;*/
        ScrollView svMainContent;
        LinearLayout llProgressBar;

        //public static List<Model.PickerItem> Payors = null;
        public static List<Model.PickerItem> AvailableDates = null;
        public static List<Model.PickerItem> AvailableTimes = null;

        SS.HealthApp.PCL.Services.AppointmentService aService = new PCL.Services.AppointmentService();

        #region activity lifecycle

        protected async override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AppointmentNewStep1);

            base.SetupToolbar(Resources.GetString(Resource.String.appointment_NewAppointment));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            etMoment = FindViewById<EditText>(Resource.Id.etMoment);
            etMoment.Click += EtPicker_Click;

            ivClearMoment = FindViewById<ImageView>(Resource.Id.ivClearMoment);
            ivClearMoment.Click += IvClearMoment_Click;

            etSlot = FindViewById<EditText>(Resource.Id.etSlot);
            etSlot.Click += EtPicker_Click;

            ivClearSlot = FindViewById<ImageView>(Resource.Id.ivClearSlot);
            ivClearSlot.Click += IvClearSlot_Click;

            /*etPayor = FindViewById<EditText>(Resource.Id.etPayor);
            etPayor.Click += EtPicker_Click;

            ivClearPayor = FindViewById<ImageView>(Resource.Id.ivClearPayor);
            ivClearPayor.Click += IvClearPayor_Click;*/

            svMainContent = FindViewById<ScrollView>(Resource.Id.svMainContent);
            llProgressBar = FindViewById<LinearLayout>(Resource.Id.llProgressBar);

            List<Model.PickerItem> payors = new List<Model.PickerItem>() ;
            List<Model.PickerItem> availDates = new List<Model.PickerItem>();
            List<Model.PickerItem> availtTimes = new List<Model.PickerItem>();

            LoadPickersData();

        }

        protected async override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (((global::Android.App.Result)resultCode) == Result.Ok)
            {
                Controls.ActivityDataPicker.PickerType type = (Controls.ActivityDataPicker.PickerType)System.Enum.Parse(typeof(Controls.ActivityDataPicker.PickerType), data.GetStringExtra("type"));
                string selectedTitle = data.GetStringExtra("title");

                if (type == Controls.ActivityDataPicker.PickerType.APPOINTMENT_DATE)
                {
                    etMoment.Text = selectedTitle;
                    ivClearMoment.Visibility = ViewStates.Visible;
                    etSlot.Text = String.Empty;
                    ivClearSlot.Visibility = ViewStates.Gone;


                    llProgressBar.Visibility = ViewStates.Visible;
                    svMainContent.Visibility = ViewStates.Gone;

                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        Toast.MakeText(this, Resource.String.ErrConnLight, ToastLength.Short).Show();
                    }
                    else
                    { 
                        AvailableTimes = await aService.GetAvailableTimeAsync(PCL.Services.AppointmentService.ApptBook);
                    }
                    svMainContent.Visibility = ViewStates.Visible;
                    llProgressBar.Visibility = ViewStates.Gone;

                }

                if (type == Controls.ActivityDataPicker.PickerType.APPOINTMENT_TIME)
                {
                    etSlot.Text = selectedTitle;
                    ivClearSlot.Visibility = ViewStates.Visible;
                    PCL.Services.AppointmentService.ApptBook.Moment = new DateTime(PCL.Services.AppointmentService.ApptBook.Moment.Year, PCL.Services.AppointmentService.ApptBook.Moment.Month, PCL.Services.AppointmentService.ApptBook.Moment.Day);
                    PCL.Services.AppointmentService.ApptBook.Moment = PCL.Services.AppointmentService.ApptBook.Moment.Add(Convert.ToDateTime(selectedTitle).TimeOfDay);
                }

                /*if (type == Controls.ActivityDataPicker.PickerType.APPOINTMENT_PAYOR)
                {
                    etPayor.Text = selectedTitle;
                    ivClearPayor.Visibility = ViewStates.Visible;
                }*/

                //clear validator state
                etMoment.Error = null;
                etSlot.Error = null;
                //etPayor.Error = null;

            }

        }

        #endregion

        #region toolbar methods

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.TopToolbarMenuSave, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            if (item.ItemId == Resource.Id.action_save)
            {

                if (!IsValid())
                {
                    return false;
                }

                Java.Util.Date apptBookMomentJavadate = PCL.Services.AppointmentService.ApptBook.Moment.ToJavaDate();
                string dateToDisplay = String.Format("{0} {1}", Java.Text.DateFormat.DateInstance.Format(apptBookMomentJavadate), Java.Text.DateFormat.GetTimeInstance(DateFormat.Short).Format(apptBookMomentJavadate));

                string alertMessage = PCL.Services.AppointmentService.ApptBook.Doctor != null ? PCL.Services.AppointmentService.ApptBook.Doctor.Title + "\n" : "" ;
                alertMessage = String.Concat(alertMessage, PCL.Services.AppointmentService.ApptBook.Specialty.Title);
                alertMessage = String.Concat(alertMessage, "\n", PCL.Services.AppointmentService.ApptBook.Service.Title);
                alertMessage = String.Concat(alertMessage, "\n", PCL.Services.AppointmentService.ApptBook.Facility.Title);
                alertMessage = String.Concat(alertMessage, "\n", dateToDisplay);
                alertMessage = String.Concat(alertMessage, "\n", PCL.Services.AppointmentService.ApptBook.Payor.Title);

                new global::Android.Support.V7.App.AlertDialog.Builder(this)
                   .SetTitle(Resource.String.appointment_book_alertDialog_Title)
                   .SetMessage(alertMessage)
                   .SetPositiveButton(Resource.String.yes, (a, b) => {
                       BookAppointementAsync();
                   })
                   .SetNegativeButton(Resource.String.no, (a, b) => {
                   }).Show();
            }

            return base.OnOptionsItemSelected(item);

        }

        #endregion

        #region control events

        private void EtPicker_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(Controls.ActivityDataPicker));

            EditText etSender = sender as EditText;

            if (etSender == etMoment)
            {
                intent.PutExtra("title", Resources.GetString(Resource.String.appointment_date));
                intent.PutExtra("type", Controls.ActivityDataPicker.PickerType.APPOINTMENT_DATE.ToString());
            }
            else if (etSender == etSlot)
            {
                //check if moment is valid
                if (!MomentIsValid())
                {
                    Toast message = Toast.MakeText(this, Resource.String.appointment_selectMomentRequired, ToastLength.Short);
                    message.Show();
                    return;
                }

                intent.PutExtra("title", Resources.GetString(Resource.String.appointment_time));
                intent.PutExtra("type", Controls.ActivityDataPicker.PickerType.APPOINTMENT_TIME.ToString());
            }
            /*else if(etSender == etPayor)
            {
                intent.PutExtra("title", Resources.GetString(Resource.String.appointment_payor));
                intent.PutExtra("type", Controls.ActivityDataPicker.PickerType.APPOINTMENT_PAYOR.ToString());
            }*/

            StartActivityForResult(intent, 1);
        }

        private void IvClearMoment_Click(object sender, EventArgs e)
        {
            //clear date and time slot
            etMoment.Text = "";
            (sender as ImageView).Visibility = ViewStates.Gone;

            etSlot.Text = "";
            ivClearSlot.Visibility = ViewStates.Gone;
        }

        private void IvClearSlot_Click(object sender, EventArgs e)
        {
            etSlot.Text = "";
            PCL.Services.AppointmentService.ApptBook.Moment = new DateTime(PCL.Services.AppointmentService.ApptBook.Moment.Year, PCL.Services.AppointmentService.ApptBook.Moment.Month, PCL.Services.AppointmentService.ApptBook.Moment.Day);
            (sender as ImageView).Visibility = ViewStates.Gone;
        }

        /*private void IvClearPayor_Click(object sender, EventArgs e)
        {
            etPayor.Text = "";
            (sender as ImageView).Visibility = ViewStates.Gone;
        }*/

        #endregion

        private async void LoadPickersData()
        {

            /*Task<List<Model.PickerItem>>[] results = new Task<List<Model.PickerItem>>[2];

            results[0] = aService.GetPayorsAsync();
            results[1] = aService.GetAvailableDatesAsync(PCL.Services.AppointmentService.ApptBook);

            await Task.WhenAll(results);

            Payors = results[0].Result;
            if (Payors.Count == 1)
            {
                etPayor.Text = Payors[0].Title;
                ivClearPayor.Visibility = ViewStates.Visible;
                PCL.Services.AppointmentService.ApptBook.Payor = Payors[0];
            }*/

            AvailableDates = await aService.GetAvailableDatesAsync(PCL.Services.AppointmentService.ApptBook);

            if(AvailableDates.Count() == 0)
            {
                Toast.MakeText(this, Resource.String.ErrNoAppointmentDates, ToastLength.Long).Show();
            }

            svMainContent.Visibility = ViewStates.Visible;
            llProgressBar.Visibility = ViewStates.Gone;

        }

        private async void BookAppointementAsync()
        {

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, Resource.String.ErrConnSevere, ToastLength.Short).Show();
                return;
            }

            ProgressDialog progressDialog = new ProgressDialog(this);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.info_wait));
            progressDialog.SetCancelable(false);
            progressDialog.SetCanceledOnTouchOutside(false);
            progressDialog.Show();

            PCL.Services.AppointmentService aService = new PCL.Services.AppointmentService();
            bool sucess = await aService.BookNewAppointmentAsync(PCL.Services.AppointmentService.ApptBook);
            if (sucess)
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.info_success), ToastLength.Long).Show();

                Utils.SaveEventOnDeviceCalendar(this,
                    PCL.Services.AppointmentService.ApptBook.Service.Title,
                    PCL.Services.AppointmentService.ApptBook.Facility.Title,
                    PCL.Services.AppointmentService.ApptBook.Moment, PCL.Services.AppointmentService.ApptBook.Moment.AddMinutes(30));

                SetResult(Result.Ok);
                Finish();

            }
            else
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.ErrCreateAppointment), ToastLength.Long).Show();
            }

            progressDialog.Dismiss();

        }

        private bool IsValid()
        {
            ViewModel.AppointmentSelectSlotValidatonHelper slotValidationHelper = new ViewModel.AppointmentSelectSlotValidatonHelper();

            slotValidationHelper.MomentSelected = !String.IsNullOrEmpty(etMoment.Text);
            slotValidationHelper.SlotSelected = !String.IsNullOrEmpty(etSlot.Text);
            //slotValidationHelper.Payor = etPayor.Text;

            ValidationResult valResult = slotValidationHelper.Validator.ValidateAll();

            etMoment.ShowErrorIfAny(valResult, nameof(slotValidationHelper.MomentSelected));
            etSlot.ShowErrorIfAny(valResult, nameof(slotValidationHelper.SlotSelected));
            //etPayor.ShowErrorIfAny(valResult, nameof(slotValidationHelper.Payor));

            return valResult.IsValid;

        }

        private bool MomentIsValid()
        {
            ViewModel.AppointmentSelectSlotValidatonHelper slotValidationHelper = new ViewModel.AppointmentSelectSlotValidatonHelper();

            slotValidationHelper.MomentSelected = !String.IsNullOrEmpty(etMoment.Text);
            slotValidationHelper.SlotSelected = !String.IsNullOrEmpty(etSlot.Text);

            return slotValidationHelper.Validator.Validate(nameof(ViewModel.AppointmentSelectSlotValidatonHelper.MomentSelected)).IsValid;

        }

    }
}