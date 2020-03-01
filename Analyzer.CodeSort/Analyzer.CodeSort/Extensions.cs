using Analyzer.CodeSort.Ordering;

namespace Analyzer.CodeSort
{
    public static class Extensions
    {
        public static string ToDiagnosticId(this DiagnosticId diagnosticId) => $"MO{(int)diagnosticId:D4}";
    }
}