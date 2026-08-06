namespace JSLSApp;

public enum SyntaxKind
{
    None,
    IdentifierToken,
    NumberToken,
    EndOfFileToken,
    FunctionKeywordToken,
    ClassKeywordToken,
    WhitespaceToken,
    StringToken,
    SingleLineCommentToken,
    MultiLineCommentToken,
    ClassDeclarationNode,
    //GlobalNode,
    FunctionDeclarationNode,
}
