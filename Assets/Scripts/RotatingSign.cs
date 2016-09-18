using UnityEngine;
using System.Collections;

public class RotatingSign : MonoBehaviour
{
    private Camera mainCamera;

    public void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
    }

    public void Update()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }
}