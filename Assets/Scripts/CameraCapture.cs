using UnityEngine;
using UnityEngine.VR.WSA.WebCam;
using System.Linq;
using System;
using System.Collections.Generic;

public class CameraCapture : MonoBehaviour {

    public string image { get; set; }
    private PhotoCapture pc = null;
    public TextMesh textMesh;

    public void CaptureImage()
    {
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        Debug.LogError("Created async capture task");
    }

    public void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        Debug.LogError("OnPhotoCaptureCreated");
        pc = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).Last();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        pc.StartPhotoModeAsync(c, false, OnPhotoModeStarted);
    }

    public void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.LogError("OnStopedPhotoMode");
        pc.Dispose();
        pc = null;
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.LogError("OnPhotoModeStarted");
        if (result.success)
        {
            pc.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
            //TextMesh temp = (TextMesh)Instantiate(prefab, transform.position + transform.forward * 10, Quaternion.identity);
            //temp.text = "FAILURE";
        }
    }

    public void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        Debug.LogError("OnCapturedPhotoToMemory");
        if (result.success)
        { 
            //// Copy the raw IMFMediaBuffer data into our empty byte list.
            List<byte> imageBufferList = new List<byte>();
            photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);
            String image = Convert.ToBase64String(imageBufferList.ToArray());

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(1, 1, 5);

            GetComponent<CardController>().AddCard(image);
        }
        pc.StopPhotoModeAsync(OnStoppedPhotoMode);
    }
}