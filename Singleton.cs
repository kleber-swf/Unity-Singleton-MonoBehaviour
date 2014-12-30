using System;
using UnityEngine;

namespace com.kleberswf.lib.core {
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
		private static T _instance;

		public static T Instance {
			get {
				if (_instance == null) CreateInstance();
				return _instance;
			}
		}

		private static void CreateInstance() {
			var type = typeof(T);
			var objects = FindObjectsOfType<T>();
			if (objects.Length > 0) {
				_instance = objects[0];
				_instance.gameObject.SetActive(true);
				if (objects.Length > 1) {
					Debug.LogWarning("There is more than one instance of Singleton of type \"" + type + "\". Keeping the first one. Destroying the others.");
					for (var i = 1; i < objects.Length; i++) Destroy(objects[i].gameObject);
				}
				return;
			}

			string prefabName;
			GameObject gameObject;
			var attribute = Attribute.GetCustomAttribute(type, typeof(PrefabAttribute)) as PrefabAttribute;

			if (attribute == null || string.IsNullOrEmpty(attribute.Name)) {
				prefabName = type.ToString();
				gameObject = new GameObject();
			} else {
				prefabName = attribute.Name;
				gameObject = Instantiate(Resources.Load<GameObject>(prefabName)) as GameObject;
				if (gameObject == null)
					throw new Exception("Could not find Prefab \"" + prefabName + "\" on Resources for Singleton of type \"" + type + "\".");
			}

			gameObject.name = prefabName;
			if (_instance == null)
				_instance = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
		}

		public bool Persistent;

		protected virtual void Awake() {
			if (_instance == null) {
				if (Persistent) {
					CreateInstance();
					DontDestroyOnLoad(gameObject);
				}
				return;
			}
			if (Persistent) DontDestroyOnLoad(gameObject);
			if (GetInstanceID() != _instance.GetInstanceID()) Destroy(gameObject);
		}
	}
}