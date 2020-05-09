using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class EmojiField : UIBehaviour,ILayoutGroup
{
    public const string reg1 = @"<size=([0-9]|\.)+>□\d+□</size>";
    public const string reg2 = @"□\d+□";
    public const string reg3 = @"color=#00000000><size=\d+>一</size></color";
    public const string rep1 = "<color=#00000000><size={0}>一</size></color>";
    public const string rep2 = "一";
    private List<Match> matches = new List<Match>();

    public System.Func<Transform, double, Transform> OnWillCreateItem;
    public System.Action<GameObject> OnWillDesrtoyItem;

    List<ImageItem> images = new List<ImageItem>();

    Text m_Text;

    private double destroyAllListenId = -1;

    override protected void Awake()
    {
        base.Awake();
        m_Text = gameObject.GetComponent<Text>();
        InitEvent();
    }

    private void InitEvent()
    {
        RemoveEvent();
        destroyAllListenId = EventDispatcher.AddEventListener(sw.game.evt.UIEventType.DESTROY_ALL_NODE, OnDestroyAllNode);
    }

    private void RemoveEvent()
    {
        if(destroyAllListenId>=0)
        {
            EventDispatcher.RemoveEventListener(sw.game.evt.UIEventType.DESTROY_ALL_NODE, destroyAllListenId);
            destroyAllListenId = -1;
        }
    }

    private void OnDestroyAllNode(object[] args)
    {
        OnWillDesrtoyItem = null;
    }

    //protected override void OnRectTransformDimensionsChange()
    //{
    //    base.OnRectTransformDimensionsChange();
    //    this.UpdateImagePosition();
    //}




    public string TransformText(string value)
    {
        matches.Clear();
        this.ClearImages();
        value = Regex.Replace(value, reg1, toEmojiStr);
        return value;
    }

    private string toEmojiStr(Match m)
    {
        matches.Add(m);
        string scale = m.Value.Substring(6, m.Value.IndexOf('>') - 6);
        float scaleFactor = float.Parse(scale);
        return string.Format(rep1, (int)(m_Text.fontSize * scaleFactor));
    }
   

    void LateUpdate()
    {
        if (matches.Count > 0 && m_Text.cachedTextGenerator.characterCount > 0)
        {
            var new_matches = Regex.Matches(m_Text.text, reg3);

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                int index = new_matches[i].Index + new_matches[i].Value.IndexOf(rep2);

                Match m = Regex.Match(match.Value, reg2);
                if (m != null)
                {
                    double code = double.Parse(m.Value.Substring(1,m.Value.Length -2));
                  //  Debug.Log("TransformText    " + index + " " + code + " " + m_Text.text[index]);
                    this.CreateImage(index, code);

                }
            }
            matches.Clear();
            StartCoroutine(UpdateImagePositionLater());
        }
    }

    IEnumerator UpdateImagePositionLater()
    {
        yield return new WaitForEndOfFrame();
        this.UpdateImagePosition();
    }
    Vector2 temp = Vector2.zero;
    void UpdateImagePosition()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        if(images.Count == 0)
        {
            return;
        }
        Vector2 extents = m_Text.rectTransform.rect.size;

        var settings = m_Text.GetGenerationSettings(extents);
        m_Text.cachedTextGeneratorForLayout.PopulateWithErrors(m_Text.text, settings, gameObject);
        //Debug.Log("EmojiField UpdateImagePosition " + m_Text.alignment.ToString() + " " + m_Text.text);
        for (int i = 0; i < images.Count; i++)
        {
            ImageItem item = images[i];
            if (m_Text.cachedTextGeneratorForLayout.characterCount > item.charIndex)
            {
                UICharInfo info = m_Text.cachedTextGeneratorForLayout.characters[item.charIndex];
                float width = info.charWidth / m_Text.pixelsPerUnit;
                temp.x = width;
                temp.y = width;
                images[i].transform.sizeDelta = temp;
                //images[i].transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, width);
                images[i].transform.localPosition = info.cursorPos / m_Text.pixelsPerUnit;
            }
        }
    }


    void ClearImages()
    {
        for (int i = 0; i < images.Count; i++)
        {
            if (OnWillDesrtoyItem != null)
            {
                OnWillDesrtoyItem(images[i].gameObject);
            }
            else
            {
                GameObject.Destroy(images[i].gameObject);
            }
        }
        images.Clear();
    }


    private void CreateImage(int charIndex, double code)
    {
        if (OnWillCreateItem != null)
        {
            RectTransform tf = OnWillCreateItem(this.transform, code) as RectTransform;

            ImageItem item = new ImageItem();
            item.transform = tf as RectTransform;
            item.gameObject = tf.gameObject;
            item.charIndex = charIndex;
            images.Add(item);
            tf.SetParent(this.transform, false);
        }
    }

    override protected void OnDestroy()
    {
        this.ClearImages();
        this.RemoveEvent();
        base.OnDestroy();
    }

    public void SetLayoutHorizontal()
    {

    }

    public void SetLayoutVertical()
    {
        this.UpdateImagePosition();
    }
}

class ImageItem
{
    public RectTransform transform;
    public GameObject gameObject;
    public int charIndex;
    public int size;

}