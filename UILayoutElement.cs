using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[RequireComponent(typeof(ILayoutElement))]
public class UILayoutElement : LayoutElement {

    [SerializeField]
    private float m_MaxWidth = -1;
    [SerializeField]
    private float m_MaxHeight = -1;

    ILayoutElement ele;

    protected override void Awake()
    {
        base.Awake();
        ele = gameObject.GetComponent<ILayoutElement>();
    }

    public override float preferredWidth
    {
        get
        {
            if (m_MaxWidth > 0 && ele.preferredWidth > m_MaxWidth)
            {
                return m_MaxWidth;
            }
            return -1;
        }
    }

    public override float preferredHeight
    {
        get
        {
            if (m_MaxHeight > 0 && ele.preferredHeight > m_MaxHeight)
            {
                return m_MaxHeight;
            }
            return -1;
        }
    }


}
