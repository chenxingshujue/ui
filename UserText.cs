using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using sw.ui.view;
using System.IO;
using System.Text;

namespace User{
	public class UserText : Text {
		[SerializeField]
		int m_id;
		public int id{
			get{
				return m_id;
			}
			set{
				m_id = value;
			}
		}
        [Tooltip("打字机效果打字速度,每秒钟打字数目,0无此效果")]
		[SerializeField]
		int m_typingSpeed;
		public int typingSpeed{
			get{
				return m_typingSpeed;
			}
			set{
				m_typingSpeed = value;
			}
		}
        int textIndex = -1;
		float tempTime = 0;
        string oriText;
		void Update()
		{
			if (m_typingSpeed > 0 && textIndex >= 0)
			{
                float time = 1.0f / typingSpeed;
				if(tempTime > time){
					tempTime -= time;
                    textIndex++;
                    base.text = oriText.Substring(0, textIndex);
                    if(textIndex >= oriText.Length)
                    {
                        textIndex = -1;
                    }
				}
				tempTime += Time.deltaTime;
			}
		}
        public override string text
        {
            get
            {
                return base.text;
            }

            set
            {
                if(typingSpeed > 0 && (!string.IsNullOrEmpty(value)))
                {
                    oriText = value;
                    textIndex = 0;
                    base.text = string.Empty;
                }else
                {
                    base.text = value;
                }
            }
        }
        //		#if UNITY_EDITOR 
        //		public override string text {
        //			get	{
        //				if (Application.isPlaying && !sw.AppConfig.useInsideUI) {
        //					return base.text;
        //				} else {
        //					return LanguagePack.GetTextById (m_id);
        //				}
        //			}
        //			set
        //			{
        //				if (Application.isPlaying) {
        //					base.text = value;
        //				}
        //			}
        //		}
        //		override public float preferredWidth
        //		{
        //			get
        //			{
        //				var settings = GetGenerationSettings(Vector2.zero);
        //				return cachedTextGeneratorForLayout.GetPreferredWidth(text, settings) / pixelsPerUnit;
        //			}
        //		}
        //
        //	
        //
        //		override public float preferredHeight
        //		{
        //			get
        //			{
        //				var settings = GetGenerationSettings(new Vector2(GetPixelAdjustedRect().size.x, 0.0f));
        //				return cachedTextGeneratorForLayout.GetPreferredHeight(text, settings) / pixelsPerUnit;
        //			}
        //		}
        //
        //		new private TextGenerator m_TextCache;
        //
        //		public TextGenerator cachedTextGenerator
        //		{
        //			get { return m_TextCache ?? (m_TextCache = (text.Length != 0 ? new TextGenerator(text.Length) : new TextGenerator())); }
        //		}
        //		#endif

    }


}
