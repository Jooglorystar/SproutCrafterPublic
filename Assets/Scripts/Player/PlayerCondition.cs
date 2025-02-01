using UnityEngine;


public class PlayerCondition : MonoBehaviour
{
    public HUDController hudController;

    Condition Health { get { return hudController.hpCondition; } }
    Condition Stamina { get { return hudController.staminaCondition; } }
    

    public void Eat()
    {
        Stamina.IncereaseStatValue(Stamina.maxValue);
        KnockOut(Stamina);
    }


    public void UseStamina(float amout)
    {
        Stamina.DecreaseStatValue(amout);
        KnockOut(Stamina);
    }

    
    private void KnockOut(Condition p_condition)
    {
        if (p_condition.currentValue <= 0)
        {
            GameManager.Instance.CharacterM.AnimationController.SetBlackOut(true);
            GameManager.Instance.TimeM.Sleep(8, Setting.SpawnPoint);
        }
        else
        {
            GameManager.Instance.CharacterM.AnimationController.SetBlackOut(false);
        }
    }
}