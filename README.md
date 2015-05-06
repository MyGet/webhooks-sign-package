# NuGet Signature

Library and web project that can sign NuGet packages, for example triggered by MyGet Webhooks. Read [my blog post](http://blog.maartenballiauw.be/post/2014/09/10/Automatically-strong-name-signing-NuGet-packages.aspx) for more information.

The project makes use of the excellent work by [Werner van Deventer](https://twitter.com/brutaldev) described in his blog post [".NET Assembly Strong-Name Signer"](http://brutaldev.com/post/2013/10/18/NET-Assembly-Strong-Name-Signer).

## Using the library

NuGet: [https://www.nuget.org/packages/Signature.Core](https://www.nuget.org/packages/Signature.Core)

The ```Signature.Core``` comes with a class (```PackageSigner```) that can sign a NuGet package. Here's an example:

	PackageSigner signer = new PackageSigner();
    signer.SignPackage("MyPackage.1.0.0.nupkg", "MyPackage.Signed.1.0.0.nupkg", "SampleKey.pfx", "password", "MyPackage.Signed");

**Note:** While the ```PackageSigner``` does not require a PFX file to be specified, it is recommended to use one. NuGet will get very confused for packages with the same id but different versions if the signature uses a different key.

## Integrating NuGet Signature with MyGet

A nice use case for this repository is automatically signing unsigned NuGet packages that are pushed to a [MyGet](http://www.myget.org) feed.

### Deploy and configure NuGet Signature

First of all, deploy NuGet Signature to a web server, for example Microsoft Azure Websites. Then, configure the application using the management dashboard. The following settings must be provided:

* ```Signature:KeyFile``` - path to the PFX file to use when signing
* ```Signature:KeyFilePassword``` - private key/password for using the PFX file
* ```Signature:PackageIdSuffix``` - suffix for signed package id's. Can be empty or something like ```.Signed```
* ```Signature:NuGetFeedUrl``` - NuGet feed to push signed packages to
* ```Signature:NuGetFeedApiKey``` - API key for pushing packages to the above feed

The configuration in the Microsoft Azure Management Dashboard could look like the following:

![Azure Websites configuration](https://raw.githubusercontent.com/myget/webhooks-sign-package/master/docs/azure-websites.png)

### Setup a MyGet Webhook

For the MyGet feed you wish to sign packages, configure a new HTTP Post webhook. The following options must be configured:

* **URL to POST JSON data to** - URL to the deployed NuGet Signature API, for example ```http://signpackage.azurewebsites.net/api/sign``` (do not forget the ```/api/sign```)
* ***Content type*** - set to ```application/json```
* ***Events that trigger this web hook*** - make sure that only ```Package Added``` is selected

![MyGet webhook configuration](https://raw.githubusercontent.com/myget/webhooks-sign-package/master/docs/edit-webhook.png)

From now on, all packages that are added to your feed will be signed when the webhook is triggered. To sign existing packages, you'll have to use the library itself.
