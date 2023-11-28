using PipServices4.Commons.Convert;
using PipServices4.Commons.Data;
using PipServices4.Components.Context;
using PipServices4.Data.Keys;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace PipServices4.Messaging.Queues
{
    /// <summary>
    /// Allows adding additional information to messages. A correlation id, message id, and a message type
    /// are added to the data being sent/received.Additionally, a MessageEnvelope can reference a lock token.
    /// 
    /// Side note: a MessageEnvelope's message is stored as a buffer, so strings are converted 
    /// using utf8 conversions.
    /// </summary>
    [DataContract]
    public class MessageEnvelope
    {
        /// <summary>
        /// Creates a new MessageEnvelope.
        /// </summary>
        public MessageEnvelope() { }

        /// <summary>
        /// Creates a new MessageEnvelop, which adds a correlation id, message id, and a
        /// type to the data being sent/received.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="messageType">a string value that defines the message's type.</param>
        /// <param name="message">the data being sent/received.</param>
        public MessageEnvelope(IContext context, string messageType, byte[] message)
        {
            TraceId = context != null ? ContextResolver.GetTraceId(context) : null;
            MessageType = messageType;
            Message = message;
            MessageId = IdGenerator.NextLong();
        }

        /// <summary>
        /// Creates a new MessageEnvelop, which adds a correlation id, message id, and a
        /// type to the data being sent/received.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="messageType">a string value that defines the message's type.</param>
        /// <param name="message">the data being sent/received.</param>
        public MessageEnvelope(IContext context, string messageType, string message)
        {
            TraceId = context != null ? ContextResolver.GetTraceId(context) : null;
            MessageType = messageType;
            SetMessageAsString(message);
            MessageId = IdGenerator.NextLong();
        }

        /// <summary>
        /// Creates a new MessageEnvelop, which adds a correlation id, message id, and a
        /// type to the data being sent/received.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="messageType">a string value that defines the message's type.</param>
        /// <param name="message">the data being sent/received.</param>
        public MessageEnvelope(IContext context, string messageType, object message)
        {
            TraceId = context != null ? ContextResolver.GetTraceId(context) : null;
            MessageType = messageType;
            SetMessageAsObject(message);
            MessageId = IdGenerator.NextLong();
        }

        /** The stored reference. */
        [IgnoreDataMember]
        public object Reference { get; set; }

        /** The unique business transaction id that is used to trace calls across components. */
        [DataMember(Name = "trace_id")]
        public string TraceId { get; set; }

        /** The message's auto-generated ID. */
        [DataMember(Name = "message_id")]
        public string MessageId { get; set; }

        /** String value that defines the stored message's type. */
        [DataMember(Name = "message_type")]
        public string MessageType { get; set; }

        /** The time at which the message was sent. */
        [DataMember(Name = "sent_time")]
        public DateTime SentTime { get; set; }

        /** The stored message. */
        [IgnoreDataMember]
        public byte[] Message { get; set; }

        /** Used for serialization */
        [DataMember(Name = "message")]
        public string MessageBase64
        {
            get
            {
                if (Message == null)
                {
                    return null;
                }
                else
                {
                    return Convert.ToBase64String(Message);
                }
            }
            set
            {
                if (value == null)
                {
                    Message = null;
                }
                else
                {
                    Message = Convert.FromBase64String(value);
                }
            }
        }

        /// <summary>
        /// Stores the given value as a string.
        /// </summary>
        /// <param name="message">A string value for this message.</param>
        public void SetMessageAsString(string message)
        {
            if (message == null)
            {
                Message = null;
            }
            else
            {
                Message = Encoding.UTF8.GetBytes(message);
            }
        }

        /// <summary>
        /// Gets the value that was stored in this message as a JSON string.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <returns>the value that was stored in this message as a JSON string.</returns>
        public string GetMessageAsString()
        {
            if (this.Message == null)
            {
                return null;
            }
            else
            {
                return Encoding.UTF8.GetString(this.Message);
            }
        }

        /// <summary>
        /// Stores the given value as a JSON string.
        /// </summary>
        /// <param name="message">the value to convert to JSON and store in this message.</param>
        public void SetMessageAsObject(object message)
        {
            if (message == null)
            {
                Message = null;
            }
            else
            {
                var json = JsonConverter.ToJson(message);
                Message = Encoding.UTF8.GetBytes(json);
            }

        }

        /// <summary>
        /// Gets the value that was stored in this message as a JSON string.
        /// </summary>
        /// <typeparam name="T">the class type</typeparam>
        /// <returns>the value that was stored in this message as a JSON string.</returns>
        public T GetMessageAs<T>()
        {
            if (this.Message == null)
            {
                return default(T);
            }
            else
            {
                var json = Encoding.UTF8.GetString(this.Message);
                return JsonConverter.FromJson<T>(json);
            }
        }

        /// <summary>
        /// Convert's this MessageEnvelope to a string, using the following format:
        /// 
        /// <code>"[trace_id, message_type, message.toString]"</code>.
        /// 
        /// If any of the values are<code>null</code>, they will be replaced with <code>---</code>.
        /// </summary>
        /// <returns>the generated string.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append('[');
            builder.Append(TraceId ?? "---");
            builder.Append(',');
            builder.Append(MessageType ?? "---");
            builder.Append(',');
            var sample = GetMessageAsString() ?? "---";
            sample = sample.Length > 150 ? sample.Substring(0, 150) : sample;
            builder.Append(sample);
            builder.Append(']');
            return builder.ToString();
        }
    }
}