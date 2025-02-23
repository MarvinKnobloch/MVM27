using UnityEngine;

public class ChildernAnimation : MonoBehaviour
{
    private Player player;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
    }
    public void ActivateInputBuffer() => playerAttack.ActivateInputBuffer();
    public void ExecuteAttack() => playerAttack.ExecuteAttack();
    public void Death() => Player.Instance.RestartGame();
}
