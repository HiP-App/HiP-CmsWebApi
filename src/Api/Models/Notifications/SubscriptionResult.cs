using Api.Models.Entity;
using Api.Models.Notifications;

public class SubscriptionResult
{
	public SubscriptionResult(Subscription sub)
    {
        this.Id = sub.SubscriptionId;
        this.SubscriberId = sub.SubscriberId;
        this.Type = sub.Type;
	}

    public int Id { get; private set; }
    public int SubscriberId { get; private set; }
    public NotificationType Type { get; private set; }
}
