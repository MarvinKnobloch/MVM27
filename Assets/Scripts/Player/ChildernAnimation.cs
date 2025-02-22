using UnityEngine;

public class ChildernAnimation : MonoBehaviour
{
    private PlayerAttack playerAttack;

    private void Awake()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
    }
    public void ActivateInputBuffer() => playerAttack.ActivateInputBuffer();
    public void ExecuteAttack() => playerAttack.ExecuteAttack();
}
