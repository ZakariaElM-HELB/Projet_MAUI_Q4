using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp.Service;
using System.Collections.ObjectModel;

namespace MyApp.ViewModel;

public partial class SelectColumnsViewModel : ObservableObject
{
    public ObservableCollection<ColumnSelection> ColumnNames { get; } = new();
    private readonly List<Member> _members;
    private readonly CSVServices _csvService = new();

    public SelectColumnsViewModel(List<Member> members)
    {
        _members = members;

        var props = typeof(Member).GetProperties();
        foreach (var prop in props)
        {
            ColumnNames.Add(new ColumnSelection(prop.Name, true));
        }
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        var selected = ColumnNames
            .Where(c => c.IsSelected)
            .Select(c => c.Name)
            .ToList();

        await _csvService.PrintData(_members, selected);
        await Shell.Current.Navigation.PopAsync();
    }
}
