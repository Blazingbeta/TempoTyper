using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAlphaScroll : MonoBehaviour {
	[SerializeField] float m_scrollRate;
	[SerializeField] float m_offSet;
	TMPro.TMP_Text m_text;
	float currentTime;
	void Start ()
	{
		m_text = GetComponent<TMPro.TMP_Text>();
		currentTime = m_offSet;
		m_text.alpha = ((-Mathf.Cos(currentTime)) + 1.0f) / 2.0f;
	}
	void Update ()
	{
		currentTime += Time.deltaTime * m_scrollRate;
		m_text.alpha = ((-Mathf.Cos(currentTime)) + 1.0f) / 2.0f;
	}
	private void OnDisable()
	{
		currentTime = m_offSet;
		m_text.alpha = ((-Mathf.Cos(currentTime)) + 1.0f) / 2.0f;
	}
}
