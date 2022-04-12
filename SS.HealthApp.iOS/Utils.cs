using UIKit;
using Foundation;
using System;
using CoreGraphics;
using ToastIOS;
using System.Collections.Generic;
using EventKit;
using System.Linq;

namespace SS.HealthApp.iOS {
    class Utils {

        internal async static void ErrorHandling(Exception ex, bool showGeneralError) {
            if(showGeneralError) ShowToast(LocalizedString("ErrGeneral"), ToastType.Error, 3000);
            var result = await new PCL.Services.ErrorService().AddAsync(ex);
        }

        internal static string LocalizedString(string key) {
            return NSBundle.MainBundle.LocalizedString(key, key);
        }

        internal static UILabel NewLabel(CGRect frame, string text, UIColor color,
                                        UIFont font, UITextAlignment align = UITextAlignment.Left) {
            return new UILabel(frame) {
                TextAlignment = align, Text = text, TextColor = color, Font = font
            };
        }

        internal static UITextField NewTextField(CGRect frame, string placeholder, UIKeyboardType keyboardType = UIKeyboardType.Default) {

            return new UITextField(frame) {
                Placeholder = placeholder,
                BorderStyle = UITextBorderStyle.RoundedRect,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                KeyboardType = keyboardType
            };
        }

        internal static void NewTextAndLabelField(UIView view, ref UITextField textField, nfloat top,
                                            string caption, string value, UIKeyboardType keyboardType = UIKeyboardType.Default) {
            nfloat width = view.Frame.Width - 20;
            view.AddSubview(NewLabel(new CGRect(10, top, width, 30), caption, Settings.FIRST_COLOR, UIFont.PreferredTitle3));
            textField = NewTextField(new CGRect(10, top + 30, width, 40), string.Empty, keyboardType);
            textField.Text = value;
            view.AddSubview(textField);
        }

        internal static UITextView NewTextViewField(CGRect frame) {
            var textView = new UITextView(frame) { AutoresizingMask = UIViewAutoresizing.FlexibleWidth };
            textView.Font = UIFont.PreferredBody;
            textView.Layer.BorderColor = UIColor.FromRGB(192, 192, 192).CGColor;
            textView.Layer.BorderWidth = 0.5f;
            return textView;
        }

        internal static void NewTextViewAndLabelField(UIView view, ref UITextView textView, nfloat top, nfloat height, string caption, string value) {
            nfloat width = view.Frame.Width - 20;
            view.AddSubview(NewLabel(new CGRect(10, top, width, 30), caption, Settings.FIRST_COLOR, UIFont.PreferredTitle3));
            textView = NewTextViewField(new CGRect(10, top + 30, width, height));
            textView.Text = value;
            view.AddSubview(textView);
        }

       

        internal static void NewPickerField(UIView view, ref UITextField txt, float top, string caption, 
            UITextFieldCondition BeginEditing, EventHandler Clear) {

            NewTextAndLabelField(view, ref txt, top, caption, string.Empty);
            txt.Placeholder = Utils.LocalizedString("Select");
            txt.ShouldBeginEditing += BeginEditing;

            var btnClear = NewButton(
                    new CGRect(0, 0, 30, 30),
                    string.Empty,
                    null,
                    UIColor.FromRGB(204, 204, 204),
                    "Icons/ic_highlight_off_36pt.png",
                    Clear);

            txt.RightView = btnClear;
            txt.ClipsToBounds = true;
            txt.RightViewMode = UITextFieldViewMode.Never;
        }

        internal static void SetPickerValue(UITextField txt, Model.PickerItem selectedItem) {
            if (selectedItem != null) {
                txt.Text = selectedItem.Title;
                txt.RightViewMode = UITextFieldViewMode.Always;
            }
            else {
                txt.Text = string.Empty;
                txt.RightViewMode = UITextFieldViewMode.Never;
            }
        }

        internal static void NewPickerAndLabelField(UIView view, ref UITextField textField, nfloat top, string caption, List<Model.PickerItem> items) {

            //HACKING: Adds an empty element at top of list because iOS always forces an element to be selected
            items.Insert(0, new Model.PickerItem(string.Empty, string.Empty));

            Utils.NewTextAndLabelField(view, ref textField, top, caption, string.Empty);
            textField.Placeholder = Utils.LocalizedString("Select");
            textField.InputView = new UIPickerView() { Model = new Components.PickerModel(textField, items) };
        }
        
        internal static void NewDateAndLabelField(UIView view, ref UITextField textField, nfloat top, string caption, DateTime value) {

            Utils.NewTextAndLabelField(view, ref textField, top, caption, string.Empty);

            UIDatePicker picker = new UIDatePicker() { Mode = UIDatePickerMode.Date };

            UITextField txt = textField; //required because ref fields aren't allowed in lambda expressions
            picker.AddTarget((s, a) => {
                DateTime dt = (DateTime)picker.Date;
                txt.Text = dt.ToShortDateString();
            }, UIControlEvent.ValueChanged);

            textField.InputView = picker;

            picker.SetDate((NSDate)value, false);
            textField.Text = value.ToShortDateString();
        }

