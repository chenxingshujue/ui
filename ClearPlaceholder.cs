using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class ClearPlaceholder : MonoBehaviour,IDeselectHandler,ISelectHandler {

	// Use this for initialization
    InputField input;
    GameObject placeholder;
    void Start()
    {
        input = gameObject.GetComponent<InputField>();
        if (input != null && input.placeholder != null)
        {
            placeholder = input.placeholder.gameObject;
        }
    }
    public void OnSelect(BaseEventData eventData)
    {
        placeholder.SetActive(false);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        placeholder.SetActive(true);
    }

}
