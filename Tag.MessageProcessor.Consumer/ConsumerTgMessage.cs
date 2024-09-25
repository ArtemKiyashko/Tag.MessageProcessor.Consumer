using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Tag.MessageProcessor.Consumer
{
    public class ConsumerTgMessage(ILogger<ConsumerTgMessage> logger)
    {
        private readonly ILogger<ConsumerTgMessage> _logger = logger;

        [Function(nameof(ConsumerTgMessage))]
        public async Task Run(
            [ServiceBusTrigger(topicName: "tgmessages", subscriptionName: "messageprocessor", IsSessionsEnabled = true, Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            try
            {
                var tgUpdate = JsonConvert.DeserializeObject<Update>(message.Body.ToString());
            }
            catch (Exception)
            {
                await messageActions.DeadLetterMessageAsync(message);
            }

            
        }
    }
}
