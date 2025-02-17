using System.Net.NetworkInformation;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement
{
    public Player player;

    public void PlayerMove(float grounddrag)
    {
        player.playerVelocity.Set(player.moveDirection.x * player.movementSpeed, grounddrag);

        player.rb.linearVelocity = player.playerVelocity;

        //Animation
        //if (player.state == Player.States.Ground)
        //{
        //    if (player.moveDirection == Vector2.zero)
        //    {
        //        player.ChangeAnimationState(idlestate);
        //    }
        //    else
        //    {
        //        player.ChangeAnimationState(walkstate);
        //    }
        //}
    }
    public void GroundMovement()
    {
        PlayerMove(-2);
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

        if (player.rb.linearVelocity.y < 2)
        {
            //Animation
            //player.ChangeAnimationState(fallstate);
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
        if (player.currentJumpCount >= player.maxJumpCount) return;

        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            Jump();
            //switch (player.state)
            //{
            //    case Player.States.Ground:
            //        Jump();
            //        break;
            //    case Player.States.GroundIntoAir:
            //        Jump();
            //        break;
            //    case Player.States.Air:
            //        Jump();
            //        break;

            //}
        }
    }
    private void Jump()
    {
        player.currentJumpCount++;
        player.rb.linearVelocity = Vector2.zero;
        player.rb.AddForce(new Vector2(0, player.jumpStrength), ForceMode2D.Impulse);

        if(player.state != Player.States.Air) player.SwitchGroundIntoAir();
    }
}
