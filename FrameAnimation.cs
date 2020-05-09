using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using sw.res;
namespace User{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RawImage))]
	public class FrameAnimation : MonoBehaviour {
		
		// Use this for initialization
		public int FrameRate = 30;
		[Tooltip("横排和竖排各有多少帧")]
		public Vector2 SizeUV;
		[Tooltip("总共有多少帧")]
		public int TotalFrames;
		public bool Loop = true;
		public bool DestroyOnStop = false;
		public Action<GameObject> OnWillDestroy;
		
		Rect uvRect;
		RawImage image ;
		RectTransform rt;
		float temptime =0f;
		int frameIndex;
		DrivenRectTransformTracker m_Tracker;
		bool Playing = false;
		
		double loadingId = 0;
		string assetbundlename ;
		public void LoadAndPlay(string name){
			assetbundlename = "texture/xuliezhen/" + name + ".unity3d";
			if (loadingId != 0) {
				AssetLoader2.Instance.RemoveUICallBack (assetbundlename, loadingId);
			}
			loadingId = AssetLoader2.Instance.LoadUIAsync(assetbundlename,name,typeof(Texture2D),(UnityEngine.Object texture,object[] param) => {
				loadingId = 0;
				image.texture = texture as Texture;
				OnChange ();
				Play();
			});
		}
		
		[ContextMenu("Play")]
		public void Play(){
			temptime = 0;
			frameIndex = 0;
			uvRect.x = 0;
			uvRect.y = 0;
			Playing = true;
			if (image != null) {
				image.enabled = true;
			}
		}
		
		[ContextMenu("Stop")]
		public void Stop(){
			Playing = false;
			if (image != null) {
				image.enabled = false;
			}
			if (DestroyOnStop) {
				if (OnWillDestroy != null) {
					OnWillDestroy (this.gameObject);
				} else {
					GameObject.Destroy (this.gameObject);
				}
			}
		}

		void Awake(){
			rt = this.GetComponent<RectTransform> ();
			image = this.GetComponent<RawImage> ();
			uvRect = image.uvRect;
		}
        private void OnEnable()
        {
            if (image.texture!=null)
            {
                Play();
            }
            
        }
        // Update is called once per frame
        void Update () {
			if (Playing && image != null) {
				if (temptime * FrameRate > 1) {
					temptime = 0f;
					uvRect.x = uvRect.width * (frameIndex % SizeUV.x);
					uvRect.y = uvRect.height * Mathf.Floor(frameIndex / SizeUV.y);
					image.uvRect = uvRect;
					frameIndex++;
					if (frameIndex >= TotalFrames) {
						if (Loop) {
							frameIndex = 0;
						} else {
							Stop ();
						}
					}
					
				}
				temptime += Time.unscaledDeltaTime;
			}
		}
		
		
		
		void OnDestroy(){
			if (loadingId > 0) {
				AssetLoader2.Instance.RemoveUICallBack (assetbundlename, loadingId);
			}
			m_Tracker.Clear ();
		}
		
		
		void OnChange(){
			if (rt == null) {
				rt = this.GetComponent<RectTransform> ();
			}
			if (image == null) {
				image = this.GetComponent<RawImage> ();
			}
			if (SizeUV.x == 0) {
				SizeUV.x = 1;
			}
			if (SizeUV.y == 0) {
				SizeUV.y = 1;
			}
			uvRect.width = 1 / SizeUV.x;
			uvRect.height = 1 / SizeUV.y;
			image.uvRect = uvRect;
			if (image.texture != null) {
				m_Tracker.Add (this, rt, DrivenTransformProperties.SizeDelta);
				rt.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, image.texture.width/SizeUV.x);
				rt.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, image.texture.height/SizeUV.y);
			}
		}
		#if UNITY_EDITOR
		void OnValidate(){
			OnChange ();
		}
		#endif
	}
	
}