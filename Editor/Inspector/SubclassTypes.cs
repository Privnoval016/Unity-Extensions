using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Extensions.EditorTools
{
    /** <summary>The concrete types a <c>[SerializeReference]</c> field of a given type can hold.</summary> */
    public static class SubclassTypes
    {
        #region Public API

        /**
         * <summary>Every type that can be created for a field of the base type: not abstract, not a Unity object, with a
         * parameterless constructor. Generic types are closed over the field's type arguments
         * (<c>AndCondition&lt;T&gt;</c> for an <c>ICondition&lt;T&gt;</c> field). Sorted by name.</summary>
         */
        public static IReadOnlyList<Type> For(Type baseType)
        {
            if (baseType == null) return Array.Empty<Type>();
            return TypeCache.GetTypesDerivedFrom(baseType).Append(baseType)
                .Concat(ClosedGenerics(baseType))
                .Where(IsCreatable)
                .Distinct()
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToList();
        }

        /** <summary>Reads a field type from Unity's "Assembly Namespace.Type" form; null when it can't be found.</summary> */
        public static Type FromManagedReferenceName(string typename)
        {
            if (string.IsNullOrEmpty(typename)) return null;
            var split = typename.IndexOf(' ');
            if (split < 0) return null;
            return Type.GetType($"{typename.Substring(split + 1).Replace('/', '+')}, {typename.Substring(0, split)}");
        }

        /** <summary>A readable label for a type: its name with spaces before capitals.</summary> */
        public static string Label(Type type)
        {
            if (type == null) return NoneLabel;
            var name = type.Name;
            var tick = name.IndexOf('`');
            return ObjectNames.NicifyVariableName(tick < 0 ? name : name.Substring(0, tick));
        }

        /** <summary>The label for no type.</summary> */
        public const string NoneLabel = "None";

        #endregion

        #region Helpers

        private static IEnumerable<Type> ClosedGenerics(Type baseType)
        {
            if (!baseType.IsGenericType) yield break;
            var arguments = baseType.GetGenericArguments();
            foreach (var open in TypeCache.GetTypesDerivedFrom(baseType.GetGenericTypeDefinition()))
            {
                if (!open.IsGenericTypeDefinition || open.GetGenericArguments().Length != arguments.Length) continue;
                Type closed;
                try
                {
                    closed = open.MakeGenericType(arguments);
                }
                catch (ArgumentException)
                {
                    continue;
                }
                if (baseType.IsAssignableFrom(closed)) yield return closed;
            }
        }

        private static bool IsCreatable(Type type) =>
            !type.IsAbstract && !type.IsInterface && !type.ContainsGenericParameters && !typeof(Object).IsAssignableFrom(type)
            && (type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null);

        #endregion
    }
}
