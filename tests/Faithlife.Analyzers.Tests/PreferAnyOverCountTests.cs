using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace Faithlife.Analyzers.Tests
{
	[TestFixture]
	public class PreferAnyOverCountTests : CodeFixVerifier
	{
		[Test]
		public void ValidUsage()
		{
			const string validProgram = c_preamble + @"
namespace TestApplication
{
	internal static class TestClass
	{
		public static void UtilityMethod()
		{
			IEnumerable<string> test = new List<string> { ""test"", ""test2"" };

			if (test.Any())
				return;
		}
	}
}";

			VerifyCSharpDiagnostic(validProgram);
		}

		[Test]
		public void InvalidUsage()
		{
			var brokenProgram = c_preamble + @"
namespace TestApplication
{
	internal static class TestClass
	{
		public static void UtilityMethod()
		{
			IEnumerable<string> test = new List<string> { ""test"", ""test2"" };

			if (test.Count() > 0)
				return;
		}
	}
}";

			var expected = new DiagnosticResult
			{
				Id = PreferAnyOverCountAnalyzer.DiagnosticId,
				Message = "Count() should not be used for checking for empty enumerables. Use .Any() instead.",
				Severity = DiagnosticSeverity.Warning,
				Locations = new[] { new DiagnosticResultLocation("Test0.cs", c_preambleLength + 8, 45) },
			};

			VerifyCSharpDiagnostic(brokenProgram, expected);
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new PreferAnyOverCountAnalyzer();

		private const string c_preamble = @"using System.Collections.Generic;
using System.Linq;";

		private static readonly int c_preambleLength = c_preamble.Split('\n').Length;
	}
}
