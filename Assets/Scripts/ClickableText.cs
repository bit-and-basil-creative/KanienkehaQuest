using UnityEngine;
using TMPro;

public class ClickableText : MonoBehaviour
{
    public TextMeshProUGUI responseText;
    public ResponseHandler responseHandler;
    private int lastHoveredLink = -1;
    private int choiceIndex;
    private string linkID;

    public void Setup(ResponseHandler handler, int index, string id)
    {
        responseHandler = handler;
        choiceIndex = index;
        linkID = id;
    }

    void Update()
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(responseText, Input.mousePosition, null);

        if (linkIndex != lastHoveredLink)
        {
            lastHoveredLink = linkIndex;
            UpdateLinkColors(linkIndex);
        }

        if (Input.GetMouseButtonDown(0) && linkIndex != -1)
        {
            if (responseHandler != null)
            {
                responseHandler.HandleResponse(choiceIndex, linkID);
            }
        }
    }

    void UpdateLinkColors(int linkIndex)
    {

        string updatedText = responseText.text;

        // Reset all colors first
        updatedText = updatedText.Replace("<color=#d06c1c>", "<color=#3c2415>"); // Reset hover effect
        
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = responseText.textInfo.linkInfo[linkIndex];

            // Apply hover effect
            updatedText = updatedText.Replace(linkInfo.GetLinkText(), $"<color=#d06c1c>{linkInfo.GetLinkText()}</color>");
        }

        responseText.text = updatedText;
    }
}
