using System.Diagnostics.CodeAnalysis;
using HotChocolate.Language;
using HotChocolate.Resolvers;

namespace template.net10.api.Core.Extensions;

/// <summary>
///     ADD DOCUMENTATION
/// </summary>
internal static class ResolverContextExtensions
{
    /// <summary>
    ///     ADD DOCUMENTATION
    /// </summary>
    private static void VisitSelection(FieldNode fieldNode, Dictionary<string, HashSet<string>> tree, string parentPath)
    {
        var currentPath = string.IsNullOrEmpty(parentPath)
            ? fieldNode.Name.Value
            : $"{parentPath}.{fieldNode.Name.Value}";

        if (fieldNode.SelectionSet != null)
        {
            foreach (var selection in fieldNode.SelectionSet.Selections.OfType<FieldNode>())
                VisitSelection(selection, tree, currentPath);
        }
        else
        {
            if (!tree.ContainsKey(parentPath))
                tree[parentPath] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            tree[parentPath].Add(fieldNode.Name.Value);
        }
    }

    extension(IResolverContext context)
    {
        /// <summary>
        ///     ADD DOCUMENTATION
        /// </summary>
        [SuppressMessage(
            "ReSharper",
            "ExceptionNotDocumentedOptional",
            Justification =
                "Potential exceptions originate from underlying implementation details and are not part of the method contract.")]
        internal Dictionary<string, HashSet<string>> GetSelectedFieldsTree()
        {
            var result = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            var root = context.Selection.SyntaxNode;

            if (root.SelectionSet is null)
                return result;

            foreach (var selection in root.SelectionSet.Selections.OfType<FieldNode>())
                VisitSelection(selection, result, string.Empty);

            return result;
        }
    }
}