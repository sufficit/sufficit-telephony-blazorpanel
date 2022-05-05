using Sufficit.Asterisk;
using Sufficit.Telephony.EventsPanel;

namespace Sufficit.Telephony.BlazorPanel
{
    public class EventsPanelReadOnlyCardMonitor : EventsPanelCardMonitor, IChannelMatch, IEventsPanelCard
    {        
        public EventsPanelReadOnlyCardMonitor(EventsPanelCard card) : base(card)
        {

        }

        public override PeerStatus State { get => PeerStatus.Unmonitored; set { } }
    }
}
