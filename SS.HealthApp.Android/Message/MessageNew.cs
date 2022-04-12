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
using Plugin.Connectivity;
using SS.HealthApp.Android.Util;
using Java.Text;
using System.Threading.Tasks;
using MvvmValidation;

namespace SS.HealthApp.Android.Message
{
    [Activity(Label = "MessageNew", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class MessageNew : BaseType.BaseActivity
    {

        EditText etRecepient;
        ImageView ivClearRecepient;
        EditText etSubject;
        ImageView ivClearSubject;
        EditText etMessage;
        ScrollView svMainContent;
        LinearLayout llProgressBar;

        private Model.MessageModels.NewMessage NewMessage = new Model.MessageModels.NewMessage();

        public static List<Model.PickerItem> MessageRecepients = null;
        public static List<Model.PickerItem> MessageSubjects = null;

        string replyMessageId = null;
        private bool IsNewMessage { get { return string.IsNullOrEmpty(replyMessageId); } }

        SS.HealthApp.PCL.Services.MessageService mService = new PCL.Services.MessageService();

        #region activity lifecycle methods

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.MessageNew);
            base.SetupToolbar(Resources.GetString(Resource.String.message_newMessage));

            if (!CrossConnectivity.Current.IsConnected)
            {
                Toast.MakeText(this, Resource.String.ErrConnLight, ToastLength.Short).Show();
            }

            etRecepient = FindViewById<EditText>(Resource.Id.etRecepient);
            etRecepient.Click += EtPicker_Click;

            ivClearRecepient = FindViewById<ImageView>(Resource.Id.ivClearRecepient);
            ivClearRecepient.Click += IvClearRecepient_Click;

            etSubject = FindViewById<EditText>(Resource.Id.etSubject);
            etSubject.Click += EtPicker_Click;

            ivClearSubject = FindViewById<ImageView>(Resource.Id.ivClearSubject);
            ivClearSubject.Click += IvClearSubject_Click;

            etMessage = FindViewById<EditText>(Resource.Id.etMessage);

            svMainContent = FindViewById<ScrollView>(Resource.Id.svMainContent);
            llProgressBar = FindViewById<LinearLayout>(Resource.Id.llProgressBar);
            
            Bundle extras = Intent.Extras;
            if (extras != null)
            {
                replyMessageId = extras.GetString("message_id");
            }

            if (IsNewMessage)
            {
                //new message
                LoadPickersData();
            }
            else
            {
                //reply to message
                (etRecepient.Parent as View).Visibility = ViewStates.Gone;
                ivClearRecepient.Visibility = ViewStates.Gone;

                (etSubject as View).Visibility = ViewStates.Gone;
                ivClearSubject.Visibility = ViewStates.Gone;

                svMainContent.Visibility = ViewStates.Visible;
                llProgressBar.Visibility = ViewStates.Gone;

            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (((global::Android.App.Result)resultCode) == Result.Ok)
            {
                Controls.ActivityDataPicker.PickerType type = (Controls.ActivityDataPicker.PickerType)System.Enum.Parse(typeof(Controls.ActivityDataPicker.PickerType), data.GetStringExtra("type"));
                string selectedTitle = data.GetStringExtra("title");
                string selectedId = data.GetStringExtra("id");

                if (type == Controls.ActivityDataPicker.PickerType.MESSAGE_RECEPIENT)
                {
                    etRecepient.Text = selectedTitle;
                    ivClearRecepient.Visibility = ViewStates.Visible;
                    Appointment.AppointmentNew a = new Appointment.AppointmentNew();

                    NewMessage.RecipientID = selectedId;
                }

                if (type == Controls.ActivityDataPicker.PickerType.MESSAGE_SUBJECT)
                {
                    etSubject.Text = selectedTitle;
                    ivClearSubject.Visibility = ViewStates.Visible;

                    NewMessage.SubjectID = selectedId;
                }

                //clear validator state
                etRecepient.Error = null;
                etSubject.Error = null;
                etMessage.Error = null;

            }

        }

        #endregion

        #region toolbar methods

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.TopToolbarMenuSave, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_save)
            {

                if (!IsValid())
                {
                    return false;
                }

                SendMessageAsync();
               
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region control events

        private void EtPicker_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(Controls.ActivityDataPicker));

            EditText etSender = sender as EditText;

            if (etSender == etRecepient)
            {
                intent.PutExtra("title", Resources.GetString(Resource.String.message_recepient));
                intent.PutExtra("type", Controls.ActivityDataPicker.PickerType.MESSAGE_RECEPIENT.ToString());
            }
            else if (etSender == etSubject)
            {
                intent.PutExtra("title", Resources.GetString(Resource.String.message_subject));
                intent.PutExtra("type", Controls.ActivityDataPicker.PickerType.MESSAGE_SUBJECT.ToString());
            }

            StartActivityForResult(intent, 1);
        }

