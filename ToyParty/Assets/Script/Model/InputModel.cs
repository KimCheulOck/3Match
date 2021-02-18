using UnityEngine;


using UnityEngine.UI;

public class InputModel
{
    private System.Action<HexaBlockUnit> onEventRayHitBlock = null;
    private bool isTouch = false;
    
    public void AddEvent(System.Action<HexaBlockUnit> onEventRayHitBlock)
    {
        this.onEventRayHitBlock = onEventRayHitBlock;
    }

    public void SetTouchFlag(bool isTouch)
    {
        this.isTouch = isTouch;
    }

    public bool CheckSelectBlock()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            isTouch = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isTouch = false;
        }
#else
        if ( Input.touchCount > 0 )
        {
            isTouch = true;
        }
        else if ( Input.touchCount == 0 )
        {
            isTouch = false;
        }
#endif

        if (!isTouch)
            return false;


#if UNITY_EDITOR
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#else
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Transform target = hit.collider.transform;
            HexaBlockUnit blockUnit = target.GetComponent<HexaBlockUnit>();
            onEventRayHitBlock(blockUnit);
        }

        return true;
    }
}