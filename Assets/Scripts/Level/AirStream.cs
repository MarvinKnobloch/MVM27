using UnityEngine;

public class AirStream : MonoBehaviour
{
    private Player player;

    [SerializeField] private StreamDirection streamDirection;
    [SerializeField] private int maxAirstreamSpeed;
    [SerializeField] private float airstreamSpeedIncrease;
    [SerializeField] private float sidestreamUpwardsMomentum = 2.1f;

    public enum StreamDirection
    {
        Upwards,
        Left,
        Right,
    }
    private void Start()
    {
        player = Player.Instance;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (player.currentElementNumber != 2) return;

            switch (streamDirection)
            {
                case StreamDirection.Upwards:
                    UpwardsStream();
                    break;
                case StreamDirection.Left:
                    LeftStream();
                    break;
                case StreamDirection.Right:
                    RightStream();
                    break;
            }
        }
    }
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        if (player.currentElementNumber != 2) return;

    //        switch (streamDirection)
    //        {
    //            case StreamDirection.Left:
    //                player.rb.gravityScale = player.baseGravityScale;
    //                break;
    //            case StreamDirection.Right:
    //                player.rb.gravityScale = player.baseGravityScale;
    //                break;
    //        }
    //    }
    //}
    private void UpwardsStream()
    {
        if (player.rb.linearVelocityY < maxAirstreamSpeed)
        {
            if (player.state == Player.States.Ground)
            {
                player.rb.linearVelocityY = 0;
                player.SwitchGroundIntoAir();
            }
            Player.Instance.rb.AddForce(new Vector2(0, airstreamSpeedIncrease), ForceMode2D.Impulse);
        }
    }
    private void LeftStream()
    {
        if (player.sidewardsStreamMovement < maxAirstreamSpeed)
        {
            if (player.state == Player.States.Ground)
            {
                player.SwitchGroundIntoAir();
            }
            if (player.rb.linearVelocityY < 1) player.rb.linearVelocityY = sidestreamUpwardsMomentum;
            player.sidewardsStreamMovement += airstreamSpeedIncrease; // player.sidewardsStreamMovement = airstreamSpeedIncrease; will result in more floating no animation switch movement
        }
    }
    private void RightStream()
    {
        if (player.sidewardsStreamMovement > -maxAirstreamSpeed)
        {
            if (player.state == Player.States.Ground)
            {
                player.SwitchGroundIntoAir();
            }
            if (player.rb.linearVelocityY < 1) player.rb.linearVelocityY = sidestreamUpwardsMomentum;
            player.sidewardsStreamMovement -= airstreamSpeedIncrease;  // player.sidewardsStreamMovement = -airstreamSpeedIncrease; will result in more floating no animation switch movement
        }
    }
}
