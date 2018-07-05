using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class that can automatically create Object pools by Resource name. 
/// Calling GetObjectFromPool will automatically attempt to load a pool from resources
/// </summary>
public static class ObjectPool
{
	private static Dictionary<string, Pool> m_pools;
	public static GameObject GetObjectFromPool(string poolName)
	{
		if (m_pools == null) m_pools = new Dictionary<string, Pool>();
		if (!m_pools.ContainsKey(poolName))
		{
			Pool pool = new Pool();
			pool.Initialize(poolName, 32);
			m_pools.Add(poolName, pool);
		}
		//If the pool is all in use still. 
		return m_pools[poolName].GetObject();
	}

	class Pool
	{
		public List<GameObject> m_objects;
		public GameObject m_prefab;
		public GameObject m_parentObject;
		public void Initialize(string objectName, int count)
		{
			m_parentObject = new GameObject(objectName + "Pool");
			m_prefab = Resources.Load<GameObject>("Prefabs/" + objectName);
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
}
