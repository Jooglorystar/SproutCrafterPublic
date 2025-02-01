using UnityEngine;


public class CharacterManager : MonoBehaviour
{
    [HideInInspector] public Inventory inventory; // InGame.cs의 Init에서 지정해줌
    
    [HideInInspector] public PlayerCondition playerCondition;
    [HideInInspector] public InputController inputController;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerInteraction playerInteraction;
    
    
    public PlayerInteraction PlayerInteraction {get {return playerInteraction;}}
    public InputController InputController { get {return inputController;}}
    public PlayerMovement PlayerMovement { get{return playerMovement;}}
    public AnimationController AnimationController { get; set; }
    public SceneName CurrentScene { get; set; }
    

    
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInteraction = GetComponent<PlayerInteraction>();
        playerCondition = GetComponent<PlayerCondition>();
        inputController = GetComponent<InputController>();
        
        CurrentScene = SceneName.Farm;
        
        GameManager.Instance.CharacterM = this;
    }
}