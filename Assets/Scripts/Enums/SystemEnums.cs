#region 시스템

    public enum SceneName
    {
        Farm,
        Village,
        Home,
        GeneralStore,
        Bridge,
        ResourcesPlace
    }


    public enum PoolTag
    {
        Audio,
        DropedItem,
        CropGrowthPrefab,
        Building,
        TreeGrowthPrefab
    }


    public enum GameEventType
    {
        GameStart,
        SlotSelected,
        OnInventory,
        SceneChange,
        ItemChange
    }


    public enum TeleportType
    {
        SceneToScene,
        SceneToBuilding,
        BuildingToScene,
    }

#endregion


# region 오디오
    public enum AudioTypeEnum
    {
        Master,
        Music,
        SFX,
        Footstep,
        Ambient
    }


    public enum MusicPlayerOption
    {
        Pause,
        UnPause,
        Stop,
        Mute,
        UnMute
    }


    public enum MusicEnums
    {
        Title = 1,
        InGame1,
        InGame2
    }


    public enum SfxEnums
    {
        CropPlant = 100,
        CropHarvest,
        
        ItemPickup,
        ItemDrop,
        
        InventoryOpen,
        InventoryClose,
        ButtonClickPositive,
        ButtonClickNegative,
        ItemSelectInventory,
        ItemDropInventory,
        CraftingItem,
        StorageOpen,
        StorageClose,
        
        Texting,
        
        UseTool,

        HitGrass,
        HitRock,
        HitWood,
        HitTreeFall,
        
        FishingCast,
        FishingCastImpact,
        FishingReel,
        FishingSwing,
        
        DoorOpen,
        DoorClose,
        
        CoinSingle,
        CoinMulti,

        BuildSelect,
        Build,

        Watering,
        Complete,
        QuestAccept,
        QuestGiveup
    }

    public enum FootStep
    {
        Footstep1 = 1000,
        Footstep2,
        Footstep3,
        Footstep4,
        Footstep5,
        Footstep6,
    }


    public enum AmbientEnums
    {
        
    }
# endregion


# region 스킬

    public enum SkillTypeEnums
    {
        Farming,
        Machine,
        Fishing
    }


    public enum FarmSkillEnums
    {
        MoreHarvest,
        HighGrade,
        IncreaseSellingPrice,
        FastGrowth,
        
        UnlockLab,
        IncreaseMutationProbability,
        IncreaseCombinationProbability,
        DecreaseCropsGrowthDay,
        
        IncreaseStamina1,
        IncreaseToolRange,
        ReduceStaminaAmount,
        IncreaseStamina2,
        
        None
    }


#endregion