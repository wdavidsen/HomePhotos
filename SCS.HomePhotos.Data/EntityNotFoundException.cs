using System.Runtime.Serialization;
using System;

namespace SCS.HomePhotos.Data
{
    /// <summary>
    /// A security access exception.
    /// </summary>
    /// <seealso cref="SCS.HomePhotos.HomePhotosException" />
    public class EntityNotFoundException : HomePhotosException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        public EntityNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// Constructor with message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public EntityNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// Constructor with message that wraps another message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The preceding exception.</param>
        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// Creates the exception using serialization.
        /// </summary>
        /// <param name="serializationInfo">The data needed to serialize or deserialize.</param>
        /// <param name="streamingContext">Describes the source and destination of a given serialized stream.</param>
        protected EntityNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}
