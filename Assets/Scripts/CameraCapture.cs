using UnityEngine;
using UnityEngine.VR.WSA.WebCam;
using System.Linq;
using System;
using System.Collections.Generic;

public class CameraCapture : MonoBehaviour {

    public string image { get; set; }
    public TextMesh prefab;
    private PhotoCapture pc = null;

    public void CaptureImage()
    {
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        Debug.LogError("Created async capture task");
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        Debug.LogError("OnPhotoCaptureCreated");
        pc = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;
        
        captureObject.StartPhotoModeAsync(c, false, OnPhotoModeStarted);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
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
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        Debug.LogError("OnCapturedPhotoToMemory");
        if (result.success)
        {
            List<byte> imageBufferList = new List<byte>();;
            // Copy the raw IMFMediaBuffer data into our empty byte list.
            photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);
            //image = Convert.ToBase64String(imageBufferList.ToArray());

            // In this example, we captured the image using the BGRA32 format.
            // So our stride will be 4 since we have a byte for each rgba channel.
            // The raw image data will also be flipped so we access our pixel data
            // in the reverse order.
            int stride = 4;
            float denominator = 1.0f / 255.0f;
            List<Color> colorArray = new List<Color>();
            for (int i = imageBufferList.Count - 1; i >= 0; i -= stride)
            {
                float a = (int)(imageBufferList[i - 0]) * denominator;
                float r = (int)(imageBufferList[i - 1]) * denominator;
                float g = (int)(imageBufferList[i - 2]) * denominator;
                float b = (int)(imageBufferList[i - 3]) * denominator;

                colorArray.Add(new Color(r, g, b, a));
            }
            convertArray(colorArray);

            TextMesh temp = (TextMesh)Instantiate(prefab, transform.position + transform.forward * 10, Quaternion.identity);
            temp.text = image;
            GetComponent<Vision>().DetectImage(image);
        }
        pc.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    public void convertArray(List<Color> anArray)
    {
        Debug.LogError("ConvertArray");
        byte[] output = new byte[anArray.Count*16];
        int pos = 0;
        for (int i = anArray.Count - 1; i>=0; i -= 4)
        {
            foreach (float value in new float[] { anArray[i].a, anArray[i].r, anArray[i].g, anArray[i].b })
            {
                foreach (byte newByte in BitConverter.GetBytes(value))
                {
                    output[pos++] = newByte;
                }
            }
        }
        image = Convert.ToBase64String(output);
    }
}