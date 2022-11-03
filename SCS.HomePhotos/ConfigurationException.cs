using System;
using System.Runtime.Serialization;

namespace SCS.HomePhotos
{
    /// <summary>
    /// A configuration exception.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
        /// The default constructor.
        /// </summary>
        public ConfigurationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
        /// Constructor with message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
        /// Constructor with message that wraps another message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The preceding exception.</param>
        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
        /// Creates the exception using serialization.
        /// </summary>
        /// <param name="serializationInfo">The data needed to serialize or deserialize.</param>
        /// <param name="streamingContext">Describes the source and destination of a given serialized stream.</param>
        protected ConfigurationException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}
