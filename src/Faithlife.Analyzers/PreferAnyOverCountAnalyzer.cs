using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Faithlife.Analyzers
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class PreferAnyOverCountAnalyzer : DiagnosticAnalyzer
	{
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

			context.RegisterCompilationStartAction(compilationStartAnalysisContext =>
			{
				var enumerable = compilationStartAnalysisContext.Compilation.GetTypeByMetadataName("System.Collections.IEnumerable");
				if (enumerable is null)
					return;

				compilationStartAnalysisContext.RegisterSyntaxNodeAction(c => AnalyzeSyntax(c, enumerable), SyntaxKind.GreaterThanOrEqualExpression);
				compilationStartAnalysisContext.RegisterSyntaxNodeAction(c => AnalyzeSyntax(c, enumerable), SyntaxKind.GreaterThanExpression);
				compilationStartAnalysisContext.RegisterSyntaxNodeAction(c => AnalyzeSyntax(c, enumerable), SyntaxKind.LessThanOrEqualExpression);
				compilationStartAnalysisContext.RegisterSyntaxNodeAction(c => AnalyzeSyntax(c, enumerable), SyntaxKind.LessThanExpression);
			});
		}

		private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context, INamedTypeSymbol enumerable)
		{
			var invocation = (InvocationExpressionSyntax) context.Node;

			// context.ReportDiagnostic(Diagnostic.Create(s_rule, memberAccess.Name.GetLocation()));
		}

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(s_rule);

		public const string DiagnosticId = "FL0017";

		private static readonly DiagnosticDescriptor s_rule = new(
			id: DiagnosticId,
			title: "Any() Vs Count() Usage",
			messageFormat: "Count() should not be used for checking for empty enumerables. Use .Any() instead.",
			category: "Usage",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			helpLinkUri: $"https://github.com/Faithlife/FaithlifeAnalyzers/wiki/{DiagnosticId}"
		);
	}
}
