using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePool : MonoBehaviour {
	public static NotePool i;
	Pool m_pool;
	public GameObject GetObjectFromPool()
	{
		if (m_pool == null)
		{
			m_pool = new Pool();
			m_pool.Initialize(16, this);
		}
		//If the pool is all in use still. 
		return m_pool.GetObject();
	}
	class Pool
	{
		public List<GameObject> m_objects;
		public GameObject m_prefab;
		public GameObject m_parentObject;
		public void Initialize(int count, NotePool parent)
		{
			m_parentObject = parent.gameObject;
			m_prefab = Resources.Load<GameObject>("Prefabs/Note");
			m_objects = new List<GameObject>();
			for (int j = 0; j < count; j++)
			{
				GameObject obj = Object.Instantiate(m_prefab, m_parentObject.transform);
				obj.SetActive(false);
				m_objects.Add(obj);
			}
		}
		public GameObject GetObject()
		{
			foreach (GameObject g in m_objects)
			{
				if (!g.activeInHierarchy) return g;
			}
			return null;
		}
	}
	void Start ()
	{
		i = this;
	}
}
