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
using Java.Lang;
using SS.HealthApp.Android.Util;
using Java.Text;

namespace SS.HealthApp.Android.Controls
{
    [Activity(Label = "PickerListView", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class ActivityDataPicker : BaseType.BaseActivity
    {

        public enum PickerType
        {
            APPOINTMENT_DOCTOR,
            APPOINTMENT_SPECIALITY,
            APPOINTMENT_SERVICE,
            APPOINTMENT_FACILITY,
            APPOINTMENT_DATE,
            APPOINTMENT_TIME,
            APPOINTMENT_PAYOR,
            MESSAGE_RECEPIENT,
            MESSAGE_SUBJECT
        }

        private PickerType type;

        SS.HealthApp.PCL.Services.AppointmentService aService = new PCL.Services.AppointmentService();

        ListView pickerListView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ActivityDataPicker);

            pickerListView = FindViewById<ListView>(Resource.Id.lvPicker);

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                string title = extras.GetString("title");
                type = (PickerType)System.Enum.Parse(typeof(PickerType), extras.GetString("type"));

                base.SetupToolbar(title);
                LoadPickerData();
            }

        }

        private void LoadPickerData()
        {
            Model.AppointmentModels.AppointmentData aData = null;

            if(type == PickerType.APPOINTMENT_DOCTOR || type == PickerType.APPOINTMENT_SPECIALITY || type == PickerType.APPOINTMENT_SERVICE || type == PickerType.APPOINTMENT_FACILITY)
            {
                //preload appointment data if needed
                aData = aService.GetFilteredData(PCL.Services.AppointmentService.ApptBook);
            }
            
            Model.PickerItem[] dataSource;

            switch (type)
            {
                case PickerType.APPOINTMENT_DOCTOR:
                    dataSource = aData.Doctors.ToArray();
                    break;
                case PickerType.APPOINTMENT_SPECIALITY:
                    dataSource = aData.Specialties.ToArray();
                    break;
                case PickerType.APPOINTMENT_SERVICE:
                    dataSource = aData.Services.ToArray();
                    break;
                case PickerType.APPOINTMENT_FACILITY:
                    dataSource = aData.Facilities.ToArray();
                    break;
                case PickerType.APPOINTMENT_DATE:
                    dataSource = Appointment.AppointmentNewStep1.AvailableDates != null ? Appointment.AppointmentNewStep1.AvailableDates.ToArray() : new Model.PickerItem[0];
                    break;
                case PickerType.APPOINTMENT_TIME:
                    dataSource = Appointment.AppointmentNewStep1.AvailableTimes != null ? Appointment.AppointmentNewStep1.AvailableTimes.ToArray() : dataSource = new Model.PickerItem[0];
                    break;
                case PickerType.APPOINTMENT_PAYOR:
                    dataSource = Appointment.AppointmentNewStep0.Payors.ToArray();
                    break;
                case PickerType.MESSAGE_RECEPIENT:
                    dataSource = Message.MessageNew.MessageRecepients.ToArray() ;
                    break;
                case PickerType.MESSAGE_SUBJECT:
                    dataSource = Message.MessageNew.MessageSubjects.ToArray();
                    break;
                default:
                    return;
            }

            pickerListView.Adapter = new PickerListScreenAdapter(this, dataSource, type);
            if(type != PickerType.APPOINTMENT_DATE && type != PickerType.APPOINTMENT_TIME) { 
                pickerListView.FastScrollEnabled = true;
            }
            pickerListView.ItemClick += PickerListView_ItemClick;
        }

        private void PickerListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView lv = sender as ListView;
            PickerListScreenAdapter lvAdapter = lv.Adapter as PickerListScreenAdapter;
            Model.PickerItem selectedItem = lvAdapter[e.Position];

