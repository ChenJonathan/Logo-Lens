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
            Debug.Log("Tap event detected");

            // Raycast
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;

            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
            {
                // Close card if in line of sight
                if (hitInfo.collider.gameObject.tag == "Card")
                {
                    GetComponent<Controller>().RemoveCard(hitInfo.collider.GetComponent<Card>());
                    return;
                }
            }
            
            // TODO Call photo capture instead
            GetComponent<Controller>().AddCard(Controller.Image64);
        };
        recognizer.StartCapturingGestures();
    }
}