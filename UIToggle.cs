using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



namespace User
{
	[ExecuteInEditMode]
	public class UIToggle : Selectable,IPointerClickHandler {
		public UnityEngine.UI.Toggle.ToggleEvent onValueChanged = new UnityEngine.UI.Toggle.ToggleEvent();
		public Graphic active;
		public Graphic deactive;
		public bool isOn {
			get {
				return _isOn;
			}	
			set{
				_isOn = value;
				this.UpdateToggle ();
			}
		}
		[SerializeField]
		private bool _isOn;
		void Start(){
			UpdateToggle();
		}

		void UpdateToggle(){
			if (this.isOn) {
				this.targetGraphic = active;
			}else{
				this.targetGraphic = deactive;
			}
			this.active.gameObject.SetActive (this.isOn);
			this.deactive.gameObject.SetActive (!this.isOn);
			if(onValueChanged!= null){
				onValueChanged.Invoke(this.isOn);
			}
		}

		public void OnPointerClick (PointerEventData eventData){
			this.isOn = !this.isOn;
			EventDispatcher.DispatchEvent(UIEventConst.CLICK_UGUI_JUST);
		}

	}
}
