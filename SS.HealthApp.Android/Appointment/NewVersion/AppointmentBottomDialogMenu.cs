using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using static Android.Provider.CalendarContract;
using SS.HealthApp.Android.Util;
using Java.Text;

namespace SS.HealthApp.Android.Appointment.NewVersion
{
    public class AppointmentBottomDialogMenu : SS.HealthApp.Android.Controls.CustomBottomSheetDialogFragment
    {

        public delegate void AppointmentStatusChangedEventHandler();

        public event AppointmentStatusChangedEventHandler AppointmentStatusChanged;

        public Model.AppointmentModels.Appointment SelectedAppointment { get; set; }

        #region activity lifecycle methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view = inflater.Inflate(Resource.Layout.AppointmentBottomDialogMenu, container, false);

            List<ViewModel.ListViewMenuItem> items = new List<ViewModel.ListViewMenuItem>();

            if (SelectedAppointment.AllowCheckIn)
            {
                items.Add(new ViewModel.ListViewMenuItem {
                    Tag = "check_in",
                    Title = Resources.GetString(Resource.String.appointment_check_in),
                    Icon = Resource.Drawable.ic_check_white_36dp
                });
            }

            if (SelectedAppointment.AllowCancel)
            {
                items.Add(new ViewModel.ListViewMenuItem {
                    Tag = "cancel_appointment",
                    Title = Resources.GetString(Resource.String.appointment_cancel),
                    Icon = Resource.Drawable.ic_cancel_white_36dp
                });
            }

            if (SelectedAppointment.AllowPresenceDeclaration)
            {
                items.Add(new ViewModel.ListViewMenuItem {
                    Tag = "presence_declaration",
                    Title = Resources.GetString(Resource.String.appointment_presence_declaration),
                    Icon = Resource.Drawable.ic_insert_drive_file_white_36dp
                });
            }

            if (SelectedAppointment.AllowParkingQRCode)
            {
                items.Add(new ViewModel.ListViewMenuItem { Tag = "parking_qr_code",
                    Title = Resources.GetString(Resource.String.appointment_parking_qr_code),
                    Icon = Resource.Drawable.ic_qrcode_white_36dp
                });
            }

            if (SelectedAppointment.AllowRateService)
            {
                items.Add(new ViewModel.ListViewMenuItem {
                    Tag = "rate_service",
                    Title = Resources.GetString(Resource.String.appointment_feedback_rating),
                    Icon = Resource.Drawable.ic_sentiment_very_satisfied_white_36dp
                });
            }

            if (SelectedAppointment.AllowAddToCalendar) {
                items.Add(new ViewModel.ListViewMenuItem {
                    Tag = "save_calendar",
                    Title = Resources.GetString(Resource.String.appointment_save_calendar),
                    Icon = Resource.Drawable.ic_event_white_36dp
                });
            }

            items.Add(new ViewModel.ListViewMenuItem { Tag = "close",
                Title = Resources.GetString(Resource.String.close),
                Icon = Resource.Drawable.ic_close_white_36dp
            });


            ListView optionsListView = view.FindViewById<ListView>(Resource.Id.lvOptions);
            optionsListView.AddHeaderView(new View(Activity));
            optionsListView.AddFooterView(new View(Activity));
            optionsListView.Adapter = new AppointmentBottomDialogMenuAdapter(this.Activity, items.Where(i => !i.Hidden).ToArray());
            optionsListView.ItemClick += OptionsListView_ItemClick;

            // Use this to return your custom view for this Fragment
            return view;
        }

        #endregion

        #region event handlers

