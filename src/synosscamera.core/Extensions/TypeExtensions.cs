using synosscamera.core.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace synosscamera.core.Extensions
{
    /// <summary>
    /// Type extensions
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Load all custom attributes for a given type
        /// </summary>
        /// <typeparam name="T">Type of the attribute which should be loaded</typeparam>
        /// <param name="type">The type which should be tagged with the attribute</param>
        /// <returns>An enumerable of attribute instances</returns>
        public static IEnumerable<T> LoadCustomAttributes<T>(this Type type) where T : Attribute
        {
            IEnumerable<T> attribs = null;

            if (type != null)
            {

                attribs = type.GetCustomAttributes(typeof(T), true).Cast<T>();

            }

            return attribs;
        }

        /// <summary>
        /// Load all custom attributes for a given property
        /// </summary>
        /// <typeparam name="T">Type of the attribute which should be loaded</typeparam>
        /// <param name="propertyInfo">The property which should be tagged with the attribute</param>
        /// <returns>An enumerable of attribute instances</returns>
        public static IEnumerable<T> LoadCustomAttributes<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            IEnumerable<T> attribs = null;

            if (propertyInfo != null)
            {

                attribs = propertyInfo.GetCustomAttributes(typeof(T), true).Cast<T>();

            }

            return attribs;
        }

        /// <summary>
        /// Check if a type has defined attributes
        /// </summary>
        /// <typeparam name="T">Type of the attribute which should be checked</typeparam>
        /// <param name="type">The type which should be checked for attributes</param>
        /// <returns>True if the type contains attributes of the given filter</returns>
        public static bool HasCustomAttribute<T>(this Type type) where T : Attribute
        {
            IEnumerable<T> attribs = null;

            if (type != null)
            {

                attribs = type.GetCustomAttributes(typeof(T), true).Cast<T>();

            }

            return attribs?.Count() > 0;
        }

        /// <summary>
        /// Check if a property has defined attributes
        /// </summary>
        /// <typeparam name="T">Type of the attribute which should be checked</typeparam>
        /// <param name="propertyInfo">The property info which should be checked for attributes</param>
        /// <returns>True if the property contains attributes of the given filter</returns>
        public static bool HasCustomAttribute<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            IEnumerable<T> attribs = null;

            if (propertyInfo != null)
            {

                attribs = propertyInfo.GetCustomAttributes(typeof(T), true).Cast<T>();

            }

            return attribs?.Count() > 0;
        }

        /// <summary>
        /// [ <c>public static object GetDefault(this Type type)</c> ]
        /// <para></para>
        /// Retrieves the default value for a given Type
        /// </summary>
        /// <param name="type">The Type for which to get the default value</param>
        /// <returns>The default value for <paramref name="type"/></returns>
        /// <remarks>
        /// If a null Type, a reference Type, or a System.Void Type is supplied, this method always returns null.  If a value type 
        /// is supplied which is not publicly visible or which contains generic parameters, this method will fail with an 
        /// exception.
        /// </remarks>
        /// <example>
        /// To use this method in its native, non-extension form, make a call like:
        /// <code>
        ///     object Default = DefaultValue.GetDefault(someType);
        /// </code>
        /// To use this method in its Type-extension form, make a call like:
        /// <code>
        ///     object Default = someType.GetDefault();
        /// </code>
        /// </example>
        public static object GetDefault(this Type type)
        {
            // If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null
            if (type == null || !type.IsValueType || type == typeof(void))
                return null;

            // If the supplied Type has generic parameters, its default value cannot be determined
            if (type.ContainsGenericParameters)
                throw new ArgumentException(
                    "{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                    "> contains generic parameters, so the default value cannot be retrieved");

            // If the Type is a primitive type, or if it is another publicly-visible value type (i.e. struct/enum), return a 
            //  default instance of the value type
            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(
                        "{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe Activator.CreateInstance method could not " +
                        "create a default instance of the supplied value type <" + type +
                        "> (Inner Exception message: \"" + e.Message + "\")", e);
                }
            }

            // Fail with exception
            throw new ArgumentException("{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                "> is not a publicly-visible type, so the default value cannot be retrieved");
        }

        /// <summary>
        /// Gets the property path from a property expression
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfoFromExpression<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            if (!(propertyLambda.Body is MemberExpression member))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        /// <summary>
        /// Gets the MaxLengthAttribute value if present otherwise -1
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="data"></param>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static int GetMaxLength<TSource, TProperty>(this TSource data, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var propertyInfo = GetPropertyInfoFromExpression(propertyLambda);
            var maxLengthAttrib = propertyInfo?.GetCustomAttribute<MaxLengthAttribute>(true);

            return maxLengthAttrib?.Length ?? -1;
        }

        /// <summary>
        /// Check if a type is a subclass of a generic type
        /// </summary>
        /// <param name="child">Type to check</param>
        /// <param name="parent">A generic type</param>
        /// <returns></returns>
        public static bool IsSubClassOfGeneric(this Type child, Type parent)
        {
            if (child == parent)
                return false;

            if (child.IsSubclassOf(parent))
                return true;

            var parameters = parent.GetGenericArguments();
            var isParameterLessGeneric = !(parameters != null && parameters.Length > 0 &&
                ((parameters[0].Attributes & TypeAttributes.BeforeFieldInit) == TypeAttributes.BeforeFieldInit));

            while (child != null && child != typeof(object))
            {
                var cur = GetFullTypeDefinition(child);
                if (parent == cur || (isParameterLessGeneric && cur.GetInterfaces().Select(i => GetFullTypeDefinition(i)).Contains(GetFullTypeDefinition(parent))))
                    return true;
                else if (!isParameterLessGeneric)
                    if (GetFullTypeDefinition(parent) == cur && !cur.IsInterface)
                    {
                        if (VerifyGenericArguments(GetFullTypeDefinition(parent), cur) && VerifyGenericArguments(parent, child))
                            return true;
                    }
                    else
                        foreach (var item in child.GetInterfaces().Where(i => GetFullTypeDefinition(parent) == GetFullTypeDefinition(i)))
                            if (VerifyGenericArguments(parent, item))
                                return true;

                child = child.BaseType;
            }

            return false;
        }
        /// <summary>
        /// Get non open matching method
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public static MethodInfo GetNonOpenMatchingMethod(
            this Type type,
            string name,
            Type[] parameterTypes)
        {
            type.CheckArgumentNull(nameof(type));
            name.CheckArgumentNull(nameof(name));

            parameterTypes ??= new Type[0];

            var methodCandidates = new List<MethodInfo>(type.GetRuntimeMethods());

            if (type.GetTypeInfo().IsInterface)
            {
                methodCandidates.AddRange(type.GetTypeInfo()
                    .ImplementedInterfaces.SelectMany(x => x.GetRuntimeMethods()));
            }

            foreach (var methodCandidate in methodCandidates)
            {
                if (!methodCandidate.GetNormalizedName().Equals(name, StringComparison.Ordinal))
                {
                    continue;
                }

                var parameters = methodCandidate.GetParameters();
                if (parameters.Length != parameterTypes.Length)
                {
                    continue;
                }

                var parameterTypesMatched = true;

                var genericArguments = methodCandidate.ContainsGenericParameters
                    ? new Type[methodCandidate.GetGenericArguments().Length]
                    : null;

                // Determining whether we can use this method candidate with
                // current parameter types.
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameterType = parameters[i].ParameterType.GetTypeInfo();
                    var actualType = parameterTypes[i].GetTypeInfo();

                    if (!TypesMatchRecursive(parameterType, actualType, genericArguments))
                    {
                        parameterTypesMatched = false;
                        break;
                    }
                }

                if (parameterTypesMatched)
                {
                    if (genericArguments != null)
                    {
                        var genericArgumentsResolved = true;

                        foreach (var genericArgument in genericArguments)
                        {
                            if (genericArgument == null)
                            {
                                genericArgumentsResolved = false;
                            }
                        }

                        if (genericArgumentsResolved)
                        {
                            return methodCandidate.MakeGenericMethod(genericArguments);
                        }
                    }
                    else
                    {
                        // Return first found method candidate with matching parameters.
                        return methodCandidate;
                    }
                }
            }

            return null;
        }

        private static bool TypesMatchRecursive(TypeInfo parameterType, TypeInfo actualType, IList<Type> genericArguments)
        {
            if (parameterType.IsGenericParameter)
            {
                var position = parameterType.GenericParameterPosition;

                // Return false if this generic parameter has been identified and it's not the same as actual type
                if (genericArguments[position] != null && genericArguments[position].GetTypeInfo() != actualType)
                {
                    return false;
                }

                genericArguments[position] = actualType.AsType();
                return true;
            }

            if (parameterType.ContainsGenericParameters)
            {
                if (parameterType.IsArray)
                {
                    // Return false if parameterType is array whereas actualType isn't
                    if (!actualType.IsArray) return false;

                    var parameterElementType = parameterType.GetElementType();
                    var actualElementType = actualType.GetElementType();

                    return TypesMatchRecursive(parameterElementType.GetTypeInfo(), actualElementType.GetTypeInfo(), genericArguments);
                }

                if (!actualType.IsGenericType || parameterType.GetGenericTypeDefinition() != actualType.GetGenericTypeDefinition())
                {
                    return false;
                }

                for (var i = 0; i < parameterType.GenericTypeArguments.Length; i++)
                {
                    var parameterGenericArgument = parameterType.GenericTypeArguments[i];
                    var actualGenericArgument = actualType.GenericTypeArguments[i];

                    if (!TypesMatchRecursive(parameterGenericArgument.GetTypeInfo(), actualGenericArgument.GetTypeInfo(), genericArguments))
                    {
                        return false;
                    }
                }

                return true;
            }

            return parameterType == actualType;
        }

        private static Type GetFullTypeDefinition(Type type)
        {
            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }

        private static bool VerifyGenericArguments(Type parent, Type child)
        {
            Type[] childArguments = child.GetGenericArguments();
            Type[] parentArguments = parent.GetGenericArguments();
            if (childArguments.Length == parentArguments.Length)
                for (int i = 0; i < childArguments.Length; i++)
                    if ((childArguments[i].Assembly != parentArguments[i].Assembly || childArguments[i].Name != parentArguments[i].Name || childArguments[i].Namespace != parentArguments[i].Namespace) &&
                        (!childArguments[i].IsSubclassOf(parentArguments[i])))
                        return false;

            return true;
        }
    }
}
