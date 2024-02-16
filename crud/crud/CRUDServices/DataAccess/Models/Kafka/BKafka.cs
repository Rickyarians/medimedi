namespace CRUDServices.Models.Kafka;

public class BKafka
{
    public BKafka()
    {
        Timestamp = DateTime.Now;
        Event = "NotificationTriggered";
        Application = "Claim.API";
        Payload = "";
    }

    public DateTime Timestamp { get; set; }
    public string Event { get; set; }
    public string Application { get; set; }
    public string Payload { get; set; }
}