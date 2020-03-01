using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TestHelper;
using Analyzer.CodeSort.Ordering;

namespace Analyzer.CodeSort.Test
{
    [TestClass]
    public class CodeFixVerifierUnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void NoDiagnosticsExpectedToShowUp()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void DiagnosticAndCodeFixForWrongPropertyOrderTriggered()
        {
            var test = File.ReadAllText(@"UnsortedClass.cs");
            var expected = new DiagnosticResult
            {
                Id = DiagnosticId.AllowMembersOrdering.ToDiagnosticId(),
                Message = "Member ordering is incorrect in this type.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 7, 18)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = File.ReadAllText(@"SortedClass.cs");
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new AllowMembersOrderingCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AllowMembersOrderingAnalyzer();
        }
    }
}
