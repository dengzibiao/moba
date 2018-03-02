using UnityEngine;

public class RotateAround : MonoBehaviour
{
	public Transform Target;
	public float Speed = 1f;

	protected Vector3 m_TargetPosition;

	void Start()
	{
		m_TargetPosition = Target.position;
	}

	void Update()
	{
		transform.RotateAround(m_TargetPosition, Vector3.up, Time.deltaTime * Speed);
	}
}
