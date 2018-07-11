using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour {
	Material m_skyboxMat;
	Color m_defaultColor;
	// Use this for initialization
	void Start () {
		m_skyboxMat = new Material(RenderSettings.skybox);
		m_defaultColor = m_skyboxMat.GetColor("_Color1");
		RenderSettings.skybox = m_skyboxMat;
		DynamicGI.UpdateEnvironment();
	}

	// Update is called once per frame
	void Update ()
	{
	}
}
