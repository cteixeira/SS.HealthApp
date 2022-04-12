using System;
using System.Net.Http;
using PCLStorage;
using System.IO;
using System.Collections.Generic;

namespace SS.HealthApp.PCL.Utils {

    public delegate void RequestCompleted(string filePath);

    public class DownloadManager {
        
        private const string DATA_FOLDER = "data";

        //race problems - if one view has the same image more than once, the file gets blocked
        private static List<string> downloadInProgress = new List<string>();

        public RequestCompleted RequestCompletedHandler;

        public static async void StartDownloadAsync(string url, RequestCompleted completedAction) {

            //TODO: IMPROVE CACHE IMPLEMENTATION USING EXPIRED IMAGE

            IFolder folder = await FileSystem.Current.LocalStorage.CreateFolderAsync(DATA_FOLDER, CreationCollisionOption.OpenIfExists);

            Uri uri = new Uri(url);
            string fileName = uri.LocalPath.Replace('/', '_');

            if (await folder.CheckExistsAsync(fileName) == ExistenceCheckResult.FileExists) {
                completedAction(String.Format("{0}/{1}", folder.Path, fileName));
            }
            else {
                if (downloadInProgress.Contains(fileName))
                    return; //race problems

                downloadInProgress.Add(fileName); //race problems
                IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                using (Stream fileHandler = await file.OpenAsync(FileAccess.ReadAndWrite)) {
                    var httpResponse = await new HttpClient().GetAsync(uri);
                    byte[] buffer = await httpResponse.Content.ReadAsByteArrayAsync();
                    await fileHandler.WriteAsync(buffer, 0, buffer.Length);
                    completedAction(file.Path);
                }
                downloadInProgress.Remove(fileName); //race problems
            }

        }

    }
}
