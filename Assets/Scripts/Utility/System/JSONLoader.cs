using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSONLoader : MonoBehaviour
{
    public TextAsset jsonFile; // JSON 파일
    private string jsonFilePath;

    void Start()
    {
        jsonFilePath = Path.Combine(Application.persistentDataPath, "Crops.json");
        InitializeJSONFile();
       // LoadJSONIntoManager();
    }

    // JSON 파일 초기화
    void InitializeJSONFile()
    {
        if (!File.Exists(jsonFilePath))
        {
            if (jsonFile != null)
            {
                File.WriteAllText(jsonFilePath, jsonFile.text);
                Debug.Log("초기 JSON 파일 생성 완료!");
            }
            else
            {
                Debug.LogWarning("초기 JSON 파일이 설정되지 않았습니다.");
                File.WriteAllText(jsonFilePath, "{\"Items\":[]}");
            }
        }
    }

    // JSON 데이터를 CropManager로 로드
    /* void LoadJSONIntoManager()
     {
         string jsonData = File.ReadAllText(jsonFilePath);
         GameState gameState = JsonUtility.FromJson<GameState>(jsonData); // `CropManager`와 동일한 GameState 사용

         CropManager cropManager = FindObjectOfType<CropManager>();
         if (cropManager != null)
         {
             cropManager.LoadFromGameState(gameState); // `CropManager`의 메서드 호출
             Debug.Log("JSON 데이터를 CropManager로 동기화 완료.");
         }
         else
         {
             Debug.LogError("CropManager를 찾을 수 없습니다.");
         }
     }*/
}
