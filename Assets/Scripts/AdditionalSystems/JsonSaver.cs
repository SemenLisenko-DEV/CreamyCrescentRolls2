using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace CreamyCrescentRolls.RoomGame.JsonSaver
{
    public static class JsonSaver
    {
        public static void Save<T>(T obj, string path) where T : class
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.LogError("[JsonSaver] Path must not be empty");
                throw new ArgumentException("Path must not be empty", nameof(path));
            }

            if (obj == null)
            {
                Debug.LogError("[JsonSaver] Object must not be null");
                throw new ArgumentNullException(nameof(obj));
            }

            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            string directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
            {
                Debug.LogWarning("[JsonSaver] Directory arent exist");
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, json);
        }

        public static T Load<T>(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.LogError("[JsonSaver] Path must not be empty");
                throw new ArgumentException("Path must not be empty", nameof(path));
            }

            string json = File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("[JsonSaver] JSON document is empty");
                throw new JsonSerializationException("JSON document is empty");
            }

            T result = JsonConvert.DeserializeObject<T>(json);

            if (result == null)
            {
                Debug.LogError("[JsonSaver] JSON document contains null");
                throw new JsonSerializationException("JSON document contains null");
            }

            return result;
        }
    }
}
