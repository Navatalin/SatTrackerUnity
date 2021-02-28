using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private GameObject popupCanvasObject;
    [SerializeField] private RectTransform popupObject;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Vector3 offset;
    private float padding;

    private Canvas popupCanvas;
    private void Awake() {
        popupCanvas = popupCanvasObject.GetComponent<Canvas>();
    }

    private void Update() {
        FollowCursor();
    }
    private void FollowCursor(){
        if(!popupCanvasObject.activeSelf) {return ;}

        Vector3 newPos  = Input.mousePosition + offset;
        newPos.z = 0f;
        float rightEdgeToScreenEdgeDifference = Screen.width - (newPos.x + popupObject.rect.width * popupCanvas.scaleFactor /2) - padding;
        if(rightEdgeToScreenEdgeDifference < 0){
            newPos.x += rightEdgeToScreenEdgeDifference;
        }
        float leftEdgeToScreenEdgeDifference = 0 - (newPos.x - popupObject.rect.width * popupCanvas.scaleFactor /2) - padding;
        if(leftEdgeToScreenEdgeDifference > 0){
            newPos.x += leftEdgeToScreenEdgeDifference;
        }
        float topEdgeToScreenEdgeDifference = Screen.height - (newPos.y + popupObject.rect.height * popupCanvas.scaleFactor) - padding;
        if(topEdgeToScreenEdgeDifference < 0){
            newPos.y += topEdgeToScreenEdgeDifference;
        }

        popupObject.transform.position = newPos;

    }
    public void DisplayInfo(satObjData sat){
        StringBuilder sb = new StringBuilder();
        sb.Append("<size=35>").Append(sat.name).Append("</size>").AppendLine();
        sb.Append("<size=16>");
        sb.Append(sat.GetAltitudeData());
        sb.Append(sat.GetPositionData());
        sb.Append(sat.GetVelocityData());
        sb.Append("</size>");

        infoText.text = sb.ToString();

        popupCanvasObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);
    }
    public void HideInfo(){
        popupCanvasObject.SetActive(false);
    }
}
