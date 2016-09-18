using UnityEngine;

public class WorldCursor:MonoBehaviour
{
    private MeshRenderer meshRenderer;

    //initialization
    void Start()
    {
        // Grab the mesh renderer that's on the same object as this script.
        meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
    }

    // Call update once per frame
    void Update()
    {
        // Raycast
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;

        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            // If the raycast hit a hologram, display mesh
            meshRenderer.enabled = true;

            //Display cursor to point where raycast hit
            this.transform.position = hitInfo.point;

            //Cursor hugging object
            this.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }
        else
        {
            //don't display mesh 
            meshRenderer.enabled = false;
        }
    }
}