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
using Java.Lang;

namespace SS.HealthApp.Android.Util
{
    public class GlobalExceptionHandler
    {

        private Context context;

        public GlobalExceptionHandler(Context Context)
        {
            context = Context;
        }

        public void Init()
        {
            global::Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += HandleAndroidException;

            System.AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
                //cannot recover and send error through REST, relauch the application
                Intent intent = new Intent(context, typeof(SplashActivity));
                intent.AddFlags(ActivityFlags.ClearTask); //clear the activity popback stack
                intent.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(intent);
            };


        }

        private void HandleAndroidException(object sender, global::Android.Runtime.RaiseThrowableEventArgs e)
        {
            e.Handled = true;
            Util.GlobalExceptionHandler.ErrorHandling(e.Exception, true, context);

            Intent intent = new Intent(context, typeof(SplashActivity));
            intent.AddFlags(ActivityFlags.ClearTask); //clear the activity popback stack
            intent.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
        }

        internal async static void ErrorHandling(System.Exception ex)
        {
            await new PCL.Services.ErrorService().AddAsync(ex);
        }

        internal async static void ErrorHandling(System.Exception ex, bool showGeneralError, Context context)
        {
            if (showGeneralError)
            {
                Toast.MakeText(context, Resource.String.ErrGeneral, ToastLength.Short).Show();
            }
            await new PCL.Services.ErrorService().AddAsync(ex);
        }

    }
}