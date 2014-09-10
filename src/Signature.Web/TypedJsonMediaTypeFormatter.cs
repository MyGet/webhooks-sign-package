using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Signature.Web
{
    public class TypedJsonMediaTypeFormatter 
        : JsonMediaTypeFormatter
    {
        private readonly Type resourceType;

        public TypedJsonMediaTypeFormatter(Type resourceType, MediaTypeHeaderValue mediaType)
        {
            this.resourceType = resourceType;

            this.SupportedMediaTypes.Clear();
            this.SupportedMediaTypes.Add(mediaType);
        }

        public override bool CanReadType(Type type)
        {
            return this.resourceType == type;
        }

        public override bool CanWriteType(Type type)
        {
            return this.resourceType == type;
        }
    }
}