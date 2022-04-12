using SS.HealthApp.iOS.Components;
using System.Collections.Generic;
using SS.HealthApp.PCL.Services;
using System.Threading.Tasks;
using UIKit;
using Foundation;

namespace SS.HealthApp.iOS.News {
    public class NewsViewController : BaseType.ListViewController<NewsDetailViewController> {

        public NewsViewController() : base(Utils.LocalizedString("NewsTitle")) {
            ROW_HEIGHT = 84;
        }

        protected override async Task<List<Cell>> LoadList() {

            ListCell = new List<Cell>();

            try {
                List<Model.NewsModels.News> items = await new NewsService().GetItemsAsync();

                foreach (var item in items) {
                    ListCell.Add(new Cell() {
                        ID = item.ID,
                        Title = item.Name,
                        Image = item.Image,
                        Detail = item.Date.ToShortDateString()
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