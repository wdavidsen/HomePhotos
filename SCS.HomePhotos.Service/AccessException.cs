using System.Runtime.Serialization;
using System;

namespace SCS.HomePhotos.Service
{
    /// <summary>
    /// A security access exception.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.HomePhotosException" />
    public class AccessException : HomePhotosException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessException"/> class.
        /// </summary>
        public AccessException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessException"/> class.
        /// Constructor with message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public AccessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessException"/> class.
        /// Constructor with message that wraps another message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The preceding exception.</param>
        public AccessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessException"/> class.
        /// Creates the exception using serialization.
        /// </summary>
        /// <param name="serializationInfo">The data needed to serialize or deserialize.</param>
        /// <param name="streamingContext">Describes the source and destination of a given serialized stream.</param>
        protected AccessException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}
