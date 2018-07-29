using System.Collections.Generic;
using UnityEngine;
public static class UtilityFunc
{
	private static Dictionary<char, KeyCode> m_keycodes;
	/// <summary>
	/// Gets the InputKeycode from a char value. Always uses lowercase values so ? = / and ! = 1
	/// </summary>
	/// <param name="c"></param>
	/// <returns>Keycode value of char c</returns>
	public static KeyCode GetKeycodeFromChar(char keyChar)
	{
		if (m_keycodes == null) SetupKeycodes();
		return m_keycodes[keyChar.ToString().ToUpper()[0]];
	}
	private static void SetupKeycodes()
	{
		m_keycodes = new Dictionary<char, KeyCode>();
		for(char c = 'A'; c <= 'Z'; c++)
		{
			m_keycodes.Add(c, (KeyCode)System.Enum.Parse(typeof(KeyCode), c.ToString()));
		}
		for(char c = '0'; c <= '9'; c++)
		{
			m_keycodes.Add(c, (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + c.ToString()));
		}
		//Special Characters
		m_keycodes.Add('?', KeyCode.Slash);
		m_keycodes.Add(',', KeyCode.Comma);
		m_keycodes.Add('.', KeyCode.Period);
		m_keycodes.Add('!', KeyCode.Alpha1);
		m_keycodes.Add('\'', KeyCode.Quote);
		m_keycodes.Add(' ', KeyCode.Space);
		m_keycodes.Add('-', KeyCode.Minus);
		m_keycodes.Add('[', KeyCode.LeftBracket);
		m_keycodes.Add(']', KeyCode.RightBracket);
	}
}
