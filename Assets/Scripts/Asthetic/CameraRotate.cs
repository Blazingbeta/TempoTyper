using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour {
	[SerializeField] float m_speed = 90.0f;
	Transform m_transform;
	void Start ()
	{
		m_transform = transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_transform.rotation *= Quaternion.Euler(m_speed*Time.deltaTime, 0, 0);
	}
}
