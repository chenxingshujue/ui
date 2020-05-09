using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace User
{
    [RequireComponent(typeof(ScrollRect))]
    public class UIStepper : MonoBehaviour
    {
        public Button left;
        public Button right;
        public ScrollRect scroll;
        public float stepSize;
        public float speed = 10;
        public RectTransform.Axis axis = RectTransform.Axis.Horizontal;
        float targetEase;
        bool can_ease = false;
        float deltaease = -1;
        UIList list;
        // Use this for initialization
        void Start()
        {
            list = gameObject.GetComponent<UIList>();
            left.onClick.AddListener(() => {
                int step = -1;
                if (list != null)
                {
                    int selectedIndex = list.selectedIndex;
                    if(selectedIndex + step >=0)
                    {
                        list.Select(selectedIndex + step);
                        step = list.selectedIndex - selectedIndex;
                    }
                }
                Step(step);
            });
            right.onClick.AddListener(() => {
                int step = 1;
                if (list != null)
                {
                    int selectedIndex = list.selectedIndex;
                    list.Select(selectedIndex + step);
                    step = list.selectedIndex - selectedIndex;
                }
                Step(step);
            });
        }

        // Update is called once per frame
        public void Step(int step)
        {
            if (deltaease < 0)
            {
                deltaease = GetDeltaEase();
            }
            float currentEase = GetEase();
            targetEase = currentEase + deltaease * step;
            if (targetEase < 0)
            {
                targetEase = 0;
            }
            else if (targetEase > 1)
            {
                targetEase = 1;
            }
            if (Mathf.Abs(targetEase - currentEase) < Mathf.Epsilon)
            {
                can_ease = false;
            }
            else
            {
                can_ease = true;
            }

            if (speed <= 0)
            {
                SetEase(targetEase);
            }
        }

        private float GetDeltaEase()
        {
            RectTransform rt = scroll.viewport;
            if(rt == null)
            {
                rt = scroll.GetComponent<RectTransform>();
            }
            if (axis == RectTransform.Axis.Horizontal)
            {
                return stepSize / Mathf.Max(0, scroll.content.rect.width - rt.rect.width);
            }
            else
            {
                return stepSize / Mathf.Max(0, scroll.content.rect.height - rt.rect.height);
            }
        }

        private void SetEase(float ease)
        {
            if (axis == RectTransform.Axis.Horizontal)
            {
                scroll.horizontalNormalizedPosition = ease;
            }
            else
            {
                scroll.verticalNormalizedPosition = ease;
            }
        }

        private float GetEase()
        {
            if (axis == RectTransform.Axis.Horizontal)
            {
                return scroll.horizontalNormalizedPosition;
            }
            else
            {
                return scroll.verticalNormalizedPosition;
            }
        }

        private void Update()
        {
            if (!can_ease)
            {
                return;
            }
            if (speed <= 0)
            {
                return;
            }
            float currenEase = 0;
            if (axis == RectTransform.Axis.Horizontal)
            {
                currenEase = scroll.horizontalNormalizedPosition;
            }
            else
            {
                currenEase = scroll.verticalNormalizedPosition;
            }
            if (Mathf.Abs(targetEase - currenEase) < Mathf.Epsilon)
            {
                can_ease = false;
                SetEase(targetEase);
            }
            else
            {
                float ease = Mathf.Lerp(currenEase, targetEase, speed * Time.unscaledDeltaTime);
                if (Mathf.Abs(ease - currenEase) < Mathf.Epsilon)
                {
                    can_ease = false;
                }
                else
                {
                    SetEase(ease);
                }
            }

        }
    }
}

