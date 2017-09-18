using System;
using System.Collections.Generic;

namespace OICNet
{
    public class OicRequest : OicMessage
    {
        public OicRequest()
        { }

        public OicRequest(IEnumerable<OicMessageContentType> accepts)
        {
            Accepts = new List<OicMessageContentType>(accepts);
        }

        /// <summary>
        /// Specific operation requested to be performed by the Server.
        /// </summary>
        public virtual OicRequestOperation Operation { get; set; } = OicRequestOperation.Get;

        public virtual IList<OicMessageContentType> Accepts { get; } = new List<OicMessageContentType>();

        /// <summary>
        /// Indicator for an observe request.
        /// </summary>
        public virtual bool Observe { get; set; }

        public static OicRequest Create(string path, OicRequestOperation operation = OicRequestOperation.Get)
        {
            return new OicRequest
            {
                Accepts =
                {
                    OicMessageContentType.ApplicationCbor,
                    OicMessageContentType.ApplicationJson
                },
                Operation = operation,
                ToUri = new Uri(path, UriKind.RelativeOrAbsolute)
            };
        }
    }
}