using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	[SerializeField] string testString = "Hello, World!";
	string m_fullScript;


	List<Note> m_activeNotes; //Contains all of the notes currently moving and checking input
	int m_currentLetter = 0;
	int m_highlightedLetterIndex = 0;
	void Start ()
	{
		m_activeNotes = new List<Note>();
		m_fullScript = testString.ToUpper();
		m_fullTextDisplay.GetComponent<TMPro.TMP_Text>().text = testString;
		m_letterHighlightText.text = testString[0].ToString();
		//Get the delay that the notes need to be spawned at for the bpm to line up
		float distanceToHitmarker = 750f + m_noteHitPosition;
		float timeOffset = distanceToHitmarker / m_noteScrollSpeed;
		StartCoroutine(SpawnNotes((float)60/m_BPM, timeOffset));
	}
	
	void Update ()
	{
		Vector2 scrollAmount = Vector2.left * m_noteScrollSpeed * Time.deltaTime;
		m_activeNotes.ForEach(n => n.m_note.anchoredPosition += scrollAmount);
		if (m_activeNotes.Count > 0 && m_activeNotes[0].IsNoteHit())
		{
			HitNote(0);
		}
		else if (m_activeNotes.Count > 0 && m_activeNotes[0].m_note.anchoredPosition.x < m_noteMissPosition)
		{
			HitNote(0);
			//m_activeNotes[0].m_text.textInfo.characterInfo[0].
		}
	}
	//Is also called when a note is scrolled off the screen
	void HitNote(int noteIndex)
	{
		m_activeNotes[noteIndex].m_note.gameObject.SetActive(false);
		float distance = Mathf.Abs(m_noteHitPosition - m_activeNotes[noteIndex].m_note.anchoredPosition.x);
		if (distance <= m_perfectDistance) StartCoroutine(ShowHitText("Perfect", 60.0f / (m_BPM * 1.1f)));
		else if (distance <= m_goodDistance) StartCoroutine(ShowHitText("Good", 60.0f / (m_BPM * 1.1f)));
		else StartCoroutine(ShowHitText("Miss", 60.0f / (m_BPM * 1.1f)));
		m_activeNotes.RemoveAt(noteIndex);

		m_highlightedLetterIndex++;
		if (m_highlightedLetterIndex < testString.Length)
		{
			if (testString[m_highlightedLetterIndex] == ' ')
			{
				m_highlightedLetterIndex++;
				m_fullTextDisplay.anchoredPosition += Vector2.left * (m_textSpaceDistance - m_textCharDistance);
			}
			m_letterHighlightText.text = testString[m_highlightedLetterIndex].ToString();
			m_fullTextDisplay.anchoredPosition += Vector2.left * m_textCharDistance;
		}
		else
		{
			m_letterHighlightText.text = "";
		}
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
