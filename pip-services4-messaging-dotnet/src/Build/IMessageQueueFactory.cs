using System;
using PipServices4.Messaging.Queues;

namespace PipServices4.Messaging.Build
{
    /// <summary>
    /// Creates message queue components
    /// </summary>
    public interface IMessageQueueFactory
    {
        /// <summary>
        /// Creates a message queue component and assigns its name.
        /// </summary>
        /// <param name="name">name of the created message queue</param>
        /// <returns>A created message queue</returns>
        IMessageQueue CreateQueue(string name);
    }
}
