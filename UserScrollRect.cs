using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace User{
	public class UserScrollRect : ScrollRect {
		
		// Use this for initialization
		public bool canDrag = true;
		public void StopDrag() {
			PointerEventData eventData = new PointerEventData(EventSystem.current);
			eventData.button = PointerEventData.InputButton.Left;
			base.OnEndDrag(eventData);
		}
		
		public override void OnDrag(PointerEventData eventData) {
			if(canDrag == false) return;
			base.OnDrag(eventData);
		}
		
		public override void OnBeginDrag(PointerEventData eventData) {
			canDrag = true;
			base.OnBeginDrag(eventData);
		}
		
		public override void OnEndDrag(PointerEventData eventData) {
			base.OnEndDrag(eventData);
			canDrag = true;
		}
	}
	
}