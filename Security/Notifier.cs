using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nistec.Logging;

namespace Nistec.Web.Security
{

    public static class NotifierExtensions
    {
        /// <summary>
        /// Adds a new UI notification of type Information
        /// </summary>
        /// <seealso cref="INotifier.Add()"/>
        /// <param name="message">A localized message to display</param>
        public static void Info(this INotifier notifier, LocalizedString message)
        {
            notifier.Add(NotifyType.Info, message);
        }

        /// <summary>
        /// Adds a new UI notification of type Warning
        /// </summary>
        /// <seealso cref="INotifier.Add()"/>
        /// <param name="message">A localized message to display</param>
        public static void Warning(this INotifier notifier, LocalizedString message)
        {
            notifier.Add(NotifyType.Warning, message);
        }

        /// <summary>
        /// Adds a new UI notification of type Error
        /// </summary>
        /// <seealso cref="INotifier.Add()"/>
        /// <param name="message">A localized message to display</param>
        public static void Error(this INotifier notifier, LocalizedString message)
        {
            notifier.Add(NotifyType.Error, message);
        }
    }

    public enum NotifyType
    {
        Info,
        Warning,
        Error
    }

    public class NotifyEntry
    {
        public NotifyType Type { get; set; }
        public LocalizedString Message { get; set; }
    }

    /// <summary>
    /// Notification manager for UI notifications
    /// </summary>
    /// <remarks>
    /// Where such notifications are displayed depends on the theme used. Default themes contain a 
    /// Messages zone for this.
    /// </remarks>
    public interface INotifier : IDependency
    {
        /// <summary>
        /// Adds a new UI notification
        /// </summary>
        /// <param name="type">
        /// The type of the notification (notifications with different types can be displayed differently)</param>
        /// <param name="message">A localized message to display</param>
        void Add(NotifyType type, LocalizedString message);

        /// <summary>
        /// Get all notifications added
        /// </summary>
        IEnumerable<NotifyEntry> List();
    }

    public class Notifier : INotifier
    {
        private readonly IList<NotifyEntry> _entries;

        public Notifier()
        {
            Log = Logger.Instance;
            _entries = new List<NotifyEntry>();
        }

        public ILogger Log { get; set; }

        public void Add(NotifyType type, LocalizedString message)
        {
            Log.Info("Notification {0} message: {1}", type, message);
            _entries.Add(new NotifyEntry { Type = type, Message = message });
        }

        public IEnumerable<NotifyEntry> List()
        {
            return _entries;
        }
    }
}
