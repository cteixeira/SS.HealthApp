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
using MvvmValidation;

namespace SS.HealthApp.Android.ViewModel
{
    public class AppointmentSelectSlotValidatonHelper
    {

        public AppointmentSelectSlotValidatonHelper()
        {
            ConfigureValidationRules();
        }

        public bool MomentSelected { get; set; }
        public bool SlotSelected { get; set; }
        public string Payor { get; set; }

        #region validation

        public ValidationHelper Validator { get; private set; }

        private void ConfigureValidationRules()
        {
            Validator = new ValidationHelper();

            Validator.AddRule(() => MomentSelected, () => {
                return RuleResult.Assert(MomentSelected, "Required");
            });

            Validator.AddRule(() => SlotSelected, () => {
                return RuleResult.Assert(SlotSelected, "Required");
            });

            //Validator.AddRequiredRule(() => Payor, "Required");

        }

        #endregion

    }
}