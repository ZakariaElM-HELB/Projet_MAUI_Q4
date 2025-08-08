using CommunityToolkit.Mvvm.ComponentModel;

namespace MyApp.ViewModel
{
    public partial class ColumnSelection : ObservableObject
    {
        public string Name { get; }

        [ObservableProperty]
        private bool isSelected;

        public ColumnSelection(string name, bool selected)
        {
            Name = name;
            isSelected = selected;
        }
    }
}
