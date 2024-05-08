using Darkengines.Data;
using Darkengines.Storage;
using Darkengines.Upload;
using Darkengines.Users.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Darkengines.Users {
	public class UserPhotoUploadHandler : IUploadHandler {
		protected ApplicationDbContext ApplicationDbContext { get; }
		protected StorageOptions StorageOptions { get; }

		public UserPhotoUploadHandler(
			ApplicationDbContext applicationDbContext,
			IOptions<StorageOptions> storageOptions
		) {
			ApplicationDbContext = applicationDbContext;
			StorageOptions = storageOptions.Value;
		}
		public long MaximumFileSize => 16777216;

		public bool CanHandle(User user, string type) {
			return type.Equals(typeof(UserPhotoUploadHandler).Name);
		}

		public async Task<bool> CanUpload(User user) {
			return await Task.FromResult(user != null);
		}

		public async Task<Uri> ProcessFile(User user, Stream stream) {
			var fileUri = new Uri(StorageOptions.DirectoryPath, Path.Join(nameof(UserPhotoUploadHandler), user.Id.ToString()));
			var filePath = fileUri.PathAndQuery;
			var directoryPath = Path.GetDirectoryName(filePath);
			Directory.CreateDirectory(directoryPath);
			using var fileStream = new FileStream(fileUri.PathAndQuery, FileMode.Create, FileAccess.Write);
			stream.CopyTo(fileStream);
			var userProfile = await ApplicationDbContext.UserProfiles.FindAsync(user.Id);
			if (userProfile == null) {
				userProfile = new UserProfile() { Id = user.Id };
				user.UserProfile = userProfile;
			}
			userProfile.ImageUri = fileUri;
			await ApplicationDbContext.SaveChangesAsync();
			return fileUri;
		}
	}
}
