using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilties
{
    public Player player;
    private float castTimer;

    const string switchState = "Switch";
    const string elementHealState = "Heal";


    public void Ability1Input(InputAction.CallbackContext ctx)
    {
        if (player.menuController.gameIsPaused) return;
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            if (player.currentElementNumber == 0) NonElementAbility1();
            if (player.currentElementNumber == 1) FireAbility1();
            if (player.currentElementNumber == 2) player.playerMovement.WallBoost();
        }
    }
    public void Ability2Input(InputAction.CallbackContext ctx)
    {
        if (player.menuController.gameIsPaused) return;
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            if (player.currentElementNumber == 0) NonElementAbility2();
        }
    }
    private void NonElementAbility1()
    {
        switch (player.state)
        {
            case Player.States.Ground:
                StartNonElementHeal();
                break;
            case Player.States.GroundIntoAir:
                StartNonElementHeal();
                break;
            case Player.States.Air:
                StartNonElementHeal();
                break;
        }
    }
    private void StartNonElementHeal()
    {
        if (player.EnergyValue < player.elementHealCosts) return;

        if (player.state == Player.States.Air || player.state == Player.States.GroundIntoAir)
        {
            player.rb.linearVelocity = Vector2.zero;
        }
        else player.rb.linearVelocityX = 0;

        player.ChangeAnimationState(elementHealState);
        player.state = Player.States.NonElementalHeal;
    }
    public void HoldHeal()
    {
        if (player.controls.Player.ElementAbility1.WasReleasedThisFrame())
        {
            player.SwitchToAir();
        }
    }
    public void NonElementHeal()
    {
        if (player.state != Player.States.NonElementalHeal) return;

        //player.rb.linearVelocity = Vector2.zero;
        player.EnergyUpdate(-player.elementHealCosts);
        player.health.Heal(player.elementHealAmount + PlayerPrefs.GetInt(Upgrades.StatsUpgrades.BonusHeal.ToString()));
        player.playerCollision.CollisionCheckAfterAbilties();
        //player.SwitchToAir();
    }
    private void NonElementAbility2()
    {
        if (player.wallbreakUnlocked == false) return;

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
        if (player.EnergyValue < player.heavyPunchCosts) return;

        player.ChangeAnimationState("HeavyPunch");
        player.state = Player.States.HeavyPunch;
    }
    public void ExecuteHeavyPunch()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(player.heavyPunchCollider.bounds.center, player.heavyPunchCollider.radius, player.heavyPunchLayer);

        foreach (Collider2D col in collider)
        {
            if (col.TryGetComponent(out Destructable destructable))
            {
                destructable.Interaction(player.transform);
            }
            else if (col.TryGetComponent(out Health health))
            {
                health.TakeDamage(player.heavyPunchDamage, false);
            }
        }
        player.EnergyUpdate(-player.heavyPunchCosts);
    }
    public void EndHeavyPunch()
    {
        if (player.state != Player.States.HeavyPunch) return;
        player.playerCollision.CollisionCheckAfterAbilties();
    }
    private void FireAbility1()
    {
        if (player.fireBallUnlocked == false) return;
        if (player.EnergyValue < player.fireballCosts) return;

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
        player.rb.linearVelocityX = 0;
        player.state = Player.States.FireBall;
    }
    public void CastFireball()
    {
        castTimer += Time.deltaTime;
        if(castTimer >= player.fireballCastTime)
        {
            player.EnergyUpdate(-player.fireballCosts);
            player.CreatePrefab(player.fireballPrefab, player.projectileSpawnPosition);
            player.playerCollision.CollisionCheckAfterAbilties();
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
        if (player.fireElementUnlocked == false) return;

        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            CheckElementalSwitch(1);
        }
    }
    public void ThirdElementInput(InputAction.CallbackContext ctx)
    {
        if (player.menuController.gameIsPaused) return;
        if (player.airElementUnlocked == false) return;

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
        player.elementalSprite[player.currentElementNumber].gameObject.SetActive(false);

        player.currentAnimator = player.elementalAnimator[slot];
        player.currentElementNumber = slot;
        player.elementalSprite[player.currentElementNumber].gameObject.SetActive(true);
        if (player.iFramesBlink) player.elementalSprite[player.currentElementNumber].color = Color.red;
        else player.elementalSprite[player.currentElementNumber].color = Color.white;

        player.currentstate = null;
        //player.ChangeAnimationState(switchState);
    }
}
