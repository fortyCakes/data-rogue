namespace data_rogue_core.Forms
{
    public class FormSelection
    {
        private string selectedItem;

        public string SelectedItem {
            get => selectedItem;
            set
            {
                selectedItem = value;
                SubItem = null;
            }
        }
        public string SubItem { get; set; }
    }
}