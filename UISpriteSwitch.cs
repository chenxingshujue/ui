using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace User{
	[RequireComponent(typeof(Image))]
	public class UISpriteSwitch : Selectable {
		Image image ;
		Sprite main_Sprite;
		public Sprite[] sprites ;
		void Awake () {
			image = GetComponent<Image> ();
			main_Sprite = image.sprite;
		}

		public void Select(int index){
			if (index == 0) {
				image.sprite = main_Sprite;
			} else {
				image.sprite = sprites [index - 1];
			}
		}
	}
	
}