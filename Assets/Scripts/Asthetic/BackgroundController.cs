using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour {
	Material m_skyboxMat;
	[SerializeField] Color m_baseColor, m_lowHealthColor = Color.red, m_deadColor = Color.black;
	[SerializeField] float m_maxSaturation = 0.5f;
	[SerializeField] float m_saturationDecayTime = 0.2f;

	public float HealthAmount { get; set; }
	float m_beatSaturationAmount = 0.0f;
	float m_saturdationDecayAmount = 0.0f;
	bool m_isDead = false;

	void Start () {
		m_skyboxMat = new Material(RenderSettings.skybox);
		RenderSettings.skybox = m_skyboxMat;
		DynamicGI.UpdateEnvironment();

		m_saturdationDecayAmount = m_maxSaturation / m_saturationDecayTime;
		HealthAmount = 1.0f;
	}
	void Update()
	{
		if (!m_isDead)
		{
			//The final color is the current health, +- the Hit/Miss saturation amount
			Color mainColor = Color.Lerp(m_lowHealthColor, m_baseColor, HealthAmount);

			if (m_beatSaturationAmount != 0)
			{
				if (m_beatSaturationAmount > 0)
				{
					//Note was hit, decay downwards
					m_beatSaturationAmount -= m_saturdationDecayAmount * Time.deltaTime;
					if (m_beatSaturationAmount < 0)
					{
						m_beatSaturationAmount = 0.0f;
					}
				}
				else
				{
					//Note was missed, decay upwards
					m_beatSaturationAmount += m_saturdationDecayAmount * Time.deltaTime;
					if (m_beatSaturationAmount > 0)
					{
						m_beatSaturationAmount = 0.0f;
					}
				}

				float h = 0, s = 0, v = 0;
				Color.RGBToHSV(mainColor, out h, out s, out v);
				s += m_beatSaturationAmount;
				mainColor = Color.HSVToRGB(h, s, v);
			}
			m_skyboxMat.SetColor("_Color1", mainColor);
		}
		else
		{
			m_skyboxMat.SetColor("_Color1", m_deadColor);
		}
	}
	public void HitNote()
	{
		m_beatSaturationAmount = m_maxSaturation;
	}
	public void MissNote()
	{
		m_beatSaturationAmount = -m_maxSaturation;
	}
	public void Dead()
	{
		m_isDead = true;
	}
}
