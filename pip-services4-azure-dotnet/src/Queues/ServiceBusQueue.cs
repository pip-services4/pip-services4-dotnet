using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Mossharbor.AzureWorkArounds.ServiceBus;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Config.Auth;
using PipServices4.Config.Connect;
using PipServices4.Messaging.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMessageReceiver = PipServices4.Messaging.Queues.IMessageReceiver;

namespace PipServices4.Azure.Queues
{
    public class ServiceBusMessageQueue : MessageQueue
    {
        private string _queueName;
        private string _connectionString;

        private IQueueClient _queueClient;
        private NamespaceManager _namespaceManager;
        private MessageReceiver _messageReceiver;

        public ServiceBusMessageQueue()
            : this(null)
        {
        }

        public ServiceBusMessageQueue(string name = null)
        {
            Name = name;
            Capabilities = new MessagingCapabilities(true, true, true, true, true, true, true, true, true);
        }

        public ServiceBusMessageQueue(string name, ConfigParams config)
            : this(name)
        {
            if (config != null)
            {
                Configure(config);
            }
        }

        private void CheckOpened(IContext context)
        {
            if (_queueClient == null || _namespaceManager == null || _messageReceiver == null)
            {
                throw new InvalidStateException(context != null ? ContextResolver.GetTraceId(context) : null, "NOT_OPENED", "The queue is not opened");
            }
        }

        public override bool IsOpen()
        {
            return _queueClient != null && _namespaceManager != null && _messageReceiver != null;
        }

        public override async Task OpenAsync(IContext context, List<ConnectionParams> connections, CredentialParams credential)
        {
            try
            {
                var connection = connections?.FirstOrDefault();
                if (connection == null)
                {
                    throw new ArgumentNullException(nameof(connections));
                }
                
                _queueName = connection.GetAsNullableString("queue") ?? Name;

                _connectionString = ConfigParams.FromTuples(
                    "Endpoint", connection.GetAsNullableString("uri") ?? connection.GetAsNullableString("Endpoint"),
                    "SharedAccessKeyName", credential.AccessId ?? credential.GetAsNullableString("SharedAccessKeyName"),
                    "SharedAccessKey", credential.AccessKey ?? credential.GetAsNullableString("SharedAccessKey")
                ).ToString();

                _logger.Info(null, "Connecting queue {0} to {1}", Name, _connectionString);

                _queueClient = new QueueClient(_connectionString, _queueName);
                _namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
                _messageReceiver = new MessageReceiver(_connectionString, _queueName);
            }
            catch (Exception ex)
            {
                _queueClient = null;
                _namespaceManager = null;
                _messageReceiver = null;

                _logger.Error(context, ex, $"Failed to open queue '{Name}'.");
            }

            await Task.CompletedTask;
        }

        public override async Task CloseAsync(IContext context)
        {
            await _queueClient.CloseAsync();

            _logger.Trace(context, "Closed queue {0}", this);
        }

        public override Task<long> ReadMessageCountAsync()
        {
            CheckOpened(null);
            var queueDescription = _namespaceManager.GetQueue(_queueName);
            return Task.FromResult(queueDescription.MessageCount);
        }

        private MessageEnvelope ToMessage(Message envelope, bool withLock = true)
        {
            if (envelope == null)
            {
                return null;
            }

            var message = new MessageEnvelope
            {
                MessageType = envelope.ContentType,
                TraceId = envelope.CorrelationId,
                MessageId = envelope.MessageId,
                SentTime = envelope.ScheduledEnqueueTimeUtc,
                Message = envelope.Body,
            };

            if (withLock)
            {
                message.Reference = envelope.SystemProperties?.LockToken;
            }

            return message;
        }

        public override async Task SendAsync(IContext context, MessageEnvelope message)
        {
            CheckOpened(context);
            
            var envelope = new Message(message.Message)
            {
                ContentType = message.MessageType,
                CorrelationId = message.TraceId,
                MessageId = message.MessageId
            };

            await _queueClient.SendAsync(envelope);

            _counters.IncrementOne("queue." + Name + ".sent_messages");
            _logger.Debug(Context.FromTraceId(message.TraceId), "Sent message {0} via {1}", message, this);
        }

        public override async Task<MessageEnvelope> PeekAsync(IContext context)
        {
            CheckOpened(context);
            var envelope = await _messageReceiver.PeekAsync();
            var message = ToMessage(envelope, false);

            if (message != null)
            {
                _logger.Trace(Context.FromTraceId(message.TraceId), "Peeked message {0} on {1}", message, this);
            }

            return message;
        }

        public override async Task<List<MessageEnvelope>> PeekBatchAsync(IContext context, int messageCount)
        {
            CheckOpened(context);

            var messages = new List<MessageEnvelope>();

            for (var count = 0; count < messageCount; count++)
            {
                messages.Add(await PeekAsync(context));
            }
            
            _logger.Trace(context, "Peeked {0} messages on {1}", messages.Count, this);

            return messages;
        }

