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
    /// A simple role for an access control list.
    /// </summary>
    public class AclRole : NotifiableBase, IAclRole
    {
        #region Fields (1)

        private readonly IList<AclResource> _RESOURCES;

        #endregion Fields (1)

        #region Constructors (2)

        /// <inheriteddoc />
        public AclRole(string name)
            : this(name: name,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        public AclRole(string name, object sync)
            : base(sync: sync,
                   isSynchronized: true)
        {
            this.Name = ParseName(name);

            this._RESOURCES = this.CreateResourceStorage() ?? new List<AclResource>();
        }

        #endregion Constructors (2)

        #region Properties (4)

        /// <summary>
        /// Gets or sets the behavior of that role.
        /// </summary>
        public virtual AclRoleAllowBehavior Behavior
        {
            get { return this.Get<AclRoleAllowBehavior>(() => this.Behavior); }

            set { this.Set<AclRoleAllowBehavior>(value, () => this.Behavior); }
        }

        /// <inheriteddoc />
        public string Name
        {
            get;
            private set;
        }

        /// <inheriteddoc />
        public AclResource this[string name]
        {
            get
            {
                AclResource result;
                this.TryGetResource(name: name,
                                    res: out result);

                return result;
            }
        }

        /// <inheriteddoc />
        IAclResource IAclRole.this[string name]
        {
            get { return this[name]; }
        }

        #endregion Properties (3)

        #region Methods (24)

        /// <summary>
        /// Adds a new resource.
        /// </summary>
        /// <param name="name">The name of the new resource.</param>
        /// <param name="isAllowed">Allow resource or not.</param>
        /// <returns>The new resource.</returns>
        /// <exception cref="ArgumentException">
        /// Resource does already exist.
        /// </exception>
        public AclResource AddResource(string name, bool isAllowed = true)
        {
            return this.InvokeForResourceList((l, s) =>
                {
                    var newRes = s.Acl.CreateResource(s.ResName);
                    if (l.Contains(newRes))
                    {
                        throw new ArgumentException("name");
                    }

                    newRes.IsAllowed = s.IsAllowed;
                    l.Add(newRes);

                    return newRes;
                }, new
                {
                    Acl = this,
                    IsAllowed = isAllowed,
                    ResName = name,
                });
        }

        /// <inheriteddoc />
        public bool AreAllowed(IEnumerable<string> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            switch (this.Behavior)
            {
                case AclRoleAllowBehavior.Anything:
                    return true;

                case AclRoleAllowBehavior.Nothing:
                    return false;
            }

            var resOther = resources.Select(rn => AclResource.ParseName(rn))
                                    .Distinct()
                                    .OrderBy(rn => rn)
                                    .Select(rn => this.CreateResource(rn))
                                    .ToArray();

            var resThis = this.Distinct()
                              .OrderBy(r => r.Name)
                              .ToArray();

            var resIntersect = resThis.Intersect(resOther)
                                      .ToArray();

            if (resIntersect.Length != resOther.Length)
            {
                return false;
            }

            return resIntersect.All(r => r.IsAllowed);
        }

        /// <inheriteddoc />
        public bool AreAllowed(params string[] resources)
        {
            return this.AreAllowed((IEnumerable<string>)resources);
        }

        /// <summary>
        /// Clears all resources.
        /// </summary>
        public void ClearResources()
        {
            this.InvokeForResourceList((l) => l.Clear());
        }

        /// <summary>
        /// Creates a new resource object.
        /// </summary>
        /// <param name="name">The name of the new object.</param>
        /// <returns>The new object.</returns>
        protected virtual AclResource CreateResource(string name)
        {
            return new AclResource(name: name)
                {
                    IsAllowed = false,
                };
        }

        /// <summary>
        /// Creates the internal resource list storage.
        /// </summary>
        /// <returns>The instance.</returns>
        protected virtual IList<AclResource> CreateResourceStorage()
        {
            return null;
        }

        /// <inheriteddoc />
        public bool Equals(IAclRole other)
        {
            return (other != null) ? (this.Name == ParseName(other.Name))
                                   : false;
        }

        /// <inheriteddoc />
        public override bool Equals(object other)
        {
            if (other is IAclRole)
            {
                return this.Equals((IAclRole)other);
            }

            return base.Equals(other);
        }

        /// <inheriteddoc />
        public override int GetHashCode()
        {
            return this.Name != null ? this.Name.GetHashCode()
                                     : 0;
        }

        /// <inheriteddoc />
        public virtual IEnumerator<AclResource> GetEnumerator()
        {
            return new SynchronizedEnumerator<AclResource>(seq: this._RESOURCES,
                                                           sync: this._SYNC);
        }

        /// <inheriteddoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheriteddoc />
        IEnumerator<IAclResource> IEnumerable<IAclResource>.GetEnumerator()
        {
#if MONO_PORTABLE
			return EnumeratorWrapper.Create<AclResource, IAclResource>(this.GetEnumerator());
#else
            return this.GetEnumerator();
#endif
        }

        /// <summary>
        /// Invokes a function for the inner resource list.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        protected void InvokeForResourceList(Action<IList<AclResource>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.InvokeForResourceList(action: (l, a) => a(l),
                                       actionState: action);
        }

        /// <summary>
        /// Invokes a function for the inner resource list.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for the action.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">The state object for <paramref name="action" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        protected void InvokeForResourceList<TState>(Action<IList<AclResource>, TState> action,
                                                     TState actionState)
        {
            this.InvokeForResourceList<TState>(action: action,
                                               actionStateProvider: (l) => actionState);
        }

        /// <summary>
        /// Invokes a function for the inner resource list.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for the action.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateProvider">The function that provides the state object for <paramref name="action" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> and/or <paramref name="actionStateProvider" /> are <see langword="null" />.
        /// </exception>
        protected void InvokeForResourceList<TState>(Action<IList<AclResource>, TState> action,
                                                     Func<IList<AclResource>, TState> actionStateProvider)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            this.InvokeForResourceList(func: (l, s) =>
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
        /// Invokes a function for the inner resource list.
        /// </summary>
        /// <typeparam name="TResult">The result of the function.</typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> is <see langword="null" />.
        /// </exception>
        protected TResult InvokeForResourceList<TResult>(Func<IList<AclResource>, TResult> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return this.InvokeForResourceList(func: (l, f) => f(l),
                                              funcState: func);
        }

        /// <summary>
        /// Invokes a function for the inner resource list.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for the function.</typeparam>
        /// <typeparam name="TResult">The result of the function.</typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcState">The state object for <paramref name="func" />.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> is <see langword="null" />.
        /// </exception>
        protected TResult InvokeForResourceList<TState, TResult>(Func<IList<AclResource>, TState, TResult> func,
                                                                 TState funcState)
        {
            return this.InvokeForResourceList<TState, TResult>(func: func,
                                                               funcStateProvider: (l) => funcState);
        }

        /// <summary>
        /// Invokes a function for the inner resource list.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for the function.</typeparam>
        /// <typeparam name="TResult">The result of the function.</typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcStateProvider">The function that provides the state object for <paramref name="func" />.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> and/or <paramref name="funcStateProvider" /> are <see langword="null" />.
        /// </exception>
        protected virtual TResult InvokeForResourceList<TState, TResult>(Func<IList<AclResource>, TState, TResult> func,
                                                                         Func<IList<AclResource>, TState> funcStateProvider)
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
                result = func(this._RESOURCES,
                              funcStateProvider(this._RESOURCES));
            }

            return result;
        }

        /// <summary>
        /// Parses a string for use as role name.
        /// </summary>
        /// <param name="name">The string to convert/parse.</param>
        /// <returns>The converted/parsed string.</returns>
        public static string ParseName(string name)
        {
            if (name != null)
            {
                name = name.Replace("\r", string.Empty)
                           .Replace("\t", "    ")
                           .Replace(" ", "_")
                           .ToUpper()
                           .Trim();
            }

            if (name == string.Empty)
            {
                name = null;
            }

            return name;
        }

        /// <summary>
        /// Removes a resource.
        /// </summary>
        /// <param name="name">The name of the resource to remove.</param>
        /// <returns>The resource was removed or not.</returns>
        public bool RemoveResource(string name)
        {
            return this.InvokeForResourceList((l, s) => l.Remove(s.Acl.CreateResource(s.ResName)),
                                              new
                                              {
                                                  Acl = this,
                                                  ResName = name,
                                              });
        }

        /// <inheriteddoc />
        public bool TryGetResource(string name, out AclResource res, AclResource defRes = null)
        {
            return this.TryGetResource(name: name,
                                       res: out res,
                                       defResProvider: (role, resName) => defRes);
        }

        /// <inheriteddoc />
        bool IAclRole.TryGetResource(string name, out IAclResource res, IAclResource defRes)
        {
            return ((IAclRole)this).TryGetResource(name: name,
                                                   res: out res,
                                                   defResProvider: (role, resName) => defRes);
        }

        /// <inheriteddoc />
        public bool TryGetResource(string name, out AclResource res, Func<AclRole, string, AclResource> defResProvider)
        {
            if (defResProvider == null)
            {
                throw new ArgumentNullException("defResProvider");
            }

            res = this.InvokeForResourceList((l, s) => l.FirstOrDefault(r => r.Name == s.ResName),
                                             new
                                             {
                                                 ResName = AclResource.ParseName(name),
                                             });

            var result = res != null;

            if (result == false)
            {
                res = defResProvider(this,
                                     AclResource.ParseName(name));
            }

            return result;
        }

        /// <inheriteddoc />
        bool IAclRole.TryGetResource(string name, out IAclResource res, Func<IAclRole, string, IAclResource> defResProvider)
        {
            if (defResProvider == null)
            {
                throw new ArgumentNullException("defResProvider");
            }

            AclResource rr;
            var result = this.TryGetResource(name,
                                             res: out rr);

            res = result ? rr : defResProvider(this,
                                               AclResource.ParseName(name));
            return result;
        }

        #endregion Methods (24)

        #region Operators (1)

        /// <summary>
        /// Creates a new instance of <see cref="AclRole" /> from a string that represents the name.
        /// </summary>
        /// <param name="name">The role name.</param>
        /// <returns>The new instance.</returns>
        public static implicit operator AclRole(string name)
        {
            return new AclRole(name: name);
        }

        #endregion Operators (1)
    }
}