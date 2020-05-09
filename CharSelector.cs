using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

namespace User{
	[RequireComponent(typeof(Text))]
	public class CharSelector : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler 
	{

		public SelectEvent selectEvent = new SelectEvent();
		public bool hasLink = false;

//		Camera m_Camera;
		RectTransform rect;
		Text text;

		// Use this for initialization
		void Start () {

			// Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
//			if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
//				m_Camera = null;
//			else
//				m_Camera = m_Canvas.worldCamera;

			rect = gameObject.GetComponent<RectTransform> ();

			text = gameObject.GetComponent<Text> ();

			if (!text.raycastTarget) {
				text.raycastTarget = true;
			}
		}


        public void OnPointerEnter (PointerEventData eventData){
//			Vector3 position = eventData.position;
//			int char_index = _get_char_index (position);
		}
		public void OnPointerExit (PointerEventData eventData){
//			Vector3 position = eventData.position;
//			int char_index = _get_char_index (position);
		}

		public void OnPointerClick (PointerEventData eventData)
		{
			Vector2 position = eventData.pressPosition;
			int char_index = -1;
			if (hasLink) {
				char_index = _get_char_index (position,eventData.pressEventCamera);
			}
			if (selectEvent != null){
				selectEvent.Invoke (char_index);
			}
		}

		private int _get_char_index(Vector2 position,Camera m_Camera){
//			position = new Vector3 (742f, 469f, 0);
			//			if (Input.touchCount > 0) {
			//				position = Input.GetTouch (0).position;
			//			}
//						Debug.Log ("mousePosition\t" +position);

//						position = position / canvas.referencePixelsPerUnit;
//			Debug.Log ("referencePixelsPerUnit\t"+position + "referencePixelsPerUnit\t" + m_Canvas.referencePixelsPerUnit);
			//			Debug.Log (camera.ScreenToViewportPoint (position));
			Vector2 local = Vector2.zero;

			RectTransformUtility.ScreenPointToLocalPointInRectangle (rect, position, m_Camera, out local);

            //			if (m_Camera != null) {
            //				position = m_Camera.ScreenToWorldPoint (position);
            //			}
            //			position =   m_Canvas.transform.TransformPoint (position);
            //						Debug.Log ("world\t"+position);

            //			Vector3[] fourCornersArray = new Vector3[4];
            //			rect.GetWorldCorners (fourCornersArray);
            //			Debug.Log (fourCornersArray [0]);
            //			Debug.Log (fourCornersArray [1]);
            //			Debug.Log (fourCornersArray [2]);
            //			Debug.Log (fourCornersArray [3]);


            //			Vector3 local = rect.InverseTransformPoint (position);
            //			Vector3 local = rect.InverseTransformPoint (new Vector3(0.4f,1.3f,-10f));
            //					Debug.Log("local" +local);
            Canvas m_Canvas = gameObject.GetComponentInParent<Canvas>();
            local = local * m_Canvas.scaleFactor;
//			Debug.Log ("scaleFactor\t"+position +"\t"+ m_Canvas.scaleFactor);

			IList<UILineInfo> lines = text.cachedTextGenerator.lines;
			int line = -1;
			for (int j = lines.Count-1; j >=0 ; j--) {
//				Debug.Log (lines [j].height + "\t" +lines[j].topY  + "\t" +lines[j].startCharIdx);
				if (lines [j].topY >= local.y && lines[j].topY - lines[j].height <= local.y) {
					line = j;
					break;
				}
				//				Debug.Log (lines [j].height + "\t" +lines[j].startCharIdx + "\t" +lines[j].topY);
			}
			//			Debug.Log ("line\t" + line);
			IList<UIVertex> verts = text.cachedTextGenerator.verts;
			int vertCount = verts.Count - 4;
			if (line < 0)
				return -1;

			int startindex = lines [line].startCharIdx;

			int endindex = text.cachedTextGenerator.characterCount;

			if (line < text.cachedTextGenerator.lineCount - 1) {
				endindex = lines [line + 1].startCharIdx;
			}

//			Debug.Log ("characterCount\t" + text.cachedTextGenerator.characterCount);

			IList<UICharInfo> characters = text.cachedTextGenerator.characters;

			int char_index = -1;
			for (int i = startindex; i < endindex; i=i+1) {
				if (characters [i].cursorPos.x <= local.x && characters [i].cursorPos.x + characters [i].charWidth >= local.x) {
					char_index = i;
					break;
				}
				//				Debug.Log ("UICharInfo\t" + characters [i].charWidth +	"\t" + characters[i].cursorPos);
			}
			//Debug.Log ("char_index\t" + char_index);
			return char_index;

		}

		[Serializable]
		public class SelectEvent : UnityEvent<int>
		{
		}
	}
}