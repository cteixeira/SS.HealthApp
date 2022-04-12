using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmValidation;
using Plugin.Connectivity;
using SS.HealthApp.Android.Util;
using System;
using System.Collections.Generic;

namespace SS.HealthApp.Android.Appointment
{
    public class AppointmentNewStep0 : BaseType.BaseFragment
    {

        const int START_DATA_PICKER_REQUEST = 1;
        const int START_NEW_APPOINTMENT_STEP2 = 2;

        public Model.Enum.AppointmentType TypeOfAppointment;

        EditText etDoctor;
        ImageView ivClearDoctor;
        EditText etSpeciality;
        ImageView ivClearSpeciality;
        EditText etService;
        ImageView ivClearService;
        EditText etFacility;
        ImageView ivClearFacility;
        EditText etPayor;
        ImageView ivClearPayor;
        //EditText etDate;

        public static List<Model.PickerItem> Payors = null;

        SS.HealthApp.PCL.Services.AppointmentService aService = new PCL.Services.AppointmentService();

        public AppointmentNewStep0(Model.Enum.AppointmentType TypeOfAppointment)
        {
            this.TypeOfAppointment = TypeOfAppointment;
        }

        #region activity lifecycle

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this.Context, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            View view = inflater.Inflate(Resource.Layout.AppointmentNewStep0, container, false);

            etDoctor = view.FindViewById<EditText>(Resource.Id.etDoctor);
            etDoctor.Click += EtPicker_Click;
            if (TypeOfAppointment == Model.Enum.AppointmentType.C)
            {
                ((RelativeLayout)etDoctor.Parent.Parent).Visibility = ViewStates.Visible;
            }
            else
            {
                ((RelativeLayout)etDoctor.Parent.Parent).Visibility = ViewStates.Gone;
            }

            ivClearDoctor = view.FindViewById<ImageView>(Resource.Id.ivClearDoctor);
            ivClearDoctor.Click += IvClearDoctor_Click;

            etSpeciality = view.FindViewById<EditText>(Resource.Id.etSpeciality);
            etSpeciality.Click += EtPicker_Click;

            ivClearSpeciality = view.FindViewById<ImageView>(Resource.Id.ivClearSpeciality);
            ivClearSpeciality.Click += IvClearSpeciality_Click;

            etService = view.FindViewById<EditText>(Resource.Id.etService);
            etService.Click += EtPicker_Click;

            ivClearService = view.FindViewById<ImageView>(Resource.Id.ivClearService);
            ivClearService.Click += IvClearService_Click;

            etFacility = view.FindViewById<EditText>(Resource.Id.etFacility);
            etFacility.Click += EtPicker_Click;

            ivClearFacility = view.FindViewById<ImageView>(Resource.Id.ivClearFacility);
            ivClearFacility.Click += IvClearFacility_Click;

            etPayor = view.FindViewById<EditText>(Resource.Id.etPayor);
            etPayor.Click += EtPicker_Click;

            ivClearPayor = view.FindViewById<ImageView>(Resource.Id.ivClearPayor);
            ivClearPayor.Click += IvClearPayor_Click;

            /*etDate = view.FindViewById<EditText>(Resource.Id.etDate);
            etDate.Text = PCL.Services.AppointmentService.ApptBook.Moment.ToLongDateString();
            etDate.Click += EtDate_Click;*/

            LoadPickersData();
            RefreshFormData();

            return view;

        }

