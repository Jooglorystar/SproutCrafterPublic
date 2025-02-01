using System;
using System.Collections.Generic;
using UnityEngine;


public class DialogueParser : MonoBehaviour
{
    /// <summary>
    /// 불러온 csv 파일의 데이터를 속성별로 나누어 리스트로 반환
    /// </summary>
    public Dialogue[] Parse(TextAsset p_csvData)
    {
        List<Dialogue> dialoguesList = new List<Dialogue>();

        string[] data = p_csvData.text.Split(new char[]{'\n'});

        for (int i = 1; i < data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' });

            Dialogue dialogue = new Dialogue();

            dialogue.characterName = row[1];

            List<string> contextList = new List<string>();
            List<FaceType> faceTypeList = new List<FaceType>();
            
            do
            {
                contextList.Add(row[2]);
                
                if (Enum.TryParse(row[3], true, out FaceType faceType))
                {
                    faceTypeList.Add(faceType);
                }

                if (++i < data.Length)
                {
                    row = data[i].Split(new char[]{','});
                }
                else
                {
                    break;
                }

            } while (row[0] == string.Empty);

            dialogue.contexts = contextList.ToArray();
            dialogue.faceType = faceTypeList.ToArray();

            dialoguesList.Add(dialogue);
        }
        
        return dialoguesList.ToArray();
    }
}