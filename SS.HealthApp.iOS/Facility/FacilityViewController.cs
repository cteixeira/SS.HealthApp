using SS.HealthApp.iOS.Components;
using System.Collections.Generic;
using SS.HealthApp.PCL.Services;
using System.Threading.Tasks;
using UIKit;
using Foundation;

namespace SS.HealthApp.iOS.Facility {
    public class FacilityViewController : BaseType.ListViewController<FacilityDetailViewController> {

        public FacilityViewController() : base(Utils.LocalizedString("FacilityTitle")) { }

        protected override async Task<List<Cell>> LoadList() {

            ListCell = new List<Cell>();

            try {
                List<Model.FacilityModels.Facility> items = await new FacilityService().GetItemsAsync();

                foreach (var item in items) {
                    ListCell.Add(new Cell() {
                        ID = item.ID,
                        Title = item.Name,
                        Image = item.Image
                    });
                }
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }

            return ListCell;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath) {

            UITableViewCell swCell = base.GetCell(tableView, indexPath);

            try {
                swCell.ImageView.Image = UIImage.FromBundle("list_placeholder");

                if (!string.IsNullOrEmpty(ListCell[indexPath.Row].Image)) {
                    PCL.Utils.DownloadManager.StartDownloadAsync(
                        ListCell[indexPath.Row].Image,
                        (s) => {
                            InvokeOnMainThread(() => {
                                if (UIImage.FromFile(s) != null)
                                    swCell.ImageView.Image = Utils.SquareCrop(UIImage.FromFile(s));
                            });
                        });
                }
            }
            catch (System.Exception ex) {
                Utils.ErrorHandling(ex, true);
            }

            return swCell;
        }
    }
}