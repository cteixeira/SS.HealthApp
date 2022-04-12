using System;
using Foundation;
using QuickLook;

namespace SS.HealthApp.iOS.Components {
    public class QuickLookDataSource: QLPreviewControllerDataSource {

        private QLPreviewItem _item;

        public QuickLookDataSource(QLPreviewItem item) {
            _item = item;
        }

        public override nint PreviewItemCount(QLPreviewController controller) {
            return 1;
        }

        public override IQLPreviewItem GetPreviewItem(QLPreviewController controller, nint index) {
            return _item;
        }

    }

    public class QuickLookItem : QLPreviewItem {
        string _fileName, _filePath;

        public QuickLookItem(string fileName, string filePath) {
            _fileName = fileName;
            _filePath = filePath;
        }

        public override string ItemTitle {
            get {
                return _fileName;
            }
        }

        public override NSUrl ItemUrl {
            get {
                return NSUrl.FromFilename(_filePath);
            }
        }
    }
}