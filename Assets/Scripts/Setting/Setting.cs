using UnityEngine;


public static class Setting
{
    [Header("자주 사용되는 WaitForSeconds 캐싱")]
    public static readonly WaitForSeconds WaitForOneSecond = new WaitForSeconds(1f);
    public static readonly WaitForSeconds NextDelay = new WaitForSeconds(0.03f);
    
    [Header("플레이어")]
    public static readonly Vector2 SpawnPoint = new Vector2(63.7f, 11f);
    public const int SlotCount = 10;
    public const float InteractRange = 2.0f;
    public const int RaycastDistance = 1;
    public const float StaminaCost = 3;
    
    [Header("레이어")]
    public static readonly int TreeLayerMask = LayerMask.GetMask("Tree");
    public static readonly int InteractableLayerMask = LayerMask.GetMask("Interactable");


    [Header("오디오")]
    public const float MinRandomPitch = 0.95f;
    public const float MaxRandomPitch = 1.05f;
    

    [Header("시네머신")]
    public const string ConfinerTag = "BoundsConfiner";
    
    
    [Header("오브젝트")]
    public const float TargetAlpha = 0.5f; 
    public const float FadeInOutSeconds = 1f;
}