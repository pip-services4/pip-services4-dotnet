using PipServices4.Components.Context;
using PipServices4.Components.Run;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Messaging.Queues
{
    /// <summary>
    /// Interface for asynchronous message queues.
    /// 
    /// Not all queues may implement all the methods.
    /// Attempt to call non-supported method will result in NotImplemented exception.
    /// To verify if specific method is supported consult with MessagingCapabilities.
    /// </summary>
    /// See <see cref="MessageEnvelope"/>, <see cref="MessagingCapabilities"/>
    public interface IMessageQueue : IOpenable
    {
        /// <summary>
        /// Gets the queue name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the queue capabilities
        /// </summary>
        MessagingCapabilities Capabilities { get; }

        /// <summary>
        /// Reads the current number of messages in the queue to be delivered.
        /// </summary>
        /// <returns>Number of messages to be delivered</returns>
        Task<long> ReadMessageCountAsync();
    
        /// <summary>
        /// Sends a message into the queue.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="envelope">a message envelop to be sent.</param>
        Task SendAsync(IContext context, MessageEnvelope envelope);

        /// <summary>
        /// Sends an object into the queue. Before sending the object is converted into JSON string and wrapped in a MessageEnvelop.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="messageType">a message type</param>
        /// <param name="message">an object value to be sent</param>
        Task SendAsObjectAsync(IContext context, string messageType, object message);

        /// <summary>
        /// Peeks a single incoming message from the queue without removing it. If there
        /// are no messages available in the queue it returns null.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <returns>a message envelop object.</returns>
        Task<MessageEnvelope> PeekAsync(IContext context);

        /// <summary>
        /// Peeks multiple incoming messages from the queue without removing them. If
        /// there are no messages available in the queue it returns an empty list.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="messageCount">a maximum number of messages to peek.</param>
        /// <returns> list with messages.</returns>
        Task<List<MessageEnvelope>> PeekBatchAsync(IContext context, int messageCount);

        /// <summary>
        /// Receives an incoming message and removes it from the queue.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="waitTimeout">a timeout in milliseconds to wait for a message to come.</param>
        /// <returns>a message envelop object.</returns>
        Task<MessageEnvelope> ReceiveAsync(IContext context, long waitTimeout);

        /// <summary>
        /// Renews a lock on a message that makes it invisible from other receivers in
        /// the queue.This method is usually used to extend the message processing time.
        /// </summary>
        /// <param name="message">a message to extend its lock.</param>
        /// <param name="lockTimeout">a locking timeout in milliseconds.</param>
        Task RenewLockAsync(MessageEnvelope message, long lockTimeout);

        /// <summary>
        /// Permanently removes a message from the queue. This method is usually used to
        /// remove the message after successful processing.
        /// </summary>
        /// <param name="message">a message to remove.</param>
        Task CompleteAsync(MessageEnvelope message);

        /// <summary>
        /// Returns message into the queue and makes it available for all subscribers to
        /// receive it again.This method is usually used to return a message which could
        /// not be processed at the moment to repeat the attempt.Messages that cause
        /// unrecoverable errors shall be removed permanently or/and send to dead letter queue.
        /// </summary>
        /// <param name="message">a message to return.</param>
        Task AbandonAsync(MessageEnvelope message);

        /// <summary>
        /// Permanently removes a message from the queue and sends it to dead letter queue.
        /// </summary>
        /// <param name="message">a message to be removed.</param>
        Task MoveToDeadLetterAsync(MessageEnvelope message);

        /// <summary>
        /// Listens for incoming messages and blocks the current thread until queue is closed.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="receiver">a receiver to receive incoming messages.</param>
        Task ListenAsync(IContext context, IMessageReceiver receiver);
        Task ListenAsync(IContext context, Func<MessageEnvelope, IMessageQueue, Task> callback);

        /// <summary>
        /// Listens for incoming messages without blocking the current thread.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="receiver">a receiver to receive incoming messages.</param>
        void BeginListen(IContext context, IMessageReceiver receiver);
        void BeginListen(IContext context, Func<MessageEnvelope, IMessageQueue, Task> callback);

        /// <summary>
        /// Ends listening for incoming messages. When this method is call listen() 
        /// unblocks the thread and execution continues.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        void EndListen(IContext context);
    }
}