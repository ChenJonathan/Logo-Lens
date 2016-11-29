using UnityEngine;
using System.Collections;

using UnityEngine.VR.WSA.WebCam;
using System.Linq;
#if !UNITY_EDITOR
using Windows.Storage;
using Windows.System;
using System.Collections.Generic;
using System;
using System.IO;
#endif

public class CameraCapture : MonoBehaviour
{

#if !UNITY_EDITOR
    PhotoCapture photoCaptureObject = null;
    bool haveFolderPath = false;
    StorageFolder picturesFolder;
    string tempFilePathAndName;
    string tempFileName;

    // Use this for initialization
    void Start()
    {
        getFolderPath();
        while (!haveFolderPath)
        {
            Debug.Log("Waiting for folder path...");
        }
        Debug.Log("About to call CreateAsync");
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        Debug.Log("Called CreateAsync");
    }

    async void getFolderPath()
    {
        StorageLibrary myPictures = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
        picturesFolder = myPictures.SaveFolder;

        foreach(StorageFolder fodler in myPictures.Folders)
        {
            Debug.Log(fodler.Name);

        }

        Debug.Log("savePicturesFolder.Path is " + picturesFolder.Path);
        haveFolderPath = true;
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;

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
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            tempFileName = string.Format(@"CapturedImage{0}_n.jpg", Time.time);

            string filePath = System.IO.Path.Combine(Application.persistentDataPath, tempFileName);
            tempFilePathAndName = filePath;
            Debug.Log("Saving photo to " + filePath);

            try
            {
                photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
            }
            catch (System.ArgumentException e)
            {
                Debug.LogError("System.ArgumentException:\n" + e.Message);
            }
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            Debug.Log("Saved Photo to disk!");
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            Debug.Log("moving "+tempFilePathAndName+" to " + picturesFolder.Path + "\\Camera Roll\\" + tempFileName);
            File.Move(tempFilePathAndName, picturesFolder.Path + "\\Camera Roll\\" + tempFileName);
            
            byte[] fileData = File.ReadAllBytes(picturesFolder.Path + "\\Camera Roll\\" + tempFileName);
            String image = Convert.ToBase64String(fileData);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(1, 1, 5);

            GetComponent<CardController>().AddCard(image);
        }
        else
        {
            Debug.Log("Failed to save Photo to disk " +result.hResult+" "+result.resultType.ToString());
        }
    }
#endif
}