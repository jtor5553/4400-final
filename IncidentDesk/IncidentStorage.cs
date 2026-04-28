using IncidentDesk.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace IncidentDesk.Services
{
    public class IncidentStorage
    {
        private readonly string filePath = "incidents.json";

        public void Save(List<Incident> incidents)
        {
            string json = JsonSerializer.Serialize(incidents, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
        }

        public List<Incident> Load()
        {
            if (!File.Exists(filePath))
            {
                return new List<Incident>();
            }

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Incident>>(json) ?? new List<Incident>();
        }
    }
}