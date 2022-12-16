using Darkengines.Expressions.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Darkengines.Expressions.Mutation {
	public static class MutationExtensions {
		public static Operation ToOperation(this EntityState entityState) {
			switch (entityState) {
				case (EntityState.Unchanged): {
						return Operation.Read;
					}
				case (EntityState.Added): {
						return Operation.Create;
					}
				case (EntityState.Modified): {
						return Operation.Edit;
					}
				case (EntityState.Deleted): {
						return Operation.Delete;
					}
				default: {
						return Operation.None;
					}
			}
		}
	}
}
