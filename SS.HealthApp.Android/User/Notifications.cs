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

namespace SS.HealthApp.Android.User
{

    [Activity(Label = "Notifications", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateAlwaysHidden)]
    public class Notifications : BaseType.BaseActivity
    {

        //EditText etOldPassword = null;
        //EditText etNewPassword = null;
        //EditText etNewPasswordConfirmation = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Notifications);

            base.SetupToolbar(Resources.GetString(Resource.String.menu_UserSettings_Notifications));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, Resource.String.ErrConnLight, ToastLength.Short).Show();
                Finish();
            }

            //etPhone = FindViewById<EditText>(Resource.Id.etPhoneNumber);
            //etAddress = FindViewById<EditText>(Resource.Id.etAddress);
            //etAddress = FindViewById<EditText>(Resource.Id.etAddress);

            //Button btSave = FindViewById<Button>(Resource.Id.btSave);
            //btSave.Click += BtSave_Click;


        }

        //private async void BtSave_Click(object sender, EventArgs e)
        //{
        //    await SaveData();
        //}

        //private async void Form_SendOnEnter(object sender, TextView.EditorActionEventArgs e)
        //{
        //    bool handled = false;
        //    if (e.ActionId == global::Android.Views.InputMethods.ImeAction.Send)
        //    {
        //        await SaveData();
        //        handled = true;
        //    }
        //    e.Handled = handled;
        //}

        //private async void LoadUserDetail()
        //{

        //    PCL.Services.UserService uService = new PCL.Services.UserService();
        //    PCL.Models.User user = await uService.GetPersonalData();
        //    etName.Text = user.Name;
        //    etEmail.Text = user.Email;
        //    etFiscalNumber.Text = user.FiscalNumber;
        //    etPhone.Text = user.PhoneNumber;
        //    etAddress.Text = user.Address;
        //    svMainContent.Visibility = ViewStates.Visible;
        //    llProgressBar.Visibility = ViewStates.Gone;

        //}

        //public bool validate()
        //{
        //    bool valid = true;

        //    String name = etName.Text;

        //    if (String.IsNullOrEmpty(name))
        //    {
        //        etName.Error = Resources.GetString(Resource.String.editText_invalid);
        //        valid = false;
        //    }
        //    else
        //    {
        //        etName.Error = null;
        //    }

        //    return valid;

        //}

        //public async Task<bool> SaveData()
        //{
        //    await Task.Delay(1000);
        //    return true;
        //}

    }
}