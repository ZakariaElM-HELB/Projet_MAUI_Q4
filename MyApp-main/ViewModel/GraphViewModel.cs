using Microcharts;
using SkiaSharp;
using MyApp.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MyApp.ViewModel;

public partial class GraphViewModel : ObservableObject
{
    [ObservableProperty]
    private Chart myWeeklyChart;

    [ObservableProperty]
    private string lastActionMessage;

    [ObservableProperty]
    private bool isBusy;

    private Member _member;

    public GraphViewModel(Member member)
    {
        _member = member;
        Console.WriteLine($"🔍 GraphViewModel instancié avec ID : {_member.Id}");

        var found = Globals.MyMembers.Any(m => m.Id == _member.Id);
        Console.WriteLine(found ? "✅ Membre trouvé dans Globals." : "❌ Membre PAS dans Globals !!");

        _ = ResetWeeklyVisitsIfNeeded();
        BuildWeeklyVisitChart();
    }




    [RelayCommand]
    public async Task IncrementTodayVisit()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var jour = TranslateDayToFrench(DateTime.Now.DayOfWeek.ToString());

            if (_member.WeeklyVisits.ContainsKey(jour))
                _member.WeeklyVisits[jour]++;
            else
                _member.WeeklyVisits[jour] = 1;

            _member.VisitCount++;

            await BuildWeeklyVisitChart();

            var success = await SaveMemberData();

            if (success)
            {
                await Shell.Current.DisplayAlert("Succès", $"Visite ajoutée pour {jour} et sauvegardée dans le JSON.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", $"Visite ajoutée pour {jour} mais NON SAUVEGARDÉE (ID introuvable).", "OK");
            }

            LastActionMessage = $"✅ Visite ajoutée pour {jour}";
            await Task.Delay(3000);
            LastActionMessage = string.Empty;
        }
        finally
        {
            IsBusy = false;
        }
    }





    private async Task BuildWeeklyVisitChart()
    {
        var entries = _member.WeeklyVisits.Select(kv => new ChartEntry(kv.Value)
        {
            Label = kv.Key,
            ValueLabel = kv.Value.ToString(),
            Color = SKColor.Parse("#66BB6A"),
            TextColor = SKColors.Black,
            ValueLabelColor = SKColors.Black
        }).ToList();

        MyWeeklyChart = null;
        await Task.Delay(50);

        MyWeeklyChart = new BarChart
        {
            Entries = entries,
            LabelOrientation = Orientation.Horizontal,
            ValueLabelOrientation = Orientation.Vertical,
            MaxValue = Math.Max(_member.WeeklyVisits.Values.Max() + 1, 5),
            BackgroundColor = SKColors.Transparent,
            BarAreaAlpha = 220
        };
    }

    private async Task ResetWeeklyVisitsIfNeeded()
    {
        var today = DateTime.Today;

        if (today.DayOfWeek == DayOfWeek.Monday && _member.LastResetDate < today)
        {
            foreach (var key in _member.WeeklyVisits.Keys.ToList())
            {
                _member.WeeklyVisits[key] = 0;
            }

            _member.LastResetDate = today;
            await SaveMemberData();
            await BuildWeeklyVisitChart(); // ✅ ajoute cette ligne
        }
    }



    private async Task<bool> SaveMemberData()
    {
        var index = Globals.MyMembers.FindIndex(m => m.Id == _member.Id);

        if (index != -1)
        {
            Globals.MyMembers[index] = _member;
            await new Service.JSONServices().SetMembers();
            Console.WriteLine($"✅ Membre ID {_member.Id} sauvegardé.");
            return true;
        }
        else
        {
            Console.WriteLine($"❌ Membre ID {_member.Id} introuvable dans Globals.MyMembers.");
            return false;
        }
    }




    private string TranslateDayToFrench(string englishDay) => englishDay switch
    {
        "Monday" => "Lundi",
        "Tuesday" => "Mardi",
        "Wednesday" => "Mercredi",
        "Thursday" => "Jeudi",
        "Friday" => "Vendredi",
        "Saturday" => "Samedi",
        "Sunday" => "Dimanche",
        _ => englishDay
    };
}
