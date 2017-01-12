using UnityEngine;
using System.Collections;

using UnityEngine.VR.WSA.WebCam;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
#if !UNITY_EDITOR
using Windows.Storage;
using Windows.System;
#endif

public class CameraCapture : MonoBehaviour
{
    public void StartCameraCapture()
    {
#if !UNITY_EDITOR
        TakePhoto();
#endif
    }
#if !UNITY_EDITOR
    PhotoCapture photoCaptureObject = null;
    bool haveFolderPath = false;
    StorageFolder picturesFolder;
    string tempFilePathAndName;
    string tempFileName;

    void Start()
    {
        GetFolderPath();
    }
    
    void TakePhoto()
    {
        if (haveFolderPath)
        {
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        }
    }

    async void GetFolderPath()
    {
        StorageLibrary myPictures = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
        picturesFolder = myPictures.SaveFolder;
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

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
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

            try
            {
                photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
            }
            catch (System.ArgumentException e)
            {
                Debug.LogError("System.ArgumentException:\n" + e.Message);
            }
        }
    }

    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            Debug.Log("moving "+tempFilePathAndName+" to " + picturesFolder.Path + "\\Camera Roll\\" + tempFileName);
            File.Move(tempFilePathAndName, picturesFolder.Path + "\\Camera Roll\\" + tempFileName);
            
            byte[] fileData = File.ReadAllBytes(picturesFolder.Path + "\\Camera Roll\\" + tempFileName);
            string image = Convert.ToBase64String(fileData);

            GetComponent<CardController>().AddCard(image);
        }
    }

    async public static void WriteToFile(string filename, string message)
    {
        StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
        StorageFile file =
            await storageFolder.CreateFileAsync(filename,
                CreationCollisionOption.ReplaceExisting);
        await FileIO.WriteTextAsync(file, message);
    }
#endif
}