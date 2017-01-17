using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.WSA.Input;

public class GestureDetector : MonoBehaviour
{
    GestureRecognizer recognizer;

    void FixedUpdate()
    {
        // Raycast calculations
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        RaycastHit hitInfo;

        // Check if HoloLens cursor is on a card
        int layer_mask = LayerMask.GetMask("Default", "UI");
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, Mathf.Infinity, layer_mask))
        {
            Collider col = hitInfo.collider;
            if (col.name == "Center" || col.name == "Top" || col.name == "Bottom" || col.name == "Left" || col.name == "Right")
            {
                col.GetComponent<SmoothHover>().Hover();
                Card card = col.GetComponentInParent<Card>();

                // Check if HoloLens cursor is on a point
                if(col.name == "Center")
                {
                    layer_mask = LayerMask.GetMask("Points");
                    if(Physics.Raycast(headPosition, gazeDirection, out hitInfo, Mathf.Infinity, layer_mask))
                    {
                        col = hitInfo.collider;
                        // Checks if a point is hit
                        if(card == col.transform.parent.parent.parent.GetComponent<Card>() && card.Busy == 0)
                        {
                            if (card.prevPoint)
                            {
                                card.prevPoint.GetComponent<Image>().enabled = false;
                            }
                            col.gameObject.GetComponent<Image>().enabled = true;
                            card.prevPoint = col.gameObject;
                            GraphPoint gp = col.GetComponent<GraphPoint>();
                            card.SetBottomElementText("Price", "Price: $" + gp.Value);
                            card.SetBottomElementText("Date", gp.DateTime.Substring(0, gp.DateTime.Length - 7));
                        }
                    }
                }
            }
        }

        // Check if mouse is on a card
        layer_mask = LayerMask.GetMask("Default", "UI");
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, layer_mask))
        {
            Collider col = hitInfo.collider;
            if (col.name == "Center" || col.name == "Top" || col.name == "Bottom" || col.name == "Left" || col.name == "Right")
            {
                col.GetComponent<SmoothHover>().Hover();
                Card card = col.GetComponentInParent<Card>();

                // Check if mouse is on a point
                if(col.name == "Center")
                {
                    layer_mask = LayerMask.GetMask("Points");
                    if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, layer_mask))
                    {
                        col = hitInfo.collider;
                        // Checks if a point is hit
                        if(card == col.transform.parent.parent.parent.GetComponent<Card>() && card.Busy == 0)
                        {
                            if (card.prevPoint)
                            {
                                card.prevPoint.GetComponent<Image>().enabled = false;
                            }
                            col.gameObject.GetComponent<Image>().enabled = true;
                            card.prevPoint = col.gameObject;
                            GraphPoint gp = col.GetComponent<GraphPoint>();
                            card.SetBottomElementText("Price", "Price: $" + gp.Value);
                            card.SetBottomElementText("Date", gp.DateTime.Substring(0, gp.DateTime.Length - 7));
                        }
                    }
                }
            }
        }
    }

    public void Awake()
    {
        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();

        // Executed when a tap event is detected
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            // Raycast calculations
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;
            RaycastHit hitInfo;
            if(Physics.Raycast(headPosition, gazeDirection, out hitInfo))
            {
                Collider col = hitInfo.collider;
                if (col.name == "Center" || col.name == "Bottom" || col.name == "Left" || col.name == "Right" || col.name == "Top")
                {
                    if (col.GetComponentInParent<Card>().Busy == 0)
                    {
                        if (col.name == "Center" || col.name == "Bottom" || col.name == "Top")
                        {
                            // Close card if in line of sight
                            CardController.Instance.RemoveCard(col.GetComponentInParent<Card>());
                        }
                        else if (col.name == "Left" || col.name == "Right")
                        {
                            // Change button range if in line of sight
                            col.GetComponent<CardButton>().ModifyRange();
                        }
                    }
                }
            }
            else
            {
                // Call photo capture
                gameObject.GetComponent<CameraCapture>().StartCameraCapture();
            }
        };
        recognizer.StartCapturingGestures();
    }
}