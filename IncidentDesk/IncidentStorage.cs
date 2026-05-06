using IncidentDesk.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace IncidentDesk.Services
{
    public class IncidentStorage
    {
        // File path for storing incidents
        private readonly string filePath = "incidents.json";

        public void Save(List<Incident> incidents)
        {
            // Serialize incidents to JSON and save to file
            string json = JsonSerializer.Serialize(incidents, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
        }

        public List<Incident> Load()
        {
            // Load incidents from file, or return an empty list if the file doesn't exist
            if (!File.Exists(filePath))
            {
                return new List<Incident>();
            }

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Incident>>(json) ?? new List<Incident>();
        }
    }
}