using JSLSApp.LspTypes;

namespace JSLSApp;

public class JavaScriptDocument
{
    public JavaScriptDocument(List<char> chars)
    {
        Chars = chars;
    }

    public List<char> Chars { get; }
    public bool HasBeenParsedAtLeastOnce { get; set; }
    public JavaScriptCompilationUnit CompilationUnit { get; set; } = new JavaScriptCompilationUnit(new(), new());
}