        private void IvClearRecepient_Click(object sender, EventArgs e)
        {
            etRecepient.Text = "";
            NewMessage.RecipientID = null;
            (sender as ImageView).Visibility = ViewStates.Gone;
        }

        private void IvClearSubject_Click(object sender, EventArgs e)
        {
            etSubject.Text = "";
            NewMessage.SubjectID = null;
            (sender as ImageView).Visibility = ViewStates.Gone;
        }

        #endregion

        private async void LoadPickersData()
        {

            Task<List<Model.PickerItem>>[] results = new Task<List<Model.PickerItem>>[2];

            results[0] = mService.GetRecipientsAsync();
            results[1] = mService.GetSubjectsAsync();

            await Task.WhenAll(results);

            MessageRecepients = results[0].Result;
            MessageSubjects = results[1].Result;

            if (MessageRecepients.Count == 1)
            {
                etRecepient.Text = MessageRecepients[0].Title;
                ivClearRecepient.Visibility = ViewStates.Visible;
                NewMessage.RecipientID = MessageRecepients[0].ID;
            }


            if (MessageSubjects.Count() == 1)
            {
                etSubject.Text = MessageRecepients[0].Title;
                ivClearSubject.Visibility = ViewStates.Visible;
                NewMessage.SubjectID = MessageSubjects[0].ID;
            }

            svMainContent.Visibility = ViewStates.Visible;
            llProgressBar.Visibility = ViewStates.Gone;

        }

        private bool IsValid()
        {

            NewMessage.IsNewMessage = IsNewMessage;
            NewMessage.Detail = etMessage.Text;

            ValidationResult valResult = NewMessage.Validator.ValidateAll();

            etRecepient.ShowErrorIfAny(valResult, nameof(NewMessage.RecipientID));
            etSubject.ShowErrorIfAny(valResult, nameof(NewMessage.SubjectID));
            etMessage.ShowErrorIfAny(valResult, nameof(NewMessage.Detail));

            return valResult.IsValid;

        }

        private async void SendMessageAsync()
        {

            ProgressDialog progressDialog = new ProgressDialog(this);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.info_wait));
            progressDialog.SetCancelable(false);
            progressDialog.SetCanceledOnTouchOutside(false);
            progressDialog.Show();

            string messageId = String.Empty;

            if (IsNewMessage)
            {
                messageId = await mService.CreateMessageAsync(NewMessage.RecipientID, NewMessage.SubjectID, etMessage.Text);
            }
            else
            {
                messageId = await mService.ReplyMessageAsync(replyMessageId, etMessage.Text);
            }

            bool sucess = !String.IsNullOrEmpty(messageId);

            if (sucess)
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.info_success), ToastLength.Long).Show();

                SetResult(Result.Ok);
                Finish();

            }
            else
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.ErrSendMessage), ToastLength.Long).Show();
            }

            progressDialog.Dismiss();

        }

    }
}