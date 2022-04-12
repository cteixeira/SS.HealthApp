using Newtonsoft.Json;
using PCLStorage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SS.HealthApp.PCL.Repositories
{
    abstract class BaseRepository<T> where T : new()
    {

        protected const string DATA_FOLDER = "data";

        protected abstract string GetRepositoryFilename();

        public virtual async Task SaveContentAsync(T Content)
        {
            IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(DATA_FOLDER, CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync(GetRepositoryFilename(), CreationCollisionOption.ReplaceExisting);
            string contentJson = JsonConvert.SerializeObject(Content);
            await file.WriteAllTextAsync(contentJson);
        }

        public virtual async Task<T> GetContentAsync()
        {
            string fileName = GetRepositoryFilename();
            string contentJson = string.Empty; //returns null if the file doesnt exist

            IFolder folder = await FileSystem.Current.LocalStorage.CreateFolderAsync(DATA_FOLDER, CreationCollisionOption.OpenIfExists);

            if (await folder.CheckExistsAsync(fileName) == ExistenceCheckResult.FileExists) {
                IFile file = await folder.GetFileAsync(fileName);
                contentJson = await file.ReadAllTextAsync();
            }

            return String.IsNullOrEmpty(contentJson) ? new T() : JsonConvert.DeserializeObject<T>(contentJson);
        }

        public virtual async Task<string> SaveContentAsync(byte[] content, string fileName, string saveFilePath = null) {

            IFolder folder = null;

            if (String.IsNullOrEmpty(saveFilePath))
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                folder = await rootFolder.CreateFolderAsync(DATA_FOLDER, CreationCollisionOption.OpenIfExists);
            }
            else
            {
                folder = await FileSystem.Current.GetFolderFromPathAsync(saveFilePath);
            }
            
            IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (Stream fileHandler = await file.OpenAsync(FileAccess.ReadAndWrite)) {
                await fileHandler.WriteAsync(content, 0, content.Length);
            }
            return file.Path;
        }
    }

}

