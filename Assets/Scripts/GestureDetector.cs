using UnityEngine;
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

        // Check HoloLens cursor
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            Collider col = hitInfo.collider;
            if (col.name == "Center" || col.name == "Bottom" || col.name == "Left" || col.name == "Right" || col.name == "Top")
            {
                col.GetComponent<SmoothHover>().Hover();
                Card card = col.GetComponentInParent<Card>();
                //card.SetTopElementText("Price", "");
                //card.SetTopElementText("Date", "");
            }
            else if (col.name == "Point")
            {
                // Checks if a point is hit
                GraphPoint gp = col.GetComponent<GraphPoint>();
                Card card = col.transform.parent.parent.parent.GetComponent<Card>();
                card.SetTopElementText("Price", "Price: $" + gp.Value);
                card.SetTopElementText("Date", gp.DateTime);
            }
        }

        // Check mouse cursor
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            Collider col = hitInfo.collider;
            if (col.name == "Point")
            {
                // Checks if a point is hit
                GraphPoint gp = col.GetComponent<GraphPoint>();
                Debug.Log(col.transform.parent.parent.parent.name);
                Card card = col.transform.parent.parent.parent.GetComponent<Card>();
                card.SetTopElementText("Price", "Price: $" + gp.Value);
                card.SetTopElementText("Date", gp.DateTime.Substring(0, gp.DateTime.Length - 7));
            }
            else if (col.name == "Center" || col.name == "Bottom" || col.name == "Left" || col.name == "Right" || col.name == "Top")
            {
                col.GetComponent<SmoothHover>().Hover();
                Card card = col.GetComponentInParent<Card>();
                card.SetTopElementText("Price", "");
                card.SetTopElementText("Date", "");
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