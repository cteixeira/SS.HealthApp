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
using MvvmValidation;
using Plugin.Connectivity;
using Android.Support.Design.Widget;
using SS.HealthApp.Android.Util;

namespace SS.HealthApp.Android.User
{

    [Activity(Label = "UserDetail", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateAlwaysHidden)]
    public class UserDetail : BaseType.BaseActivity
    {

        LinearLayout llProgressBar;
        ScrollView svMainContent = null;
        EditText etName = null;
        EditText etEmail = null;
        EditText etTaxNumber = null;
        EditText etPhone = null;
        EditText etMobile = null;
        EditText etAddress = null;

        #region Activity Lifecycle Methods

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.UserDetail);

            base.SetupToolbar(Resources.GetString(Resource.String.menu_PersonalData));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, Resource.String.ErrConnLight, ToastLength.Short).Show();
                Finish();
            }

            llProgressBar = FindViewById<LinearLayout>(Resource.Id.llProgressBar);
            svMainContent = FindViewById<ScrollView>(Resource.Id.svMainContent);
            etName = FindViewById<EditText>(Resource.Id.etName);
            etName.Disable();
            etEmail = FindViewById<EditText>(Resource.Id.etEmail);
            etTaxNumber = FindViewById<EditText>(Resource.Id.etTaxNumber);
            etTaxNumber.Disable();
            etPhone = FindViewById<EditText>(Resource.Id.etPhoneNumber);
            etMobile = FindViewById<EditText>(Resource.Id.etMobileNumber);
            etAddress = FindViewById<EditText>(Resource.Id.etAddress);
            etAddress.Disable();

            etAddress.EditorAction += Form_SendOnEnter;

            Button btSave = FindViewById<Button>(Resource.Id.btSave);
            btSave.Click += BtSave_Click;

            LoadUserDetailAsync();

        }

        #endregion

        #region Control Events

        private async void BtSave_Click(object sender, EventArgs e)
        {
            await SaveData();
        }

        private async void Form_SendOnEnter(object sender, TextView.EditorActionEventArgs e)
        {
            bool handled = false;
            if (e.ActionId == global::Android.Views.InputMethods.ImeAction.Send)
            {
                await SaveData();
                handled = true;
            }
            e.Handled = handled;
        }

        #endregion


        private async void LoadUserDetailAsync()
        {

            PCL.Services.UserService uService = new PCL.Services.UserService();
            Model.UserModels.PersonalData user = await uService.GetPersonalData();
            etName.Text = user.Name;
            etEmail.Text = user.Email;
            etTaxNumber.Text = user.TaxNumber;
            etPhone.Text = user.PhoneNumber;
            etMobile.Text = user.Mobile;
            etAddress.Text = user.Address;
            svMainContent.Visibility = ViewStates.Visible;
            llProgressBar.Visibility = ViewStates.Gone;

        }

        public async Task SaveData()
        {
            Model.UserModels.PersonalData pData = new Model.UserModels.PersonalData();
            pData.Name = etName.Text;
            pData.Email = etEmail.Text;
            pData.TaxNumber = etTaxNumber.Text;
            pData.PhoneNumber = etPhone.Text;
            pData.Mobile = etMobile.Text;
            pData.Address = etAddress.Text;

            ValidationResult valresult = pData.Validator.ValidateAll();

            if (!valresult.IsValid)
            {
                etName.ShowErrorIfAny(valresult, nameof(pData.Name));
                etEmail.ShowErrorIfAny(valresult, nameof(pData.Email));
                etTaxNumber.ShowErrorIfAny(valresult, nameof(pData.TaxNumber));
                etPhone.ShowErrorIfAny(valresult, nameof(pData.PhoneNumber));
                etMobile.ShowErrorIfAny(valresult, nameof(pData.Mobile));
                etAddress.ShowErrorIfAny(valresult, nameof(pData.Address));

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
            progressDialog.Show();

            PCL.Services.UserService uService = new PCL.Services.UserService();
            bool sucess = await uService.SavePersonalData(pData);

            if (sucess)
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.info_success), ToastLength.Long).Show();
                Finish();
            }
            else
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.info_error), ToastLength.Long).Show();
            }

            progressDialog.Dismiss();

        }

    }
}