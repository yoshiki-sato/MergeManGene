using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MergeManGene
{
	public abstract class Scene : MonoBehaviour {

		protected void Start() {
			//シーン開始時に使用されていないリソースを破棄する
			Resources.UnloadUnusedAssets();
			StartCoroutine(GameLoop());
		}

		protected void Update() {

		}

		public abstract IEnumerator OnEnter();

		public abstract IEnumerator OnUpdate();

		public abstract IEnumerator OnExit();

		private IEnumerator GameLoop() {
			
				//シーンを開始する
				yield return OnEnter();

				//シーンを実行する
				yield return OnUpdate();

				//シーンを終了する
				yield return OnExit();
			
		}
	}
}