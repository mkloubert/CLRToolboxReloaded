// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Acl
{
    /// <summary>
    /// A simple thread safe access control list.
    /// </summary>
    public class AccessControlList : NotifiableBase, IAccessControlList
    {
        #region Fields (1)

        private readonly IList<AclRole> _ROLES;

        #endregion Fields (1)

        #region Constructors (2)

        /// <inheriteddoc />
        public AccessControlList(object sync)
            : base(isSynchronized: true,
                   sync: sync)
        {
            this._ROLES = this.CreateRoleStorage() ?? new List<AclRole>();
        }

        /// <inheriteddoc />
        public AccessControlList()
            : this(sync: new object())
        {
        }

        #endregion Constructors (2)

        #region Methods (20)

        /// <summary>
        /// Adds a new role.
        /// </summary>
        /// <param name="name">The name of the new role.</param>
        /// <returns>The new role.</returns>
        /// <exception cref="ArgumentException">
        /// Role does already exist.
        /// </exception>
        public AclRole AddRole(string name)
        {
            return this.InvokeForRoleList((l, s) =>
                {
                    var newRole = s.Acl.CreateRole(s.RoleName);
                    if (l.Contains(newRole))
                    {
                        throw new ArgumentException("name");
                    }

                    l.Add(newRole);
                    return newRole;
                }, new
                {
                    Acl = this,
                    RoleName = name,
                });
        }

        /// <inheriteddoc />
        public bool AreAllowed(string role, IEnumerable<string> resources)
        {
            // (null) check is done by AclRole.AreAllowed(IEnumerable<string>)
            // method

            switch (this.Behavior)
            {
                case AclAllowBehavior.Anything:
                    return true;

                case AclAllowBehavior.Nothing:
                    return false;
            }

            var r = this[role];
            if (r == null)
            {
                return false;
            }

            return (resources is string[]) ? r.AreAllowed((string[])resources)
                                           : r.AreAllowed(resources);
        }

        /// <inheriteddoc />
        public bool AreAllowed(string role, params string[] resources)
        {
            return this.AreAllowed(role,
                                   (IEnumerable<string>)resources);
        }

        /// <summary>
        /// Clears all roles.
        /// </summary>
        public void ClearRoles()
        {
            this.InvokeForRoleList((l) => l.Clear());
        }

        /// <summary>
        /// Creates a new role object.
        /// </summary>
        /// <param name="name">The name of the role.</param>
        /// <returns>The new instance.</returns>
        protected virtual AclRole CreateRole(string name)
        {
            return new AclRole(name: name)
                {
                    Behavior = AclRoleAllowBehavior.CheckResources,
                };
        }

        /// <summary>
        /// Creates the internal role list storage.
        /// </summary>
        /// <returns>The instance.</returns>
        protected virtual IList<AclRole> CreateRoleStorage()
        {
            // create default/common instance
            return null;
        }

        /// <inheriteddoc />
        public virtual IEnumerator<AclRole> GetEnumerator()
        {
            return new SynchronizedEnumerator<AclRole>(seq: this._ROLES,
                                                       sync: this._SYNC);
        }

        /// <inheriteddoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheriteddoc />
        IEnumerator<IAclRole> IEnumerable<IAclRole>.GetEnumerator()
        {
#if MONO_PORTABLE
			return EnumeratorWrapper.Create<AclRole, IAclRole>(this.GetEnumerator());
#else
            return this.GetEnumerator();
#endif
        }

        /// <summary>
        /// Invokes a action for the inner role list.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        protected void InvokeForRoleList(Action<IList<AclRole>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.InvokeForRoleList(action: (l, a) => a(l),
                                   actionState: action);
        }

        /// <summary>
        /// Invokes a action for the inner role list.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for the action.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">The state object for <paramref name="action" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        protected void InvokeForRoleList<TState>(Action<IList<AclRole>, TState> action,
                                                 TState actionState)
        {
            this.InvokeForRoleList<TState>(action: action,
                                           actionStateProvider: (l) => actionState);
        }

        /// <summary>
        /// Invokes a action for the inner role list.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for the action.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateProvider">The function that provides the state object for <paramref name="action" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> and/or <paramref name="actionStateProvider" /> are <see langword="null" />.
        /// </exception>
        protected void InvokeForRoleList<TState>(Action<IList<AclRole>, TState> action,
                                                 Func<IList<AclRole>, TState> actionStateProvider)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            this.InvokeForRoleList(func: (l, s) =>
                {
                    s.Action(l,
                             s.StateProvider(l));

                    return (object)null;
                }, funcState: new
                {
                    Action = action,
                    StateProvider = actionStateProvider,
                });
        }

        /// <summary>
        /// Invokes a function for the inner role list.
        /// </summary>
        /// <typeparam name="TResult">The result of the function.</typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> is <see langword="null" />.
        /// </exception>
        protected TResult InvokeForRoleList<TResult>(Func<IList<AclRole>, TResult> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return this.InvokeForRoleList(func: (l, f) => f(l),
                                          funcState: func);
        }

        /// <summary>
        /// Invokes a function for the inner role list.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for the function.</typeparam>
        /// <typeparam name="TResult">The result of the function.</typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcState">The state object for <paramref name="func" />.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> is <see langword="null" />.
        /// </exception>
        protected TResult InvokeForRoleList<TState, TResult>(Func<IList<AclRole>, TState, TResult> func,
                                                             TState funcState)
        {
            return this.InvokeForRoleList<TState, TResult>(func: func,
                                                           funcStateProvider: (l) => funcState);
        }

        /// <summary>
        /// Invokes a function for the inner role list.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for the function.</typeparam>
        /// <typeparam name="TResult">The result of the function.</typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcStateProvider">The function that provides the state object for <paramref name="func" />.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> and/or <paramref name="funcStateProvider" /> are <see langword="null" />.
        /// </exception>
        protected virtual TResult InvokeForRoleList<TState, TResult>(Func<IList<AclRole>, TState, TResult> func,
                                                                     Func<IList<AclRole>, TState> funcStateProvider)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            if (funcStateProvider == null)
            {
                throw new ArgumentNullException("funcStateProvider");
            }

            TResult result;

            lock (this._SYNC)
            {
                result = func(this._ROLES,
                              funcStateProvider(this._ROLES));
            }

            return result;
        }

        /// <summary>
        /// Removes a role.
        /// </summary>
        /// <param name="name">The name of the role to remove.</param>
        /// <returns>The role was removed or not.</returns>
        public bool RemoveRole(string name)
        {
            return this.InvokeForRoleList((l, s) => l.Remove(s.Acl.CreateRole(s.RoleName)),
                                          new
                                          {
                                              Acl = this,
                                              RoleName = name,
                                          });
        }

        /// <inheriteddoc />
        public bool TryGetRole(string name, out AclRole role, AclRole defRole = null)
        {
            return this.TryGetRole(name: name,
                                   role: out role,
                                   defRoleProvider: (acl, roleName) => defRole);
        }

        /// <inheriteddoc />
        bool IAccessControlList.TryGetRole(string name, out IAclRole role, IAclRole defRole)
        {
            return ((IAccessControlList)this).TryGetRole(name: name,
                                                         role: out role,
                                                         defRoleProvider: (acl, roleName) => defRole);
        }

        /// <inheriteddoc />
        public bool TryGetRole(string name, out AclRole role, Func<AccessControlList, string, AclRole> defRoleProvider)
        {
            if (defRoleProvider == null)
            {
                throw new ArgumentNullException("defRoleProvider");
            }

            // try find role
            role = this.InvokeForRoleList((l, s) => l.FirstOrDefault(r => r.Name == s.RoleName),
                                          new
                                          {
                                              RoleName = AclRole.ParseName(name),
                                          });

            var result = role != null;

            if (result == false)
            {
                role = defRoleProvider(this,
                                       AclResource.ParseName(name));
            }

            return result;
        }

        /// <inheriteddoc />
        bool IAccessControlList.TryGetRole(string name, out IAclRole role, Func<IAccessControlList, string, IAclRole> defRoleProvider)
        {
            if (defRoleProvider == null)
            {
                throw new ArgumentNullException("defRoleProvider");
            }

            AclRole r;
            var result = this.TryGetRole(name,
                                         role: out r);

            role = result ? r : defRoleProvider(this,
                                                AclRole.ParseName(name));
            return result;
        }

        #endregion Methods (20)

        #region Properties (3)

        /// <summary>
        /// Gets or sets the behavior of that access control list.
        /// </summary>
        public virtual AclAllowBehavior Behavior
        {
            get { return this.Get<AclAllowBehavior>(() => this.Behavior); }

            set { this.Set<AclAllowBehavior>(value, () => this.Behavior); }
        }

        /// <inheriteddoc />
        public AclRole this[string name]
        {
            get
            {
                AclRole result;
                this.TryGetRole(name: name,
                                role: out result);

                return result;
            }
        }

        /// <inheriteddoc />
        IAclRole IAccessControlList.this[string name]
        {
            get { return this[name]; }
        }

        #endregion Properties (3)
    }
}