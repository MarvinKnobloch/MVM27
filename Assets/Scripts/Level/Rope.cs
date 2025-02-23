using UnityEngine;

public class SwingingRope : MonoBehaviour
{
	private Rigidbody2D rb;
	public float swayForce = 10f;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();

		if (rb == null)
		{
			Debug.LogError("Rigidbody2D is missing on the Rope object!");
			return;
		}

		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.gravityScale = 1; 

		
		if (GetComponent<HingeJoint2D>() == null)
		{
			Debug.LogError("HingeJoint2D is required for the rope to pivot.");
		}
	}

	
	public void ApplySwingForce(float direction)
	{
		if (rb != null)
		{
			rb.AddForce(new Vector2(direction * swayForce, 0), ForceMode2D.Force);
		}
	}
}
