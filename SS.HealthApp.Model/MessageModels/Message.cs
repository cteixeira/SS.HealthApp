using System;

namespace SS.HealthApp.Model.MessageModels {
    public class Message : _BaseModel {

        public string Name { get; set; }

        public string Subject { get; set; }

        public string Detail { get; set; }

        public DateTime Moment { get; set; }

        public bool Read { get; set; }

        public bool Received { get; set; }

        public Guid? ConversationID { get; set; }

        //Used for REST data transport
        public string SubjectID { get; set; }

        //Used for REST data transport
        public string RecipientID { get; set; }
    }
}
