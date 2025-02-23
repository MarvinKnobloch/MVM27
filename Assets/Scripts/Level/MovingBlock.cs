using System.Collections;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
	[Header("Movement Settings")]
	public float moveSpeed = 2f;
	public float moveDistance = 3f;
	public float pauseTime = 1f;
	public bool startMovingUp = true;
	
	private Vector3 startPosition;
	private Vector3 targetPosition;
	private bool movingUp;
	private bool isPaused = false;
	private float cycleOffset;

	private void Start()
	{
		startPosition = transform.position;
		targetPosition = startPosition + Vector3.up * moveDistance;

		if (!startMovingUp)
		{
			transform.position = targetPosition;
		}

		movingUp = startMovingUp;

		cycleOffset = Random.Range(0f, pauseTime + (moveDistance / moveSpeed));

		StartCoroutine(StartWithOffset());
	}

	private IEnumerator StartWithOffset()
	{
		yield return new WaitForSeconds(cycleOffset);
		StartCoroutine(MoveBlock());
	}

	private IEnumerator MoveBlock()
	{
		while (true)
		{
			if (!isPaused)
			{
				transform.position = Vector3.MoveTowards(transform.position, movingUp ? targetPosition : startPosition, moveSpeed * Time.deltaTime);

				if (Vector3.Distance(transform.position, movingUp ? targetPosition : startPosition) < 0.01f)
				{
					movingUp = !movingUp;
					isPaused = true;
					yield return new WaitForSeconds(pauseTime);
					isPaused = false;
				}
			}
			yield return null;
		}
	}
}
