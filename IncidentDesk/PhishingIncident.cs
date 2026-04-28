namespace IncidentDesk.Models
{
    public class PhishingIncident : Incident
    {
        public string SenderEmail { get; set; }

        public override string GetSummary()
        {
            return $"{Title} - Phishing - {Severity} - {Status}";
        }
    }
}