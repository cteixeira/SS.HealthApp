
namespace SS.HealthApp.ClientConnector.Local.Models
{
    class Banner
    {

        public string Id { get; set; }
        public string Link { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
        public string HealthCareFacilityId { get; set; }


        public Model.HomeModels.Banner ToConnectorModel()
        {
            return new Model.HomeModels.Banner {
                ID = Id,
                Link = Link,
                Image = ImageUrl,
            };
        }

    }
}
