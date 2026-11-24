using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace CreamyCrescentRolls.RoomGame.JsonSaver
{
    public static class JsonSaver
    {
        public static bool Save<T>(T obj, string path) where T : class
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.LogError("[JsonSaver] Path must not be empty");
                return false;
            }

            if (obj == null)
            {
                Debug.LogError("[JsonSaver] Object must not be null");
                return false;
            }

            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            string directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
            {
                Debug.LogWarning("[JsonSaver] Directory arent exist");
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, json);
            return true;
        }

        public static bool Load<T>(string path, out T result) where T : class
        {
            result = null;

            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.LogError("[JsonSaver] Path must not be empty");
                return false;
            }

            if (!File.Exists(path))
            {
                Debug.LogWarning("[JsonSaver] File arent exists");
                return false;
            }

            string json = File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("[JsonSaver] JSON document is empty");
                return false;
            }

            result = JsonConvert.DeserializeObject<T>(json);

            if (result == null)
            {
                Debug.LogError("[JsonSaver] JSON document contains null");
                return false;
            }

            return true;
        }
    }
}
