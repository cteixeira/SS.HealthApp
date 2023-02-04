using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace SS.HealthApp.Android
{

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Splash", MainLauncher = true,
                Icon = "@drawable/ic_launcher", NoHistory = true,
                ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                bool comeFromError = extras.GetBoolean("come_from_error");
                if (comeFromError)
                {
                    string errorSource = extras.GetString("error_source");
                    string errorDetail = extras.GetString("error_detail");
                    Util.GlobalExceptionHandler.ErrorHandling(new System.Exception(errorDetail), true, this);
                }
            }

            //setup global exception handling
            new Util.GlobalExceptionHandler(this).Init();

        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();

                PCL.Services.UserService uService = new PCL.Services.UserService();
                bool userloggedIn = await uService.IsLoggedIn();

                if (userloggedIn)
                {
                    StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                }
                else
                {
                    StartActivity(new Intent(Application.Context, typeof(User.Login)));
                }

            }
            catch (System.Exception ex)
            {
                Util.GlobalExceptionHandler.ErrorHandling(ex, true, this);
            }

        }

    }

}