using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Service;

public class CSVServices
{
    public async Task<List<Member>> LoadData()
    {
        List<Member> list = new();
        var erreurs = new List<string>();

        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Sélectionnez un fichier CSV"
        });

        if (result != null)
        {
            var lines = await File.ReadAllLinesAsync(result.FullPath, Encoding.UTF8);

            if (lines.Length < 2)
                return list;

            var headers = lines[0].Split(';');

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var values = line.Split(';');

                if (values.Length != headers.Length)
                {
                    erreurs.Add($"Ligne {i + 1} : nombre de colonnes incorrect ({values.Length} au lieu de {headers.Length})");
                    continue;
                }

                var obj = new Member();

                // Initialiser le dictionnaire
                obj.WeeklyVisits = new()
            {
                { "Lundi", 0 },
                { "Mardi", 0 },
                { "Mercredi", 0 },
                { "Jeudi", 0 },
                { "Vendredi", 0 },
                { "Samedi", 0 },
                { "Dimanche", 0 }
            };

                for (int j = 0; j < headers.Length; j++)
                {
                    var col = headers[j];
                    var val = values[j];

                    try
                    {
                        if (col.StartsWith("Weekly_", StringComparison.OrdinalIgnoreCase))
                        {
                            var day = col.Substring("Weekly_".Length);
                            if (obj.WeeklyVisits.ContainsKey(day))
                                obj.WeeklyVisits[day] = int.TryParse(val, out var count) ? count : 0;
                        }
                        else if (col == "LastResetDate")
                        {
                            obj.LastResetDate = DateTime.TryParse(val, out var date) ? date : DateTime.Today;
                        }
                        else
                        {
                            var prop = typeof(Member).GetProperty(col);
                            if (prop != null && prop.CanWrite && prop.SetMethod?.IsPublic == true)
                            {
                                object value = Convert.ChangeType(val, prop.PropertyType);
                                prop.SetValue(obj, value);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        erreurs.Add($"Ligne {i + 1}, colonne '{col}' : {ex.Message}");
                    }
                }

                list.Add(obj);
            }

            if (erreurs.Any())
            {
                string message = string.Join("\n", erreurs.Take(5));
                if (erreurs.Count > 5)
                    message += $"\n...et {erreurs.Count - 5} erreur(s) supplémentaire(s)";

                await Shell.Current.DisplayAlert("Erreurs dans le CSV", message, "OK");
            }
        }

        return list;
    }




    public async Task PrintData(List<Member> data, List<string> selectedProperties)
    {
        var csv = new StringBuilder();

        // Colonnes fixes de WeeklyVisits
        var weeklyDays = new[]
        {
        "Weekly_Lundi", "Weekly_Mardi", "Weekly_Mercredi",
        "Weekly_Jeudi", "Weekly_Vendredi", "Weekly_Samedi", "Weekly_Dimanche"
    };

        // ✅ Filtrer les propriétés à éviter
        var properties = typeof(Member).GetProperties()
                                       .Where(p =>
                                           selectedProperties.Contains(p.Name) &&
                                           p.Name != "WeeklyVisits" &&
                                           p.Name != "Age" &&
                                           p.Name != "ProfileBackground")
                                       .ToList();

        var headers = properties.Select(p => p.Name).ToList();
        headers.AddRange(weeklyDays);
        headers.Add("LastResetDate");

        csv.AppendLine(string.Join(";", headers));

        // Construire les lignes
        foreach (var item in data)
        {
            var values = new List<string>();

            // Propriétés simples
            foreach (var prop in properties)
            {
                var val = prop.GetValue(item);
                values.Add(val?.ToString() ?? "");
            }

            // Colonnes Weekly_
            foreach (var day in weeklyDays)
            {
                var shortDay = day.Replace("Weekly_", "");
                values.Add(item.WeeklyVisits.TryGetValue(shortDay, out var count) ? count.ToString() : "0");
            }

            // Date de reset
            values.Add(item.LastResetDate.ToString("yyyy-MM-dd"));

            csv.AppendLine(string.Join(";", values));
        }

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));
        var fileSaverResult = await FileSaver.Default.SaveAsync("Members.csv", stream);

        if (fileSaverResult.IsSuccessful)
            await Shell.Current.DisplayAlert("Succès", "Fichier CSV exporté avec succès.", "OK");
        else
            await Shell.Current.DisplayAlert("Erreur", "Erreur lors de l'exportation du fichier CSV.", "OK");
    }



}
