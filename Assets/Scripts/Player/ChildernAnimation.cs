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
    public void Death() => player.RestartGame();
    public void NonElementHeal() => player.playerAbilties.NonElementHeal();
    public void SwitchToIdle()
    {
        player.ChangeAnimationState("Idle");
        player.SwitchToGround();
    }
}
