﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MarcelJoachimKloubert.CLRToolbox.Objects
{
    /// <summary>
    /// Class that builds proxy types and object for an interface.
    /// </summary>
    /// <typeparam name="T">Type of the interface.</typeparam>
    public sealed class ProxyBuilder<T> : ObjectBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of <see cref="ProxyBuilder{T}" /> class.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Generic type does not represent an interface type.
        /// </exception>
        public ProxyBuilder()
        {
            if (typeof(T).IsInterface == false)
            {
                throw new ArgumentException("T");
            }
        }

        #endregion Constructors

        #region Properties (1)

        /// <summary>
        /// Gets the interface <see cref="Type" />.
        /// </summary>
        public Type InterfaceType
        {
            get { return typeof(T); }
        }

        #endregion Properties

        #region Delegates and Events (1)

        // Delegates (1) 

        /// <summary>
        /// Describes a function or method that provides a name (or a part) by a
        /// </summary>
        /// <param name="builder">The underlying builder instance.</param>
        /// <param name="type">The type the name (part) is (or should be) based on.</param>
        /// <returns>The name (part).</returns>
        public delegate string TypeNameProvider(ProxyBuilder<T> builder, Type type);

        #endregion Delegates and Events

        #region Methods (6)

        private static void CollectProperties(ICollection<PropertyInfo> properties, Type type)
        {
            CollectProperties(properties,
                              type,
                              handledTypes: new HashSet<Type>());
        }

        private static void CollectProperties(ICollection<PropertyInfo> properties, Type type, ICollection<Type> handledTypes)
        {
            properties.AddRange(type.GetProperties());

            type.GetInterfaces()
                .ForEach(ctx => CollectProperties(ctx.State.Properties, ctx.Item,
                                                  ctx.State.HandlesTypes),
                         actionState: new
                         {
                             HandlesTypes = handledTypes,
                             Properties = properties,
                         });
        }

        /// <summary>
        /// Creates a propxy <see cref="Type" /> for the interface of <typeparamref name="T" />.
        /// </summary>
        /// <param name="modBuilder">The module builder to use.</param>
        /// <returns>The created type object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="modBuilder" /> is <see langword="null" />.
        /// </exception>
        public Type CreateType(ModuleBuilder modBuilder)
        {
            return this.CreateType(modBuilder,
                                   proxyTypeNamePrefixProvider: (builder, interfaceType) => "TMImplOf_");
        }

        /// <summary>
        /// Creates a propxy <see cref="Type" /> for the interface of <typeparamref name="T" />.
        /// </summary>
        /// <param name="modBuilder">The module builder to use.</param>
        /// <param name="proxyTypeNamePrefixProvider">The logic that returns the prefix for the full name of the proxy type to create.</param>
        /// <returns>The created type object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="modBuilder" /> is <see langword="null" />.
        /// </exception>
        public Type CreateType(ModuleBuilder modBuilder,
                               TypeNameProvider proxyTypeNamePrefixProvider)
        {
            return this.CreateType(modBuilder,
                                   proxyTypeNamePrefixProvider: proxyTypeNamePrefixProvider,
                                   proxyTypeNameSuffixProvider: (builder, interfaceType) => string.Format("_{0:N}_{1}",
                                                                                                          Guid.NewGuid(),
                                                                                                          builder.GetHashCode()));
        }

        /// <summary>
        /// Creates a propxy <see cref="Type" /> for the interface of <typeparamref name="T" />.
        /// </summary>
        /// <param name="modBuilder">The module builder to use.</param>
        /// <param name="proxyTypeNamePrefixProvider">The logic that returns the prefix for the full name of the proxy type to create.</param>
        /// <param name="proxyTypeNameSuffixProvider">The logic that returns the suffix for the full name of the proxy type to create.</param>
        /// <returns>The created type object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="modBuilder" /> is <see langword="null" />.
        /// </exception>
        public Type CreateType(ModuleBuilder modBuilder,
                               TypeNameProvider proxyTypeNamePrefixProvider,
                               TypeNameProvider proxyTypeNameSuffixProvider)
        {
            return this.CreateType(modBuilder,
                                   proxyTypeNamePrefixProvider: proxyTypeNamePrefixProvider,
                                   proxyTypeNameProvider: (builder, interfaceType) => interfaceType.Name,
                                   proxyTypeNameSuffixProvider: proxyTypeNameSuffixProvider);
        }

        /// <summary>
        /// Creates a propxy <see cref="Type" /> for the interface of <typeparamref name="T" />.
        /// </summary>
        /// <param name="modBuilder">The module builder to use.</param>
        /// <param name="proxyTypeNamePrefixProvider">The logic that returns the prefix for the full name of the proxy type to create.</param>
        /// <param name="proxyTypeNameProvider">The logic that returns the name part of the proxy type to create.</param>
        /// <param name="proxyTypeNameSuffixProvider">The logic that returns the suffix for the full name of the proxy type to create.</param>
        /// <returns>The created type object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="modBuilder" /> is <see langword="null" />.
        /// </exception>
        public Type CreateType(ModuleBuilder modBuilder,
                               TypeNameProvider proxyTypeNamePrefixProvider,
                               TypeNameProvider proxyTypeNameProvider,
                               TypeNameProvider proxyTypeNameSuffixProvider)
        {
            if (modBuilder == null)
            {
                throw new ArgumentNullException("modBuilder");
            }

            var prefix = proxyTypeNamePrefixProvider != null ? (proxyTypeNamePrefixProvider(this, this.InterfaceType) ?? string.Empty).Trim()
                                                             : null;
            var name = proxyTypeNameProvider != null ? (proxyTypeNameProvider(this, this.InterfaceType) ?? string.Empty).Trim()
                                                     : null;
            var suffix = proxyTypeNameSuffixProvider != null ? (proxyTypeNameSuffixProvider(this, this.InterfaceType) ?? string.Empty).Trim()
                                                             : null;

            var baseType = typeof(object);

            var typeBuilder = modBuilder.DefineType(string.Format("{0}{1}{2}",
                                                                  prefix, name, suffix),
                                                    TypeAttributes.Public | TypeAttributes.Class,
                                                    baseType);

            typeBuilder.AddInterfaceImplementation(this.InterfaceType);

            var properties = new List<PropertyInfo>();
            CollectProperties(properties, this.InterfaceType);

            // build properties
            properties.ForEach(ctx =>
                {
                    var p = ctx.Item;

                    var propertyName = p.Name;
                    var propertyType = p.PropertyType;

                    var propertyBuilder = ctx.State.TypeBuilder
                                                   .DefineProperty(propertyName,
                                                                   PropertyAttributes.None,
                                                                   propertyType,
                                                                   Type.EmptyTypes);

                    var fieldBaseName = char.ToLower(propertyName[0]) +
                                        new string(propertyName.Skip(1).ToArray());

                    // find unique field name
                    var fieldName = fieldBaseName;
                    {
                        long fieldNameIndex = -1;
                        while (ctx.State.FieldNamesInUse
                                        .Contains(fieldName))
                        {
                            fieldName = fieldBaseName + (++fieldNameIndex).ToString();
                        }

                        ctx.State.FieldNamesInUse
                                 .Add(fieldName);
                    }

                    var field = ctx.State.TypeBuilder
                                         .DefineField("_" + fieldName,
                                                      propertyType,
                                                      FieldAttributes.Family);

                    // getter
                    {
                        var methodBuilder = ctx.State.TypeBuilder
                                                     .DefineMethod("get_" + propertyName,
                                                                   MethodAttributes.Public | MethodAttributes.Virtual,
                                                                   propertyType,
                                                                   Type.EmptyTypes);

                        var ilGen = methodBuilder.GetILGenerator();

                        ilGen.Emit(OpCodes.Ldarg_0);      // load "this"
                        ilGen.Emit(OpCodes.Ldfld, field); // load the property's underlying field onto the stack
                        ilGen.Emit(OpCodes.Ret);          // return the value on the stack

                        propertyBuilder.SetGetMethod(methodBuilder);
                    }

                    // setter
                    {
                        var methodBuilder = ctx.State.TypeBuilder
                                                     .DefineMethod("set_" + propertyName,
                                                                   MethodAttributes.Public | MethodAttributes.Virtual,
                                                                   typeof(void),
                                                                   new Type[] { propertyType });

                        var ilGen = methodBuilder.GetILGenerator();

                        ilGen.Emit(OpCodes.Ldarg_0);      // load "this"
                        ilGen.Emit(OpCodes.Ldarg_1);      // load "value" onto the stack
                        ilGen.Emit(OpCodes.Stfld, field); // set the field equal to the "value" on the stack
                        ilGen.Emit(OpCodes.Ret);          // return nothing

                        propertyBuilder.SetSetMethod(methodBuilder);
                    }
                }, actionState: new
                {
                    FieldNamesInUse = new HashSet<string>(),
                    TypeBuilder = typeBuilder,
                });

            // constructor
            {
                var baseConstructor = baseType.GetConstructor(new Type[0]);

                var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public,
                                                                       CallingConventions.Standard,
                                                                       Type.EmptyTypes);

                var ilGen = constructorBuilder.GetILGenerator();
                ilGen.Emit(OpCodes.Ldarg_0);                  // load "this"
                ilGen.Emit(OpCodes.Call, baseConstructor);    // call the base constructor

                //TODO
                // define initial values

                ilGen.Emit(OpCodes.Ret);    // return nothing
            }

            return typeBuilder.CreateType();
        }

        #endregion Methods
    }
}