            switch (type)
            {
                case PickerType.APPOINTMENT_DOCTOR:
                    PCL.Services.AppointmentService.ApptBook.Doctor = selectedItem;
                    break;
                case PickerType.APPOINTMENT_SPECIALITY:
                    PCL.Services.AppointmentService.ApptBook.Specialty = selectedItem;
                    break;
                case PickerType.APPOINTMENT_SERVICE:
                    PCL.Services.AppointmentService.ApptBook.Service = selectedItem;
                    break;
                case PickerType.APPOINTMENT_FACILITY:
                    PCL.Services.AppointmentService.ApptBook.Facility = selectedItem;
                    break;
                case PickerType.APPOINTMENT_DATE:
                    PCL.Services.AppointmentService.ApptBook.Moment = DateTime.Parse(selectedItem.ID);
                    break;
                case PickerType.APPOINTMENT_TIME:
                    PCL.Services.AppointmentService.ApptBook.SlotID = selectedItem.ID;
                    break;
                case PickerType.APPOINTMENT_PAYOR:
                    PCL.Services.AppointmentService.ApptBook.Payor = selectedItem;
                    break;
                default:
                    break;
            }

            Intent resultIntent = new Intent();
            resultIntent.PutExtra("type", type.ToString());
            resultIntent.PutExtra("id", selectedItem.ID);
            resultIntent.PutExtra("title", selectedItem.Title);

            SetResult(Result.Ok, resultIntent); // set result code and bundle data for response

            Finish();

        }


        class PickerListScreenAdapter : BaseAdapter<Model.PickerItem>, ISectionIndexer
        {

            public Model.PickerItem[] items;
            PickerType type;
            Dictionary<string, int> alphaIndex;
            string[] sections;
            Java.Lang.Object[] sectionsObjects;
            Activity context;

            public PickerListScreenAdapter(Activity context, Model.PickerItem[] items, PickerType Type) : base()
            {
                this.context = context;
                this.items = items;
                this.type = Type;

                alphaIndex = new Dictionary<string, int>();
                for (int i = 0; i < items.Length; i++)
                { // loop through items
                    var key = items[i].Title.ToString().Substring(0, 1);
                    if (!alphaIndex.ContainsKey(key))
                        alphaIndex.Add(key, i); // add each 'new' letter to the index
                }
                sections = new string[alphaIndex.Keys.Count];
                alphaIndex.Keys.CopyTo(sections, 0); // convert letters list to string[]
                                                     // Interface requires a Java.Lang.Object[], so we create one here
                sectionsObjects = new Java.Lang.Object[sections.Length];
                for (int i = 0; i < sections.Length; i++)
                {
                    sectionsObjects[i] = new Java.Lang.String(sections[i]);
                }

            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override Model.PickerItem this[int position]
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
                    view = context.LayoutInflater.Inflate(global::Android.Resource.Layout.SimpleListItem1, null);

                if (type == PickerType.APPOINTMENT_DATE)
                {
                    view.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = Java.Text.DateFormat.DateInstance.Format(Convert.ToDateTime(items[position].ID).ToJavaDate());
                }
                if (type == PickerType.APPOINTMENT_TIME)
                {
                    view.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = Java.Text.DateFormat.GetTimeInstance(DateFormat.Short).Format(DateTime.Today.Add(Convert.ToDateTime(items[position].Title).TimeOfDay).ToJavaDate());
                }
                else
                { 
                    view.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = items[position].Title;
                }

                return view;
            }

            public Java.Lang.Object[] GetSections()
            {
                return sectionsObjects;
            }

            public int GetPositionForSection(int section)
            {
                return alphaIndex[sections[section]];
            }

            public int GetSectionForPosition(int position)
            {      // this method isn't called in this example, but code is provided for completeness
                int prevSection = 0;
                for (int i = 0; i < sections.Length; i++)
                {
                    if (GetPositionForSection(i) > position)
                    {
                        break;
                    }
                    prevSection = i;
                }
                return prevSection;
            }
        }

    }



}