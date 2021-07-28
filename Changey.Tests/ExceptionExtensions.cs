using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Changey.Tests
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public static class ExceptionExtensions
	{
		public static Exception SetStackTrace(this Exception target, StackTrace stack) => _setStackTrace(target, stack);

		private static readonly Func<Exception, StackTrace, Exception> _setStackTrace =
			new Func<Func<Exception, StackTrace, Exception>>(() =>
			{
				var target = Expression.Parameter(typeof(Exception));
				var stack = Expression.Parameter(typeof(StackTrace));
				var traceFormatType = typeof(StackTrace).GetNestedType("TraceFormat", BindingFlags.NonPublic);
				if (traceFormatType != null)
				{
					var normalTraceFormat = Enum.GetValues(traceFormatType).GetValue(0);

					var toString = typeof(StackTrace).GetMethod("ToString", BindingFlags.NonPublic | BindingFlags.Instance,
						null, new[] {traceFormatType}, null);
					if (toString != null)
					{
						var stackTraceString =
							Expression.Call(stack, toString, Expression.Constant(normalTraceFormat, traceFormatType));
						var stackTraceStringField =
							typeof(Exception).GetField("_stackTraceString", BindingFlags.NonPublic | BindingFlags.Instance);

						if (stackTraceStringField != null)
						{
							var assign = Expression.Assign(Expression.Field(target, stackTraceStringField), stackTraceString);
							return Expression
								.Lambda<Func<Exception, StackTrace, Exception>>(Expression.Block(assign, target), target, stack)
								.Compile();
						}
					}
				}

				return (exception, _) => exception;
			})();
	}
}