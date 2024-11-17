using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
    File name: CursorMoveable.cs
    Summary: Allows a UI object to be moved by the cursor when it is held over the object
    Creation Date: 07/10/2024
    Last Modified: 18/11/2024
*/
public class CursorMoveable : MonoBehaviour, IDragHandler
{
    public Canvas m_canvas;
    public RectTransform m_uiBounds;
    public bool m_clampToPlayArea = true;
    RectTransform m_rectTransform;
    Vector3 m_clampedPosition = Vector3.zero;

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

        // if this gameobject should be limited to be within the play area
        if (m_clampToPlayArea)
        {
            // reset the clamp vector
            m_clampedPosition = m_rectTransform.transform.localPosition;

            // if the left most bound of this object is beyond the left most point of the ui area
            if (m_rectTransform.transform.localPosition.x + m_rectTransform.rect.x < m_uiBounds.transform.localPosition.x + m_uiBounds.rect.x)
            {
                // get the left most valid position of this object and store it in the clamp vector
                m_clampedPosition.x = m_uiBounds.transform.localPosition.x + m_uiBounds.rect.x - m_rectTransform.rect.x;
            }
            // if the right most bound of this object is beyond the right most point of the ui area
            else if (m_rectTransform.transform.localPosition.x - m_rectTransform.rect.x > m_uiBounds.transform.localPosition.x - m_uiBounds.rect.x)
            {
                // get the right most valid position of this object and store it in the clamp vector
                m_clampedPosition.x = m_uiBounds.transform.localPosition.x - m_uiBounds.rect.x + m_rectTransform.rect.x;
            }

            // if the lowest bound of this object is beyond the lowest point of the ui area
            if (m_rectTransform.transform.localPosition.y + m_rectTransform.rect.y < m_uiBounds.transform.localPosition.y + m_uiBounds.rect.y)
            {
                // get the lowest valid position of this object and store it in the clamp vector
                m_clampedPosition.y = m_uiBounds.transform.localPosition.y + m_uiBounds.rect.y - m_rectTransform.rect.y;
            }
            // if the highest bound of this object is beyond the highest point of the puilay area
            else if (m_rectTransform.transform.localPosition.y - m_rectTransform.rect.y > m_uiBounds.transform.localPosition.y - m_uiBounds.rect.y)
            {
                // get the highest valid position of this object and store it in the clamp vector
                m_clampedPosition.y = m_uiBounds.transform.localPosition.y - m_uiBounds.rect.y + m_rectTransform.rect.y;
            }

            // apply the clamp to this game object
            m_rectTransform.transform.localPosition = m_clampedPosition;
        }
    }
}
