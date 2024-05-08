using Darkengines.Users.Entities;
using System.Threading.Tasks;

namespace Darkengines.Upload {
	public interface IUploadHandler {
		bool CanHandle(User user, string type);
		long MaximumFileSize { get; }
		Task<bool> CanUpload(User identity);
		Task<Uri> ProcessFile(User identity, Stream bytes);
	}
}