        private async void LoadPickersData()
        {
            Payors = await aService.GetPayorsAsync();

            if (Payors.Count == 1)
            {
                etPayor.Text = Payors[0].Title;
                ivClearPayor.Visibility = ViewStates.Visible;
                PCL.Services.AppointmentService.ApptBook.Payor = Payors[0];
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (((global::Android.App.Result)resultCode) == Result.Ok)
            {
                if (requestCode == START_DATA_PICKER_REQUEST)
                {
                    RefreshFormData();
                }
                else if (requestCode == START_NEW_APPOINTMENT_STEP2)
                {
                    //appointment book completed, return to appointment list
                    this.Activity.Finish();
                }
                
            }

        }

        #endregion

        #region control events

        private void IvClearDoctor_Click(object sender, EventArgs e)
        {
            PCL.Services.AppointmentService.ApptBook.Doctor = null;
            etDoctor.Text = "";
            (sender as ImageView).Visibility = ViewStates.Gone;
        }

        private void IvClearSpeciality_Click(object sender, EventArgs e)
        {
            PCL.Services.AppointmentService.ApptBook.Specialty = null;
            etSpeciality.Text = "";
            (sender as ImageView).Visibility = ViewStates.Gone;
        }

        private void IvClearService_Click(object sender, EventArgs e)
        {
            PCL.Services.AppointmentService.ApptBook.Service = null;
            etService.Text = "";
            (sender as ImageView).Visibility = ViewStates.Gone;
        }

        private void IvClearFacility_Click(object sender, EventArgs e)
        {
            PCL.Services.AppointmentService.ApptBook.Facility = null;
            etFacility.Text = "";
            (sender as ImageView).Visibility = ViewStates.Gone;
        }

        private void IvClearPayor_Click(object sender, EventArgs e)
        {
            PCL.Services.AppointmentService.ApptBook.Payor = null;
            etPayor.Text = "";
            (sender as ImageView).Visibility = ViewStates.Gone;
        }

        private void EtPicker_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this.Activity, typeof(Controls.ActivityDataPicker));

            EditText etSender = sender as EditText;

            if (etSender == etDoctor)
            {
                intent.PutExtra("title", Resources.GetString(Resource.String.appointment_doctor));
                intent.PutExtra("type", Controls.ActivityDataPicker.PickerType.APPOINTMENT_DOCTOR.ToString());
            }
            else if (etSender == etSpeciality)
            {
                intent.PutExtra("title", Resources.GetString(Resource.String.appointment_speciality));
                intent.PutExtra("type", Controls.ActivityDataPicker.PickerType.APPOINTMENT_SPECIALITY.ToString());
            }
            else if (etSender == etService)
            {
                intent.PutExtra("title", Resources.GetString(Resource.String.appointment_service));
                intent.PutExtra("type", Controls.ActivityDataPicker.PickerType.APPOINTMENT_SERVICE.ToString());
            }
            else if (etSender == etFacility)
            {
                intent.PutExtra("title", Resources.GetString(Resource.String.appointment_facility));
                intent.PutExtra("type", Controls.ActivityDataPicker.PickerType.APPOINTMENT_FACILITY.ToString());
            }
            else if (etSender == etPayor)
            {
                intent.PutExtra("title", Resources.GetString(Resource.String.appointment_payor));
                intent.PutExtra("type", Controls.ActivityDataPicker.PickerType.APPOINTMENT_PAYOR.ToString());
            }

