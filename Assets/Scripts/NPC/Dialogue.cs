using UnityEngine;


[System.Serializable]
public class Dialogue
{
    public string characterName;
    public string[] contexts;
    public FaceType[] faceType;
}


[System.Serializable]
public class DialogueEvent
{
    public string name; // 이벤트 이름

    public Vector2 line;
    public Dialogue[] dialogues;
}


public enum NpcName
{
    Mane,
    Harvie,
    Fiona,
    Shu
}