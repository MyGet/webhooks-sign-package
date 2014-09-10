using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using Brutal.Dev.StrongNameSigner;
using NuGet;
using ZipPackage = System.IO.Packaging.ZipPackage;

namespace Signature.Core
{
    public class PackageSigner
    {
        public bool SignPackage(string packagePath, string outputPath, string keyPath, string keyPassword, string signedPackageId)
        {
            if (!File.Exists(packagePath))
            {
                throw new FileNotFoundException(string.Format("File {0} does not exist.", packagePath));
            }

            string package = Path.GetFullPath(packagePath);
            if (!string.IsNullOrEmpty(outputPath) && packagePath != outputPath)
            {
                File.Copy(packagePath, outputPath, true);
                package = Path.GetFullPath(outputPath);
            }

            try
            {
                using (Stream stream = new FileStream(package, FileMode.Open, FileAccess.ReadWrite))
                {
                    using (ZipPackage zipPackage = Package.Open(stream, FileMode.Open, FileAccess.ReadWrite) as ZipPackage)
                    {
                        return SignPackage(zipPackage, keyPath, keyPassword, signedPackageId);
                    }
                }
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(outputPath))
                {
                    File.Delete(outputPath);
                }
                throw;
            }
        }

        public bool SignPackage(ZipPackage package, string keyPath, string keyPassword, string signedPackageId)
        {
            bool signedPackage = false;

            foreach (var packagePart in package.GetParts())
            {
                if (TrySignPackagePart(packagePart, signedPackage))
                {
                    signedPackage = true;
                }
            }

            if (!string.IsNullOrEmpty(signedPackageId))
            {
                TryUpdatePackageId(package, signedPackageId);
            }

            return signedPackage;
        }

        private static bool TrySignPackagePart(PackagePart packagePart, bool signedPackage)
        {
            if (packagePart.Uri.ToString().EndsWith(".exe")
                || packagePart.Uri.ToString().EndsWith(".dll"))
            {
                string tempPath = Path.GetTempFileName();
                try
                {
                    using (var stream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        packagePart.GetStream().CopyTo(stream);
                    }

                    if (!SigningHelper.GetAssemblyInfo(tempPath).IsSigned)
                    {
                        signedPackage = true;

                        SigningHelper.SignAssembly(tempPath);

                        using (var stream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.Read))
                        {
                            stream.CopyTo(packagePart.GetStream(FileMode.Create, FileAccess.Write));
                        }
                    }
                }
                finally
                {
                    File.Delete(tempPath);
                }
            }
            return signedPackage;
        }

        private static void TryUpdatePackageId(ZipPackage package, string signedPackageId)
        {
            // Update package id
            var manifestRelationType = package.GetRelationshipsByType("http://schemas.microsoft.com/packaging/2010/07/manifest").SingleOrDefault();
            if (manifestRelationType != null)
            {
                var manifestPart = package.GetPart(manifestRelationType.TargetUri);
                var manifest = Manifest.ReadFrom(manifestPart.GetStream(), NullPropertyProvider.Instance, false);
                manifest.Metadata.Id = signedPackageId;

                package.DeleteRelationship(manifestRelationType.Id);
                package.DeletePart(manifestPart.Uri);

                Uri uri = PackUriHelper.CreatePartUri(new Uri(string.Format("/{0}.nuspec", signedPackageId), UriKind.Relative));
                var newManifestPart = package.CreatePart(uri, "application/octet", CompressionOption.Maximum);
                manifest.Save(newManifestPart.GetStream(FileMode.Create));
                package.CreateRelationship(uri, TargetMode.Internal, "http://schemas.microsoft.com/packaging/2010/07/manifest");
            }
        }
    }
}