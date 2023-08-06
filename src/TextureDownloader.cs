using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace VSMenuHelper
{
    internal class TextureDownloader
    {
        private Uri textureUri;
        //private Texture2D? DownloadedTexture;
        //private bool IsDownloading = false;
        private byte[]? DownloadedBytes;
        private Action<byte[]> onComplete;
        private Action<string> onError;

        //public TextureDownloader(Uri textureUri)
        //{
        //    this.textureUri = textureUri;
        //}

        public TextureDownloader(Uri textureUri, Action<byte[]> onComplete, Action<string> onError)
        {
            this.textureUri = textureUri;
            this.onComplete = onComplete;
            this.onError = onError;
        }

        // This causes:
        //      Method not found: 'Void UnityEngine.Networking.DownloadHandlerTexture..ctor(Boolean)'.
        // I believe it is due to the fact the Texture is created then immediately destroyed by the game. Possibly because we're changing scenes?
        // Work around: Manually download via UnityWebRequest.Get() and use the byte array as input.
        //private async Task<Texture2D> DownloadTexture()
        //{
        //    while (IsDownloading)
        //        await Task.Delay(100);

        //    IsDownloading = true;
        //    UnityWebRequest? uwr = null;
        //    Texture2D texture;
        //    try
        //    {
        //        uwr = UnityWebRequestTexture.GetTexture(textureUri.AbsoluteUri);
        //        var request = uwr.SendWebRequest();

        //        while (!request.isDone)
        //            await Task.Delay(100);

        //        if (uwr.result != UnityWebRequest.Result.Success)
        //        {
        //            throw new Exception(uwr.error);
        //        }

        //        texture = uwr.downloadHandler.Cast<DownloadHandlerTexture>().texture;
        //        // DownloadHandlerTexture.GetContent(uwr);
        //    } catch
        //    {
        //        uwr?.Dispose();
        //        IsDownloading = false;
        //        throw;
        //    }

        //    uwr.Dispose();
        //    IsDownloading = false;
        //    return texture;
        //}

        // Can't invoke unity commands from different thread...
        //private async Task<byte[]> DownloadTexture()
        //{
        //    while (IsDownloading)
        //        await Task.Delay(100);

        //    IsDownloading = true;
        //    UnityWebRequest? uwr = null;
        //    byte[] bytes;
        //    try
        //    {
        //        uwr = UnityWebRequest.Get(textureUri.AbsoluteUri);
        //        var request = uwr.SendWebRequest();

        //        while (!request.isDone)
        //            await Task.Delay(100);

        //        if (uwr.result != UnityWebRequest.Result.Success)
        //        {
        //            throw new Exception(uwr.error);
        //        }

        //        bytes = uwr.downloadHandler.data;
        //    }
        //    catch
        //    {
        //        uwr?.Dispose();
        //        IsDownloading = false;
        //        throw;
        //    }

        //    uwr.Dispose();
        //    IsDownloading = false;
        //    return bytes;
        //}

        private void DownloadTexture()
        {
            //while (IsDownloading)
            //    await Task.Delay(100);

            //IsDownloading = true;
            UnityWebRequest? uwr = null;
            byte[] bytes;
            try
            {
                uwr = UnityWebRequest.Get(textureUri.AbsoluteUri);
                var request = uwr.SendWebRequest();

                while (!request.isDone)
                    System.Threading.Thread.Sleep(50);

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    onError?.Invoke(uwr.error);
                }

                bytes = uwr.downloadHandler.data;
            }
            catch
            {
                uwr?.Dispose();
                //IsDownloading = false;
                throw;
            }

            uwr.Dispose();
            //IsDownloading = false;
            DownloadedBytes = bytes;
            onComplete(bytes);
        }

        public byte[]? GetBytes()
        {
            if (DownloadedBytes == null || DownloadedBytes?.Count() == 0)
            {
                DownloadTexture();
                //DownloadedBytes = task.Result;
            }

            return DownloadedBytes;
        }
        

        //public Texture2D GetTexture()
        //{
        //    if (DownloadedTexture == null)
        //    {
        //        Task<Texture2D> task = Task.Run(DownloadTexture);
        //        DownloadedTexture = task.Result;
        //    }

        //    return DownloadedTexture;
        //}
    }
}
