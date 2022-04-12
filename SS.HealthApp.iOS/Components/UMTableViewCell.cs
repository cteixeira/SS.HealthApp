using System;
using UIKit;
using SWTableViewCells;

namespace SS.HealthApp.iOS.Components {

	partial class UMTableViewCell : SWTableViewCell
	{
		public UMTableViewCell (IntPtr handle)
			: base (handle)
		{
		}

		public UILabel Label {
			get { return label; }
		}
	}
}
