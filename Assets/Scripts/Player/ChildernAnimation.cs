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
}
