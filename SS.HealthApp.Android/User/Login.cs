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
using System.Threading.Tasks;
using Plugin.Connectivity;

namespace SS.HealthApp.Android.User
{

    [Activity(Label = "Login", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, NoHistory = true, WindowSoftInputMode = SoftInput.StateAlwaysVisible)]
    public class Login : BaseType.BaseActivity
    {

        #region controls

        Button btLogin = null;
        EditText etUsername = null;
        EditText etPassword = null;

        #endregion

        #region activity lifecycle events

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Login);
            //base.SetupToolbar();

            Bundle extras = Intent.Extras;

            btLogin = FindViewById<Button>(Resource.Id.btLogin);
            etUsername = FindViewById<EditText>(Resource.Id.etUsername);
            etPassword = FindViewById<EditText>(Resource.Id.etPassword);


            etUsername.TextChanged += editText_TextChanged;
            etPassword.TextChanged += editText_TextChanged;

            etPassword.EditorAction += Form_SendOnEnter;

            btLogin.Click += btLogin_Click;

        }

        #endregion

        #region Control Events

        private async void Form_SendOnEnter(object sender, TextView.EditorActionEventArgs e)
        {
            bool handled = false;
            if (e.ActionId == global::Android.Views.InputMethods.ImeAction.Send)
            {
                await ExecuteLoginAsync();
                handled = true;
            }
            e.Handled = handled;
        }

        private void editText_TextChanged(object sender, global::Android.Text.TextChangedEventArgs e)
        {
            EditText et = sender as EditText;
            if (!String.IsNullOrEmpty(et.Text))
            {
                et.Error = null;
            }
            else
            {
                et.Error = Resources.GetString(Resource.String.validationError_required);
            }
        }

        private async void btLogin_Click(object sender, EventArgs e)
        {
            await ExecuteLoginAsync();

        }

        #endregion

        public bool Validate()
        {
            bool valid = true;

            String username = etUsername.Text;
            String password = etPassword.Text;

            if (String.IsNullOrEmpty(username))
            {
                etUsername.Error = Resources.GetString(Resource.String.validationError_required);
                valid = false;
            }
            else
            {
                etUsername.Error = null;
            }

            if (String.IsNullOrEmpty(password))
            {
                etPassword.Error = Resources.GetString(Resource.String.validationError_required);
                valid = false;
            }
            else
            {
                etPassword.Error = null;
            }

            return valid;

        }

        private async Task ExecuteLoginAsync()
        {
            string username = etUsername.Text;
            string password = etPassword.Text;


            if (!Validate())
            {
                return;
            }

            this.CloseKeyboardIfOpened();

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, Resource.String.ErrConnSevere, ToastLength.Long).Show();
                return;
            }

            btLogin.Enabled = false;

            ProgressDialog progressDialog = new ProgressDialog(this);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.login_authenticating));
            progressDialog.Show();

            PCL.Services.UserService uService = new PCL.Services.UserService();
            bool sucess = await uService.LoginAsync(username, password);

            if (sucess)
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }
            else
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.login_error), ToastLength.Long).Show();
            }

            btLogin.Enabled = true;
            progressDialog.Dismiss();
        }

    }

}