using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextHueScroll : MonoBehaviour
{
	[SerializeField] float m_scrollSpeed;
	TMPro.TMP_Text m_text;
	float m_currentHue;
	private void Start()
	{
		m_currentHue = Random.Range(0.0f, 1.0f);
		m_text = GetComponent<TMPro.TMP_Text>();
	}
	void Update ()
	{
		m_currentHue += Time.deltaTime * m_scrollSpeed;
		m_currentHue %= 1;
		m_text.color = Color.HSVToRGB(m_currentHue, 1, 1);
	}
}
