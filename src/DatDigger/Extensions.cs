using System;
using System.Collections.Generic;

namespace DatDigger {
    public static class Extensions
    {
        /// <summary>Retrieve the first parent node of the given type (recursive)</summary>
        /// <typeparam name="T">The desired parent type</typeparam>
        /// <param name="_this">The base node</param>
        /// <returns>The first parent that has the given type, or null if no parents match the desired type</returns>
        public static T GetParent<T>(this INavigable _this) where T : class
        {
            INavigable parent = _this.Parent;
            while (parent != null)
            {
                if (parent is T)
                {
                    return parent as T;
                }

                parent = parent.Parent;
            }
            return null;
        }

        /// <summary>Retrieve the Nth child of a given type</summary>
        /// <typeparam name="T">The desired node type</typeparam>
        /// <param name="_this">The parent node</param>
        /// <param name="idx">Zero-based index, 0 = first child, 1 = second child</param>
        /// <returns>The Nth child of a given type</returns>
        public static T GetChildOfType<T>(this INavigable _this, int idx = 0) where T : class
        {
            if (_this.Children == null)
            {
                return null;
            }

            int i = 0;
            foreach (INavigable child in _this.Children)
            {
                if (child is T)
                {
                    if (i == idx)
                    {
                        return child as T;
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            return null;
        }

        /// <summary>Find all children of a given type</summary>
        /// <typeparam name="T">The desired type</typeparam>
        /// <param name="_this">The parent node</param>
        /// <returns>A list of all children of the given type</returns>
        public static List<T> GetChildrenOfType<T>(this INavigable _this) where T : class
        {
            List<T> results = new List<T>();
            if (_this.Children == null)
            {
                return results;
            }

            foreach (INavigable child in _this.Children)
            {
                if (child is T)
                {
                    results.Add((T)child);
                }
            }

            return results;
        }

        /// <summary>Find the first child of a given type that satisfies a given predicate (non-recursive)</summary>
        /// <typeparam name="T">The desired child node type</typeparam>
        /// <param name="_this">The parent node</param>
        /// <param name="predicate">The predicate that must be satisfied</param>
        /// <returns>The first child of the given type that satisfies the given predicate</returns>
        public static T GetChild<T>(this INavigable _this, Predicate<T> predicate) where T : class
        {
            if (_this.Children == null)
            {
                return null;
            }

            foreach (INavigable child in _this.Children)
            {
                if (child is T)
                {
                    if (predicate((T)child))
                    {
                        return child as T;
                    }
                }
            }

            return null;
        }

        /// <summary>Find all children nodes of a given type that satisfy a given predicate (non-recursive)</summary>
        /// <typeparam name="T">The desired child node type</typeparam>
        /// <param name="_this">The parent node</param>
        /// <param name="predicate">The predicate that must be met</param>
        /// <returns>A list of all children nodes of the given type that satisfy the given predicate</returns>
        public static List<T> GetChildren<T>(this INavigable _this, Predicate<T> predicate) where T : class
        {
            List<T> results = new List<T>();
            if (_this.Children == null)
            {
                return results;
            }

            foreach (INavigable child in _this.Children)
            {
                if (child is T)
                {
                    if (predicate((T)child))
                    {
                        results.Add((T)child);
                    }
                }
            }

            return results;
        }

        /// <summary>Find the first child node of the given type (recursive, depth first search)</summary>
        /// <typeparam name="T">The desired child node type</typeparam>
        /// <param name="node">The parent node</param>
        /// <returns>The first child node of the given type</returns>
        public static T FindChild<T>(this INavigable node) where T : class
        {
            if (node is T) { return node as T; }
            if (node.Children == null) { return null; }
            foreach (var child in node.Children)
            {
                var childResult = FindChild<T>(child);
                if (childResult != null)
                {
                    return childResult;
                }
            }
            return null;
        }

        /// <summary>Find the first child node of the given type that satisfies the given predicate (recursive, depth first search)</summary>
        /// <typeparam name="T">The desired child node type</typeparam>
        /// <param name="node">The parent node</param>
        /// <param name="predicate">Predicate that must be satisfied</param>
        /// <returns>The first child node that satisfies the given predicate</returns>
        public static T FindChild<T>(this INavigable node, Predicate<T> predicate) where T : class
        {
            if ((node is T) && (predicate((T)node))) { return node as T; }
            if (node.Children == null) { return null; }
            foreach (var child in node.Children)
            {
                var childResult = FindChild<T>(child, predicate);
                if (childResult != null)
                {
                    return childResult;
                }
            }
            return null;
        }

        /// <summary>Retrieve all children nodes of the given node that match the specified type</summary>
        /// <typeparam name="T">The type we wish to find</typeparam>
        /// <param name="node">The root node of the subtree we are searching</param>
        /// <returns>A list containing all nodes matching the criteria</returns>
        public static List<T> FindAllChildren<T>(this INavigable node) where T : class
        {
            List<T> results = new List<T>();
            FindAllChildrenHelper<T>(node, results);
            return results;
        }

        /// <summary>Recursive function to implement FindAllChildren{T}</summary>
        private static void FindAllChildrenHelper<T>(INavigable node, List<T> results) where T : class
        {
            if (node is T) { results.Add(node as T); }
            if (node.Children == null) { return; }
            node.Children.ForEach(x => FindAllChildrenHelper<T>(x, results));
        }
    }
}