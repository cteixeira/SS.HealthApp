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

namespace SS.HealthApp.Android.Controls
{
    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {

        // TAG can be any string of your choice.
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();
        public DateTime? SelectedDate { get; set; }
        public bool DisablePastDates { get; set; }


        // Initialize this value to prevent NullReferenceExceptions.
        Action<DateTime> _dateSelectedHandler = delegate { };

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
        {
            DatePickerFragment frag = new DatePickerFragment();
            frag._dateSelectedHandler = onDateSelected;
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = SelectedDate ?? DateTime.Now;
            DatePickerDialog dialog = new DatePickerDialog(Activity,
                                                           this,
                                                           currently.Year,
                                                           currently.Month - 1, //java date month is zero based
                                                           currently.Day);
            if (DisablePastDates) { 
                dialog.DatePicker.MinDate = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }

            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            SelectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            _dateSelectedHandler(SelectedDate.Value);
        }

    }
}