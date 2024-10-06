using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
    File name: CursorMoveable.cs
    Summary: Allows a UI object to be moved by the cursor when it is held over the object
    Creation Date: 07/10/2024
    Last Modified: 07/10/2024
*/
public class CursorMoveable : MonoBehaviour, IDragHandler
{
    public Canvas m_canvas;
    RectTransform m_rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        // get the rect transform of this object
        m_rectTransform = GetComponent<RectTransform>();
    }

    void IDragHandler.OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        // move this game object based on the cursor position
        m_rectTransform.anchoredPosition += eventData.delta / m_canvas.scaleFactor;
    }
}
