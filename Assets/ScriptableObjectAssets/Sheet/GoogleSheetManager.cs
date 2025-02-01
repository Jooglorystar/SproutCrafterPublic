using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class GoogleSheetManager : MonoBehaviour
{
    [Tooltip("true: google sheet, false: local json")]
    [SerializeField] bool isAccessGoogleSheet = true;
    [Tooltip("Google sheet appsscript webapp url")]
    [SerializeField] string googleSheetUrl;
    [Tooltip("Google sheet avail sheet tabs. seperate `/`. For example `Sheet1/Sheet2`")]
    [SerializeField] string availSheets = "Sheet1/Sheet2";
    [Tooltip("For example `/GenerateGoogleSheet`")]
    [SerializeField] string generateFolderPath = "/GenerateGoogleSheet";
    [Tooltip("You must approach through `GoogleSheetManager.SO<GoogleSheetSO>()`")]
    public ScriptableObject googleSheetSO;

    string JsonPath => $"{Application.dataPath}{generateFolderPath}/GoogleSheetJson.json";
    string ClassPath => $"{Application.dataPath}{generateFolderPath}/GoogleSheetClass.cs";
    string SOPath => $"Assets{generateFolderPath}/GoogleSheetSO.asset";

    string[] availSheetArray;
    string json;
    bool refeshTrigger;
    static GoogleSheetManager instance;

    public static T SO<T>() where T : ScriptableObject
    {
        if (GetInstance().googleSheetSO == null)
        {
            Debug.Log("googleSheetSO is null");
            return null;
        }

        return GetInstance().googleSheetSO as T;
    }

#if UNITY_EDITOR
    [ContextMenu("FetchGoogleSheet")]
    async void FetchGoogleSheet()
    {
        availSheetArray = availSheets.Split('/');

        if (isAccessGoogleSheet)
        {
            Debug.Log("Loading from google sheet..");
            json = await LoadDataGoogleSheet(googleSheetUrl);
        }
        else
        {
            Debug.Log("Loading from local json..");
            json = LoadDataLocalJson();
        }
        if (json == null) return;

        bool isJsonSaved = SaveFileOrSkip(JsonPath, json);
        string allClassCode = GenerateCSharpClass(json);
        bool isClassSaved = SaveFileOrSkip(ClassPath, allClassCode);

        if (isJsonSaved || isClassSaved)
        {
            refeshTrigger = true;
            UnityEditor.AssetDatabase.Refresh();
        }
        else
        {
            CreateGoogleSheetSO();
            Debug.Log("Fetch done.");
        }
    }

    async Task<string> LoadDataGoogleSheet(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                byte[] dataBytes = await client.GetByteArrayAsync(url);
                return Encoding.UTF8.GetString(dataBytes);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request error: {e.Message}");
                return null;
            }
        }
    }

    string LoadDataLocalJson()
    {
        if (File.Exists(JsonPath))
        {
            return File.ReadAllText(JsonPath);
        }

        Debug.Log($"File not exist.\n{JsonPath}");
        return null;
    }

    bool SaveFileOrSkip(string path, string contents)
    {
        string directoryPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(path) && File.ReadAllText(path).Equals(contents))
            return false;

        File.WriteAllText(path, contents);
        return true;
    }

    bool IsExistAvailSheets(string sheetName)
    {
        return Array.Exists(availSheetArray, x => x == sheetName);
    }

    string GenerateCSharpClass(string jsonInput)
    {
        JObject jsonObject = JObject.Parse(jsonInput);
        StringBuilder classCode = new();

        // Scriptable Object
        classCode.AppendLine("using System;\nusing System.Collections.Generic;\nusing UnityEngine;\n");
        classCode.AppendLine("/// <summary>You must approach through `GoogleSheetManager.SO<GoogleSheetSO>()`</summary>");
        classCode.AppendLine("public class GoogleSheetSO : ScriptableObject\n{");

        foreach (var sheet in jsonObject)
        {
            string className = sheet.Key;
            if (!IsExistAvailSheets(className))
                continue;

            classCode.AppendLine($"\tpublic List<{className}> {className}List;");
        }
        classCode.AppendLine("}\n");

        // Class
        foreach (var jObject in jsonObject)
        {
            string className = jObject.Key;

            if (!IsExistAvailSheets(className))
                continue;

            var items = (JArray)jObject.Value;
            var firstItem = (JObject)items[0];
            classCode.AppendLine($"[Serializable]\npublic class {className}\n{{");

            int itemIndex = 0;
            int propertyCount = firstItem.Properties().Count();
            string[] propertyTypes = new string[propertyCount];

            foreach (JToken item in items)
            {
                itemIndex = 0;
                foreach (var property in ((JObject)item).Properties())
                {
                    string propertyType = InferPropertyType(property.Value.ToString());
                    string oldPropertyType = propertyTypes[itemIndex];

                    if (oldPropertyType == null)
                    {
                        propertyTypes[itemIndex] = propertyType;
                    }
                    itemIndex++;
                }
            }

            itemIndex = 0;
            foreach (var property in firstItem.Properties())
            {
                string propertyName = property.Name;
                string propertyType = propertyTypes[itemIndex];
                classCode.AppendLine($"\tpublic {propertyType} {propertyName};");
                itemIndex++;
            }

            classCode.AppendLine("}\n");
        }

        return classCode.ToString();
    }

    string InferPropertyType(string value)
    {
        if (int.TryParse(value, out _))
            return "int";
        if (float.TryParse(value, out _))
            return "float";
        if (bool.TryParse(value, out _))
            return "bool";
        if (Enum.TryParse(typeof(ItemType), value, out _))
            return "ItemType";
        if (Enum.TryParse(typeof(ESeason), value, out _))
            return "ESeason";
        if (Enum.TryParse(typeof(TileType), value, out _))
            return "TileType";
        if (value.Contains("/"))
        {
            if (value.StartsWith("Prefabs/"))
                return "GameObject";
            return "Sprite";
        }
        return "string";
    }

    bool CreateGoogleSheetSO()
    {
        if (Type.GetType("GoogleSheetSO") == null)
            return false;

        googleSheetSO = ScriptableObject.CreateInstance("GoogleSheetSO");
        JObject jsonObject = JObject.Parse(json);

        try
        {
            foreach (var jObject in jsonObject)
            {
                string className = jObject.Key;
                if (!IsExistAvailSheets(className))
                    continue;

                Type classType = Type.GetType(className);
                Type listType = typeof(List<>).MakeGenericType(classType);
                IList listInst = (IList)Activator.CreateInstance(listType);
                var items = (JArray)jObject.Value;

                foreach (var item in items)
                {
                    object classInst = Activator.CreateInstance(classType);

                    foreach (var property in ((JObject)item).Properties())
                    {
                        FieldInfo fieldInfo = classType.GetField(property.Name);
                        if (fieldInfo == null) continue;

                        object value = ConvertPropertyValue(property.Value.ToString(), fieldInfo.FieldType);
                        fieldInfo.SetValue(classInst, value);
                    }

                    listInst.Add(classInst);
                }

                googleSheetSO.GetType().GetField($"{className}List").SetValue(googleSheetSO, listInst);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"CreateGoogleSheetSO error: {e.Message}");
        }

        UnityEditor.AssetDatabase.CreateAsset(googleSheetSO, SOPath);
        UnityEditor.AssetDatabase.SaveAssets();
        return true;
    }

    object ConvertPropertyValue(string value, Type targetType)
    {
        if (targetType == typeof(int))
        {
            if (int.TryParse(value, out int intValue))
                return intValue;
        }
        else if (targetType == typeof(float))
        {
            if (float.TryParse(value, out float floatValue))
                return floatValue;
        }
        else if (targetType == typeof(bool))
        {
            if (bool.TryParse(value, out bool boolValue))
                return boolValue;
        }
        else if (targetType == typeof(string))
        {
            return value;
        }
        else if (targetType == typeof(Sprite))
        {
            return LoadSpriteFromPath(value);
        }
        else if (targetType == typeof(GameObject))
        {
            return Resources.Load<GameObject>(value);
        }
        else if (targetType == typeof(ItemType) || targetType == typeof(ESeason) || targetType == typeof(TileType))
        {
            if (Enum.IsDefined(targetType, value))
                return Enum.Parse(targetType, value);
        }

        Debug.LogWarning($"Unhandled type or conversion failed for type: {targetType}, value: {value}");
        return null;
    }

    Sprite LoadSpriteFromPath(string path)
    {
        string[] segments = path.Split('/');
        if (segments.Length < 2)
        {
            Debug.LogError($"Invalid sprite path: {path}. Expected format: 'FolderName/SpriteName'.");
            return null;
        }

        string folderPath = string.Join("/", segments.Take(segments.Length - 1));
        string spriteName = segments.Last();

        Sprite[] sprites = Resources.LoadAll<Sprite>(folderPath);
        return sprites.FirstOrDefault(s => s.name == spriteName);
    }

    void OnValidate()
    {
        if (refeshTrigger)
        {
            bool isCompleted = CreateGoogleSheetSO();
            if (isCompleted)
            {
                refeshTrigger = false;
                Debug.Log("Fetch done.");
            }
        }
    }
#endif

    static GoogleSheetManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<GoogleSheetManager>();
        }
        return instance;
    }
}
