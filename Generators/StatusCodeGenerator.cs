namespace Genesis.Generators;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Text;
using System.Text;

class StatusCodeSyntaxReceiver : ISyntaxReceiver {

    public ClassDeclarationSyntax? Candidate { get; private set; }

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
        if(syntaxNode is ClassDeclarationSyntax cds && cds.Identifier.ValueText is "GenesisStatusCodes")
            Candidate = cds;
    }
}

[Generator]
class StatusCodeGenerator : ISourceGenerator {

    PropertyDeclarationSyntax MakeField(string name, int statusCode) {
        var prop = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("StatusCode"), name)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(
                SyntaxKind.GetKeyword,
                (BlockSyntax) SyntaxFactory.ParseStatement($"{{ return new StatusCode({statusCode}, Default[{statusCode}]) }}")
            ));

        return prop;
    }

    public void Execute(GeneratorExecutionContext context) {
        if(context.SyntaxReceiver is StatusCodeSyntaxReceiver r) {
            var fields = typeof(StatusCodes)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .Select(fi => (fi.Name, Value: (int) fi.GetValue(null)!))
            .Distinct();

            foreach(var (Name, Value) in fields)
                r?.Candidate?.AddMembers(MakeField(Name, Value));

            context.AddSource(nameof(StatusCodeGenerator), SourceText.From(r.Candidate.ToFullString(), Encoding.UTF8));


            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("", "", description: r.Candidate.ToFullString())));
        }
    }

    public void Initialize(GeneratorInitializationContext context) {
        context.RegisterForSyntaxNotifications(() => new StatusCodeSyntaxReceiver());
    }
}