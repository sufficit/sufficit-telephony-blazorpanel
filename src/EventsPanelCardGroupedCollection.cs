using Sufficit.Telephony.EventsPanel;

namespace Sufficit.Telephony.BlazorPanel
{

    public class EventsPanelCardGroupedCollection : EventsPanelCardCollection
    {
        public override EventsPanelCardMonitor Add(EventsPanelCard card)
        {
            var monitor = Create(card);
            if(!Contains(monitor))
                base.Add(monitor);

            return monitor;
        }

        public override EventsPanelCardMonitor Create(EventsPanelCard card)
        {
            if (card.Label.StartsWith("000"))
            {
                var client = card.Label.Substring(0, 6);
                var cardClient = this[client];
                if (cardClient.Any())
                {
                    return cardClient.First();
                }
                else
                {
                    card.Label = client;
                    card.Channels.Clear();
                    card.Channels.Add("^SIP/" + client);
                    return new EventsPanelReadOnlyCardMonitor(card);
                }
            }
            else
            {
                return base.Create(card);
            }            
        }
    }
}
