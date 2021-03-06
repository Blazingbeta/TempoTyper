﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteController : MonoBehaviour {

	#region Note
	class Note
	{
		public RectTransform m_note;
		public TMPro.TMP_Text m_text;
		public KeyCode m_keycode;
		public Note(GameObject obj, char letter)
		{
			obj.SetActive(true);
			m_note = obj.GetComponent<RectTransform>();
			m_text = obj.GetComponentInChildren<TMPro.TMP_Text>();
			m_text.text = letter.ToString();
			m_keycode = UtilityFunc.GetKeycodeFromChar(letter);
		}
		public bool IsNoteHit()
		{
			return Input.GetKeyDown(m_keycode);
		}
	}
	#endregion

	[SerializeField] int m_BPM = 140;
	[SerializeField] int m_noteScrollSpeed = 200;
	[SerializeField] int m_noteHitPosition = -530;
	[SerializeField] int m_noteMissPosition = -750;
	[SerializeField] int m_perfectDistance = 30;
	[SerializeField] int m_goodDistance = 100;
	[SerializeField] int m_maximumHitDistance = 300;
	[SerializeField] float m_gameDelayAmount = 1.5f;

	[SerializeField] float m_textCharDistance = 38.422f;
	[SerializeField] float m_textSpaceDistance = 129.362f;

	[SerializeField] RectTransform m_fullTextDisplay = null;
	[SerializeField] TMPro.TMP_Text m_letterHighlightText = null;

	string m_noteScript;
	string m_textScript;

	private int m_score = 0;
	private int m_hitMultiplier = 0;
	[SerializeField] TMPro.TMP_Text m_scoreText = null;
	[SerializeField] TMPro.TMP_Text m_multiplierText = null;

	[SerializeField] Image m_progressMeter = null;

	[SerializeField] AudioSource m_BGM = null;
	[SerializeField] CanvasGroup m_scrollingPanel = null;
	[SerializeField] CanvasGroup m_gameoverPanel = null;

	EffectManager m_effectManager = null;
	HealthController m_healthController = null;

	List<Note> m_activeNotes; //Contains all of the notes currently moving and checking input
	int m_currentLetter = 0;
	int m_highlightedLetterIndex = 0;
	float m_totalSongTime;
	float m_songStartTime;
	bool m_isGameOver = false;
	void Start ()
	{
		InitializeSong();
		m_effectManager = GetComponent<EffectManager>();
		m_healthController = GetComponent<HealthController>();

		m_effectManager.Initialize(m_BPM);
		m_activeNotes = new List<Note>();
		StartGame();
	}
	void InitializeSong()
	{
		BeatData data = Resources.Load(MainMenuController.DataFilePath) as BeatData;
		m_BPM = data.BPM;
		m_BGM.clip = data.Song;
		m_noteScrollSpeed = data.NoteScrollSpeed;
		m_textScript = data.Text;
		m_noteScript = data.Text.ToUpper();
		if (MainMenuController.Difficulty == 1)
		{
			//m_noteScrollSpeed *= 2;
			m_BPM *= 2;
			m_textScript = data.HardText;
			m_noteScript = data.HardText.ToUpper();
		}

		m_fullTextDisplay.GetComponent<TMPro.TMP_Text>().text = m_textScript;
		m_letterHighlightText.text = m_textScript[0].ToString();
	}
	void StartGame()
	{
		//Get the delay that the notes need to be spawned at for the bpm to line up
		float distanceToHitmarker = Mathf.Abs(750f - m_noteHitPosition);
		float timeOffset = distanceToHitmarker / m_noteScrollSpeed;
		//DELAY THE SONG START NOT THE NOTE SPAWN
		m_songStartTime = Time.time;
		//The total time of the song is the offset + timePerNote*notes
		m_totalSongTime = timeOffset + (60 / (float)m_BPM) * m_textScript.Length;
		//Both of these coroutines wait the game delay just so everything fades in correctly
		StartCoroutine(SpawnNotes((float)60 / m_BPM));
		StartCoroutine(DelaySongStart(timeOffset));
	}
	List<KeyCode> m_alreadyCheckedKeys = new List<KeyCode>();
	void Update ()
	{
		if (m_isGameOver) return;
		//For each, if not already checked keycode, then check Isnotehit
		List<Note> ienumarbleNotes = new List<Note>(m_activeNotes);
		foreach (Note note in ienumarbleNotes)
		{
			if (!m_alreadyCheckedKeys.Contains(note.m_keycode))
			{
				m_alreadyCheckedKeys.Add(note.m_keycode);
				if (note.IsNoteHit() && Mathf.Abs(m_noteHitPosition - note.m_note.anchoredPosition.x) <= m_maximumHitDistance)
				{
					HitNote(note, true);
					//StartCoroutine(ShrinkHitIndicator());
				}
				else if (note.m_note.anchoredPosition.x < m_noteMissPosition)
				{
					HitNote(note, false);
					//m_activeNotes[0].m_text.textInfo.characterInfo[0].
				}
			}
		}
		m_alreadyCheckedKeys.Clear();
		Vector2 scrollAmount = Vector2.left * m_noteScrollSpeed * Time.deltaTime;
		m_activeNotes.ForEach(n => n.m_note.anchoredPosition += scrollAmount);
		UpdateProgressBar();
	}
	//Called from HealthController
	public void GameOver()
	{
		//Freeze Input
		m_isGameOver = true;
		StartCoroutine(EndGame());
	}
	IEnumerator EndGame()
	{
		//Yeah this really shouldn't be in this class
		float fadeTime = 4.0f;
		float timer = fadeTime;
		while(timer > 0)
		{
			timer -= Time.deltaTime;
			float percent = timer / fadeTime;
			//Song Slowdown
			m_BGM.pitch = percent;
			//Notes fade
			m_scrollingPanel.alpha = Mathf.Clamp01((timer - (fadeTime/2))/fadeTime);
			//Display Gameover
			m_gameoverPanel.alpha = 1.0f - percent;
			yield return null;
		}
		//Back to menu
		yield return new WaitForSeconds(1.0f);
		{
			SceneLoader.i.LoadScene(0);
		}
	}
	void UpdateProgressBar()
	{
		m_progressMeter.fillAmount = (Time.time - m_songStartTime) / m_totalSongTime;
	}
	void UpdateScore()
	{
		m_scoreText.text = "Score: " + m_score.ToString("D6");
		m_multiplierText.text = "x" + m_hitMultiplier;
		//If combo is a multiple of 15 or 50, show notify but not if the 15 is within 5 of a 50 combo mark
		if(m_hitMultiplier != 0 && (m_hitMultiplier % 15 == 0 && m_hitMultiplier % 50 != 45 && m_hitMultiplier % 50 != 5
			|| m_hitMultiplier % 50 == 0))
		{
			m_effectManager.ShowComboText(m_hitMultiplier);
		}
	}
	//Is also called when a note is scrolled off the screen
	void HitNote(Note note, bool wasPressed)
	{
		note.m_note.gameObject.SetActive(false);
		float distance = Mathf.Abs(m_noteHitPosition - note.m_note.anchoredPosition.x);

		if (distance <= m_goodDistance)
		{
			m_hitMultiplier++;
			m_score += (int)((m_goodDistance - distance) / 5.0f) * m_hitMultiplier;
			UpdateScore();
			if (distance <= m_perfectDistance)
			{
				m_effectManager.HitPerfect();
				m_healthController.HitPerfect();
			}
			else
			{
				m_effectManager.HitGood();
				m_healthController.HitGood();
			}
		}
		else
		{
			m_hitMultiplier = 0;
			UpdateScore();
			m_effectManager.MissNote(wasPressed);
			m_healthController.HitMiss();
		}
		m_activeNotes.Remove(note);

		m_highlightedLetterIndex++;
		if (m_highlightedLetterIndex < m_textScript.Length)
		{
			if (m_textScript[m_highlightedLetterIndex] == ' ')
			{
				m_highlightedLetterIndex++;
				m_fullTextDisplay.anchoredPosition += Vector2.left * (m_textSpaceDistance - m_textCharDistance);
			}
			m_letterHighlightText.text = m_textScript[m_highlightedLetterIndex].ToString();
			m_fullTextDisplay.anchoredPosition += Vector2.left * m_textCharDistance;
		}
		else
		{
			m_letterHighlightText.text = "";
			m_effectManager.EndSong(m_score);
		}
	}
	IEnumerator SpawnNotes(float delay)
	{
		yield return new WaitForSeconds(m_gameDelayAmount);
		float timePassed = 0.0f;
		while (m_currentLetter < m_textScript.Length && !m_isGameOver)
		{
			if(m_textScript[m_currentLetter] != ' ')
				SpawnNextNote();
			m_currentLetter++;
			while(timePassed < delay)
			{
				timePassed += Time.deltaTime;
				yield return null;
			}
			timePassed -= delay;
		}
	}
	IEnumerator DelaySongStart(float offset)
	{
		yield return new WaitForSeconds(offset + m_gameDelayAmount);
		m_BGM.Play();
		//throw new System.Exception();
	}
	void SpawnNextNote()
	{
		Note note = new Note(NotePool.i.GetObjectFromPool(), m_noteScript[m_currentLetter]);
		note.m_note.anchoredPosition = Vector2.right * 750;
		m_activeNotes.Add(note);
	}
}
