using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefLayoutElement : LayoutElement {

	[SerializeField]
	private LayoutObject[] m_Elements = new LayoutObject[0];

	public LayoutObject[] elements {
		get {
			return m_Elements;
		}
		set {
			m_Elements = value;
		}
	}

	[SerializeField]
	private float m_Space = 0;



	public System.Func<int,float> calculatePreferWidth;
	public System.Func<int,float> calculatePreferHeight;


	public float space {
		get {
			return m_Space;
		}
		set {
			m_Space = value;
		}
	}


	public override float preferredWidth {
		get {
			if (calculatePreferWidth != null) {
				return calculatePreferWidth (0);
			}
			return this.GetRefWidth ();
		}
		set {
			base.preferredWidth = preferredWidth;
		}
	}

	public override float preferredHeight {
		get {
			if (calculatePreferHeight != null) {
				return calculatePreferHeight(0);
			}
			return this.GetRefHeight();
		}
		set {
			base.preferredHeight = preferredHeight;
		}
	}

	private float GetRefWidth(){
		float width = 0;

		for (int i = 0; i < elements.Length; i++) {
			if (elements [i] != null) {
				width = width + elements [i].preferredWidth + space;
			}
		}
		if (width > 0) {
			width -= space;
		}
		return width;
	}

	private float GetRefHeight(){
		float height = 0;
		for (int i = 0; i < elements.Length; i++) {
			if (elements [i] != null) {
				height = height + elements[i].preferredHeight + space;
			}
		}
		if (height > 0) {
			height -= space;
		}
		return height;
	}

	public enum LayoutType{
		None,
		LayoutElement,
		Text,
		Image,
		LayoutGroup,
		RectTransform,
	}
	[System.Serializable]
	public class LayoutObject{

		[SerializeField]
		private LayoutType m_LayoutType = LayoutType.None;

		public LayoutType layoutType {
			get {
				return m_LayoutType;
			}
			set {
				m_LayoutType = value;
			}
		}
		[SerializeField]
		private LayoutElement m_LayoutElement;

		public LayoutElement layoutElement {
			get {
				return m_LayoutElement;
			}
			set {
				m_LayoutElement = value;
			}
		}
		[SerializeField]
		private Text m_Text;

		public Text text {
			get {
				return m_Text;
			}
			set {
				m_Text = value;
			}
		}
		[SerializeField]
		private Image m_Image;

		public Image image {
			get {
				return m_Image;
			}
			set {
				m_Image = value;
			}
		}
		[SerializeField]
		private LayoutGroup m_LayoutGroup;

		public LayoutGroup layoutGroup {
			get {
				return m_LayoutGroup;
			}
			set {
				m_LayoutGroup = value;
			}
		}

		[SerializeField]
		private RectTransform m_RectTransform;

		public RectTransform rectTransform {
			get {
				return m_RectTransform;
			}
			set {
				m_RectTransform = value;
			}
		}


		public  float preferredWidth {
			get {
				if (this.layoutType == LayoutType.Image && this.image != null) {
					return this.image.preferredWidth;
				}else if (this.layoutType == LayoutType.LayoutElement && this.layoutElement != null) {
					return this.layoutElement.preferredWidth;
				}else if (this.layoutType == LayoutType.LayoutGroup && this.layoutGroup != null) {
					return this.layoutGroup.preferredWidth;
				}else if (this.layoutType == LayoutType.Text && this.text != null) {
					return this.text.preferredWidth;
				}else if (this.layoutType == LayoutType.RectTransform && this.rectTransform != null) {
					return LayoutUtility.GetPreferredWidth (this.rectTransform);
				}
				return 0;
			}

		}

		public  float preferredHeight {
			get {
				if (this.layoutType == LayoutType.Image && this.image != null) {
					return this.image.preferredHeight;
				}else if (this.layoutType == LayoutType.LayoutElement && this.layoutElement != null) {
					return this.layoutElement.preferredHeight;
				}else if (this.layoutType == LayoutType.LayoutGroup && this.layoutGroup != null) {
					return this.layoutGroup.preferredHeight;
				}else if (this.layoutType == LayoutType.Text && this.text != null) {
					return this.text.preferredHeight;
				}else if (this.layoutType == LayoutType.RectTransform && this.rectTransform != null) {
					return LayoutUtility.GetPreferredHeight(this.rectTransform);
				}
				return 0;
			}

		} 
	}
}