using Darkengines.Data;
using Darkengines.Users.Entities;
using Darkengines.Users.Rules;
using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Darkengines.Users.Interceptors;
using Darkengines.Expressions.Mutation;
using Darkengines.Upload;

namespace Darkengines.Users {
	public static class Extensions {
		public static IServiceCollection AddUsers(this IServiceCollection serviceCollection) {
			return serviceCollection
				.AddScoped<IUploadHandler, UserPhotoUploadHandler>()
				.AddSingleton<IRuleMap, UserRule>()
				.AddSingleton<IRuleMap, UserProfileRule>()
				.AddSingleton<IRuleMap, UserEmailAddressRuleMap>()
				.AddScoped<IInterceptor, UserSaveChangesInterceptor>();
			//.AddScoped<IMutationInterceptor, UserMutationInterceptor>();
		}
		public static ModelBuilder ConfigureUsers(this ModelBuilder modelBuilder) {
			var userEntityTypeBuilder = modelBuilder.Entity<User>();
			userEntityTypeBuilder.HasKey(user => user.Id);
			userEntityTypeBuilder.AsMonitored();
			userEntityTypeBuilder.AsActiveStateOwner();
			userEntityTypeBuilder.Ignore(user => user.Password);
			userEntityTypeBuilder.Ignore(user => user.IsVerified);
			userEntityTypeBuilder.Property(user => user.Login).HasMaxLength(256).IsRequired();
			userEntityTypeBuilder.HasIndex(user => user.Login).IsUnique();
			userEntityTypeBuilder.Property(user => user.HashedPassword).IsRequired();
			userEntityTypeBuilder.Property(user => user.LastIpAddress);
			userEntityTypeBuilder.HasOne(user => user.UserProfile).WithOne(userProfile => userProfile.User).HasForeignKey<UserProfile>(userProfile => userProfile.Id);
			userEntityTypeBuilder.HasOne(user => user.UserSettings).WithOne(userSettings => userSettings.User).HasForeignKey<UserSettings>(userSettings => userSettings.Id);
			userEntityTypeBuilder.HasMany(user => user.UserEmailAddresses).WithOne(userEmailAddress => userEmailAddress.User).HasForeignKey(userEmailAddress => userEmailAddress.UserId);
			userEntityTypeBuilder.HasMany(user => user.UserPasswordResetRequests).WithOne(userPasswordResetRequest => userPasswordResetRequest.User).HasForeignKey(userPasswordResetRequest => userPasswordResetRequest.UserId);

			var userEmailAddressTypeEntityBuilder = modelBuilder.Entity<UserEmailAddress>();
			userEmailAddressTypeEntityBuilder.AsMonitored();
			userEmailAddressTypeEntityBuilder.HasKey(userEmailAddress => userEmailAddress.HashedEmailAddress);
			userEmailAddressTypeEntityBuilder.Property(user => user.EmailAddress).HasMaxLength(256);
			userEmailAddressTypeEntityBuilder.Property(user => user.HashedEmailAddress).IsRequired();
			userEmailAddressTypeEntityBuilder.Property(user => user.GuidExpirationDate).HasDefaultValue(null);
			userEmailAddressTypeEntityBuilder.Property(user => user.IsVerified).HasDefaultValue(false);
			userEmailAddressTypeEntityBuilder.HasOne(userProfile => userProfile.User).WithMany(user => user.UserEmailAddresses).HasForeignKey(userEmailAddress => userEmailAddress.UserId);
			userEmailAddressTypeEntityBuilder.Property(userEmailAddress => userEmailAddress.Guid).IsRequired().HasValueGenerator<GuidValueGenerator>();

			var userProfileTypeBuilder = modelBuilder.Entity<UserProfile>();
			userProfileTypeBuilder.AsMonitored();
			userProfileTypeBuilder.HasKey(userProfile => userProfile.Id);
			userProfileTypeBuilder.HasOne(userProfile => userProfile.User).WithOne(user => user.UserProfile).HasForeignKey<UserProfile>(userProfile => userProfile.Id); ;
			userProfileTypeBuilder.Property(userProfile => userProfile.DisplayName).HasMaxLength(256).IsRequired();
			userProfileTypeBuilder.Property(userProfile => userProfile.Firstname).HasMaxLength(256).IsRequired();
			userProfileTypeBuilder.Property(userProfile => userProfile.Lastname).HasMaxLength(256).IsRequired();
			userProfileTypeBuilder.Property(userProfile => userProfile.Gender);
			userProfileTypeBuilder.Property(userProfile => userProfile.ImageUri).HasMaxLength(512);


			var userSettingsTypeBuilder = modelBuilder.Entity<UserSettings>();
			userSettingsTypeBuilder.AsMonitored();
			userSettingsTypeBuilder.HasKey(userSettings => userSettings.Id);
			userSettingsTypeBuilder.HasOne(userSettings => userSettings.User).WithOne(user => user.UserSettings).HasForeignKey<UserSettings>(userSettings => userSettings.Id); ;

			return modelBuilder;
		}
	}
}
