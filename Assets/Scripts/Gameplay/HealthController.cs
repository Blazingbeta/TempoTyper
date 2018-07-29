using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
	[SerializeField] int m_maxHealth = 100;
	[SerializeField] int m_healthLoss = 25;
	[SerializeField] int m_perfectGain = 25;
	[SerializeField] int m_goodGain = 10;

	BackgroundController m_BGC = null;
	int m_currentHealth;
	private void Start()
	{
		m_currentHealth = m_maxHealth;
		m_BGC = GetComponent<BackgroundController>();
		if (MainMenuController.Difficulty == 1) m_healthLoss *= 2;
	}
	public void HitPerfect()
	{
		m_currentHealth = Mathf.Min(m_currentHealth + m_perfectGain, m_maxHealth);
		UpdateBG();
	}
	public void HitGood()
	{
		m_currentHealth = Mathf.Min(m_currentHealth + m_goodGain, m_maxHealth);
		UpdateBG();
	}
	public void HitMiss()
	{
		m_currentHealth -= m_healthLoss;
		UpdateBG();
		if(m_currentHealth <= 0)
		{
			//is dead
			GetComponent<NoteController>().GameOver();
			m_BGC.Dead();
		}
	}
	void UpdateBG()
	{
		m_BGC.HealthAmount = (float)m_currentHealth / m_maxHealth;

	}
}
