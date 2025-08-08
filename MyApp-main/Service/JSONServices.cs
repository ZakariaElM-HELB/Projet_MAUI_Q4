using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyApp.Service;

public class JSONServices
{
    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MyMembers.json");
    string uploadUrl = "https://185.157.245.38:5000/json";

    internal async Task<List<Member>> GetMembers()
    {
        List<Member> membersList = new();

        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var httpClient = new HttpClient(handler);
            var downloadUrl = uploadUrl + "?FileName=MyMembers.json";

            var response = await httpClient.GetAsync(downloadUrl);

            if (response.IsSuccessStatusCode)
            {
                var rawJson = await response.Content.ReadAsStringAsync();
                membersList = JsonSerializer.Deserialize<List<Member>>(rawJson) ?? new List<Member>();

                // ✅ Patch de sécurité après désérialisation
                foreach (var member in membersList)
                {
                    // Si WeeklyVisits est null ou incomplet
                    if (member.WeeklyVisits == null || member.WeeklyVisits.Count != 7)
                    {
                        member.WeeklyVisits = new()
                    {
                        { "Lundi", 0 },
                        { "Mardi", 0 },
                        { "Mercredi", 0 },
                        { "Jeudi", 0 },
                        { "Vendredi", 0 },
                        { "Samedi", 0 },
                        { "Dimanche", 0 }
                    };
                    }

                    // Si LastResetDate est vide ou default
                    if (member.LastResetDate == default)
                    {
                        member.LastResetDate = DateTime.Today;
                    }

                    // Si Id est null (sécurité)
                    if (string.IsNullOrWhiteSpace(member.Id))
                    {
                        member.Id = Guid.NewGuid().ToString(); // pour éviter les erreurs fatales
                    }
                }

                Console.WriteLine($"✅ {membersList.Count} membres chargés depuis le serveur !");
            }
            else
            {
                Console.WriteLine($"❌ Erreur serveur : {response.StatusCode}");
                await Shell.Current.DisplayAlert("Erreur serveur", "Impossible de récupérer les membres.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur de chargement", $"Impossible de charger les membres depuis le serveur : {ex.Message}", "OK");
        }

        return membersList;
    }

    /// Méthode pour charger les membres depuis le fichier JSON local sur Desktop
    internal async Task<List<Member>> GetMembersDesktop()
    {
        List<Member> membersList = new();

        try
        {
            if (!File.Exists(filePath))
            {
                await Shell.Current.DisplayAlert("Fichier introuvable", "Le fichier JSON est introuvable. Un nouveau sera créé.", "OK");
                return new List<Member>();
            }

            string jsonContent = await File.ReadAllTextAsync(filePath);
            membersList = JsonSerializer.Deserialize<List<Member>>(jsonContent) ?? new List<Member>();
            await Shell.Current.DisplayAlert("Chargement local", $"{membersList.Count} membres chargés depuis le fichier local.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur JSON", $"Impossible de lire le fichier JSON : {ex.Message}", "OK");
        }

        return membersList;
    }


    internal async Task SetMembers()
    {
        try
        {
            string json = JsonSerializer.Serialize(Globals.MyMembers, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
            Console.WriteLine("✅ Fichier JSON local mis à jour.");

            await UploadJsonToServer();
            Console.WriteLine("✅ Sauvegarde complète terminée.");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur de sauvegarde JSON", ex.Message, "OK");
        }
    }


    private async Task UploadJsonToServer()
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using HttpClient client = new(handler);

            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            var content = new MultipartFormDataContent
            {
                { fileContent, "file", "MyMembers.json" }
            };

            var response = await client.PostAsync(uploadUrl, content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("✅ JSON envoyé au serveur distant avec succès !");
            else
                Console.WriteLine($"❌ Échec de l'envoi du JSON : {response.StatusCode}");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur d'envoi serveur", ex.Message, "OK");
        }
    }

    internal static async Task<Member?> GetMemberByIdAsync(string id)
    {
        var service = new JSONServices();
        var members = await service.GetMembers();
        return members.FirstOrDefault(m => m.Id == id);
    }

    public async Task ForceUploadJson()
    {
        await UploadJsonToServer();
    }

}
