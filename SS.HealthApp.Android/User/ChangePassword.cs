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
using Plugin.Connectivity;
using MvvmValidation;
using SS.HealthApp.Android.Util;

namespace SS.HealthApp.Android.User
{

    [Activity(Label = "ChangePassword", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class ChangePassword : BaseType.BaseActivity
    {

        EditText etOldPassword = null;
        EditText etNewPassword = null;
        EditText etNewPasswordConfirmation = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ChangePassword);

            base.SetupToolbar(Resources.GetString(Resource.String.menu_UserSettings_ChangePassword));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, Resource.String.ErrConnLight, ToastLength.Short).Show();
                Finish();
            }

            etOldPassword = FindViewById<EditText>(Resource.Id.etOldPassword);
            etNewPassword = FindViewById<EditText>(Resource.Id.etNewPassword);
            etNewPasswordConfirmation = FindViewById<EditText>(Resource.Id.etConfirmPassword);

            Button btSave = FindViewById<Button>(Resource.Id.btSave);
            btSave.Click += BtSave_Click;


        }

        private async void BtSave_Click(object sender, EventArgs e)
        {
            await SaveData();
        }

        public async Task SaveData()
        {
            Model.UserModels.ChangePassword cpData = new Model.UserModels.ChangePassword();
            cpData.oldPassword = etOldPassword.Text;
            cpData.newPassword = etNewPassword.Text;
            cpData.ConfirmNewPassword = etNewPasswordConfirmation.Text;

            ValidationResult valresult = cpData.Validator.ValidateAll();

            if (!valresult.IsValid)
            {
                etOldPassword.ShowErrorIfAny(valresult, nameof(cpData.oldPassword));
                etNewPassword.ShowErrorIfAny(valresult, nameof(cpData.newPassword));
                etNewPasswordConfirmation.ShowErrorIfAny(valresult, nameof(cpData.ConfirmNewPassword));
                return;
            }

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, Resource.String.ErrConnSevere, ToastLength.Long).Show();
                return;
            }

            ProgressDialog progressDialog = new ProgressDialog(this);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.info_saving));
            progressDialog.SetCancelable(false);
            progressDialog.SetCanceledOnTouchOutside(false);
            progressDialog.Show();

            PCL.Services.UserService uService = new PCL.Services.UserService();
            bool sucess = await uService.ChangePassword(cpData);

            if (sucess)
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.info_success), ToastLength.Long).Show();

                //show login activity
                Intent loginIntent = new Intent(Application.Context, typeof(User.Login));
                loginIntent.AddFlags(ActivityFlags.ClearTask); //clear the activity popback stack
                loginIntent.AddFlags(ActivityFlags.NewTask);
                StartActivity(loginIntent);
            }
            else
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.ErrChangePassword), ToastLength.Long).Show();
            }

            progressDialog.Dismiss();

            

        }
    }
}