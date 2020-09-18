using System.Dynamic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;


[RequireComponent(typeof(RectTransform))]
public class DragDrop : MonoBehaviour , IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    ,IPointerEnterHandler, IPointerExitHandler//, IDragAndDropEvent
{

    //private RectTransform _rectTransform;
    [SerializeField] private RectTransform targetPanel;
    [SerializeField] private RectTransform mainPanelRectTransform;
    [SerializeField] private Canvas canvas;
    private Vector2 _originalResolution;
    private Camera _camera;
    private void Awake()
    {
        _originalResolution = canvas.gameObject.GetComponent<RectTransform>().sizeDelta;
        _camera = Camera.main;
        
        //_rectTransform = GetComponent<RectTransform>();
        #if UNITY_EDITOR
            Assert.IsNotNull(targetPanel);
        #endif
        var rect = mainPanelRectTransform.rect;
        Debug.Log($" {rect.x}, {rect.x} {rect.width} {rect.height}");
        var res = Screen.currentResolution;
        Debug.Log(res);
        Debug.Log($"pixel rect{_camera.pixelRect}\nscaledPixelHeight{_camera.scaledPixelHeight} pixelHeight:{_camera.pixelHeight}");
        var screenRect = new  Rect(0, 0, Screen.currentResolution.width, Screen.currentResolution.height);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("[DragDrop] OnPointerDown");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("[DragDrop] OnBeginDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("[DragDrop] OnEndDrag");
        //TODO: check if by some reason we drag out of the screen
        //
        // targetPanel.Contr
        //if (targetPanel.rect.Contains(Vector3.zero))
        //if (targetPanel.rect.Overlaps(mainPanelRectTransform.rect))
        
        Resolution screenResolution = Screen.currentResolution;
        Rect screenRect = new Rect(0, 0, screenResolution.width, screenResolution.height);
        
        
        if (GetWorldSpaceRect(mainPanelRectTransform).Overlaps(GetWorldSpaceRect(targetPanel)))
        //if (targetPanel.rect.Overlaps(screenRect))
        {
            Debug.Log($"--- Inside --- {targetPanel.rect.ToString()}");
        }
        else
        {
            Debug.Log($"--- Outside --- {targetPanel.rect.ToString()}");
        }
        
        MoveBackToScreen(targetPanel);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("[DragDrop] OnDrag");
        targetPanel.anchoredPosition += eventData.delta;
    }

    
    // TODO: when enter/exit, change the alpha, if this option is enabled
    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
    
    private Rect GetWorldSpaceRect(RectTransform rectTransform)
    {    
        var rect = rectTransform.rect;
        rect.center = rectTransform.TransformPoint(rect.center);
        rect.size = rectTransform.TransformVector(rect.size);
        return rect;
    }
    
    private Rect GetWorldSpaceRect2(RectTransform rectTransform)
    {
        var rect = rectTransform.rect;
        Vector3 bottomLeft = new Vector3(rect.x, rect.y);
        Vector3 topRight = new Vector3(rect.xMax, rect.yMax);
        
        
        var localToWorldMatrix = rectTransform.localToWorldMatrix;
        
        Vector2 bottomLeftWorld = localToWorldMatrix.MultiplyPoint(bottomLeft);
        Vector2 topLRightWorld = localToWorldMatrix.MultiplyPoint(topRight);
        
        var worldRect = new Rect(bottomLeftWorld, topLRightWorld);
        return worldRect;
    }

    private void MoveBackToScreen(RectTransform rectTransform)
    {
        Resolution screenResolution = Screen.currentResolution;
        Rect screenRect = new Rect(0, 0, screenResolution.width, screenResolution.height);
        
        var rect = GetWorldRect(rectTransform, canvas.transform.localScale);
        Debug.Log($"Rect World Space {rect}");
        
        var currentAnchor = rectTransform.anchoredPosition;
        
        if (rect.xMax < 0)
        {
            //Debug.Log("left outside");
            rectTransform.anchoredPosition = new Vector2(0, currentAnchor.y);
        }
        else if (rect.xMin > screenRect.width)
        {
            //Debug.Log("right outside");
            rectTransform.anchoredPosition = new Vector2(_originalResolution.y - rectTransform.rect.width, currentAnchor.y);
        }
        else if (rect.yMin > screenRect.height)
        {
            //Debug.Log("top outside");
            rectTransform.anchoredPosition = new Vector2(currentAnchor.x, _originalResolution.y);
        }
        else if (rect.y < 0)
        {
            //Debug.Log("bottom outside");
            rectTransform.anchoredPosition = new Vector2(currentAnchor.x, rectTransform.rect.height);
        }
    }
    
    /// <summary>
    /// Converts RectTransform.rect's local coordinates to world space
    /// Usage example RectTransformExt.GetWorldRect(myRect, Vector2.one);
    /// </summary>
    /// <returns>The world rect.</returns>
    /// <param name="rectTransform">RectangleTransform we want to convert to world coordinates.</param>
    /// <param name="scale">Optional scale pulled from the CanvasScaler. Default to using Vector2.one.</param>
    public static Rect GetWorldRect(RectTransform rectTransform, Vector2 scale)
    {
        if (rectTransform == null)
        {
            Debug.LogWarning((object) "Trying to GetWorldRect of a null rectTransform");
            return new Rect(Vector2.up, Vector2.zero);
        }

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector3 topLeft = corners[1];

        // Rescale
        Vector2 size = rectTransform.rect.size;
        Vector2 scaledSize = new Vector2(scale.x * size.x, scale.y * size.y);

        return new Rect(topLeft, scaledSize);
    }
}
