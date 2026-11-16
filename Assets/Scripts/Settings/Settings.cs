using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonFile = CreamyCrescentRolls.RoomGame.JsonSaver.JsonSaver;

namespace CreamyCrescentRolls.RoomGame.Settings
{
    [Serializable]
    public abstract class SettingDefinition
    {
        public string settingName;

        public abstract Type ValueType { get; }
        public abstract object DefaultValue { get; }
    }

    [Serializable]
    public sealed class FloatSetting : SettingDefinition
    {
        public float defaultValue = 1f;

        public override Type ValueType => typeof(float);
        public override object DefaultValue => defaultValue;
    }

    [Serializable]
    public sealed class IntSetting : SettingDefinition
    {
        public int defaultValue;

        public override Type ValueType => typeof(int);
        public override object DefaultValue => defaultValue;
    }

    [Serializable]
    public sealed class BoolSetting : SettingDefinition
    {
        public bool defaultValue;

        public override Type ValueType => typeof(bool);
        public override object DefaultValue => defaultValue;
    }

    [Serializable]
    public sealed class StringSetting : SettingDefinition
    {
        public string defaultValue = string.Empty;

        public override Type ValueType => typeof(string);
        public override object DefaultValue => defaultValue;
    }

    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(SettingsDataConverter))]
    public sealed class SettingsData
    {
        [JsonProperty("version", Required = Required.Always)]
        public int version = 1;

        [JsonProperty("values", Required = Required.Always)]
        public Dictionary<string, JToken> values = new(StringComparer.Ordinal);
    }

    public sealed class SettingsDataConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanConvert(Type objectType) => objectType == typeof(SettingsData);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DateParseHandling previousDateHandling = reader.DateParseHandling;
            reader.DateParseHandling = DateParseHandling.None;
            try
            {
                var document = JObject.Load(reader, new JsonLoadSettings
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
                });

                if (document["version"]?.Type != JTokenType.Integer || !(document["values"] is JObject values))
                    throw new JsonSerializationException("Settings require an integer version and an object of values.");

                var data = new SettingsData { version = document["version"].ToObject<int>() };
                foreach (var property in values.Properties())
                    data.values.Add(property.Name, property.Value);

                return data;
            }
            finally { reader.DateParseHandling = previousDateHandling; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotSupportedException("Use the default settings serializer for writing.");
    }

    public sealed class SettingsState
    {
        private readonly Dictionary<string, SettingDefinition> _definitions = new(StringComparer.Ordinal);
        private readonly Dictionary<string, object> _applied = new(StringComparer.Ordinal);
        private readonly Dictionary<string, object> _pending = new(StringComparer.Ordinal);

        public SettingsState(IEnumerable<SettingDefinition> definitions)
        {
            if (definitions == null)
                throw new ArgumentNullException(nameof(definitions));

            foreach (var definition in definitions)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.settingName))
                    throw new ArgumentException("Every setting must have a non-empty name.");

                string name = definition.settingName;
                if (name != name.Trim() || _definitions.ContainsKey(name))
                    throw new ArgumentException($"Duplicate name or surrounding whitespace: '{name}'.");

                ValidateValue(definition, definition.DefaultValue);
                _definitions.Add(name, definition);
                _applied.Add(name, definition.DefaultValue);
                _pending.Add(name, definition.DefaultValue);
            }
        }

        public T Get<T>(string name, bool pending = false)
        {
            if (!TryGet(name, out T value, pending))
                throw new ArgumentException($"Setting '{name}' does not exist or is not {typeof(T).Name}.");

            return value;
        }

        public bool TryGet<T>(string name, out T value, bool pending = false)
        {
            value = default;
            if (name == null || !_definitions.TryGetValue(name, out var definition)
                || definition.ValueType != typeof(T))
                return false;

            value = (T)(pending ? _pending[name] : _applied[name]);
            return true;
        }

        public void SetPending<T>(string name, T value)
        {
            if (name == null || !_definitions.TryGetValue(name, out var definition)
                || definition.ValueType != typeof(T))
                throw new ArgumentException($"Setting '{name}' does not exist or is not {typeof(T).Name}.");

            ValidateValue(definition, value);
            _pending[name] = value;
        }

        public SettingsData CapturePending()
        {
            var data = new SettingsData();
            foreach (var pair in _pending)
                data.values.Add(pair.Key, JToken.FromObject(pair.Value));

            return data;
        }

        public void Restore(SettingsData data)
        {
            if (data == null || data.version != 1 || data.values == null)
                throw new ArgumentException("Unsupported or incomplete settings document.");

            var candidate = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var definition in _definitions.Values)
                candidate.Add(definition.settingName, definition.DefaultValue);

            foreach (var pair in data.values)
            {
                if (_definitions.TryGetValue(pair.Key, out var definition))
                    candidate[pair.Key] = ReadValue(definition, pair.Value);
            }

            foreach (var pair in candidate)
            {
                _applied[pair.Key] = pair.Value;
                _pending[pair.Key] = pair.Value;
            }
        }

        public bool CommitPending()
        {
            bool changed = false;
            foreach (var pair in _pending)
            {
                changed |= !Equals(_applied[pair.Key], pair.Value);
                _applied[pair.Key] = pair.Value;
            }

            return changed;
        }

        public void CancelChanges()
        {
            foreach (var pair in _applied)
                _pending[pair.Key] = pair.Value;
        }

        public void ResetToDefaults()
        {
            foreach (var definition in _definitions.Values)
                _pending[definition.settingName] = definition.DefaultValue;
        }

        private static void ValidateValue(SettingDefinition definition, object value)
        {
            Type type = definition.ValueType;
            if (type != typeof(float) && type != typeof(int) && type != typeof(bool) && type != typeof(string))
                throw new ArgumentException($"Unsupported setting type: {type}.");

            if (value == null || value.GetType() != type
                || value is float number && (float.IsNaN(number) || float.IsInfinity(number)))
                throw new ArgumentException($"Invalid value for setting '{definition.settingName}'.");
        }

        private static object ReadValue(SettingDefinition definition, JToken token)
        {
            object value;
            Type type = definition.ValueType;
            if (token == null)
                throw new ArgumentException($"Missing value for '{definition.settingName}'.");

            if (type == typeof(float) && (token.Type == JTokenType.Float || token.Type == JTokenType.Integer))
                value = token.ToObject<float>();
            else if (type == typeof(int) && token.Type == JTokenType.Integer)
                value = token.ToObject<int>();
            else if (type == typeof(bool) && token.Type == JTokenType.Boolean)
                value = token.ToObject<bool>();
            else if (type == typeof(string) && token.Type == JTokenType.String)
                value = token.ToObject<string>();
            else
                throw new ArgumentException($"Wrong JSON type for '{definition.settingName}': {token.Type}.");

            ValidateValue(definition, value);
            return value;
        }
    }

    public static class SettingsStorage
    {
        public static SettingsData Load(string path) => JsonFile.Load<SettingsData>(path);

        public static void Save(SettingsData data, string path)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Settings path must not be empty.", nameof(path));

            string temporaryPath = path + ".tmp";
            try
            {
                JsonFile.Save(data, temporaryPath);
                if (File.Exists(path))
                    File.Replace(temporaryPath, path, null);
                else
                    File.Move(temporaryPath, path);
            }
            finally
            {
                try
                {
                    if (File.Exists(temporaryPath))
                        File.Delete(temporaryPath);
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }
    }
}
