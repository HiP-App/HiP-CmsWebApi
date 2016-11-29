using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Notifications
{
    public enum NotificationType
    {
        TOPIC_CREATED,
        TOPIC_ASSIGNED_TO,
        TOPIC_REMOVED_FROM,
        TOPIC_STATE_CHANGED,
        TOPIC_DEADLINE_CHANGED,
        TOPIC_UPDATED,
        TOPIC_DELETED,
        TOPIC_ATTACHMENT_ADDED,

        UNKNOWN
            // TODO association
    }
}
