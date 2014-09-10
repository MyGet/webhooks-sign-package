using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NuGet;
using Signature.Core;
using Signature.Web.Models;
using HttpClient = System.Net.Http.HttpClient;

namespace Signature.Web.Controllers
{
    public class SignController : ApiController
    {
        // POST /api/sign
        public async Task<HttpResponseMessage> Post([FromBody]WebHookEvent payload)
        {
            if (payload.Payload.PackageIdentifier.EndsWith(ConfigurationManager.AppSettings["Signature:PackageIdSuffix"]))
            {
                return new HttpResponseMessage(HttpStatusCode.OK) {ReasonPhrase = "Package is already signed. "};
            }

            string tempPath = Path.GetTempFileName();
            try
            {
                // Download the package
                var httpClient = new HttpClient();
                var packageStream = await httpClient.GetStreamAsync(payload.Payload.PackageDownloadUrl);

                using (var stream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    packageStream.CopyTo(stream);
                }

                // Sign the package
                PackageSigner signer = new PackageSigner();
                if (signer.SignPackage(tempPath, tempPath,
                    ConfigurationManager.AppSettings["Signature:KeyFile"],
                    ConfigurationManager.AppSettings["Signature:KeyFilePassword"],
                    payload.Payload.PackageIdentifier + ConfigurationManager.AppSettings["Signature:PackageIdSuffix"]))
                {
                    var server = new PackageServer(ConfigurationManager.AppSettings["Signature:NuGetFeedUrl"], "Signature/1.0");
                    server.PushPackage(ConfigurationManager.AppSettings["Signature:NuGetFeedApiKey"], new OptimizedZipPackage(tempPath), new FileInfo(tempPath).Length, 60 * 1000, true);
                    OptimizedZipPackage.PurgeCache();

                    return new HttpResponseMessage(HttpStatusCode.Created) { ReasonPhrase = "Package has been signed." };
                }
            }
            finally
            {
                File.Delete(tempPath);
            }

            return new HttpResponseMessage(HttpStatusCode.OK) { ReasonPhrase = "Package is already signed." };
        }
    }
}
