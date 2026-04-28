using System;

namespace IncidentDesk.Models
{
    public class Incident
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Severity { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime DateReported { get; set; }

        public Incident()
        {
            DateReported = DateTime.Now;
        }

        public virtual string GetSummary()
        {
            return $"{Title} - {Severity} - {Status}";
        }
    }
}