        internal static UIButton NewButton(CGRect frame, string title, UIColor backgroundColor, UIColor tintColor, string bundleImage) {

            UIButton btn = new UIButton(UIButtonType.RoundedRect) {
                Frame = frame,
                BackgroundColor = backgroundColor,
                TintColor = tintColor,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

            btn.SetTitle(title, UIControlState.Normal);
            btn.Layer.CornerRadius = 5f;

            if (!string.IsNullOrEmpty(bundleImage)) {
                btn.SetImage(UIImage.FromBundle(bundleImage).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                btn.ImageView.TintColor = tintColor;
                btn.ImageView.BackgroundColor = backgroundColor;
            }

            return btn;
        }

        internal static UIButton NewButton(CGRect frame, string title, string bundleImage = null) {
            return NewButton(frame, title, Settings.FIRST_COLOR, UIColor.White, bundleImage);
        }

        internal static UIButton NewButton(CGRect frame, string title, UIColor backgroundColor, UIColor tintColor, string bundleImage, EventHandler action) {
            UIButton btn = NewButton(frame, title, backgroundColor, tintColor, bundleImage);
            btn.TouchUpInside += action;
            return btn;
        }

        internal static bool CheckRequiredField(UITextField txt) {
            if (txt.Text.Length <= 0) {
                txt.Layer.BorderColor = UIColor.Red.CGColor;
                txt.Layer.BorderWidth = 1;
                return false;
            }
            return true;
        }

        internal static bool CheckRequiredField(UITextView txt) {
            if (txt.Text.Length <= 0) {
                txt.Layer.BorderColor = UIColor.Red.CGColor;
                txt.Layer.BorderWidth = 1;
                return false;
            }
            return true;
        }

        internal static Model.PickerItem GetPickerItem(UITextField txt) {
            UIPickerView picker = (UIPickerView)txt.InputView;
            return ((Components.PickerModel)picker.Model).SelectedItem;
        }

        internal static void ShowToast(string text, ToastType type) {
            ShowToast(text, type, int.MaxValue);
        }

        internal static void ShowToast(string text, ToastType type, int duration) {
            Toast.MakeText(text)
                .SetDuration(duration)
                .SetGravity(ToastGravity.Center)
                .SetUseShadow(false)
                .Show(ToastType.None);
        }

        internal static UIImage ImageFromUrl(string uri) { 
            //Sync image download
            using (var url = new NSUrl(uri)) {
                using (var data = NSData.FromUrl(url))
                    return data == null ? null : UIImage.LoadFromData(data);
            }
        }

        internal static UIImage SquareCrop(UIImage img) {

            if (img == null) return null;

            // Use smallest side length as crop square length
            double squareLength = Math.Min(img.Size.Width, img.Size.Height);

            nfloat x, y;
            x = (nfloat)((img.Size.Width - squareLength) / 2.0);
            y = (nfloat)((img.Size.Height - squareLength) / 2.0);

            //This Rect defines the coordinates to be used for the crop
            CGRect croppedRect = CGRect.FromLTRB(x, y, x + (nfloat)squareLength, y + (nfloat)squareLength);

            // Center-Crop the image
            UIGraphics.BeginImageContextWithOptions(croppedRect.Size, false, img.CurrentScale);
            img.Draw(new CGPoint(-croppedRect.X, -croppedRect.Y));
            UIImage croppedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return croppedImage;
        }

        internal static string CleanURL(string url) {
            return url.Replace(" ", "%20").Replace("|", "%20");
        }

        internal static DateTime ConvertNSDateToDateTime(NSDate date) {
            DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
                new DateTime(2001, 1, 1, 0, 0, 0));
            return reference.AddSeconds(date.SecondsSinceReferenceDate);
        }

        internal static NSDate ConvertDateTimeToNSDate(DateTime date) {
            DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
                new DateTime(2001, 1, 1, 0, 0, 0));
            return NSDate.FromTimeIntervalSinceReferenceDate(
                (date - reference).TotalSeconds);
        }

        internal static void HideKeyboardOnTapping(UIView editView, UIView containerView){
            //hide keyboard on tapping the view
            var gest = new UITapGestureRecognizer(() => editView.EndEditing(true));
            gest.CancelsTouchesInView = false;
            containerView.AddGestureRecognizer(gest);
        }
        
        internal static void CreateICS(string title, string location, DateTime moment) {
            
            EKEventStore eventStore = new EKEventStore();

            eventStore.RequestAccess(EKEntityType.Event, (bool granted, NSError e) => {

                if (granted) {
                    moment = moment.AddHours(-1);
                    EKEvent newEvent = EKEvent.FromStore(eventStore);
                    newEvent.Calendar = eventStore.DefaultCalendarForNewEvents;
                    newEvent.StartDate = Utils.ConvertDateTimeToNSDate(moment);
                    newEvent.EndDate = Utils.ConvertDateTimeToNSDate(moment.AddMinutes(30)); //30 minutes event
                    newEvent.Title = title;
                    newEvent.Location = location;
                    newEvent.AddAlarm(EKAlarm.FromDate(Utils.ConvertDateTimeToNSDate(moment.AddDays(-1)))); //24 hours reminder

                    NSError err = new NSError();
                    eventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out err);

                    if (err != null)
                        throw new Exception(err.LocalizedDescription);
                }

            });
        }
    }
}
