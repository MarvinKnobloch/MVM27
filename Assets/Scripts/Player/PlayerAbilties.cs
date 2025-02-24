using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilties
{
    public Player player;
    const string switchState = "PlayerSwitch";
    private float castTimer;


    public void Ability1Input(InputAction.CallbackContext ctx)
    {
        if (player.menuController.gameIsPaused) return;
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            if (player.currentElementNumber == 0) NonElementAbility1();
            if (player.currentElementNumber == 1) FireAbility1();
            //if (player.currentElementNumber == 2) AirElementAbiltiy1();
        }
    }
    private void NonElementAbility1()
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
    private void FireAbility1()
    {
        switch (player.state)
        {
            case Player.States.Ground:
                StartShootFireball();
                break;
            case Player.States.GroundIntoAir:
                StartShootFireball();
                break;
            case Player.States.Air:
                StartShootFireball();
                break;
        }
    }
    private void StartShootFireball()
    {
        castTimer = 0;
        player.state = Player.States.FireBall;
    }
    public void CastFireball()
    {
        castTimer += Time.deltaTime;
        if(castTimer >= player.fireballCastTime)
        {
            player.CreatePrefab(player.fireballPrefab, player.projectileSpawnPosition);
            player.SwitchToAir();
        }
    }
    private void AirAbility1()
    {

    }
    public void FirstElementInput(InputAction.CallbackContext ctx)
    {
        if (player.menuController.gameIsPaused) return;
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            CheckElementalSwitch(0);
        }
    }
    public void SecondElementInput(InputAction.CallbackContext ctx)
    {
        if (player.menuController.gameIsPaused) return;
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            CheckElementalSwitch(1);
        }
    }
    public void ThirdElementInput(InputAction.CallbackContext ctx)
    {
        if (player.menuController.gameIsPaused) return;
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            CheckElementalSwitch(2);
        }
    }
    private void CheckElementalSwitch(int slot)
    {
        if (player.currentElementNumber == slot) return;
        switch (player.state)
        {
            case Player.States.Ground:
                ElementalSwitch(slot);
                break;
            case Player.States.GroundIntoAir:
                ElementalSwitch(slot);
                break;
            case Player.States.Air:
                ElementalSwitch(slot);
                break;
        }
    }
    public void ElementalSwitch(int slot)
    {
        player.elementalSprite[player.currentElementNumber].SetActive(false);

        player.currentAnimator = player.elementalAnimator[slot];
        player.currentElementNumber = slot;
        player.elementalSprite[player.currentElementNumber].SetActive(true);

        player.ChangeAnimationState(switchState);
    }
}
