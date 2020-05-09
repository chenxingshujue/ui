using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

namespace User
{

    public class CenterOnChildWithoutDrag : MonoBehaviour,IBeginDragHandler
    {
        public UnityEvent OnCompleted = new UnityEvent();
        public float speed = 10;
        public bool StopScrollOnElastic = true;
        public bool Horizontal = true;
        public bool Vertical = true ; 
        Vector3 targetPositon;
        bool can_ease = false;
        Vector3 viewWorldCenter ;
        bool has_viewWorldCenter = false;
        public RectTransform content;
        RectTransform viewport;
        ScrollRect scroll;
        // Use this for initialization
        void Awake()
        {

            if(scroll == null)
            {
                scroll = gameObject.GetComponent<ScrollRect>();
                if(scroll != null)
                {
                    content = scroll.content;
                    if(scroll.viewport != null)
                    {
                        viewport = scroll.viewport;
                    }
                }
            }
            if(viewport == null)
            {
                viewport = GetComponent<RectTransform>();
            }
        }

        // Update is called once per frame
        public void Center(RectTransform child)
        {
            if (has_viewWorldCenter == false)
            {
                viewWorldCenter = GetWorldCenter(viewport);
                has_viewWorldCenter = true;
            }
            Vector3 center = GetWorldCenter(child);

            targetPositon = content.position + viewWorldCenter - center;
            targetPositon = content.parent.InverseTransformPoint(targetPositon);
            targetPositon.z = content.localPosition.z;
            if (!Horizontal){
                targetPositon.x = content.localPosition.x;
            }
            if(!Vertical){

                targetPositon.y = content.localPosition.y;
            }
            if (speed <= 0)
            {
                SetPostion(targetPositon);
            }
            else
            {
                can_ease = true;
            }
        }

        public void CenterImmediately(RectTransform child)
        {
            if (has_viewWorldCenter == false)
            {
                viewWorldCenter = GetWorldCenter(viewport);
                has_viewWorldCenter = true;
            }
            Vector3 center = GetWorldCenter(child);

            targetPositon = content.position + viewWorldCenter - center;
            targetPositon = content.parent.InverseTransformPoint(targetPositon);
            targetPositon.z = content.localPosition.z;
            if (!Horizontal){
                targetPositon.x = content.localPosition.x;
            }
            if(!Vertical){

                targetPositon.y = content.localPosition.y;
            }
            SetPostion(targetPositon);
        }


        private Vector3 GetWorldCenter(RectTransform rt)
        {
            Vector3[] v = new Vector3[4];
            rt.GetWorldCorners(v);
            return  (v[0] + v[2]) / 2;
        }
        private void SetPostion(Vector3 pos)
        {
            Vector3 oldpos = content.localPosition;
            content.localPosition = pos;
            if(scroll != null)
            {
                if(scroll.movementType != ScrollRect.MovementType.Unrestricted && StopScrollOnElastic)
                {
                    if (scroll.horizontalNormalizedPosition > 1)
                    {
                        content.localPosition = oldpos;
                        Complete();
                    }
                    else if (scroll.horizontalNormalizedPosition < 0)
                    {
                        content.localPosition = oldpos;
                        Complete();
                    }
                    if (scroll.verticalNormalizedPosition > 1)
                    {
                        content.localPosition = oldpos;
                        Complete();
                    }
                    else if (scroll.verticalNormalizedPosition < 0)
                    {
                        content.localPosition = oldpos;
                        Complete();
                    }
                }
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
           
            if ((content.localPosition - targetPositon).sqrMagnitude < 0.008f)
            {
                SetPostion(targetPositon);
                Complete();
            }
            else
            {
                Vector3 pos = Vector3.Lerp(content.localPosition, targetPositon, speed * Time.unscaledDeltaTime);

                if ((pos - targetPositon).sqrMagnitude < 0.008f)
                {
                    SetPostion(targetPositon);
                    Complete();
                }
                else
                {
                    SetPostion(pos);
                }
            }

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            can_ease = false;
        }

        public void Complete()
        {
            can_ease = false;
            if (OnCompleted != null)
                OnCompleted.Invoke();
        }

        public bool IsMoving
        {
            get
            {
                return can_ease;
            }
        }
    }
}

