using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVScroll : MonoBehaviour {
	[SerializeField] private Vector2 m_scrollSpeed;
	Vector2 currentOffset = Vector2.zero;
	Material m_mat;
	private void Start()
	{
		m_mat = GetComponent<Renderer>().material;
	}
	void LateUpdate ()
	{
		currentOffset += m_scrollSpeed * Time.deltaTime;
		m_mat.SetTextureOffset("_MainTex", currentOffset);
	}
}
