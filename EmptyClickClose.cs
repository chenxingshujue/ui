using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class EmptyClickClose : UIBehaviour,IPointerDownHandler,IPointerExitHandler,IPointerUpHandler,IDropHandler{



	#region IDropHandler implementation

	public void OnDrop (PointerEventData eventData)
	{
		//		Debug.Log ("EmptyClickClose\tOnDrop");
		IsPointerDown = false;
	}

	#endregion


	public bool DisableOnClose = true;
	[System.NonSerialized]
	public bool IsPointerDown = false;
	public UnityEvent OnClose = new UnityEvent ();
	public Action OnClosePanel = null;
	public virtual void OnPointerDown(PointerEventData eventData)
	{
		//		Debug.Log ("EmptyClickClose\tOnPointerDown");
		IsPointerDown = true;
		EventSystem.current.SetSelectedGameObject (eventData.pointerPress);
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		//		Debug.Log ("EmptyClickClose\tOnPointerUp");
		if (!eventData.dragging) {
			IsPointerDown = false;
		}
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		//		Debug.Log ("EmptyClickClose\tOnPointerExit");
		if (!eventData.dragging) {
			IsPointerDown = false;
		}
	}



	void Update(){
		if (Input.touchCount > 0 || Input.GetMouseButton(0)) {
			if (IsPointerDown == false) {
				GameObject obj = EventSystem.current.currentSelectedGameObject;
				Transform parent = null;
				if (obj != null) {
					parent = obj.transform.parent;
					while (parent != this.transform && parent != null) {
						parent = parent.parent;
					}
				}
				if (parent != this.transform) {
					if (DisableOnClose) {
						if(OnClosePanel != null) {
							OnClosePanel();
						} else{
							gameObject.SetActive(false);
						}
					}
					if (OnClose != null) {
						OnClose.Invoke ();
					}
				}

			}
		}
	}
}
