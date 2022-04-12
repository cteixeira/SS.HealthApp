using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace SS.HealthApp.iOS.Components {

	[Register ("UMTableViewCell")]
	partial class UMTableViewCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView image { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel label { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (image != null) {
				image.Dispose ();
				image = null;
			}
			if (label != null) {
				label.Dispose ();
				label = null;
			}
		}
	}
}
