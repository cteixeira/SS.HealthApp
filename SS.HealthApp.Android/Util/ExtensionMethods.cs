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
using Android.Views.InputMethods;
using MvvmValidation;
using static Android.Views.ViewGroup;

namespace SS.HealthApp.Android.Util
{
    public static class ExtensionMethods
    {

        #region android views

        public static void CloseKeyboardIfOpened(this Activity CurrentActivity)
        {
            //// Check if no view has focus:
            View view = CurrentActivity.CurrentFocus;
            if (view != null)
            {
                InputMethodManager imm = (InputMethodManager)CurrentActivity.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
        }

        #endregion

        #region c# classes

        public static Java.Util.Date ToJavaDate(this System.DateTime DateTimeToConvert)
        {
            return new Java.Util.Date(DateTimeToConvert.Year - 1900, DateTimeToConvert.Month - 1, DateTimeToConvert.Day, DateTimeToConvert.Hour, DateTimeToConvert.Minute, DateTimeToConvert.Second);
        }

        #endregion

        #region view extension methods

        public static void Disable(this EditText et)
        {
            //this works with layouts/drawable/edit_text_disable_style to display the disable look and feel
            et.Enabled = false;
            et.Focusable = false;
            et.FocusableInTouchMode = false;
        }

        public static void ShowErrorIfAny(this EditText et, ValidationResult ValResult, string MemberName)
        {
            ValidationError nameError = ValResult.ErrorList.FirstOrDefault(entry => entry.Target.ToString() == MemberName);
            if (nameError != null)
            {
                int resId = et.Context.Resources.GetIdentifier(String.Concat("validationError_", nameError.ErrorText.ToLower()), "string", et.Context.PackageName);
                et.Error = resId > 0 ? et.Context.Resources.GetString(resId) : nameError.ErrorText;
            }
            else
            {
                et.Error = null;
            }
        }

        #endregion

        #region layout parameters extension methods

        public static void SetMarginsDp(this MarginLayoutParams layoutParams, Context ctx, int left, int top, int right, int bottom)
        {
            float d = ctx.Resources.DisplayMetrics.Density;
            layoutParams.SetMargins((int)(left * d), (int)(top * d), (int)(right * d), (int)(bottom * d));
        }

        #endregion

    }
}