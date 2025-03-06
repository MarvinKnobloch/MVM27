using UnityEngine;

public static class Utility
{
    public static bool LayerCheck(Collider2D collision, LayerMask layer)
    {
        if (((1 << collision.gameObject.layer) & layer) != 0)
        {
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// A function to check if two vectors are aproximatley equivalent
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Approximately(this Vector3 a, Vector3 b, float tolerance = 0.1f)
    {
        return Vector3.Distance(a, b) <= tolerance;
    }

    /// <summary>
    /// A function to check if two vectors are aproximatley equivalent
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Approximately(this Vector2 a, Vector2 b, float tolerance = 0.1f)
    {
        return Vector2.Distance(a, b) <= tolerance;
    }
}
