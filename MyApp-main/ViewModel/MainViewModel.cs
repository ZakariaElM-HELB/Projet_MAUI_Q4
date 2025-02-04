using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ViewModel;

public partial class MainViewModel: ObservableObject
{
    [ObservableProperty]
    private string myVar = "Blabla";

    [RelayCommand]
    internal void ChangeBindedLabel()
    {
        MyVar += "blabla";
    }
}
