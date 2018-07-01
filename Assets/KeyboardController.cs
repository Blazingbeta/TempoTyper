using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardController : MonoBehaviour {
	[SerializeField] AnimationCurve m_noteCurve = null;
	[SerializeField] float m_noteTime;

	class KeyData
	{
		public KeyCode m_keyCode;
		public RawImage m_image;
		public RectTransform m_note;
		public Text m_hitText;
		public bool m_hasNote;

		public IEnumerator StartNote(KeyboardController parent)
		{
			m_hasNote = true;
			float timer = 0.0f;
			m_note.gameObject.SetActive(true);
			m_note.localScale = Vector3.zero;
			bool noteHit = false;
			while (timer < parent.m_noteTime && !noteHit)
			{
				m_note.localScale = Vector3.one * (parent.m_noteCurve.Evaluate(timer / parent.m_noteTime));
				if (Input.GetKeyDown(m_keyCode))
				{
					//Note hit
					parent.NoteHit(timer / parent.m_noteTime, this);
					noteHit = true;
				}
				timer += Time.deltaTime;
				yield return null;
			}
			m_hasNote = false;
			m_note.gameObject.SetActive(false);
			if (!noteHit)
			{
				//Note missed
				parent.NoteMissed(this);
			}
		}
		public IEnumerator NoteHitText(string text)
		{
			m_hitText.gameObject.SetActive(true);
			m_hitText.text = text;
			yield return new WaitForSeconds(0.4f);
			m_hitText.gameObject.SetActive(false);
		}
	}
	Dictionary<char, KeyData> m_keyboard;
	void Start ()
	{
		m_keyboard = new Dictionary<char, KeyData>();

		for (int j = 0; j < transform.childCount; j++)
		{
			Transform child = transform.GetChild(j);
			char keyChar = child.name.ToUpper()[0];
			KeyCode keycode = keyChar == ';' ? KeyCode.Semicolon : keyChar == ',' ? KeyCode.Comma : keyChar == '.' ? KeyCode.Period
				: keyChar == '?' ? KeyCode.Slash : (KeyCode)System.Enum.Parse(typeof(KeyCode), keyChar.ToString());
			KeyData data = new KeyData
			{
				m_keyCode = keycode,
				m_image = child.GetComponent<RawImage>(),
				m_note = child.GetChild(0).GetComponent<RectTransform>(),
				m_hitText = child.GetChild(2).GetComponent<Text>()
			};
			m_keyboard.Add(child.name[0], data);
		}
		StartCoroutine(SpawnRandomNotes());
	}
	public IEnumerator SpawnRandomNotes()
	{
		
		char[] possibleChars = new char[m_keyboard.Count];
		m_keyboard.Keys.CopyTo(possibleChars, 0);
		while (true)
		{
			StartCoroutine(m_keyboard[possibleChars[Random.Range(0, possibleChars.Length)]].StartNote(this));
			yield return new WaitForSeconds(0.8f);
		}
	}
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			StartCoroutine(m_keyboard['d'].StartNote(this));
		}
	}
	
	void NoteHit(float percent, KeyData key)
	{
		GetComponent<AudioSource>().Play();
		float value = m_noteCurve.Evaluate(percent);
		StartCoroutine(key.NoteHitText((value >= .95f) ? "Perfect" : (value >= .5f) ? "Good" : "Miss"));
	}
	void NoteMissed(KeyData key)
	{
		StartCoroutine(key.NoteHitText("Miss"));
	}
}
