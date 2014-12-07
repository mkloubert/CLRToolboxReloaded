// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE45)
#define GET_ASSEMBLY_OF_TYPE_FROM_PROPERTY
#define GET_TYPE_ATTRIBUTES_FROM_METHODS
#endif

#if (PORTABLE45)
#define GET_ATTRIBUTES_OF_MEMBER_FROM_PROPERTY
#define GET_MEMBERS_FROM_EXTENSION_METHODS
#define GET_TYPES_OF_ASSEMBLY_FROM_PROPERTY
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Helpers
{
    /// <summary>
    /// Helper class for reflection operations.
    /// </summary>
    public static class ReflectionHelper
    {
        #region Methods (5)

        /// <summary>
        /// Returns an <see cref="Assembly" /> from a <see cref="Type" />.
        /// </summary>
        /// <param name="type">The type from where to get the assembly from.</param>
        /// <returns>The assembly.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type" /> is <see langword="null" />.
        /// </exception>
        public static Assembly GetAssembly(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

#if GET_ASSEMBLY_OF_TYPE_FROM_PROPERTY
            return type.Assembly;
#else
            return Assembly.Load(new AssemblyName(type.AssemblyQualifiedName));
#endif
        }

        /// <summary>
        /// Returns attributes of a specific member.
        /// </summary>
        /// <typeparam name="TAttrib">The type of the attributes.</typeparam>
        /// <param name="member">The member from where to get the attributes from.</param>
        /// <param name="inherit"><see langword="true" /> to search this member's inheritance chain to find the attributes; otherwise, <see langword="false" />.</param>
        /// <returns>The found attributes.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<TAttrib> GetCustomAttributes<TAttrib>(MemberInfo member, bool inherit = false)
            where TAttrib : global::System.Attribute
        {
            return GetCustomAttributes(member: member,
                                       attributeType: typeof(TAttrib),
                                       inherit: inherit).OfType<TAttrib>();
        }

        /// <summary>
        /// Returns attributes of a specific member.
        /// </summary>
        /// <param name="member">The member from where to get the attributes from.</param>
        /// <param name="attributeType">The type of the attributes.</param>
        /// <param name="inherit"><see langword="true" /> to search this member's inheritance chain to find the attributes; otherwise, <see langword="false" />.</param>
        /// <returns>The found attributes.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member" /> and/or <paramref name="attributeType" /> are <see langword="null" />.
        /// </exception>
        public static IEnumerable<object> GetCustomAttributes(MemberInfo member, Type attributeType, bool inherit = false)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }

            return member.GetCustomAttributes(attributeType: attributeType,
                                              inherit: inherit);
        }

        /// <summary>
        /// Returns attributes of a specific type.
        /// </summary>
        /// <typeparam name="TAttrib">The type of the attributes.</typeparam>
        /// <param name="type">The type from where to get the attributes from.</param>
        /// <param name="inherit"><see langword="true" /> to search this member's inheritance chain to find the attributes; otherwise, <see langword="false" />.</param>
        /// <returns>The found attributes.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<TAttrib> GetCustomAttributes<TAttrib>(Type type, bool inherit = false)
            where TAttrib : global::System.Attribute
        {
            return GetCustomAttributes(type: type,
                                       attributeType: typeof(TAttrib),
                                       inherit: inherit).OfType<TAttrib>();
        }

        /// <summary>
        /// Returns attributes of a specific type.
        /// </summary>
        /// <param name="type">The type from where to get the attributes from.</param>
        /// <param name="attributeType">The type of the attributes.</param>
        /// <param name="inherit"><see langword="true" /> to search this member's inheritance chain to find the attributes; otherwise, <see langword="false" />.</param>
        /// <returns>The found attributes.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type" /> and/or <paramref name="attributeType" /> are <see langword="null" />.
        /// </exception>
        public static IEnumerable<object> GetCustomAttributes(Type type, Type attributeType, bool inherit = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }

#if GET_TYPE_ATTRIBUTES_FROM_METHODS
            return type.GetCustomAttributes(attributeType, inherit);
#else
            //TODO: implement
            yield break;
#endif
        }

        /// <summary>
        /// Returns all known methods from a <see cref="Type" />.
        /// </summary>
        /// <param name="type">The type from where to get the methods from.</param>
        /// <returns>The methods of <paramref name="type" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<MethodInfo> GetMethods(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

#if GET_MEMBERS_FROM_EXTENSION_METHODS
            return type.GetRuntimeMethods();
#else
            return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                   BindingFlags.Instance | BindingFlags.Static);
#endif
        }

        /// <summary>
        /// Returns all available types of an <see cref="Assembly" />.
        /// </summary>
        /// <param name="asm">The assembly from where to get the types from.</param>
        /// <returns>The types of the assembly.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asm" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<Type> GetTypes(Assembly asm)
        {
            if (asm == null)
            {
                throw new ArgumentNullException("asm");
            }

#if GET_TYPES_OF_ASSEMBLY_FROM_PROPERTY
            return asm.ExportedTypes;
#else
            return asm.GetTypes();
#endif
        }

        #endregion Methods (5)
    }
}