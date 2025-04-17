using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement
{
    public Player player;
    private float dashTimer;

    const string idleState = "Idle";
    const string runState = "Run";
    const string jumpState = "Jump";
    const string fallState = "Fall";
    const string dashState = "Dash";

    private float platformGroundDrag = -10;

    public void PlayerMove(float grounddrag)
    {
        if (player.XWallBoostMovement > 0.1f) player.XWallBoostMovement -= Time.fixedDeltaTime * 7;
        else if (player.XWallBoostMovement < -0.1f) player.XWallBoostMovement += Time.fixedDeltaTime * 7;
        else player.XWallBoostMovement = 0;

        if (player.sidewardsStreamMovement > 0.1f) player.sidewardsStreamMovement -= Time.fixedDeltaTime * 10;
        else if (player.sidewardsStreamMovement < -0.1f) player.sidewardsStreamMovement += Time.fixedDeltaTime * 10;
        else player.sidewardsStreamMovement = 0;

        if (player.movingPlatform != null)
        {
            if(player.state == Player.States.GroundIntoAir || player.state == Player.States.Air)
            {
                float additionalMovement = player.XWallBoostMovement + player.sidewardsStreamMovement;
                player.playerVelocity.Set(player.moveDirection.x * player.movementSpeed + additionalMovement, grounddrag);
            }
            else
            {
                float additionalMovement = player.XWallBoostMovement + player.sidewardsStreamMovement + player.movingPlatform.velocity.x;
                player.playerVelocity.Set((player.moveDirection.x * player.movementSpeed) + additionalMovement, player.movingPlatform.velocity.y + platformGroundDrag);
            }
        }
        else
        {
            float additionalMovement = player.XWallBoostMovement + player.sidewardsStreamMovement;
            player.playerVelocity.Set(player.moveDirection.x * player.movementSpeed + additionalMovement, grounddrag);
        }

        player.rb.linearVelocity = player.playerVelocity;

        //Animation
        if (player.state == Player.States.Ground)
        {
            if (player.moveDirection == Vector2.zero)
            {
                player.ChangeAnimationState(idleState);
            }
            else
            {
                player.ChangeAnimationState(runState);
            }
        }
    }
    public void GroundMovement()
    {
        PlayerMove(player.playerGroundDrag);
    }
    public void GroundIntoAirTransition()
    {
        player.groundIntoAirTimer += Time.deltaTime;

        if (player.groundIntoAirTimer > player.groundIntoAirOffset)
        {
            player.SwitchToAir();
        }
    }
    public void AirMovement()
    {
        if (player.rb.linearVelocity.y < player.maxFallSpeed) PlayerMove(player.maxFallSpeed);
        else PlayerMove(player.rb.linearVelocity.y);

        if(player.rb.linearVelocity.y < -12)
        {
            player.ChangeAnimationState("FastFall");
        }
        else if (player.rb.linearVelocity.y < 2)
        {
            player.ChangeAnimationState(fallState);
        }
        else
        {
            player.ChangeAnimationState(jumpState);
        }
    }
    public void AttackMovement()
    {
        if (player.autoAttackMovement)
        {
            if(player.faceRight) player.playerVelocity.Set(-player.attackMovementSpeed, player.rb.linearVelocityY);
            else player.playerVelocity.Set(player.attackMovementSpeed, player.rb.linearVelocityY);

            player.rb.linearVelocity = player.playerVelocity;
        }
        else
        {
            if (player.movingPlatform != null)
            {
                float additionalMovement = player.XWallBoostMovement + player.sidewardsStreamMovement + player.movingPlatform.velocity.x;
                player.playerVelocity.Set((player.moveDirection.x * player.attackMovementSpeed) + additionalMovement, player.movingPlatform.velocity.y + player.playerGroundDrag);

                player.rb.linearVelocity = player.playerVelocity;
            }
            else
            {
                player.playerVelocity.Set(player.moveDirection.x * player.attackMovementSpeed, player.rb.linearVelocityY);
                player.rb.linearVelocity = player.playerVelocity;
            }
        }
    }
    public void AbilityMovement()
    {
        RaycastHit2D downwardhit = Physics2D.BoxCast(player.playerCollider.bounds.center, player.playerCollider.bounds.size * 0.99f, 0, -player.transform.up, 0.05f, player.groundCheckLayer);
        if (downwardhit)
        {
            if (player.movingPlatform != null)
            {
                float additionalMovement = player.XWallBoostMovement + player.sidewardsStreamMovement + player.movingPlatform.velocity.x;
                player.playerVelocity.Set(additionalMovement, player.movingPlatform.velocity.y + player.playerGroundDrag);

                player.rb.linearVelocity = player.playerVelocity;
            }
            else
            {
                player.rb.linearVelocityX = 0;
            }
        }
        else
        {
            if (player.rb.linearVelocity.y < player.maxFallSpeed) PlayerMove(player.maxFallSpeed);
            else PlayerMove(player.rb.linearVelocity.y);
        }
    }
    public void RotatePlayer()
    {
        if (player.moveDirection.x > 0 && player.faceRight == true) flip();
        if (player.moveDirection.x < 0 && player.faceRight == false) flip();
    }
    private void flip()
    {
        player.faceRight = !player.faceRight;
        Vector3 localScale;
        localScale = player.transform.localScale;
        localScale.x *= -1;
        player.transform.localScale = localScale;
    }
    public void JumpInput(InputAction.CallbackContext ctx)
    {

        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            JumpInputPerformed();
        }
    }
    private void JumpInputPerformed()
    {
        if (player.menuController.gameIsPaused) return;

        int count = player.maxJumpCount;
        if (player.currentElementNumber == 2) count += 1;
        if (player.currentJumpCount >= count) return;

        switch (player.state)
        {
            case Player.States.Ground:
                Jump();
                break;
            case Player.States.GroundIntoAir:
                Jump();
                break;
            case Player.States.Air:
                Jump();
                break;
        }
    }
    private void Jump()
    {
        player.currentJumpCount++;
        player.rb.linearVelocity = Vector2.zero;
        player.rb.AddForce(new Vector2(0, player.jumpStrength), ForceMode2D.Impulse);

        player.jumpPerformed = true;
        player.jumpTimer = 0;
        //player.ChangeAnimationState(jumpState);

        if(player.state != Player.States.Air) player.SwitchGroundIntoAir();

        if (player.currentElementNumber == 0) AudioManager.Instance.PlayRandomOneShot(AudioManager.Instance.nonJumpSounds);
        else if (player.currentElementNumber == 1) AudioManager.Instance.PlayRandomOneShot(AudioManager.Instance.fireJumpSounds);
        else if (player.currentElementNumber == 2) AudioManager.Instance.PlayRandomOneShot(AudioManager.Instance.airJumpSounds);

    }
    public void JumpIsPressed()
    {
        if (player.jumpPerformed == false) return;

        player.jumpTimer += Time.deltaTime;
        if(player.jumpTimer > player.maxJumpTime)
        {
            player.jumpPerformed = false;
        }
        if (player.controls.Player.Jump.WasReleasedThisFrame() || Input.GetButtonUp("Jump"))
        {
            float velocityReduce = player.maxJumpTime - player.jumpTimer;
            player.rb.AddForce(new Vector2(0, velocityReduce * -20), ForceMode2D.Impulse);
            player.jumpPerformed = false;
        }
    }
    public void DashInput(InputAction.CallbackContext ctx)
    {
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            DashInputPerformed();
        }
    }
    private void DashInputPerformed()
    {
        if (player.menuController.gameIsPaused) return;
        if (player.fireElementUnlocked == false) return;
        if (player.currentDashCount >= player.maxDashCount) return;

        switch (player.state)
        {
            case Player.States.Ground:
                StartDash();
                break;
            case Player.States.GroundIntoAir:
                StartDash();
                break;
            case Player.States.Air:
                StartDash();
                break;
            case Player.States.NonElementalHeal:
                StartDash();
                break;
            case Player.States.Attack:
                StartDash();
                break;
            case Player.States.HeavyPunch:
                StartDash();
                break;
        }
    }
    private void StartDash()
    {
        player.playerAttack.state = PlayerAttack.States.Empty;

        player.currentDashCount++;
        player.rb.linearVelocity = Vector2.zero;
        player.rb.gravityScale = 0;

        //if(player.faceRight) player.rb.AddForce(-player.transform.right * player.dashStrength, ForceMode2D.Impulse);
        //else player.rb.AddForce(player.transform.right * player.dashStrength, ForceMode2D.Impulse);

        RotatePlayer();

        player.ChangeAnimationState(dashState);
        dashTimer = 0;
        player.state = Player.States.Dash;

        if (player.currentElementNumber == 0) AudioManager.Instance.PlayRandomOneShot(AudioManager.Instance.nonDashSounds);
        else if (player.currentElementNumber == 1) AudioManager.Instance.PlayRandomOneShot(AudioManager.Instance.fireDashSounds);
        else if (player.currentElementNumber == 2) AudioManager.Instance.PlayRandomOneShot(AudioManager.Instance.airDashSounds);
    }
    public void DashMovement()
    {
        Vector2 movement = new Vector2(player.dashStrength, 0);

        if (player.faceRight) player.rb.linearVelocity = movement * -player.transform.right;
        else player.rb.linearVelocity = movement * player.transform.right;
    }
    public void DashTime()
    {
        dashTimer += Time.deltaTime;
        if(dashTimer >= player.dashTime)
        {
            player.SwitchToAir();
        }
    }
    public void WallBoost()
    {
        if (player.wallBoostUnlocked == false) return;

        if (player.state == Player.States.Air)
        {
            if (player.canWallBoost && player.performedWallBoost == false)
            {
                player.performedWallBoost = true;
                if (player.faceRight)
                {
                    player.XWallBoostMovement = player.XWallBoostStrength;
                    player.rb.AddForce(player.transform.up * player.YWallBoostStrength, ForceMode2D.Impulse);
                }
                else
                {
                    player.XWallBoostMovement = -player.XWallBoostStrength;
                    player.rb.AddForce(player.transform.up * player.YWallBoostStrength, ForceMode2D.Impulse);
                }
            }
        }
    }
    public void PlayerHitStun()
    {
        player.hitStunTimer += Time.deltaTime;
        if(player.hitStunTimer >= player.hitStunTime)
        {
            player.SwitchToAir();
        }
    }

    public void ControllerDashInput()
    {
        if (Input.GetButtonDown("Dash")) DashInputPerformed();
    }
    public void ControllerJumpInput()
    {
        if (Input.GetButtonDown("Jump")) JumpInputPerformed();
    }

}
