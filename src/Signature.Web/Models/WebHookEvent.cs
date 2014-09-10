using System;

namespace Signature.Web.Models
{
    public class WebHookEvent
    {
        public Guid Identifier { get; set; }
        public string Username { get; set; }
        public DateTime When { get; set; }
        public string PayloadType { get; set; }
        public PackageAddedWebHookEventPayloadV1 Payload { get; set; }
    }
}