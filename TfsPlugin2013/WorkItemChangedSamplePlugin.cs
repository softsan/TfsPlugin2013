namespace TfsPlugin2013
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.TeamFoundation.Common;
    using Microsoft.TeamFoundation.Framework.Server;
    using Microsoft.TeamFoundation.WorkItemTracking.Server;

    /// <summary>
    /// Define TFS server plugins for handling TFS change events.
    /// </summary>
    public sealed class WorkItemChangedSamplePlugin : ISubscriber
    {
        #region Properties
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return "WorkItemChangedSamplePlugin";
            }
        }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        public SubscriberPriority Priority
        {
            get
            {
                return SubscriberPriority.Normal;
            }
        }

        #endregion

        /// <summary>
        /// The process event.
        /// </summary>
        /// <param name="requestContext">  The request context. </param>
        /// <param name="notificationType"> The notification type. </param>
        /// <param name="notificationEventArgs"> The notification event args. </param>
        /// <param name="statusCode"> The status code. </param>
        /// <param name="statusMessage"> The status message. </param>
        /// <param name="properties"> The properties. </param>
        /// <returns> The <see cref="EventNotificationStatus"/>. </returns>
        public EventNotificationStatus ProcessEvent(
            TeamFoundationRequestContext requestContext,
            NotificationType notificationType,
            object notificationEventArgs,
            out int statusCode,
            out string statusMessage,
            out ExceptionPropertyCollection properties)
        {
            statusCode = 0;
            properties = null;
            statusMessage = string.Empty;
            try
            {
                return Task.Run(() => UpdateLog(requestContext, notificationType, notificationEventArgs)).Result;
            }
            catch (Exception ex)
            {
                Logger.Log("Exception: " + ex.Message + "\n" + ex.StackTrace);
            }

            return EventNotificationStatus.ActionPermitted;
        }


        /// <summary>
        /// Subscribed types.
        /// </summary>
        /// <returns>  array of Work item type changed  </returns>
        public Type[] SubscribedTypes()
        {
            return new[] { typeof(WorkItemChangedEvent) };
        }

        /// <summary>
        /// The update log.
        /// </summary>
        /// <param name="requestContext">  The request context. </param>
        /// <param name="notificationType">  The notification type. </param>
        /// <param name="notificationEventArgs"> The notification event args. </param>
        /// <returns> The <see cref="Task"/>. </returns>
        private static async Task<EventNotificationStatus> UpdateLog(
            TeamFoundationRequestContext requestContext,
            NotificationType notificationType,
            object notificationEventArgs)
        {
            return await Task.Run(
                () =>
                    {
                        if (notificationType == NotificationType.Notification
                            && notificationEventArgs is WorkItemChangedEvent)
                        {
                            var notificationEvent = notificationEventArgs as WorkItemChangedEvent;

                            if (notificationEvent.CoreFields != null
                                && notificationEvent.CoreFields.StringFields != null
                                && notificationEvent.CoreFields.IntegerFields != null)
                            {
                                Logger.Log("event called");
                                var uri = GetTfsUri(requestContext);
                                Logger.Log("URI " + uri);

                                var workItemType =
                                    notificationEvent.CoreFields.StringFields.Where(
                                        field => field.ReferenceName == "System.WorkItemType")
                                        .Select(field => field.NewValue)
                                        .FirstOrDefault();
                                if (notificationEvent.ChangeType == ChangeTypes.Change
                                    && notificationEvent.CoreFields.IntegerFields.Length > 0
                                    && (workItemType != null && (workItemType == "Task"))) // you can specify any work item type here
                                {
                                    Logger.Log("work item Task found");
                                }
                            }
                        }
                        return EventNotificationStatus.ActionPermitted;
                    });
        }

        /// <summary>
        /// Get TFS uri from request context.
        /// </summary>
        /// <param name="requestContext">  The request Context. </param>
        /// <returns> TFS Uri    </returns>
        private static Uri GetTfsUri(TeamFoundationRequestContext requestContext)
        {
            var locationService = requestContext.GetService<TeamFoundationLocationService>();
            var uri =
                new Uri(
                    string.Format("{0}/{1}", locationService.GetServerAccessMapping(requestContext).AccessPoint, requestContext.ServiceHost.Name));

            return uri;
        }
    }
}