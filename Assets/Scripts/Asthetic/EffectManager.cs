using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
	[SerializeField] RawImage m_hitCircle = null;
	[SerializeField] ParticleSystem m_particleHitFX = null;
	[SerializeField] AudioSource m_BGM;
	[SerializeField] TMPro.TMP_Text m_hitText = null;
	[SerializeField] TMPro.TMP_Text m_comboNotif = null;
	[SerializeField] AudioClip m_missNoteSFX;
	[SerializeField] AudioClip m_applauseSFX;
	[SerializeField] CanvasGroup m_gameWinPanel = null;

	[SerializeField] float m_minPitch;
	[SerializeField] float m_maxPitch;

	private BackgroundController m_bgController = null;
	private AudioSource m_hitsoundPlayer = null;

	int m_BPM;
	//Particles, screenshake, background controller, SFX,
	//TODO where to put hit indicator
	private void Start()
	{
		m_hitsoundPlayer = GetComponent<AudioSource>();
		m_bgController = GetComponent<BackgroundController>();
	}
	public void Initialize(int bpm)
	{
		m_BPM = bpm;
	}
	public void HitPerfect()
	{
		m_particleHitFX.Play();
		UIShake.i.Shake(0.2f);
		m_bgController.HitNote();
		StartCoroutine(ShowHitText("Perfect", 60.0f / (m_BPM * 1.1f)));
		StartCoroutine(ShrinkHitIndicator(true));
	}
	public void HitGood()
	{
		m_particleHitFX.Play();
		UIShake.i.Shake(0.15f);
		m_bgController.HitNote();
		StartCoroutine(ShowHitText("Good", 60.0f / (m_BPM * 1.1f)));
		StartCoroutine(ShrinkHitIndicator(true));
	}
	public void MissNote(bool wasPressed)
	{
		m_bgController.MissNote();
		StartCoroutine(ShowHitText("Miss", 60.0f / (m_BPM * 1.1f)));
		m_hitsoundPlayer.PlayOneShot(m_missNoteSFX);
		if (wasPressed) StartCoroutine(ShrinkHitIndicator(false));
	}
	int m_currentRunning = 0;
	IEnumerator ShowHitText(string text, float duration)
	{
		m_currentRunning++;
		m_hitText.text = text;
		yield return new WaitForSeconds(duration);
		m_currentRunning--;
		if (m_currentRunning == 0)
			m_hitText.text = "";
	}
	IEnumerator ShrinkHitIndicator(bool wasHit)
	{
		m_hitCircle.rectTransform.sizeDelta = Vector2.one * 125;
		if (wasHit)
		{
			m_hitsoundPlayer.pitch = Random.Range(m_minPitch, m_maxPitch);
			m_hitsoundPlayer.Play();
		}
		yield return new WaitForSeconds(0.2f);
		m_hitCircle.rectTransform.sizeDelta = Vector2.one * 150;
	}
	public void EndSong()
	{
		//Fade out bgm
		StartCoroutine(SongFade());
		StartCoroutine(SongWin());
	}
	//Automatically ends the song, careful
	IEnumerator SongFade()
	{
		float timer = 3.0f;
		while (timer > 0)
		{
			m_BGM.volume = timer / 3;
			timer -= Time.deltaTime;
			yield return null;
		}
		m_BGM.volume = 0;
	}

	IEnumerator SongWin()
	{
		//Show victory text
		//wait for applause to end to end song
		yield return new WaitForSeconds(1.0f);
		m_hitsoundPlayer.PlayOneShot(m_applauseSFX);
		float halfApplauseTime = (m_applauseSFX.length/2.0f) + 0.1f;
		float timer = 0.0f;
		while(timer < halfApplauseTime)
		{
			yield return null;
			timer += Time.deltaTime;
			m_gameWinPanel.alpha = timer / halfApplauseTime;
		}
		m_gameWinPanel.alpha = 1.0f;
		//wait for applause to end
		yield return new WaitForSeconds(halfApplauseTime);
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
	public void ShowComboText(int combo)
	{
		StartCoroutine(ShowComboNotif(combo));

	}
	IEnumerator ShowComboNotif(int combo)
	{
		//Slide in from the bottom, slow in the middle, fade out sliding up
		m_comboNotif.text = combo + "x COMBO";
		m_comboNotif.gameObject.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		m_comboNotif.gameObject.SetActive(false);
	}
}
