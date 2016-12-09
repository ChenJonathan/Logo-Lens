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
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            Collider col = hitInfo.collider;
            if (col.name == "Center" || col.name == "Bottom" || col.name == "Left" || col.name == "Right")
            {
                col.GetComponent<SmoothHover>().Hover();
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
                if(col.name == "Center" || col.name == "Bottom")
                {
                    // Close card if in line of sight
                    CardController.Instance.RemoveCard(col.transform.parent.GetComponent<Card>());
                }
                else if(col.name == "Left" || col.name == "Right")
                {
                    // Change button range if in line of sight
                    col.GetComponent<CardButton>().ModifyRange();
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