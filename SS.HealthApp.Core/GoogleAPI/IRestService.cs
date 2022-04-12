using System.Threading.Tasks;

namespace SS.HealthApp.Core.GoogleAPI {
	public interface IRestService
	{
		Task<int> GetDurationAsync(string Origin, string destination);

        Task<int> GetDistanceAsync(string Origin, string destination);
    }
}
