using UnityEngine;

public class ChildernAnimation : MonoBehaviour
{
    private Player player;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        playerAttack = GetComponentInParent<PlayerAttack>();
    }
    public void ActivateInputBuffer() => playerAttack.ActivateInputBuffer();
    public void ExecuteAttack() => playerAttack.ExecuteAttack();
    public void EndAttack() => playerAttack.EndAttack();
    public void Death() => player.RestartGame();
    public void NonElementHeal() => player.playerAbilties.NonElementHeal();
    public void ExecuteHeavyPunck() => player.playerAbilties.ExecuteHeavyPunch();
    public void EndHeavyPunch() => player.playerAbilties.EndHeavyPunch();
    public void SwitchToIdle()
    {
        player.ChangeAnimationState("Idle");
        player.SwitchToGround(false);
    }
    public void NonPlayLeftFoot()
    {
        int element = Player.Instance.currentElementNumber;
        int number = Random.Range(0, 3);
        switch (element)
        {
            case 0:
                AudioManager.Instance.PlayFootSteps(AudioManager.Instance.nonStepsSounds, number);
                break;
            case 1:
                AudioManager.Instance.PlayFootSteps(AudioManager.Instance.fireStepSounds, number);
                break;
            case 2:
                AudioManager.Instance.PlayFootSteps(AudioManager.Instance.airStepSounds, number);
                break;
        }
    }
    public void NonPlayRightFoot()
    {
        int element = Player.Instance.currentElementNumber;
        int number = Random.Range(3, 6);
        switch (element)
        {
            case 0:
                AudioManager.Instance.PlayFootSteps(AudioManager.Instance.nonStepsSounds, number);
                break;
            case 1:
                AudioManager.Instance.PlayFootSteps(AudioManager.Instance.fireStepSounds, number);
                break;
            case 2:
                AudioManager.Instance.PlayFootSteps(AudioManager.Instance.airStepSounds, number);
                break;
        }
    }
}
