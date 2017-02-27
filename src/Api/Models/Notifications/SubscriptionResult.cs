using Api.Models.Entity;
using Api.Models.Notifications;

public class SubscriptionResult
{
	public SubscriptionResult(Subscription sub)
    {
        Id = sub.SubscriptionId;
        SubscriberId = sub.SubscriberId;
        Type = sub.Type;
	}

    public int Id { get; set; }
    public int SubscriberId { get; set; }
    public NotificationType Type { get; set; }
}
