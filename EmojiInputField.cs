using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(InputField))]
public class EmojiInputField : MonoBehaviour {
    InputField inputField;
    //string rep1 = @"\uD83C[\uDC00-\uDFFF]";
    //string rep2 = @"\uD83D[\uDC00-\uDFFF]";
    string rep = @"(\uD83C[\uDDE8-\uDDFF]\uD83C[\uDDE7-\uDDFF])|[\uD800-\uDBFF][\uDC00-\uDFFF]|[\u2600-\u27ff][\uFE0F]|[\u2600-\u27ff]";
    string code = "□001";
    void Start()
    {
        inputField = gameObject.GetComponent<InputField>();

       // inputField.onValueChanged.AddListener(OnValueChanged);

        inputField.onValidateInput += OnValidateInput;
    }
    private char OnValidateInput(string input, int charIndex, char addedChar)
    {
            if (char.GetUnicodeCategory(addedChar) == UnicodeCategory.Surrogate)
            {
                return '\0';
            }
            return addedChar;
    }
    


    void OnValueChanged(string text)
    {
        text = inputField.text;
        string s = Regex.Replace(text, rep, code);
        //s = Regex.Replace(s, rep2, code);
        inputField.text = s;
        if(text != s)
        {
            inputField.caretPosition += 2;
        }
    }
    private string toEmojiStr(Match m)
    {
        //string s = string.Empty;
        //string ms = m.Value;
        //if (ms.Length > 1)
        //{
        //    long code = (long)ms[1];
        //    s = code.ToString();
        //}
        return code;
    }

}