            StartActivityForResult(intent, START_DATA_PICKER_REQUEST);
        }

        /*private void EtDate_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time) {
                PCL.Services.AppointmentService.ApptBook.Moment = time;
                etDate.Text = time.ToLongDateString();
            });
            frag.SelectedDate = DateTime.Parse(etDate.Text);
            frag.DisablePastDates = true;
            frag.Show(Activity.FragmentManager, DatePickerFragment.TAG);
        }*/

        #endregion

        #region toolbar methods

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.TopToolbarMenuSave, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_save)
            {

                if (!IsValid())
                {
                    return false;
                }

                var intent = new Intent(this.Context, typeof(AppointmentNewStep1));
                StartActivityForResult(intent, START_NEW_APPOINTMENT_STEP2);

            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        private bool IsValid()
        {

            //validate only the data from this screen
            ValidationResult valresult = PCL.Services.AppointmentService.ApptBook.Validator.Validate(nameof(PCL.Services.AppointmentService.ApptBook.Doctor));
            etDoctor.ShowErrorIfAny(valresult, nameof(PCL.Services.AppointmentService.ApptBook.Doctor));

            valresult = valresult.Combine(PCL.Services.AppointmentService.ApptBook.Validator.Validate(nameof(PCL.Services.AppointmentService.ApptBook.Specialty)));
            etSpeciality.ShowErrorIfAny(valresult, nameof(PCL.Services.AppointmentService.ApptBook.Specialty));

            valresult = valresult.Combine(PCL.Services.AppointmentService.ApptBook.Validator.Validate(nameof(PCL.Services.AppointmentService.ApptBook.Service)));
            etService.ShowErrorIfAny(valresult, nameof(PCL.Services.AppointmentService.ApptBook.Service));

            valresult = valresult.Combine(PCL.Services.AppointmentService.ApptBook.Validator.Validate(nameof(PCL.Services.AppointmentService.ApptBook.Facility)));
            etFacility.ShowErrorIfAny(valresult, nameof(PCL.Services.AppointmentService.ApptBook.Facility));

            valresult = valresult.Combine(PCL.Services.AppointmentService.ApptBook.Validator.Validate(nameof(PCL.Services.AppointmentService.ApptBook.Payor)));
            etPayor.ShowErrorIfAny(valresult, nameof(PCL.Services.AppointmentService.ApptBook.Payor));

            /*valresult = valresult.Combine(PCL.Services.AppointmentService.ApptBook.Validator.Validate(nameof(PCL.Services.AppointmentService.ApptBook.Moment)));
            etDate.ShowErrorIfAny(valresult, nameof(PCL.Services.AppointmentService.ApptBook.Moment));*/

            return valresult.IsValid;
        }

        public void RefreshFormData()
        {
            Model.AppointmentModels.AppointmentData aData = aService.GetFilteredData(PCL.Services.AppointmentService.ApptBook);

            //doctor
            if (SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Doctor != null)
            {
                etDoctor.Text = SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Doctor.Title;
                ivClearDoctor.Visibility = ViewStates.Visible;
            }
            else if (aData.Doctors.Count == 1)
            {
                SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Doctor = aData.Doctors[0];
                etDoctor.Text = SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Doctor.Title;
                ivClearDoctor.Visibility = ViewStates.Visible;
            }
            else
            {
                etDoctor.Text = "";
                ivClearDoctor.Visibility = ViewStates.Gone;
            }

            //speciality
            if (SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Specialty != null)
            {
                etSpeciality.Text = SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Specialty.Title;
                ivClearSpeciality.Visibility = ViewStates.Visible;
            }
            else if (aData.Specialties.Count == 1)
            {
                SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Specialty = aData.Specialties[0];
                etSpeciality.Text = SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Specialty.Title;
                ivClearSpeciality.Visibility = ViewStates.Visible;
            }
            else
            {
                etSpeciality.Text = "";
                ivClearSpeciality.Visibility = ViewStates.Gone;
            }

            //service
            if (SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Service != null)
            {
                etService.Text = SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Service.Title;
                ivClearService.Visibility = ViewStates.Visible;
            }
            else if (aData.Services.Count == 1)
            {
                SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Service = aData.Services[0];
                etService.Text = SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Service.Title;
                ivClearService.Visibility = ViewStates.Visible;
            }
            else
            {
                etService.Text = "";
                ivClearService.Visibility = ViewStates.Gone;
            }

            //facility
            if (SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Facility != null)
            {
                etFacility.Text = SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Facility.Title;
                ivClearFacility.Visibility = ViewStates.Visible;
            }
            else if (aData.Facilities.Count == 1)
            {
                SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Facility = aData.Facilities[0];
                etFacility.Text = SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Facility.Title;
                ivClearFacility.Visibility = ViewStates.Visible;
            }
            else
            {
                etFacility.Text = "";
                ivClearFacility.Visibility = ViewStates.Gone;
            }

            //payor
            if (SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Payor != null)
            {
                etPayor.Text = SS.HealthApp.PCL.Services.AppointmentService.ApptBook.Payor.Title;
                ivClearPayor.Visibility = ViewStates.Visible;
            }
            else
            {
                etPayor.Text = "";
                ivClearPayor.Visibility = ViewStates.Gone;
            }

            /*if (PCL.Services.AppointmentService.ApptBook.Moment != DateTime.MinValue)
            {
                etDate.Text = PCL.Services.AppointmentService.ApptBook.Moment.ToLongDateString();
            }
            else
            {
                etDate.Text = "";
            }*/

            //clear validator state
            etDoctor.Error = null;
            etService.Error = null;
            etSpeciality.Error = null;
            etFacility.Error = null;
            etPayor.Error = null;

        }

    }
}