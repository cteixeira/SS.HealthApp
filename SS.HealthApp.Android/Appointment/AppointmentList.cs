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

namespace SS.HealthApp.Android.Appointment
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

            if(selectedItem.Status == Model.Enum.AppointmentStatus.Booked)
            {
                AppointmentBottomDialogMenu bottomSheetDialog = new AppointmentBottomDialogMenu();
                bottomSheetDialog.SelectedAppointment = selectedItem;
                bottomSheetDialog.Show(this.Activity.SupportFragmentManager, "CustomBottomSheet");
            }

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
            Activity context;

            public AppointmentListScreenAdapter(Activity context, Model.AppointmentModels.Appointment[] items) : base()
            {
                this.context = context;
                this.items = items;
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

                //show group header on first item and if the previous item has a diferent group from the current one
                bool showGroupHeader = position == 0 || items[position].Status != items[position - 1].Status;

                RelativeLayout rlGroup = view.FindViewById<RelativeLayout>(Resource.Id.rlGroup);
                TextView tvGroup = view.FindViewById<TextView>(Resource.Id.tvGroup);
                if (showGroupHeader)
                {
                    tvGroup.Text = GetGroupLabel(items[position].Status);
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

                //only show the arrow on booked and booked today 
                if (items[position].Status != Model.Enum.AppointmentStatus.Booked && items[position].Status != Model.Enum.AppointmentStatus.BookedToday)
                {
                    view.FindViewById<ImageView>(Resource.Id.ivNavigateIcon).Visibility = ViewStates.Gone;
                }
                else
                {
                    view.FindViewById<ImageView>(Resource.Id.ivNavigateIcon).Visibility = ViewStates.Visible;
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

            string GetGroupLabel(Model.Enum.AppointmentStatus Status)
            {
                if (Status == Model.Enum.AppointmentStatus.Closed)
                {
                    return context.Resources.GetString(Resource.String.appointment_Status_History);
                }
                else if (Status == Model.Enum.AppointmentStatus.Booked || 
                        Status == Model.Enum.AppointmentStatus.Arrived || 
                        Status == Model.Enum.AppointmentStatus.Called)
                {
                    return context.Resources.GetString(Resource.String.appointment_Status_Booked);
                }
                return string.Empty;
            }

            void SortItems()
            {

                IEnumerable<Model.AppointmentModels.Appointment> closedItems = items.Where(a => a.Status == Model.Enum.AppointmentStatus.Closed).ToArray().OrderByDescending(a => a.Moment);
                IEnumerable<Model.AppointmentModels.Appointment> otherItems = items.Where(a => a.Status != Model.Enum.AppointmentStatus.Closed).OrderBy(a => a.Status).ThenBy(a => a.Moment);

                items = otherItems.Concat(closedItems).ToArray();
                
                //items = items.OrderBy(a => a.Status).ThenByDescending(a => a.Moment).ToArray();
            }

        }
    }
}