using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace MyApp.Model
{
    public class Member
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string ProfilePicture { get; set; }
        public string SubscriptionType { get; set; }
        public int VisitCount { get; set; }

        // Age calculé dynamiquement (non stocké dans CSV ou JSON)
        [BsonIgnore]
        public int Age
        {
            get
            {
                if (DateTime.TryParse(BirthDate, out var birthDate))
                {
                    var today = DateTime.Today;
                    var age = today.Year - birthDate.Year;
                    if (birthDate > today.AddYears(-age)) age--;
                    return age;
                }
                return 0;
            }
        }

        // Chemin d’image selon le type d’abonnement
        [BsonIgnore]
        public string ProfileBackground => SubscriptionType switch
        {
            "Premium" => "bg_gold.jpg",
            "Standard" => "bg_silver.jpg",
            _ => "bg_bronze.jpg"
        };

        // Dictionnaire des visites par jour de la semaine
        public Dictionary<string, int> WeeklyVisits { get; set; } = new()
        {
            { "Lundi", 0 },
            { "Mardi", 0 },
            { "Mercredi", 0 },
            { "Jeudi", 0 },
            { "Vendredi", 0 },
            { "Samedi", 0 },
            { "Dimanche", 0 }
        };

        // Date du dernier reset automatique (lundi)
        public DateTime LastResetDate { get; set; } = DateTime.Today;
    }
}
