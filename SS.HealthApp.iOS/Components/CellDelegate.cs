using System;
using System.Collections.Generic;
using SWTableViewCells;
using Foundation;
using UIKit;

namespace SS.HealthApp.iOS.Components {
    public class CellDelegate : SWTableViewCellDelegate {
        private readonly List<Cell> itens;
        private readonly UITableView tableView;

        public CellDelegate(List<Cell> itens, UITableView tableView) {
            this.itens = itens;
            this.tableView = tableView;
        }

        public override void ScrollingToState(SWTableViewCell cell, SWCellState state) {
            /*switch (state) {
                case SWCellState.Center:
                    Console.WriteLine("utility buttons closed");
                    break;
                case SWCellState.Left:
                    Console.WriteLine("left utility buttons open");
                    break;
                case SWCellState.Right:
                    Console.WriteLine("right utility buttons open");
                    break;
            }*/
        }

        public override void DidTriggerLeftUtilityButton(SWTableViewCell cell, nint index) { }

        public override void DidTriggerRightUtilityButton(SWTableViewCell cell, nint index) {
            UIButton btn = itens[tableView.IndexPathForCell(cell).Row].Buttons[(int)index];
            btn.SendActionForControlEvents(UIControlEvent.TouchUpInside);
            cell.HideUtilityButtons(true);
        }

        public override bool ShouldHideUtilityButtonsOnSwipe(SWTableViewCell cell) {
            // allow just one cell's utility button to be open at once
            return true;
        }

        public override bool CanSwipeToState(SWTableViewCell cell, SWCellState state) {
            switch (state) {
                case SWCellState.Left:
                    // set to false to disable all left utility buttons appearing
                    return false;
                case SWCellState.Right:
                    // set to false to disable all right utility buttons appearing
                    return true;
            }
            return true;
        }
    }
}
