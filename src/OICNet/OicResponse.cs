namespace OICNet
{
    public class OicResponse : OicMessage
    {
        /// <summary>
        /// Indicator of the result of the request; whether it was accepted and what the conclusion of the operation was.
        /// </summary>
        /// <remarks>
        /// The values of the response code for CRUDN operations shall conform to those as defined in section 5.9 and 12.1.2 in IETF RFC 7252.
        /// </remarks>
        public virtual OicResponseCode ResposeCode { get; set; }

        /// <summary>
        /// Indicator for an observe response.
        /// </summary>
        public virtual bool Observe { get; set; }
    }
}