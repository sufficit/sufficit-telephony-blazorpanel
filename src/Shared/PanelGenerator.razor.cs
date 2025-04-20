using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.Telephony.EventsPanel;
using System.Text.Json;

namespace Sufficit.Telephony.BlazorPanel.Shared
{
    public partial class PanelGenerator : ComponentBase, IDisposable
    {
        [Inject]
        private EventsPanelService EPService { get; set; } = default!;

        [Inject]
        private APIClientService APIClient { get; set; } = default!;

        [Inject]
        IServiceProvider SProvider { get; set; } = default!;

        [Inject]
        ILogger<PanelGenerator> Logger { get; set; } = default!;

        public Panel Panel { get; internal set; } = default!;

        /// <summary>
        /// Important for avoid duplicate event processing
        /// </summary>
        public void Dispose()
        {
            Panel?.Dispose();
        }

        public EventsPanelCardCollection Cards { get; internal set; }

        public PanelGenerator()
        {
            Cards = new EventsPanelCardCollection();            
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Panel = new Panel(Cards, EPService);

            // ----- Get options and cards from settings file

            var optionsFromSettings = SProvider.GetService<IOptions<EventsPanelServiceOptions>>();
            if (optionsFromSettings != null)
            {
                Logger.LogDebug($"applying cards from settings");
                Panel.Update(optionsFromSettings.Value);
            }
            else Logger.LogDebug("no cards from settings");

            var cardsFromSettings = SProvider.GetService<IOptions<EventsPanelServiceCards>>();
            if (cardsFromSettings != null)
            {
                Logger.LogDebug($"({ cardsFromSettings.Value.Count }) cards from settings");
                Panel.Update(cardsFromSettings.Value, true);
            }
            else Logger.LogDebug("no cards from settings");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (false && firstRender)
            {
                // Getting options
                var options = await APIClient.Telephony.EventsPanel.GetUserOptions();
                if (options != null)
                {
                    var weps = JsonSerializer.Serialize(options);
                    Console.WriteLine(weps);
                    
                    Panel.Update(options);
                }
                else { Console.WriteLine("null options received from api"); }

                // Getting options
                var cards = await APIClient.Telephony.EventsPanel.GetCardsByUser(CancellationToken.None);
                if (cards != null)
                {
                    var weps = JsonSerializer.Serialize(cards);
                    Console.WriteLine(weps);

                    Panel.Update(cards, true);
                }
                else { Console.WriteLine("null cards received from api"); }
            }
        }                
    }
}
