using PCLStorage;
using System.Threading.Tasks;

namespace SS.HealthApp.PCL.Utils {

    //https://github.com/dsplaisted/PCLStorage

    class PCLStorage {

        private const string DATA_FOLDER = "data";

        internal static async void SaveContentAsync(string fileName, string content) {
            IFolder folder = await FileSystem.Current.LocalStorage.CreateFolderAsync(DATA_FOLDER, CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(content);
        }

        internal static async Task<string> GetContentAsync(string fileName) {
            IFolder folder = await FileSystem.Current.LocalStorage.CreateFolderAsync(DATA_FOLDER, CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            return await file.ReadAllTextAsync();
        }

    }
}