        public override async Task<MessageEnvelope> ReceiveAsync(IContext context, long waitTimeout)
        {
            CheckOpened(context);
            var envelope = await _messageReceiver.ReceiveAsync(TimeSpan.FromMilliseconds(waitTimeout));
            var message = ToMessage(envelope);

            if (message != null)
            {
                _counters.IncrementOne("queue." + Name + ".received_messages");
                _logger.Debug(Context.FromTraceId(message.TraceId), "Received message {0} via {1}", message, this);
            }

            return message;
        }

        public override async Task RenewLockAsync(MessageEnvelope message, long lockTimeout)
        {
            var context = Context.FromTraceId(message.TraceId);
            CheckOpened(context);

            var reference = message.Reference?.ToString();

            if (!string.IsNullOrWhiteSpace(reference))
            {
                await _messageReceiver.RenewLockAsync(message.Reference?.ToString());
                _logger.Trace(context, "Renewed lock for message {0} at {1}", message, this);
            }
        }

        public override async Task AbandonAsync(MessageEnvelope message)
        {
            var context = Context.FromTraceId(message.TraceId);
            CheckOpened(context);

            var reference = message.Reference?.ToString();

            if (!string.IsNullOrWhiteSpace(reference))
            {
                await _messageReceiver.AbandonAsync(reference);
                message.Reference = null;
                _logger.Trace(context, "Abandoned message {0} at {1}", message, this);
            }
        }

        public override async Task CompleteAsync(MessageEnvelope message)
        {
            var context = Context.FromTraceId(message.TraceId);
            CheckOpened(context);

            var reference = message.Reference?.ToString();

            if (!string.IsNullOrWhiteSpace(reference))
            {
                await _queueClient.CompleteAsync(reference);
                message.Reference = null;
                _logger.Trace(context, "Completed message {0} at {1}", message, this);
            }
        }

        public override async Task MoveToDeadLetterAsync(MessageEnvelope message)
        {
            var context = Context.FromTraceId(message.TraceId);
            CheckOpened(context);

            var reference = message.Reference?.ToString();

            if (!string.IsNullOrWhiteSpace(reference))
            {
                await _queueClient.DeadLetterAsync(reference);
                message.Reference = null;
                _counters.IncrementOne("queue." + Name + ".dead_messages");
                _logger.Trace(context, "Moved to dead message {0} at {1}", message, this);
            }
        }

        public override async Task ListenAsync(IContext context, IMessageReceiver receiver)
        {
            CheckOpened(context);
            _logger.Trace(context, "Started listening messages at {0}", this);

            _queueClient.RegisterMessageHandler(async (envelope, cancellationToken) =>
                {
                    var message = ToMessage(envelope);

                    if (message != null)
                    {
                        _counters.IncrementOne("queue." + Name + ".received_messages");
                        _logger.Debug(Context.FromTraceId(message.TraceId), "Received message {0} via {1}", message, this);
                    }

                    try
                    {
                        await receiver.ReceiveMessageAsync(message, this);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(context, ex, "Failed to process the message");
                        throw;
                    }

                    await _queueClient.CompleteAsync(envelope.SystemProperties.LockToken);
                },
                new MessageHandlerOptions(exception => 
                    {
                        _logger.Error(context, exception.Exception, "Failed to process the message");
                        return Task.CompletedTask;
                    })
                {
                    AutoComplete = false,
                    MaxConcurrentCalls = 1
                });
            
            await Task.CompletedTask;
        }

        public override void EndListen(IContext context)
        {
            CheckOpened(context);

            // Close old client
            try
            {
                if (_queueClient != null && !_queueClient.IsClosedOrClosing)
                {
                    _queueClient.CloseAsync().Wait();
                    _queueClient = null;
                }

                if (_messageReceiver != null && !_messageReceiver.IsClosedOrClosing)
                {
                    _messageReceiver.CloseAsync().Wait();
                    _messageReceiver = null;
                }
            }
            catch
            {
                // Ignore...
            }

            // Create a new client
            _queueClient = new QueueClient(_connectionString, _queueName);
            _messageReceiver = new MessageReceiver(_connectionString, _queueName);
        }

        public override async Task ClearAsync(IContext context)
        {
            CheckOpened(context);

            while (true)
            {
                var envelope = await _messageReceiver.ReceiveAsync(TimeSpan.FromMilliseconds(0));
                if (envelope == null)
                {
                    break;
                }

                await _messageReceiver.CompleteAsync(envelope.SystemProperties.LockToken);
            }

            _logger.Trace(context, "Cleared queue {0}", this);
        }
    }
}
