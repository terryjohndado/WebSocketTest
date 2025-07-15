// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using EventCollection;
//
//    var bsEvent = BsEvent.FromJson(jsonString);

namespace EventCollection
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class BsEvent
    {
        [JsonProperty("Event", Required = Required.Always)]
        public virtual Event Event { get; set; }
    }

    public partial class Event
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public virtual long Id { get; set; }

        [JsonProperty("event_type_id")]
        public virtual EventTypeId EventTypeId { get; set; }

        [JsonProperty("index")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public virtual long Index { get; set; }

        [JsonProperty("datetime")]
        public virtual DateTimeOffset Datetime { get; set; }

        [JsonProperty("server_datetime")]
        public virtual DateTimeOffset ServerDatetime { get; set; }

        [JsonProperty("device_id")]
        public virtual DeviceId DeviceId { get; set; }

        [JsonProperty("user_id")]
        public virtual UserId UserId { get; set; }

        [JsonProperty("tna_key")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public virtual long TnaKey { get; set; }

        [JsonProperty("parameter")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public virtual long Parameter { get; set; }

        [JsonProperty("is_dst")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public virtual long IsDst { get; set; }

        [JsonProperty("timezone")]
        public virtual Timezone Timezone { get; set; }

        [JsonProperty("temperature")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public virtual long Temperature { get; set; }
    }

    public partial class DeviceId
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public virtual long Id { get; set; }

        [JsonProperty("name")]
        public virtual string Name { get; set; }
    }

    public partial class EventTypeId
    {
        [JsonProperty("code")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public virtual long Code { get; set; }

        [JsonProperty("name")]
        public virtual string Name { get; set; }

        [JsonProperty("description")]
        public virtual string Description { get; set; }

        [JsonProperty("enable_alert")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public virtual bool EnableAlert { get; set; }

        [JsonProperty("alertable")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public virtual bool Alertable { get; set; }

        [JsonProperty("alert_name")]
        public virtual string AlertName { get; set; }

        [JsonProperty("alert_message")]
        public virtual string AlertMessage { get; set; }
    }

    public partial class Timezone
    {
        [JsonProperty("half")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public virtual long Half { get; set; }

        [JsonProperty("hour")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public virtual long Hour { get; set; }

        [JsonProperty("negative")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public virtual long Negative { get; set; }
    }

    public partial class UserId
    {
        [JsonProperty("user_id")]
        [JsonConverter(typeof(PurpleParseStringConverter))]
        public virtual long UserIdUserId { get; set; }

        [JsonProperty("name")]
        public virtual string Name { get; set; }

        [JsonProperty("photo_exists")]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public virtual bool PhotoExists { get; set; }
    }

    public partial class BsEvent
    {
        public static BsEvent FromJson(string json) => JsonConvert.DeserializeObject<BsEvent>(json, ApiResponse.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this BsEvent self) => JsonConvert.SerializeObject(self, ApiResponse.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class PurpleParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly PurpleParseStringConverter Singleton = new PurpleParseStringConverter();
    }

    internal class FluffyParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(bool) || t == typeof(bool?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            bool b;
            if (Boolean.TryParse(value, out b))
            {
                return b;
            }
            throw new Exception("Cannot unmarshal type bool");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (bool)untypedValue;
            var boolString = value ? "true" : "false";
            serializer.Serialize(writer, boolString);
            return;
        }

        public static readonly FluffyParseStringConverter Singleton = new FluffyParseStringConverter();
    }
}
