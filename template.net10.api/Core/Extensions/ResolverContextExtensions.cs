using System.Diagnostics.CodeAnalysis;
using HotChocolate.Language;
using HotChocolate.Resolvers;

namespace template.net10.api.Core.Extensions;

/// <summary>
///     Provides extension methods for <see cref="IResolverContext" /> to extract GraphQL field selection information.
/// </summary>
internal static class ResolverContextExtensions
{
    /// <summary>
    ///     Recursively visits a GraphQL field selection node and populates the selected fields tree.
    /// </summary>
    /// <param name="fieldNode">The GraphQL field node to visit.</param>
    /// <param name="tree">The dictionary tree mapping parent paths to their selected field names.</param>
    /// <param name="parentPath">The dot-separated path of the parent field in the selection hierarchy.</param>
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
        ///     Builds a hierarchical tree of selected GraphQL fields from the current resolver context.
        /// </summary>
        /// <returns>A dictionary mapping dot-separated field paths to their selected leaf field names.</returns>
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