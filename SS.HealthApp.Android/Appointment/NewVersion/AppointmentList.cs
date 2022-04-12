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
using SS.HealthApp.Android.Util;
using SS.HealthApp.Model.AppointmentModels;
using Plugin.Connectivity;
using Java.Text;

namespace SS.HealthApp.Android.Appointment.NewVersion
{
    public class AppointmentList : BaseType.BaseFragment
    {

        LinearLayout llProgressBar = null;
        ListView appointmentListView = null;

        #region activity lifecycle methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.AppointmentList, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            //Set top bar title
            ((global::Android.Support.V7.App.AppCompatActivity)Activity).SupportActionBar.Title = Resources.GetString(Resource.String.menu_Appointment);

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this.Context, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            llProgressBar = Activity.FindViewById<LinearLayout>(Resource.Id.llProgressBar);
            appointmentListView = Activity.FindViewById<ListView>(Resource.Id.lvAppointment);
            appointmentListView.ItemClick += AppointmentViewList_ItemClick;

        }

        public override void OnResume()
        {
            base.OnResume();

            //load list here, when an appointment is created and the user come back to this screen the appointment list will be refreshed
            LoadAppointmentsListAsync();

        }

        #endregion

        #region control events

        private void AppointmentViewList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

            ListView lv = sender as ListView;
            AppointmentListScreenAdapter lvAdapter = lv.Adapter as AppointmentListScreenAdapter;
            Model.AppointmentModels.Appointment selectedItem = lvAdapter[e.Position];

