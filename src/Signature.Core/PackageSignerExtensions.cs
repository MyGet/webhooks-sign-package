using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;

namespace Signature.Core
{
    public static class PackageSignerExtensions
    {
        public static bool SignPackage(this PackageSigner current, string packagePath)
        {
            return current.SignPackage(packagePath, string.Empty, string.Empty, string.Empty, string.Empty);
        }
        public static bool SignPackage(this PackageSigner current, string packagePath, string signedPackageId)
        {
            return current.SignPackage(packagePath, string.Empty, string.Empty, string.Empty, signedPackageId);
        }

        public static bool SignPackage(this PackageSigner current, string packagePath, string outputPath, string signedPackageId)
        {
            return current.SignPackage(packagePath, outputPath, string.Empty, string.Empty, signedPackageId);
        }
    }
}
