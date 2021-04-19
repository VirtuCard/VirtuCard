using System;
using System.IO;
using Firebase;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace FirebaseScripts
{
    public class ImageStorage
    {
        private static FirebaseStorage storage;
        private static StorageReference storageRef;

        public static void setApp(FirebaseApp firebaseApp)
        {
            storage = FirebaseStorage.GetInstance(firebaseApp);
            storageRef =
                storage.GetReferenceFromUrl("gs://virtucard-4529f.appspot.com");
        }

        public static void getImage(string imageName, Action<byte[]> imageCallback)
        {
            StorageReference imageRef = storageRef.Child(imageName);
            imageRef.GetBytesAsync(1024 * 1024).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    imageCallback(task.Result);
                }
                else
                {
                    Debug.Log("Error Loading Avatar");
                    imageCallback(null);
                }
            });
        }

        public static void getAvatarImage(string imageName, Action<bool> imageCallback)
        {
            StorageReference imageRef = storageRef.Child(imageName);
            imageRef.GetBytesAsync(1024 * 1024).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    ClientData.ImageData = task.Result;
                    imageCallback(true);
                }
                else
                {
                    Debug.Log("Error Loading Avatar");
                    imageCallback(false);
                }
            });
        }


        public static void uploadImage(string imageName, byte[] rawImageBytes, Action<bool> callback)
        {
            StorageReference imageRef = storageRef.Child(imageName);
            imageRef.PutBytesAsync(rawImageBytes).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    callback(true);
                }
                else
                {
                    Debug.Log("Error Loading Avatar");
                    callback(false);
                }
            });
        }
    }
}