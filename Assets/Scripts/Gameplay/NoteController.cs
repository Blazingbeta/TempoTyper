using System.Collections;
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

	[SerializeField] float m_textCharDistance = 38.422f;
	[SerializeField] float m_textSpaceDistance = 129.362f;

	[SerializeField] RectTransform m_fullTextDisplay = null;
	[SerializeField] TMPro.TMP_Text m_letterHighlightText = null;
	[SerializeField] TMPro.TMP_Text m_hitText = null;

	string m_fullScript;

	private int m_score = 0;
	private int m_hitMultiplier = 0;
	[SerializeField] TMPro.TMP_Text m_scoreText = null;
	[SerializeField] TMPro.TMP_Text m_multiplierText = null;

	[SerializeField] RawImage m_hitCircle = null;
	[SerializeField] Image m_progressMeter = null;
	[SerializeField] AudioSource m_BGM;

	private AudioSource m_hitsoundPlayer = null;

	List<Note> m_activeNotes; //Contains all of the notes currently moving and checking input
	int m_currentLetter = 0;
	int m_highlightedLetterIndex = 0;
	float m_totalSongTime;
	float m_songStartTime;
	void Start ()
	{
		m_hitsoundPlayer = GetComponent<AudioSource>();
		m_activeNotes = new List<Note>();
		m_fullScript = MainMenuController.GameString.ToUpper();
		m_fullTextDisplay.GetComponent<TMPro.TMP_Text>().text = MainMenuController.GameString;
		m_letterHighlightText.text = MainMenuController.GameString[0].ToString();
		//Get the delay that the notes need to be spawned at for the bpm to line up
		float distanceToHitmarker = Mathf.Abs(750f - m_noteHitPosition);
		float timeOffset = distanceToHitmarker / m_noteScrollSpeed;
		m_songStartTime = Time.time;
		//The total time of the song is the offset + timePerNote*notes
		m_totalSongTime = timeOffset*2 + (60 / (float)m_BPM) * m_fullScript.Length;
		Debug.Log(m_totalSongTime);
		StartCoroutine(SpawnNotes((float)60/m_BPM, timeOffset));
	}
	
	void Update ()
	{
		Vector2 scrollAmount = Vector2.left * m_noteScrollSpeed * Time.deltaTime;
		m_activeNotes.ForEach(n => n.m_note.anchoredPosition += scrollAmount);
		if (m_activeNotes.Count > 0 && m_activeNotes[0].IsNoteHit())
		{
			HitNote(0);
			StartCoroutine(ShrinkHitIndicator());
		}
		else if (m_activeNotes.Count > 0 && m_activeNotes[0].m_note.anchoredPosition.x < m_noteMissPosition)
		{
			HitNote(0);
			//m_activeNotes[0].m_text.textInfo.characterInfo[0].
		}
		UpdateProgressBar();
	}
	void UpdateProgressBar()
	{
		m_progressMeter.fillAmount = (Time.time - m_songStartTime) / m_totalSongTime;
	}
	void UpdateScore()
	{
		m_scoreText.text = "Score: " + m_score.ToString("D6");
		m_multiplierText.text = "x" + m_hitMultiplier;
	}
	//Is also called when a note is scrolled off the screen
	void HitNote(int noteIndex)
	{
		m_activeNotes[noteIndex].m_note.gameObject.SetActive(false);
		float distance = Mathf.Abs(m_noteHitPosition - m_activeNotes[noteIndex].m_note.anchoredPosition.x);

		if (distance <= m_goodDistance)
		{
			m_hitMultiplier++;
			m_score += (int)((m_goodDistance - distance) / 5.0f) * m_hitMultiplier;
			UpdateScore();
			if (distance <= m_perfectDistance)
			{
				StartCoroutine(ShowHitText("Perfect", 60.0f / (m_BPM * 1.1f)));
			}
			else
			{
				StartCoroutine(ShowHitText("Good", 60.0f / (m_BPM * 1.1f)));
			}
		}
		else
		{
			m_hitMultiplier = 0;
			UpdateScore();
			StartCoroutine(ShowHitText("Miss", 60.0f / (m_BPM * 1.1f)));
		}
		m_activeNotes.RemoveAt(noteIndex);

		m_highlightedLetterIndex++;
		if (m_highlightedLetterIndex < MainMenuController.GameString.Length)
		{
			if (MainMenuController.GameString[m_highlightedLetterIndex] == ' ')
			{
				m_highlightedLetterIndex++;
				m_fullTextDisplay.anchoredPosition += Vector2.left * (m_textSpaceDistance - m_textCharDistance);
			}
			m_letterHighlightText.text = MainMenuController.GameString[m_highlightedLetterIndex].ToString();
			m_fullTextDisplay.anchoredPosition += Vector2.left * m_textCharDistance;
		}
		else
		{
			m_letterHighlightText.text = "";
			StartCoroutine(EndSong());
		}
	}
	IEnumerator ShrinkHitIndicator()
	{
		m_hitCircle.rectTransform.sizeDelta = Vector2.one * 125;
		m_hitsoundPlayer.Play();
		yield return new WaitForSeconds(0.2f);
		m_hitCircle.rectTransform.sizeDelta = Vector2.one * 150;
	}
	IEnumerator EndSong()
	{
		float timer = 3.0f;
		while(timer > 0)
		{
			m_BGM.volume = timer / 3;
			timer -= Time.deltaTime;
			yield return null;
		}
		m_BGM.volume = 0;
		yield return new WaitForSeconds(0.5f);
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
	int m_currentRunning = 0;
	IEnumerator ShowHitText(string text, float duration)
	{
		m_currentRunning++;
		m_hitText.text = text;
		yield return new WaitForSeconds(duration);
		m_currentRunning--;
		if(m_currentRunning == 0)
			m_hitText.text = "";
	}
	IEnumerator SpawnNotes(float delay, float offset)
	{
		yield return new WaitForSeconds(offset);
		while (m_currentLetter < m_fullScript.Length)
		{
			yield return new WaitForSeconds(delay);
			if(m_fullScript[m_currentLetter] != ' ')
				SpawnNextNote();
			m_currentLetter++;
		}
	}
	void SpawnNextNote()
	{
		Note note = new Note(NotePool.i.GetObjectFromPool(), m_fullScript[m_currentLetter]);
		note.m_note.anchoredPosition = Vector2.right * 750;
		m_activeNotes.Add(note);
	}
}
