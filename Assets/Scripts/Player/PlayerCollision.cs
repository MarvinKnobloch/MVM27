using UnityEngine;

public class PlayerCollision
{
    public Player player;

    public void GroundCheck()
    {
        RaycastHit2D downwardhit = Physics2D.BoxCast(player.playerCollider.bounds.center, player.playerCollider.bounds.size * 0.96f, 0, -player.transform.up, 0.3f, player.groundCheckLayer);
        if (downwardhit)
        {
            Debug.DrawRay(downwardhit.point, downwardhit.normal, Color.green);
        }
        else
        {
            player.SwitchGroundIntoAir();
        }
    }
    public void AirCheck()
    {
        if (player.rb.linearVelocity.y <= 0.1f)
        {
            ForwardCheck();

            RaycastHit2D downwardhit = Physics2D.BoxCast(player.playerCollider.bounds.center, player.playerCollider.bounds.size * 0.96f, 0, -player.transform.up, 0.2f, player.groundCheckLayer);
            if (downwardhit)
            {
                {
                    player.SwitchToGround();
                }
            }
        }
        else
        {
            ForwardCheck();
        }
    }
    private void ForwardCheck()
    {
        if (!player.faceRight)
        {
            RaycastHit2D forwardhit = Physics2D.BoxCast(player.playerCollider.bounds.center, player.playerCollider.bounds.size * 0.96f, 0, player.transform.right, 0.2f, player.groundCheckLayer);
            if (forwardhit)
            {
                player.canWallBoost = true;
                player.playerVelocity.Set(0, player.rb.linearVelocity.y);
                player.rb.linearVelocity = player.playerVelocity;
            }
            else player.canWallBoost = false;
        }
        else
        {
            RaycastHit2D forwardhit = Physics2D.BoxCast(player.playerCollider.bounds.center, player.playerCollider.bounds.size * 0.96f, 0, -player.transform.right, 0.2f, player.groundCheckLayer);

            if (forwardhit)
            {
                player.canWallBoost = true;
                player.playerVelocity.Set(0, player.rb.linearVelocity.y);
                player.rb.linearVelocity = player.playerVelocity;
            }
            else player.canWallBoost = false;
        }
    }
}
