using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SinMover : MonoBehaviour {

	[SerializeField] float rate = 1.0f;
	[SerializeField] float offset = 0.0f;
	[SerializeField] float amplitude = 5.0f;
	float currentTime;

	RectTransform m_transform;
	Vector2 startPos;
	void Start ()
	{
		m_transform = GetComponent<RectTransform>();
		startPos = m_transform.anchoredPosition;
		currentTime = offset;
	}
	
	void Update ()
	{
		currentTime += Time.deltaTime * rate;
		m_transform.anchoredPosition = startPos + Vector2.up * (Mathf.Sin(currentTime) * amplitude);
	}
}
