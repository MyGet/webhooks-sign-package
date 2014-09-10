namespace Signature.Web.Models
{
    public class PackageAddedWebHookEventPayloadV1
    {
        public string PackageIdentifier { get; set; }
        public string PackageVersion { get; set; }
        public string PackageDetailsUrl { get; set; }
        public string PackageDownloadUrl { get; set; }
        public PackageMetadata PackageMetadata { get; set; }

        public string FeedIdentifier { get; set; }
        public string FeedUrl { get; set; }
    }
}