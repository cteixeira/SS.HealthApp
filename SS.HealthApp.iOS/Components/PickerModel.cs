using System;
using System.Collections.Generic;
using UIKit;

namespace SS.HealthApp.iOS.Components {
    class PickerModel : UIPickerViewModel {
        
        protected List<Model.PickerItem> Items;
        protected int selectedItem = 0;
        protected UITextField TextField;

        public PickerModel(UITextField textField, List<Model.PickerItem> items) {
            this.TextField = textField;
            this.Items = items;
        }

        public Model.PickerItem SelectedItem {
            get {
                return Items[selectedItem];
            }
        }

        public override nint GetComponentCount(UIPickerView picker) {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView picker, nint component) {
            return Items.Count;
        }

        public override string GetTitle(UIPickerView picker, nint row, nint component) {
            return Items[(int)row].Title;

        }

        public override void Selected(UIPickerView picker, nint row, nint component) {
            selectedItem = (int)row;
            TextField.Text = Items[selectedItem].Title;
        }
        
    }
}