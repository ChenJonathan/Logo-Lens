using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GestureDetector : MonoBehaviour
{
    public static GestureDetector Instance { get; private set; }

    GestureRecognizer recognizer;
    
    public void Awake()
    {
        Instance = this;

        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();

        // Executed when a tap event is detected
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            // Raycast calculations
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
            {
                // Close card if in line of sight
                Collider col = hitInfo.collider;
                if (col.tag == "Card")
                {
                    GetComponent<CardController>().RemoveCard(col.GetComponent<Card>());
                    return;
                }
            }
            else
            {
                // TODO Call photo capture instead
                gameObject.AddComponent<CameraCapture>();
            }
        };
        recognizer.StartCapturingGestures();
    }
}