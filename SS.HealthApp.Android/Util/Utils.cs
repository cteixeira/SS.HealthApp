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
using System.Threading.Tasks;
using Android.Content.Res;

namespace SS.HealthApp.Android.Util
{
    class Utils
    {

        public static void SaveEventOnDeviceCalendar(Context Context, string Title, string Location, DateTime Begin, DateTime End)
        {

            try
            {
                Java.Util.Calendar appointmentStartDate = new Java.Util.GregorianCalendar();
                appointmentStartDate.Time = Begin.ToJavaDate();

                Java.Util.Calendar appointmentEndDate = new Java.Util.GregorianCalendar();
                appointmentEndDate.Time = End.ToJavaDate();

                ContentResolver cr = Context.ContentResolver;
                ContentValues values = new ContentValues();
                values.Put(global::Android.Provider.CalendarContract.EventsColumns.Title, Title);
                values.Put(global::Android.Provider.CalendarContract.EventsColumns.EventLocation, Location);
                values.Put(global::Android.Provider.CalendarContract.EventsColumns.AllDay, false);
                values.Put(global::Android.Provider.CalendarContract.EventsColumns.Dtstart, appointmentStartDate.Time.Time);
                values.Put(global::Android.Provider.CalendarContract.EventsColumns.Dtend, appointmentEndDate.Time.Time);

                values.Put(global::Android.Provider.CalendarContract.EventsColumns.CalendarId, 1);
                values.Put(global::Android.Provider.CalendarContract.EventsColumns.EventTimezone, appointmentStartDate.TimeZone.ID);

                global::Android.Net.Uri uri = cr.Insert(global::Android.Provider.CalendarContract.Events.ContentUri, values);

                // Save the eventId into the Task object for possible future delete.
                var eventId = long.Parse(uri.LastPathSegment);
                //
                if (eventId > 0)
                {
                    setReminder(cr, eventId, 1440);
                    Toast.MakeText(Context, Context.Resources.GetString(Resource.String.appointment_eventcreation_sucess), ToastLength.Long).Show();
                }
                else
                {
                    //when user does not have permissions to save in calendar 0 is returned
                    Toast.MakeText(Context, Context.Resources.GetString(Resource.String.ErrAccessCalendar), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(Context, Context.Resources.GetString(Resource.String.ErrAccessCalendar), ToastLength.Long).Show();
            }

        }

        private static void setReminder(ContentResolver cr, long eventID, int timeBefore)
        {
            try
            {
                ContentValues values = new ContentValues();
                values.Put(global::Android.Provider.CalendarContract.RemindersColumns.Minutes, timeBefore);
                values.Put(global::Android.Provider.CalendarContract.RemindersColumns.EventId, eventID);
                //values.Put(global::Android.Provider.CalendarContract.RemindersColumns.MethodAlert, "");
                global::Android.Net.Uri uri = cr.Insert(global::Android.Provider.CalendarContract.Reminders.ContentUri, values);
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }

}