            if (selectedItem.HasActionsAllowed)
            {
                AppointmentBottomDialogMenu bottomSheetDialog = new AppointmentBottomDialogMenu();
                bottomSheetDialog.SelectedAppointment = selectedItem;
                bottomSheetDialog.Show(this.Activity.SupportFragmentManager, "CustomBottomSheet");
                bottomSheetDialog.AppointmentStatusChanged += BottomSheetDialog_AppointmentStatusChanged;
            }

        }

        private void BottomSheetDialog_AppointmentStatusChanged()
        {
            LoadAppointmentsListAsync();
        }

        #endregion

        #region toolbar methods

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.TopToolbarMenuAddNew, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            if (item.ItemId == Resource.Id.action_new)
            {

                var intent = new Intent(this.Context, typeof(AppointmentNew));
                StartActivity(intent);

            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        private async void LoadAppointmentsListAsync()
        {
            appointmentListView.Visibility = ViewStates.Gone;
            llProgressBar.Visibility = ViewStates.Visible;

            SS.HealthApp.PCL.Services.AppointmentService aService = new PCL.Services.AppointmentService();
            List<Model.AppointmentModels.Appointment> appointments = await aService.GetItemsAsync();

            appointmentListView.Visibility = ViewStates.Visible;
            Activity.FindViewById<LinearLayout>(Resource.Id.llProgressBar).Visibility = ViewStates.Gone;

            if (appointments != null && appointments.Count > 0)
            {
                appointmentListView.Adapter = new AppointmentListScreenAdapter(this.Activity, appointments.ToArray());
            }
            else
            {
                //no items to display
                Toast.MakeText(this.Context, Resources.GetString(Resource.String.no_records), ToastLength.Long).Show();
            }

        }

        public class AppointmentListScreenAdapter : BaseAdapter<Model.AppointmentModels.Appointment>
        {

            public Model.AppointmentModels.Appointment[] items;
            private string[] groupLabels;
            Activity context;

            public AppointmentListScreenAdapter(Activity context, Model.AppointmentModels.Appointment[] items) : base()
            {
                this.context = context;
                this.items = items;
                this.groupLabels = new string[items.Length];
                SortItems();
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override Model.AppointmentModels.Appointment this[int position]
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
                    view = context.LayoutInflater.Inflate(Resource.Layout.CustomListItemTwoLinesNavigateIconGrouped, null);

                RelativeLayout rlGroup = view.FindViewById<RelativeLayout>(Resource.Id.rlGroup);
                TextView tvGroup = view.FindViewById<TextView>(Resource.Id.tvGroup);

                tvGroup.Text = GetGroupLabel(position);
                if (ShouldDisplayGroupHeader(position))
                {
                    rlGroup.Visibility = ViewStates.Visible;
                }
                else
                {
                    rlGroup.Visibility = ViewStates.Gone;
                }

                view.FindViewById<TextView>(Resource.Id.tvTitle).Text = items[position].Description;
                view.FindViewById<TextView>(Resource.Id.tvDescription).Text = String.Format("{0} | {1} | {2}",
                    items[position].Facility, Java.Text.DateFormat.DateInstance.Format(items[position].Moment.ToJavaDate()),
                    Java.Text.DateFormat.GetTimeInstance(DateFormat.Short).Format(items[position].Moment.ToJavaDate()));

                //only show the arrow if any option is allowed
                if (items[position].HasActionsAllowed)
                {
                    view.FindViewById<ImageView>(Resource.Id.ivNavigateIcon).Visibility = ViewStates.Visible;
                }
                else
                {
                    view.FindViewById<ImageView>(Resource.Id.ivNavigateIcon).Visibility = ViewStates.Gone;
                }

                rlGroup.Touch += RlGroup_StopTouchPropagation;

                return view;

            }

            private void RlGroup_StopTouchPropagation(object sender, View.TouchEventArgs e)
            {
                //prevent event of touch on group being propagated to list view that handles it as an item click.
                e.Handled = true;
            }

            public override void NotifyDataSetChanged()
            {
                SortItems();
                base.NotifyDataSetChanged();
            }

            string GetGroupLabel(int position)
            {
                string groupLabel = string.Empty;
                Model.AppointmentModels.Appointment currentAppointment = items[position];


                switch (currentAppointment.Status)
                {
                    case Model.Enum.AppointmentStatus.Arrived:
                    case Model.Enum.AppointmentStatus.Called:
                        if (!string.IsNullOrEmpty(currentAppointment.TicketNumber))
                        {
                            //arrived or called WITH ticket number
                            groupLabel = string.Format("{0} {1}",
                                context.Resources.GetString(Resource.String.appointment_Status_Arrived_with_number),
                                currentAppointment.TicketNumber);
                        }
                        else
                        {
                            //arrived or called WITHOUT ticket number
                            groupLabel = context.Resources.GetString(Resource.String.appointment_Status_Arrived_without_number);
                        }
                        break;
                    case Model.Enum.AppointmentStatus.Booked:
                        groupLabel = context.Resources.GetString(Resource.String.appointment_Status_Booked);
                        break;
                    case Model.Enum.AppointmentStatus.Closed:
                        groupLabel = context.Resources.GetString(Resource.String.appointment_Status_Closed);
                        break;
                    default:
                        groupLabel = string.Empty;
                        break;
                }

                groupLabels[position] = groupLabel;

                return groupLabel;
            }

            bool ShouldDisplayGroupHeader(int position)
            {

                //show group header on first item
                if (position == 0)
                {
                    return true;
                }

                //show group header if previous item has a different group label than the current item
                return groupLabels[position] != groupLabels[position - 1];

            }

            void SortItems()
            {
                //Called and Arrived 
                IEnumerable<Model.AppointmentModels.Appointment> itemsSection1 = items
                    .Where(a => a.Status == Model.Enum.AppointmentStatus.Called || a.Status == Model.Enum.AppointmentStatus.Arrived)
                    .OrderBy(a => a.TicketNumber)
                    .ThenBy(a => a.Moment);

                //Booked
                IEnumerable<Model.AppointmentModels.Appointment> itemsSection2 = items
                    .Where(a => a.Status == Model.Enum.AppointmentStatus.Booked)
                    .OrderBy(a => a.Moment);

                //Closed
                IEnumerable<Model.AppointmentModels.Appointment> itemsSection3 = items
                    .Where(a => a.Status == Model.Enum.AppointmentStatus.Closed)
                    .OrderByDescending(a => a.Moment);

                items = itemsSection1.Union(itemsSection2).Union(itemsSection3).ToArray();
                this.groupLabels = new string[items.Length];

            }

        }
    }
}