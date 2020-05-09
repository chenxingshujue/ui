using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[RequireComponent(typeof(UnityEngine.UI.Text))]
public class TextLayoutElement : LayoutElement {

	// Use this for initialization
    Text text;
    [SerializeField]
    private float m_MaxWidth = -1;
    [SerializeField]
    private float m_MaxHeight = -1;

    public bool autoAlignment = true;

    override protected void Awake () {
        base.Awake();
        text = gameObject.GetComponent<Text>();
	}
    override protected void OnRectTransformDimensionsChange(){
        base.OnRectTransformDimensionsChange();
        UpdateAlignment();
    }

    public void UpdateAlignment()
    {
		if (text!=null && autoAlignment) {
			if (minWidth > 0 && text.preferredWidth < minWidth) {
				text.alignment = TextAnchor.UpperCenter;
			} else {
				text.alignment = TextAnchor.UpperLeft;
			}
		}
    }

    public override float preferredWidth
    {
        get
        {
            if (m_MaxWidth > 0 && text.preferredWidth > m_MaxWidth)
            {
                return m_MaxWidth;
            }
            return text.preferredWidth;
        }
    }

    public override float preferredHeight
    {
        get
        {
            if (m_MaxHeight > 0 && text.preferredHeight > m_MaxHeight)
            {
                return m_MaxHeight;
            }
            return text.preferredHeight;
        }
    }

    public override float minHeight
    {
        get
        {
            return text.minHeight;
        }
    }

    public override float flexibleWidth
    {
        get
        {
            return text.flexibleWidth;
        }
    }

    public override float flexibleHeight
    {
        get
        {
            return text.flexibleHeight;
        }
    }

}
