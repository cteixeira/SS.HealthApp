using SS.HealthApp.Model.UserModels;

namespace SS.HealthApp.ClientConnector.Local.Models
{
    class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string TaxNumber { get; set; }
        public string Mobile { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public PersonalData ToConnectorModel()
        {
            return new PersonalData
            {
                Name = Name,
                Email = Email,
                Address = Address,
                PhoneNumber = PhoneNumber,
                Mobile = Mobile,
                TaxNumber = TaxNumber
            };
        }

    }
}
