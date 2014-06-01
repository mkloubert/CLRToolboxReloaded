// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region CLASS: ForAllAsyncTuple<T, TState>

        private sealed class ForAllAsyncTuple<T, TState>
        {
            #region Fields (6)

            internal readonly Action<IForAllItemContext<T, TState>> ACTION;
            internal readonly Func<T, long, TState> ACTION_STATE_PROVIDER;
            internal readonly ICollection<Exception> ERRORS;
            internal readonly long INDEX;
            internal readonly T ITEM;
            internal readonly object SYNC;

            #endregion Fields (6)

            #region Constructors (1)

            internal ForAllAsyncTuple(Action<IForAllItemContext<T, TState>> action,
                                 Func<T, long, TState> actionStateProvider,
                                 long index,
                                 T item,
                                 ICollection<Exception> errors,
                                 object sync)
            {
                this.ACTION = action;
                this.ACTION_STATE_PROVIDER = actionStateProvider;
                this.ERRORS = errors;
                this.INDEX = index;
                this.ITEM = item;
                this.SYNC = sync;
            }

            #endregion Constructors (1)

            #region Methods (1)

            internal Exception Invoke()
            {
                Exception result = null;

                try
                {
                    var ctx = new ForAllItemContext<T, TState>(sync: this.SYNC,
                                                               isSynchronized: true)
                    {
                        Index = this.INDEX,
                        Item = this.ITEM,
                    };
                    ctx.State = this.ACTION_STATE_PROVIDER(ctx.Item, ctx.Index);

                    this.ACTION(ctx);
                }
                catch (Exception ex)
                {
                    result = ex;

                    lock (this.SYNC)
                    {
                        this.ERRORS.Add(ex);
                    }
                }

                return result;
            }

            #endregion Methods (1)
        }

        #endregion CLASS: ForAllAsyncTuple<T, TState>
    }
}