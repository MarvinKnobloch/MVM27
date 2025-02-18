using UnityEngine;

public static class Utility
{
    public static bool LayerCheck(Collider2D collision, LayerMask layer)
    {
        if (((1 << collision.gameObject.layer) & layer) != 0)
        {
            return true;
        }
        else return false;
    }
}
