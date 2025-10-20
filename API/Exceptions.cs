using System;

namespace SendSequenceCL
{
    /// <summary>
    /// Base exception class for all SendSequenceCL library errors.
    /// </summary>
    public class SendSequenceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendSequenceException"/> class.
        /// </summary>
        public SendSequenceException()
            : base("An error occurred in SendSequenceCL library.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendSequenceException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SendSequenceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendSequenceException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public SendSequenceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when the Tetherscript HID virtual driver cannot be found or accessed.
    /// This typically indicates the driver is not installed or the device is not connected.
    /// </summary>
    public class DriverNotFoundException : SendSequenceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriverNotFoundException"/> class.
        /// </summary>
        public DriverNotFoundException()
            : base("Tetherscript HID driver not found. Ensure the driver is installed and device is connected.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverNotFoundException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DriverNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverNotFoundException"/> class with a specified error message
        /// and a reference to the inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DriverNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when communication with the HID driver fails.
    /// This may indicate a transient error that can be retried.
    /// </summary>
    public class DriverCommunicationException : SendSequenceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriverCommunicationException"/> class.
        /// </summary>
        public DriverCommunicationException()
            : base("Failed to communicate with HID driver. The operation may need to be retried.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverCommunicationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DriverCommunicationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverCommunicationException"/> class with a specified error message
        /// and a reference to the inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DriverCommunicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when invalid screen coordinates are provided for mouse operations.
    /// Coordinates must be within the valid screen bounds.
    /// </summary>
    public class InvalidCoordinateException : SendSequenceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCoordinateException"/> class.
        /// </summary>
        public InvalidCoordinateException()
            : base("Invalid screen coordinates provided. Coordinates must be within screen bounds.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCoordinateException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidCoordinateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCoordinateException"/> class with a specified error message
        /// and a reference to the inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InvalidCoordinateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when an invalid or unsupported virtual key code is provided.
    /// </summary>
    public class InvalidKeyCodeException : SendSequenceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidKeyCodeException"/> class.
        /// </summary>
        public InvalidKeyCodeException()
            : base("Invalid or unsupported virtual key code provided.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidKeyCodeException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidKeyCodeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidKeyCodeException"/> class with a specified error message
        /// and a reference to the inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InvalidKeyCodeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
