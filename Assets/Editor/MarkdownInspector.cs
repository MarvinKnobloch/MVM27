using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A simple class to render a markdown in the inspector.
/// This does not support all markdown, and its performance... well....
/// Best not to think about that.
/// </summary>
[CustomEditor(typeof(TextAsset), true)]
public class MarkdownInspector : Editor
{
    private string markdownText;

    public override VisualElement CreateInspectorGUI()
    {
        string path = AssetDatabase.GetAssetPath(target);
        if (!path.EndsWith(".md"))
        {
            return new VisualElement();
        }

        TextAsset mdFile = (TextAsset)target;
        markdownText = mdFile?.text ?? "No content found.";

        VisualElement root = new VisualElement();
        root.style.flexGrow = 1;

        ScrollView scrollView = new ScrollView();
        root.Add(scrollView);

        Label markdownLabel = new Label(RenderMarkdown(markdownText));
        markdownLabel.style.whiteSpace = WhiteSpace.Normal; // Wrap text
        markdownLabel.style.unityTextAlign = TextAnchor.UpperLeft;
        markdownLabel.enableRichText = true; // Enable Unity rich text tags
        scrollView.Add(markdownLabel);

        return root;
    }

    private string RenderMarkdown(string input)
    {
        // I definitley snatched this regex from online

        string output = input;
        output = System.Text.RegularExpressions.Regex.Replace(output, @"^#### (.+)$", "<b><size=12>$1</size></b>", System.Text.RegularExpressions.RegexOptions.Multiline); // H1
        output = System.Text.RegularExpressions.Regex.Replace(output, @"^### (.+)$", "<b><size=14>$1</size></b>", System.Text.RegularExpressions.RegexOptions.Multiline); // H2
        output = System.Text.RegularExpressions.Regex.Replace(output, @"^## (.+)$", "<b><size=16>$1</size></b>", System.Text.RegularExpressions.RegexOptions.Multiline); // H3
        output = System.Text.RegularExpressions.Regex.Replace(output, @"^# (.+)$", "<b><size=18>$1</size></b>", System.Text.RegularExpressions.RegexOptions.Multiline); // H4
        output = System.Text.RegularExpressions.Regex.Replace(output, @"\*\*(.*?)\*\*", "<b>$1</b>"); // Bold
        output = System.Text.RegularExpressions.Regex.Replace(output, @"\*(.*?)\*", "<i>$1</i>"); // Italic
        output = System.Text.RegularExpressions.Regex.Replace(output, @"^\s*$", "\n"); // Paragraph breaks
        output = System.Text.RegularExpressions.Regex.Replace(output, @"^  [-*+] (.+)$", "  • $1", System.Text.RegularExpressions.RegexOptions.Multiline); // List Level 2
        output = System.Text.RegularExpressions.Regex.Replace(output, @"^[-*+] (.+)$", "• $1", System.Text.RegularExpressions.RegexOptions.Multiline); // List Level 1
        return output;
    }
}