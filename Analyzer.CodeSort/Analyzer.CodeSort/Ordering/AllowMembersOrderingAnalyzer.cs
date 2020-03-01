using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzer.CodeSort.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AllowMembersOrderingAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "Member ordering is incorrect in this type.";
        internal const string MessageFormat = "Member ordering is incorrect in this type.";
        internal const string Category = "Ordering";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId.AllowMembersOrdering.ToDiagnosticId(),
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) =>
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var typeDeclarationSyntax = context.Node as TypeDeclarationSyntax;
            if (!CanOrder(typeDeclarationSyntax)) return;
            context.ReportDiagnostic(Diagnostic.Create(Rule, typeDeclarationSyntax.Identifier.GetLocation()));
        }

        private static bool CanOrder(TypeDeclarationSyntax typeDeclarationSyntax) => typeDeclarationSyntax != null && typeDeclarationSyntax.Members.Count > 1;
    }
}