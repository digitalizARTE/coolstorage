namespace System.Linq.Expressions
{
    using System.Collections.ObjectModel;
	using System.Linq;

	public static class CSExpressionExtensions
	{
		public static string PropertyName<T> (this Expression<Func<T>> property)
		{
			if (property != null) {
				MemberExpression memberExp;
				if (!TryFindMemberExpression (property.Body, out memberExp))
					return string.Empty;

				var memberNames = new Collection<string> ();
				do {
					memberNames.Add (memberExp.Member.Name);
				} while (TryFindMemberExpression(memberExp.Expression, out memberExp));

				var memberNamesArray = memberNames.Reverse ().ToArray ();

				var result = string.Join (".", memberNamesArray);
				return result;
			}

			return null;
		}

		private static bool TryFindMemberExpression (Expression expression, out MemberExpression memberExpression)
		{
			memberExpression = expression as MemberExpression;
			if (memberExpression != null)
				return true;

			// IsConversion checks for cases where the compiler created an automatic conversion, 
			// obj => Convert(obj.Property) [e.g., int -> object] 
			// OR: 
			// obj => ConvertChecked(obj.Property) [e.g., int -> long] 
			var unaryExpression = expression as UnaryExpression;
			if (IsConversion (expression) && (unaryExpression != null)) {
				memberExpression = unaryExpression.Operand as MemberExpression;
				if (memberExpression != null)
					return true;
			}

			return false;
		}

		private static bool IsConversion (Expression expression)
		{
			return (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked);
		}
	}
}