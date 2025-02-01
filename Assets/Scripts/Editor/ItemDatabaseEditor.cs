using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemDatabase))]
public class ItemDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ItemDatabase database = (ItemDatabase)target;

        // 버튼 추가
        if (GUILayout.Button("Sort by ItemCode"))
        {
            database.SortEntriesByItemCode();
            EditorUtility.SetDirty(database); // 변경사항 저장
            Debug.Log("ItemDatabase 정렬 완료: ItemCode 기준");
        }
    }
}
