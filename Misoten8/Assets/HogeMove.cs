using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HogeMove : MonoBehaviour
{
	float m_angle = 0.0f;

	void Update ()
	{
		m_angle += 0.03f;
		transform.position = new Vector3(Mathf.Sin(m_angle), 0.0f, Mathf.Cos(m_angle)) * 10.0f;
	}
}