        private void OptionsListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView lv = sender as ListView;
            AppointmentBottomDialogMenuAdapter lvAdapter = ((lv.Adapter as HeaderViewListAdapter).WrappedAdapter as AppointmentBottomDialogMenuAdapter);
            //Position - 1 because of header view
            ViewModel.ListViewMenuItem selectedItem = lvAdapter[e.Position - 1];
            switch (selectedItem.Tag)
            {
                case "cancel_appointment":
                    new global::Android.Support.V7.App.AlertDialog.Builder(this.Context)
                        .SetTitle(Resource.String.appointment_cancel_alertDialog_Title)
                        .SetMessage(String.Format(Resources.GetString(Resource.String.appointment_cancel_alertDialog_Message),
                            SelectedAppointment.Description,
                            Java.Text.DateFormat.DateInstance.Format(SelectedAppointment.Moment.ToJavaDate()),
                            Java.Text.DateFormat.GetTimeInstance(DateFormat.Short).Format(SelectedAppointment.Moment.ToJavaDate())))
                        .SetPositiveButton(Resource.String.yes, (a, b) => {
                            CancelAppointmentAsync();
                        })
                        .SetNegativeButton(Resource.String.no, (a, b) => {
                            //do nothing
                        }).Show();
                    break;
                case "save_calendar":
                    SaveEventOnCalendar();
                    break;
                case "check_in":
                    new global::Android.Support.V7.App.AlertDialog.Builder(this.Context)
                       .SetTitle(Resource.String.appointment_checkin_alertDialog_Title)
                       .SetMessage(String.Format(Resources.GetString(Resource.String.appointment_checkin_alertDialog_Message),
                           SelectedAppointment.Description,
                           Java.Text.DateFormat.DateInstance.Format(SelectedAppointment.Moment.ToJavaDate()),
                           Java.Text.DateFormat.GetTimeInstance(DateFormat.Short).Format(SelectedAppointment.Moment.ToJavaDate())))
                       .SetPositiveButton(Resource.String.yes, (a, b) => {
                           CheckInAppointmentAsync();
                       })
                       .SetNegativeButton(Resource.String.no, (a, b) => {
                            //do nothing
                        }).Show();
                    break;
                case "rate_service":
                    var intent = new Intent(this.Context, typeof(RateService));
                    intent.PutExtra("appointmentId", SelectedAppointment.ID);
                    intent.PutExtra("appointmentTitle", SelectedAppointment.Description);
                    intent.PutExtra("appointmentDescription", String.Format("{0} | {1} | {2}",
                        SelectedAppointment.Facility, Java.Text.DateFormat.DateInstance.Format(SelectedAppointment.Moment.ToJavaDate()),
                        Java.Text.DateFormat.GetTimeInstance(DateFormat.Short).Format(SelectedAppointment.Moment.ToJavaDate())));
                    
                    StartActivity(intent);
                    this.Dismiss();
                    break;
                case "presence_declaration":
                    DownloadPresenceDeclaration();
                    break;
                case "parking_qr_code":
                    DisplayParkingQrCode();
                    break;
                case "close":
                    this.Dismiss();
                    break;
                default:
                    break;
            }
        }

        #endregion

        private async void CancelAppointmentAsync()
        {
            ProgressDialog progressDialog = new ProgressDialog(this.Activity);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.info_wait));
            progressDialog.SetCancelable(false);
            progressDialog.SetCanceledOnTouchOutside(false);
            progressDialog.Show();

            PCL.Services.AppointmentService aService = new PCL.Services.AppointmentService();
            bool sucess = await aService.CancelAppointmentAsync(SelectedAppointment.ID);

            if (sucess)
            {
                AppointmentList.AppointmentListScreenAdapter adapter = this.Activity.FindViewById<ListView>(Resource.Id.lvAppointment).Adapter as AppointmentList.AppointmentListScreenAdapter;
                adapter.NotifyDataSetChanged();
                Toast.MakeText(this.Activity, Resources.GetString(Resource.String.info_success), ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this.Activity, Resources.GetString(Resource.String.ErrCancelAppointment), ToastLength.Long).Show();
            }

            AppointmentStatusChanged();
            progressDialog.Dismiss();
            this.Dismiss();

        }

        private void SaveEventOnCalendar()
        {
            ProgressDialog progressDialog = new ProgressDialog(this.Activity);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.info_saving));
            progressDialog.SetCancelable(false);
            progressDialog.SetCanceledOnTouchOutside(false);
            progressDialog.Show();

            Utils.SaveEventOnDeviceCalendar(Context, SelectedAppointment.Description, SelectedAppointment.Facility, SelectedAppointment.Moment, SelectedAppointment.Moment.AddMinutes(30));

            progressDialog.Dismiss();
            this.Dismiss();
        }

        private async void CheckInAppointmentAsync()
        {
            ProgressDialog progressDialog = new ProgressDialog(this.Activity);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.info_wait));
            progressDialog.SetCancelable(false);
            progressDialog.SetCanceledOnTouchOutside(false);
            progressDialog.Show();

            PCL.Services.AttendanceService aService = new PCL.Services.AttendanceService();
            Model.AttendanceModels.CheckInResult result = await aService.CheckInAsync(SelectedAppointment.ID);

            if (result!= null && result.Sucess)
            {
                AppointmentList.AppointmentListScreenAdapter adapter = this.Activity.FindViewById<ListView>(Resource.Id.lvAppointment).Adapter as AppointmentList.AppointmentListScreenAdapter;
                Toast.MakeText(
                    this.Activity, 
                    string.Format("{0} {1}",Resources.GetString(Resource.String.home_ticket_goto), result.Message), 
                    ToastLength.Long).Show();
            }
            else
            {
                if(result != null && !string.IsNullOrEmpty(result.Message))
                {
                    //message from server
                    Toast.MakeText(this.Activity, result.Message, ToastLength.Long).Show();
                }
                else
                {
                    //default error message
                    Toast.MakeText(this.Activity, Resources.GetString(Resource.String.ErrCheckinAppointment), ToastLength.Long).Show();
                }
                
            }

            AppointmentStatusChanged();
            progressDialog.Dismiss();
            this.Dismiss();
        }

        private async void DisplayParkingQrCode()
        {
            ProgressDialog progressDialog = new ProgressDialog(this.Activity);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.info_wait));
            progressDialog.SetCancelable(false);
            progressDialog.SetCanceledOnTouchOutside(false);
            progressDialog.Show();

            PCL.Services.AttendanceService aService = new PCL.Services.AttendanceService();

            string savePath = global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryDownloads).Path;
            string filepath = await aService.DownloadParkingQRCodeAsync(SelectedAppointment.ID, savePath);

            if (!string.IsNullOrEmpty(filepath))
            {

                global::Android.Net.Uri fileUri = null;
                using (Java.IO.File file = new Java.IO.File(filepath))
                {
                    file.SetReadable(true);
                    fileUri = global::Android.Net.Uri.FromFile(file);
                }
                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(fileUri, "image/*");
                intent.SetFlags(ActivityFlags.NoHistory);

                try
                {
                    StartActivity(intent);
                }
                catch (Exception)
                {
                    Toast.MakeText(this.Activity, Resources.GetString(Resource.String.ErrNoAppViewFile), ToastLength.Long).Show();
                }

            }
            else
            {
                //qr code not downloaded
                Toast.MakeText(this.Activity, Resources.GetString(Resource.String.ErrOpenQrCode), ToastLength.Long).Show();
            }

            progressDialog.Dismiss();
            this.Dismiss();
        }

        private async void DownloadPresenceDeclaration()
        {
            ProgressDialog progressDialog = new ProgressDialog(this.Activity);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.info_wait));
            progressDialog.SetCancelable(false);
            progressDialog.SetCanceledOnTouchOutside(false);
            progressDialog.Show();

            PCL.Services.DeclarationService dService = new PCL.Services.DeclarationService();
            string savePath = global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryDownloads).Path;
            string filepath = await dService.DownloadPresenceDeclarationByAppointmentIdAsync(SelectedAppointment.ID, savePath);

            if (!string.IsNullOrEmpty(filepath))
            {

                global::Android.Net.Uri fileUri = null;
                using (Java.IO.File file = new Java.IO.File(filepath))
                {
                    file.SetReadable(true);
                    fileUri = global::Android.Net.Uri.FromFile(file);
                }
                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(fileUri, "application/pdf");
                intent.SetFlags(ActivityFlags.NoHistory);

                try
                {
                    StartActivity(intent);
                }
                catch (Exception)
                {
                    Toast.MakeText(this.Activity, Resources.GetString(Resource.String.ErrNoAppViewFile), ToastLength.Long).Show();
                }


            }
            else
            {
                //declaration not downloaded
                Toast.MakeText(this.Activity, Resources.GetString(Resource.String.ErrOpenDeclaration), ToastLength.Long).Show();
            }

            progressDialog.Dismiss();
            this.Dismiss();
        }

        private class AppointmentBottomDialogMenuAdapter : BaseAdapter<ViewModel.ListViewMenuItem>
        {

            public ViewModel.ListViewMenuItem[] items;
            Activity context;

            public AppointmentBottomDialogMenuAdapter(Activity context, ViewModel.ListViewMenuItem[] items) : base()
            {
                this.context = context;
                this.items = items;
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override ViewModel.ListViewMenuItem this[int position]
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
                    view = context.LayoutInflater.Inflate(Resource.Layout.AppointmentBottomDialogMenuItem, null);

                view.FindViewById<TextView>(Resource.Id.tvTitle).Text = items[position].Title;
                view.FindViewById<ImageView>(Resource.Id.ivIcon).SetImageResource(items[position].Icon);

                return view;
            }

        }

    }
}