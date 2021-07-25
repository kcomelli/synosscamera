using Microsoft.Extensions.DependencyModel;
using synosscamera.core.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace synosscamera.core
{
    /// <summary>
    /// Class for caching type resolvals
    /// </summary>
    public static class TypeCacheUtility
    {
        private static MethodInfo _genericGetInstanceReadWriteableProperties;
        private static Type[] _allTypes;
        private static ConcurrentDictionary<Type, List<Type>> _resolvedTypesPerBaseClass = new ConcurrentDictionary<Type, List<Type>>();
        private static ConcurrentDictionary<Type, List<PropertyInfo>> _typeProperties = new ConcurrentDictionary<Type, List<PropertyInfo>>();
        private static ConcurrentDictionary<Type, List<Type>> _resolvedTypesWithAttributes = new ConcurrentDictionary<Type, List<Type>>();
        private static object _lockObject = new object();
        private static List<string> _excludeList = new List<string>()
        {
            "Microsoft.AspNetCore.Mvc.Razor"
        };

        private static void EnsureTypesLoaded()
        {
            lock (_lockObject)
            {
                if (_allTypes == null)
                {
                    var entryAssembly = Assembly.GetEntryAssembly();

                    IEnumerable<Assembly> assembliesToScan = DependencyContext.Default.RuntimeLibraries
                            .SelectMany(lib => lib.GetDefaultAssemblyNames(DependencyContext.Default).Select(Assembly.Load));

                    // load referenced assemblies from the same product or company as the 
                    foreach (var loadedAssembly in assembliesToScan.ToList())
                        assembliesToScan = assembliesToScan.Union(loadedAssembly.GetReferencedAssemblies().Where(a => SourceCheck(a.Name, entryAssembly.FullName)).Select(Assembly.Load));

                    _allTypes = assembliesToScan.Distinct().Where(a => !_excludeList.Any(s => a.FullName.Contains(s))).SelectMany(a => a.ExportedTypes).ToArray();
                }
            }
        }

        private static bool SourceCheck(string nameToCheck, string entryAssemblyFullName)
        {
            if (nameToCheck.IsPresent() && entryAssemblyFullName.IsPresent())
            {
                var compareName = nameToCheck;
                if (nameToCheck.Contains("."))
                    compareName = nameToCheck.Substring(0, nameToCheck.IndexOf("."));

                // check if the entry assembly starts with the same word as the name to check
                // Logicx.lib will match Logicx.service.a
                return entryAssemblyFullName.StartsWith(compareName, StringComparison.OrdinalIgnoreCase);

            }
            return false;
        }

        private static void ResolveInheritedTypes<TBase>()
        {
            EnsureTypesLoaded();

            _resolvedTypesPerBaseClass[typeof(TBase)] =
                _allTypes.Where(t => typeof(TBase).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
                    .Where(t => !t.GetTypeInfo().IsAbstract)
                    .ToList();
        }

        private static void ResolveTypesWithAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            EnsureTypesLoaded();

            _resolvedTypesWithAttributes[typeof(TAttribute)] =
                _allTypes.Where(t => t.HasCustomAttribute<TAttribute>())
                    .Where(t => !t.GetTypeInfo().IsAbstract)
                    .ToList();
        }

        private static void LoadProperties<T>()
        {
            EnsureTypesLoaded();


            _typeProperties[typeof(T)] = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToList();

        }

        private static void ClearCache()
        {
            _resolvedTypesPerBaseClass.Clear();
            _typeProperties.Clear();
            _resolvedTypesWithAttributes.Clear();
        }

        /// <summary>
        /// Get all types which inherit from a given base type
        /// </summary>
        /// <typeparam name="TBase">Type of the base class</typeparam>
        /// <returns>A list of types (including the base class type) which are inheriting from the base class.</returns>
        public static List<Type> GetInheritedTypes<TBase>()
        {
            try
            {
                if (!_resolvedTypesPerBaseClass.ContainsKey(typeof(TBase)))
                {
                    ResolveInheritedTypes<TBase>();
                }
            }
            catch
            {
                ClearCache();
                ResolveInheritedTypes<TBase>();
            }

            return _resolvedTypesPerBaseClass[typeof(TBase)];
        }

        /// <summary>
        /// Get all types which are marked with a given custom attribute
        /// </summary>
        /// <typeparam name="TAttribute">Attribute to search for</typeparam>
        /// <returns>A list of types which are implementing this attribute</returns>
        public static List<Type> GetAllTypesWithAttribute<TAttribute>() where TAttribute : Attribute
        {
            try
            {
                if (!_resolvedTypesWithAttributes.ContainsKey(typeof(TAttribute)))
                {
                    ResolveTypesWithAttribute<TAttribute>();
                }
            }
            catch
            {
                ClearCache();
                ResolveTypesWithAttribute<TAttribute>();
            }

            return _resolvedTypesWithAttributes[typeof(TAttribute)];
        }

        /// <summary>
        /// Get all instance read/write able properties
        /// </summary>
        /// <typeparam name="T">Type which properties should be loaded</typeparam>
        /// <returns>A list containing <see cref="PropertyInfo"/> instances for each property which can be read or written</returns>
        public static List<PropertyInfo> GetInstanceReadWriteableProperties<T>()
        {
            try
            {
                if (!_typeProperties.ContainsKey(typeof(T)))
                {
                    LoadProperties<T>();
                }
            }
            catch
            {
                ClearCache();
                LoadProperties<T>();
            }

            return _typeProperties[typeof(T)].Where(pi => pi.CanRead && pi.CanWrite && !pi.IsSpecialName).ToList();
        }

        /// <summary>
        /// Get all instance read/write able properties
        /// </summary>
        /// <param name="type">>Type which properties should be loaded</param>
        /// <returns>A list containing <see cref="PropertyInfo"/> instances for each property which can be read or written</returns>
        public static List<PropertyInfo> GetInstanceReadWriteableProperties(Type type)
        {
            if (_genericGetInstanceReadWriteableProperties == null)
            {
                _genericGetInstanceReadWriteableProperties = typeof(TypeCacheUtility).GetMethods()
                    .FirstOrDefault(m => m.Name == "GetInstanceReadWriteableProperties" && m.IsGenericMethodDefinition);

            }

            var method = _genericGetInstanceReadWriteableProperties.MakeGenericMethod(type);
            return (List<PropertyInfo>)method.Invoke(null, null);
        }
    }
}
