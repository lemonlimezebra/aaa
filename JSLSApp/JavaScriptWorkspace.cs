using JSLSApp.LspTypes;

namespace JSLSApp;

public class JavaScriptWorkspace
{
    /// <summary>
    /// CAREFUL: EITHER THIS OR _workspaceFolders
    /// </summary>
    private readonly string? _rootAbsolutePath;
    /// <summary>
    /// CAREFUL: EITHER THIS OR _rootAbsolutePath
    /// </summary>
    private readonly List<WorkspaceFolder>? _workspaceFolders;

    public List<string> SourceFileAbsolutePathList { get; } = new();
    public Dictionary<string, JavaScriptDocument> OpenedSourceFileAbsolutePathToInMemoryContentMap { get; set; } = new();

    public JavaScriptWorkspace(string rootAbsolutePath)
    {
        _rootAbsolutePath = rootAbsolutePath;
        Recursive_FileDiscovery(_rootAbsolutePath);
    }

    public JavaScriptWorkspace(List<WorkspaceFolder>? workspaceFolders)
    {
        _workspaceFolders = workspaceFolders;
        foreach (var workspaceFolder in _workspaceFolders)
        {
            Recursive_FileDiscovery(workspaceFolder.uri);
        }
    }

    public void DidOpenTextDocumentNotification(string myPath, string sourceFileAbsolutePath, string text)
    {
        File.AppendAllText(myPath, $"\n====DidOpenTextDocumentNotification(string sourceFileAbsolutePath)====\n");
        OpenedSourceFileAbsolutePathToInMemoryContentMap.Add(sourceFileAbsolutePath, new JavaScriptDocument(text.ToList()));
    }
    
    public void DidCloseTextDocumentNotification(string myPath, string sourceFileAbsolutePath)
    {
        File.AppendAllText(myPath, $"\n====DidCloseTextDocumentNotification(string sourceFileAbsolutePath)____====\n");
        var wasRemoved = OpenedSourceFileAbsolutePathToInMemoryContentMap.Remove(sourceFileAbsolutePath);
        File.AppendAllText(myPath, $"\n====DidCloseTextDocumentNotification(string sourceFileAbsolutePath)_{wasRemoved}====\n");
    }

    public void DidChangeTextDocumentNotification(string myPath, string sourceFileAbsolutePath, TextDocumentContentChangeEvent[] contentChanges)
    {
        if (OpenedSourceFileAbsolutePathToInMemoryContentMap.TryGetValue(sourceFileAbsolutePath, out var doc))
        {
            if (contentChanges.Length == 0)
            {
                File.AppendAllText(myPath, $"\n====DidChangeTextDocumentNotification; {nameof(contentChanges)} length was 0====\n");
            }
            else
            {
                foreach (var change in contentChanges)
                {
                    if (change.range is null)
                    {
                        File.AppendAllText(myPath, $"\n====DidChangeTextDocumentNotification; TODO: support 'if (change.range is null)'====\n");
                    }
                    else
                    {
                        if (change.range.start.line != change.range.end.line ||
                            change.range.start.character != change.range.end.character)
                        {
                            if (change.text is null)
                            {
                                File.AppendAllText(myPath, $"\n====DidChangeTextDocumentNotification; TODO: support when range.start '!=' range.end AND change.text is null====\n");
                            }
                            else
                            {
                                var startIndexPosition = FindPositionFromLineAndCharacter(myPath, doc.Chars, change.range.start.line, change.range.start.character);
                                var endIndexPosition = FindPositionFromLineAndCharacter(myPath, doc.Chars, change.range.end.line, change.range.end.character);
                                doc.Chars.RemoveRange(startIndexPosition, endIndexPosition - startIndexPosition);
                                File.AppendAllText(myPath, $"\n====DidChangeTextDocumentNotification; successRemove====\n");
                            }
                        }
                        else
                        {
                            var indexPosition = FindPositionFromLineAndCharacter(myPath, doc.Chars, change.range.start.line, change.range.start.character);
                            if (indexPosition == -1)
                            {
                                File.AppendAllText(myPath, $"\n====DidChangeTextDocumentNotification; if (indexPosition == -1)====\n");
                            }
                            else
                            {
                                doc.Chars.InsertRange(indexPosition, change.text);
                                File.AppendAllText(myPath, $"\n====DidChangeTextDocumentNotification; successInsert====\n");
                            }
                        }
                    }
                }
            }
        }
        else
        {
            File.AppendAllText(myPath, $"\n====DidChangeTextDocumentNotification did not find {sourceFileAbsolutePath} in {nameof(OpenedSourceFileAbsolutePathToInMemoryContentMap)}====\n");
        }
    }

    public void Recursive_FileDiscovery(string targetDir)
    {
        foreach (var childFile in Directory.EnumerateFiles(targetDir))
        {
            if (Path.GetExtension(childFile) == ".js" || Path.GetExtension(childFile) == ".cjs")
            {
                SourceFileAbsolutePathList.Add(childFile);
            }
        }

        foreach (var childDir in Directory.EnumerateDirectories(targetDir))
        {
            if (Path.GetFileName(childDir) == "node_modules")
            {
                //
            }
            else if (Path.GetFileName(childDir) == ".git")
            {
                //
            }
            else if (Path.GetFileName(childDir) == ".vscode")
            {
                //
            }
            else if (Path.GetFileName(childDir) == "out")
            {
                //
            }
            else if (Path.GetFileName(childDir) == "bin")
            {
                //
            }
            else if (Path.GetFileName(childDir) == "obj")
            {
                //
            }
            else
            {
                Recursive_FileDiscovery(childDir);
            }
        }
    }

    /// <summary>
    /// Returns the positionIndex if found, otherwise -1.
    /// </summary>
    public int FindPositionFromLineAndCharacter(string myPath, List<char> chars, int indexLine, int indexCharacter)
    {
        // current line index
        var line = 0;
        // current character index amongst a line
        var character = 0;
        if (line == indexLine && character == indexCharacter)
        {
            // TODO: chars.Count == 0; write it in a way that isn't scuffed?
            // TODO: this is actually saying a bug exists when the "position" turns out to be count
            return 0;
        }
        for (var i = 0; i < chars.Count; i++)
        {
            if (line == indexLine && character == indexCharacter)
            {
                return i;
            }
            else if (line > indexLine)
            {
                return -1;
            }

            switch (chars[i])
            {
                case '\r':
                    line++;
                    character = 0;
                    if (i <= chars.Count - 2)
                    {
                        if (chars[i + 1] == '\n')
                        {
                            i++;
                        }
                    }
                    break;
                case '\n':
                    line++;
                    character = 0;
                    break;
                default:
                    character++;
                    break;
            }
        }
        if (line == indexLine && character == indexCharacter)
        {
            // TODO: chars.Count == 0; write it in a way that isn't scuffed?
            // TODO: this is actually saying a bug exists when the "position" turns out to be count
            return chars.Count;
        }
        return -1;
    }
}
