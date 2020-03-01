using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyzer.CodeSort.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AllowMembersOrderingCodeFixProvider)), Shared]
    public class AllowMembersOrderingCodeFixProvider : BaseAllowMembersOrderingCodeFixProvider
    {
        public AllowMembersOrderingCodeFixProvider() :
            base("Order {0}'s members following StyleCop patterns")
        { }

        protected override IComparer<MemberDeclarationSyntax> GetMemberDeclarationComparer(Document d, CancellationToken c) =>
            new MembersComparer();

        public class MembersComparer : IComparer<MemberDeclarationSyntax>
        {
            private readonly Dictionary<Type, int> _typeRank = new Dictionary<Type, int>
            {
                { typeof(FieldDeclarationSyntax),       1 },
                { typeof(ConstructorDeclarationSyntax), 2 },
                { typeof(DestructorDeclarationSyntax),  3 },
                { typeof(DelegateDeclarationSyntax),    4 },
                { typeof(EventFieldDeclarationSyntax),  5 },
                { typeof(EventDeclarationSyntax),       6 },
                { typeof(EnumDeclarationSyntax),        7 },
                { typeof(InterfaceDeclarationSyntax),   8 },
                { typeof(PropertyDeclarationSyntax),    9 },
                { typeof(IndexerDeclarationSyntax),     10 },
                { typeof(OperatorDeclarationSyntax),    11 },
                { typeof(MethodDeclarationSyntax),      12 },
                { typeof(StructDeclarationSyntax),      13 },
                { typeof(ClassDeclarationSyntax),       14 },
            };

            private readonly Dictionary<SyntaxKind, int> _specialModifierRank = new Dictionary<SyntaxKind, int>
            {
                { SyntaxKind.ConstKeyword,   1 },
                { SyntaxKind.StaticKeyword,  2 },
            };

            private readonly Dictionary<SyntaxKind, int> _accessLevelRank = new Dictionary<SyntaxKind, int>
            {
                { SyntaxKind.PublicKeyword,     -4 },
                { SyntaxKind.InternalKeyword,   -2 },
                { SyntaxKind.ProtectedKeyword,   1 },
                { SyntaxKind.PrivateKeyword,     2 },
            };

            public int Compare(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return 1;
                if (y == null) return -1;

                var comparedPoints = GetRankPoints(x).CompareTo(GetRankPoints(y));
                if (comparedPoints != 0)
                    return comparedPoints;

                var xModifiers = GetModifiers(x);
                var yModifiers = GetModifiers(y);

                comparedPoints = GetAccessLevelPoints(xModifiers).CompareTo(GetAccessLevelPoints(yModifiers));
                if (comparedPoints != 0)
                    return comparedPoints;

                comparedPoints = GetSpecialModifierPoints(xModifiers).CompareTo(GetSpecialModifierPoints(yModifiers));
                if (comparedPoints != 0)
                    return comparedPoints;

                return comparedPoints != 0 ? comparedPoints : GetName(x).CompareTo(GetName(y));
            }

            private int GetRankPoints(MemberDeclarationSyntax node)
            {
                return _typeRank.TryGetValue(node.GetType(), out var points) ? points : 0;
            }

            private static SyntaxTokenList GetModifiers(MemberDeclarationSyntax node)
            {
                switch (node)
                {
                    case BaseMethodDeclarationSyntax methodSyntax:
                        return methodSyntax.Modifiers;
                    case BaseFieldDeclarationSyntax fieldSyntax:
                        return fieldSyntax.Modifiers;
                    case DelegateDeclarationSyntax delegateSyntax:
                        return delegateSyntax.Modifiers;
                    case BasePropertyDeclarationSyntax propertySyntax:
                        return propertySyntax.Modifiers;
                    default:
                        return default;
                }
            }

            private int GetSpecialModifierPoints(SyntaxTokenList tokenList) => SumRankPoints(tokenList, _specialModifierRank, 100);

            private int GetAccessLevelPoints(SyntaxTokenList tokenList) => SumRankPoints(tokenList, _accessLevelRank, _accessLevelRank[SyntaxKind.PrivateKeyword]);

            private static int SumRankPoints(SyntaxTokenList tokenList, IReadOnlyDictionary<SyntaxKind, int> rank, int defaultSumValue)
            {
                var points = tokenList
                        .Select(s => s.Kind())
                        .Sum(tokenKind => rank.ContainsKey(tokenKind) ? rank[tokenKind] : 0);
                return points == 0 ? defaultSumValue : points;
            }

            private static string GetName(SyntaxNode node)
            {
                switch (node)
                {
                    case FieldDeclarationSyntax fieldDeclarationSyntax:
                        return GetName(fieldDeclarationSyntax.Declaration);
                    case EventFieldDeclarationSyntax eventFieldDeclarationSyntax:
                        return GetName(eventFieldDeclarationSyntax.Declaration);
                    case OperatorDeclarationSyntax operatorDeclarationSyntax:
                        return operatorDeclarationSyntax.OperatorToken.Text;
                    case IndexerDeclarationSyntax _:
                        return "this";
                    case PropertyDeclarationSyntax propertyDeclarationSyntax:
                        return propertyDeclarationSyntax.Identifier.Text;
                    case MethodDeclarationSyntax methodDeclarationSyntax:
                        return methodDeclarationSyntax.Identifier.Text;
                    case ConstructorDeclarationSyntax constructorDeclarationSyntax:
                        return constructorDeclarationSyntax.Identifier.Text;
                    case DestructorDeclarationSyntax destructorDeclarationSyntax:
                        return destructorDeclarationSyntax.Identifier.Text;
                    case DelegateDeclarationSyntax delegateDeclarationSyntax:
                        return delegateDeclarationSyntax.Identifier.Text;
                    case EventDeclarationSyntax eventDeclarationSyntax:
                        return eventDeclarationSyntax.Identifier.Text;
                    case BaseTypeDeclarationSyntax baseTypeDeclarationSyntax:
                        return baseTypeDeclarationSyntax.Identifier.Text;
                    default:
                        return string.Empty;
                }
            }

            private static string GetName(VariableDeclarationSyntax declaration)
            {
                var str = new StringBuilder();
                declaration.Variables.Aggregate(str, (accumulate, seed) => accumulate.Append(seed.Identifier.Text));
                return str.ToString();
            }
        }
    }
}