using Darkengines.Applications;
using Darkengines.Users.Entities;
using Darkengines.Expressions.Security;

namespace Darkengines.Users.Rules {
    public class UserRule : TypeRuleMap<User, IApplicationContext> {
        public UserRule() {
            WithOperation(
                Operation.Read,
                (instance, context) => instance.UserUserGroups.Any(leftUserUserGroup =>
                    leftUserUserGroup.UserGroup.UserUserGroups.Any(rightUserUserGroup =>
                        rightUserUserGroup.UserId == context.CurrentUser.Id
                    )
                )
            );
            Expose(user => user.Id).WithOperation(Operation.Read);
            Expose(user => user.CreatedById).WithOperation(Operation.Read);
            Expose(user => user.IsVerified).WithOperation(Operation.Read);
            Expose(user => user.IsActive).WithOperation(Operation.Read);
            Expose(user => user.CreatedOn).WithOperation(Operation.Read);
            Expose(user => user.ModifiedById).WithOperation(Operation.Read);
            Expose(user => user.ModifiedOn).WithOperation(Operation.Read);
            Expose(user => user.DeactivatedByUserId).WithOperation(Operation.Read);
            Expose(user => user.DeactivatedOn).WithOperation(Operation.Read);
            Expose(user => user.LastIpAddress).WithOperation(Operation.Read);
            Expose(user => user.Login).WithOperation(Operation.Read);
            Expose(user => user.UserEmailAddresses).WithOperation(Operation.Write, (user, context) => user.Id == context.CurrentUser.Id);
            Expose(user => user.UserEmailAddresses).WithOperation(Operation.Write, (user, context) => user.Id == context.CurrentUser.Id);
            Expose(user => user.HashedPassword).WithOperation(Operation.Write, (user, context) => user.Id == context.CurrentUser.Id);
            Expose(user => user.Password).WithOperation(Operation.Write, (user, context) => user.Id == context.CurrentUser.Id);
            //Property(user => user.EmailAddress!).WithOperation(
            //	Operation.ReadWrite,
            //	(instance, context) => instance.Id == context.CurrentUser.Id
            //);
        }
    }
}
