using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SaveAndLoad
{
    public static class SaveSystem
    {
        private static readonly string SAVE_PATH =
            Path.Combine(Application.persistentDataPath, "SaveFile.sv");

        public static bool FileExists => File.Exists(SAVE_PATH);

        public static void Save(params object[] objects)
        {
            SaveData saveData = new SaveData();

            foreach (var obj in objects)
            {
                if (obj == null)
                    continue;

                Type type = obj.GetType();

                saveData.serializedObjects.Add(
                    new SerializableObject(
                        type.AssemblyQualifiedName,
                        JsonUtility.ToJson(obj)
                    )
                );
            }

            File.WriteAllText(SAVE_PATH, JsonUtility.ToJson(saveData, true));
        }

        public static List<object> Load()
        {
            if (!FileExists)
                return null;

            string json = File.ReadAllText(SAVE_PATH);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            List<object> loadedObjects = new();

            foreach (var entry in saveData.serializedObjects)
            {
                Type type = Type.GetType(entry.typeName);
                if (type == null)
                {
                    Debug.LogWarning($"Type not found: {entry.typeName}");
                    continue;
                }

                object obj = JsonUtility.FromJson(entry.json, type);
                loadedObjects.Add(obj);
            }

            return loadedObjects;
        }

        public static T Load<T>()
        {
            if (!FileExists)
                return default;

            string json = File.ReadAllText(SAVE_PATH);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            foreach (var entry in saveData.serializedObjects)
            {
                Type type = Type.GetType(entry.typeName);
                if (type == typeof(T))
                    return JsonUtility.FromJson<T>(entry.json);
            }

            return default;
        }

        [Serializable]
        class SaveData
        {
            public List<SerializableObject> serializedObjects = new();
        }

        [Serializable]
        class SerializableObject
        {
            public string typeName;
            public string json;

            public SerializableObject(string typeName, string json)
            {
                this.typeName = typeName;
                this.json = json;
            }
        }
    }
}
