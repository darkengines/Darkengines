using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkengines.Serialization {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddJsonSerializer(this IServiceCollection serviceCollection) {
            var serializer = new JsonSerializer() {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
            serializer.Converters.Add(new ByteArrayJsonConverter());
            serviceCollection.AddSingleton(serviceProvider => {
                var traceWriter = serviceProvider.GetRequiredService<ITraceWriter>();
                serializer.TraceWriter = traceWriter;
                return serializer;
            });
            return serviceCollection;
        }
    }

    public class ByteArrayJsonConverter : JsonConverter<byte[]> {
        public override byte[]? ReadJson(JsonReader reader, Type objectType, byte[]? existingValue, bool hasExistingValue, JsonSerializer serializer) {
            return Convert.FromBase64String((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, byte[]? value, JsonSerializer serializer) {
            writer.WriteValue(Convert.ToBase64String(value));
        }
    }
}
