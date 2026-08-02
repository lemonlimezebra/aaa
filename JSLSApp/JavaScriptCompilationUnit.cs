namespace JSLSApp;

public class JavaScriptCompilationUnit
{
    public JavaScriptCompilationUnit(List<FunctionDefinitionSyntax> functionDefinitionStartPositionList)
    {
        FunctionDefinitionStartPositionList = functionDefinitionStartPositionList;
    }

    // TODO: This isn't optimal (and is incorrect given the lack of contextual information) but I want to get a "proof of concept"...
    // ...by getting a list of all the functions and then goto definition-ing one or something like this.
    public List<FunctionDefinitionSyntax> FunctionDefinitionStartPositionList { get; set; }
}
