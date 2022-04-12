using MvvmValidation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.HealthApp.Model.MessageModels
{
    public class NewMessage
    {

        public NewMessage()
        {
            ConfigureValidationRules();
        }

        public bool IsNewMessage { get; set; }

        public string SubjectID { get; set; }

        public string RecipientID { get; set; }

        public string Detail { get; set; }

        #region validation

        [JsonIgnore]
        public ValidationHelper Validator { get; private set; }


        private void ConfigureValidationRules()
        {
            Validator = new ValidationHelper();

            Validator.AddRule(() => SubjectID, () => {
                if (IsNewMessage)
                {
                    return RuleResult.Assert(!string.IsNullOrEmpty(SubjectID), "Required");
                }
                return RuleResult.Valid();
            }) ;

            Validator.AddRule(() => RecipientID, () => {
                if (IsNewMessage)
                {
                    return RuleResult.Assert(!string.IsNullOrEmpty(RecipientID), "Required");
                }
                return RuleResult.Valid();
            });

            Validator.AddRule(() => Detail, () => {
                if (String.IsNullOrEmpty(Detail))
                {
                    return RuleResult.Invalid("Required");
                }
                if (String.IsNullOrWhiteSpace(Detail))
                {
                    return RuleResult.Invalid("Required");
                }
                string cleanDetail = Detail.Trim(new char[] { ' ', '\n' });
                return RuleResult.Assert(!String.IsNullOrEmpty(cleanDetail), "Required");
            });
            Validator.AddRequiredRule(() => Detail, "Required");
        }

        #endregion

    }
}
