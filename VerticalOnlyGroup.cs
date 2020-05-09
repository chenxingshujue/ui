using UnityEngine;
using UnityEngine.UI;
namespace User
{
    public class VerticalOnlyGroup : HorizontalOrVerticalLayoutGroup
    {
        protected VerticalOnlyGroup()
        { }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, true);
        }

        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, true);
        }

        public override void SetLayoutHorizontal()
        {
           
        }

        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, true);
        }

    }
}