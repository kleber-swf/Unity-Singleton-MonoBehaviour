/*
 * Unity Singleton MonoBehaviour
 * This file is licensed under the terms of the MIT license. 
 * 
 * Copyright (c) 2014 Kleber Lopes da Silva (http://kleber-swf.com)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/
using System;
using UnityEngine;

namespace com.kleberswf.lib.core {
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
		private static T _instance;

		public static T Instance {
			get {
				if (!Instantiated) CreateInstance();
				return _instance;
			}
		}

		private static void CreateInstance() {
			if (Destroyed) return;
			var type = typeof (T);
			var objects = FindObjectsOfType<T>();
			if (objects.Length > 0) {
				if (objects.Length > 1) {
					Debug.LogWarning("There is more than one instance of Singleton of type \"" + type +
					                 "\". Keeping the first one. Destroying the others.");
					for (var i = 1; i < objects.Length; i++) Destroy(objects[i].gameObject);
				}
				_instance = objects[0];
				_instance.gameObject.SetActive(true);
				Instantiated = true;
				Destroyed = false;
				return;
			}

			string prefabName;
			GameObject gameObject;
			var attribute = Attribute.GetCustomAttribute(type, typeof (PrefabAttribute)) as PrefabAttribute;

			if (attribute == null || string.IsNullOrEmpty(attribute.Name)) {
				prefabName = type.ToString();
				gameObject = new GameObject();
			} else {
				prefabName = attribute.Name;
				gameObject = Instantiate(Resources.Load<GameObject>(prefabName));
				if (gameObject == null)
					throw new Exception("Could not find Prefab \"" + prefabName + "\" on Resources for Singleton of type \"" + type +
					                    "\".");
			}

			gameObject.name = prefabName;
			if (_instance == null)
				_instance = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
			Instantiated = true;
			Destroyed = false;
		}

		public bool Persistent;
		public static bool Instantiated { get; private set; }
		public static bool Destroyed { get; private set; }

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

		private void OnDestroy() {
			Destroyed = true;
			Instantiated = false;
		}

		public void Touch() { }
	}
}