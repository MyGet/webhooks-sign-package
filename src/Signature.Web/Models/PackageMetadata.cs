namespace Signature.Web.Models
{
    public class PackageMetadata
    {
        public string IconUrl { get; set; }
        public long Size { get; set; }
        public string Authors { get; set; }
        public string Description { get; set; }
        public string LicenseUrl { get; set; }
        public string LicenseNames { get; set; }
        public string ProjectUrl { get; set; }
        public string Tags { get; set; }
        public string MinClientVersion { get; set; }
        public string ReleaseNotes { get; set; }

        // may contain dupliate id/version if there are different framework dependencies
        public Package[] Dependencies { get; set; }
    }
}