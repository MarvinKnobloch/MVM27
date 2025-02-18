using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilties
{
    public Player player;

    public void HeavyPunshInput(InputAction.CallbackContext ctx)
    {
        if (player.menuController.gameIsPaused) return;
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            switch (player.state)
            {
                case Player.States.Ground:
                    StartHeavyPunsh();
                    break;
                case Player.States.GroundIntoAir:
                    StartHeavyPunsh();
                    break;
                case Player.States.Air:
                    StartHeavyPunsh();
                    break;
            }
        }
    }
    private void StartHeavyPunsh()
    {
        player.state = Player.States.HeavyPunch;
    }
    public void HeavyPunch()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(player.heavyPunchCollider.bounds.center, player.heavyPunchCollider.radius, player.heavyPunchLayer);

        foreach (Collider2D col in collider)
        {
            if(col.TryGetComponent(out Destructable destructable))
            {
                destructable.Interaction(player.transform);
            }
        }
        player.SwitchToAir();
    }
}
