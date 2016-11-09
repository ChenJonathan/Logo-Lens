using System;
using Windows.Storage;

public class DocumentWriter
{
	public static void WriteToFile(string filename, string message)
	{
        StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
        StorageFile file =
            await storageFolder.CreateFileAsync(filename,
                CreationCollisionOption.ReplaceExisting);
        await FileIO.WriteTextAsync(file, message);
    }
}