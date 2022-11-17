using System;
using System.Runtime.Serialization;

namespace SCS.HomePhotos
{
    /// <summary>
    /// Base exception class for HomePhotos exceptions.
    /// </summary>
    /// <remarks>All exceptions in this namespaces must inherit from this class.</remarks>
    [Serializable]
    public abstract class HomePhotosException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomePhotosException"/> class.
        /// The default constructor.
        /// </summary>
        public HomePhotosException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomePhotosException"/> class.
        /// Constructor with message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public HomePhotosException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomePhotosException"/> class.
        /// Constructor with message that wraps another message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The preceding exception.</param>
        public HomePhotosException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomePhotosException"/> class.
        /// Creates the exception using serialization.
        /// </summary>
        /// <param name="serializationInfo">The data needed to serialize or deserialize.</param>
        /// <param name="streamingContext">Describes the source and destination of a given serialized stream.</param>
        protected HomePhotosException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}
