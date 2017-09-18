using System;

namespace OICNet
{
    public class OicMessage
    {
        /// <summary>
        /// The URI of the recipient of the message.
        /// </summary>
        public virtual Uri ToUri { get; set; }

        /// <summary>
        /// The URI of the message originator.
        /// </summary>
        public virtual Uri FromUri { get; set; }

        /// <summary>
        /// The identifier that uniquely identifies the message in the originator and the recipient.
        /// </summary>
        public virtual int RequestId { get; set; }

        /// <summary>
        /// Information specific to the operation.
        /// </summary>
        public virtual byte[] Content { get; set; }

        public virtual OicMessageContentType ContentType { get; set; } = OicMessageContentType.ApplicationCbor;
    }
}