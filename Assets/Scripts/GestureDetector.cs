using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GestureDetector : MonoBehaviour
{
    public static GestureDetector Instance { get; private set; }

    GestureRecognizer recognizer;

    int num = 0;
    
    void Awake()
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
                    Destroy(hitInfo.collider.gameObject);
                    return;
                }
            }
  
            //GetComponent<CameraCapture>().CaptureImage();
            if (num == 0)
            {
                GetComponent<GetStockData>().CallNasdaqAPI("09/16/2016", "09/16/2016", "DNKN");
            }
            else if (num == 1)
            {
                GetComponent<GetStockData>().CallNasdaqAPI("09/16/2016", "09/16/2016", "MSFT");
            }
            else if (num == 2)
            {
                GetComponent<GetStockData>().CallNasdaqAPI("09/16/2016", "09/16/2016", "GOOG");
            }
            num++;
        };
        recognizer.StartCapturingGestures();
    }
}
