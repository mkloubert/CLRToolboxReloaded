// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Workflows
{
    /// <summary>
    /// Marks a member as start point for a workflow.
    /// </summary>
    public abstract class WorkflowAttributeBase : Attribute
    {
        #region Constructors (3)

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowAttributeBase"/> class.
        /// </summary>
        /// <param name="contract">The value for the <see cref="WorkflowAttributeBase.Contract" /> property.</param>
        protected WorkflowAttributeBase(Type contract)
            : this(GetContractName(contract))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowAttributeBase"/> class.
        /// </summary>
        /// <param name="contractName">The value for the <see cref="WorkflowAttributeBase.Contract" /> property.</param>
        protected WorkflowAttributeBase(string contractName)
        {
            this.Contract = ParseContractName(contractName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowAttributeBase"/> class.
        /// </summary>
        protected WorkflowAttributeBase()
            : this((string)null)
        {
        }

        #endregion Constructors

        #region Properties (1)

        /// <summary>
        /// Gets the name of the contract.
        /// </summary>
        public string Contract
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods (2)

        /// <summary>
        /// Returns the contract name from a <see cref="Type" /> object.
        /// </summary>
        /// <param name="type">The type from where to get the contract name from.</param>
        /// <returns>
        /// The contract name or <see langword="null" /> is <paramref name="type" /> is also <see langword="null" />.
        /// </returns>
        public static string GetContractName(Type type)
        {
            return type != null ? string.Format("{0}{1}{2}",
#if !(PORTABLE || PORTABLE40)
                                                type.Assembly,
                                                "\n",
#else
                                                null,
                                                null,
#endif
                                                type.FullName) : null;
        }

        /// <summary>
        /// Parses a contract name.
        /// </summary>
        /// <param name="contract">The input value.</param>
        /// <returns>The parsed value.</returns>
        public static string ParseContractName(string contract)
        {
            if (contract == null)
            {
                return null;
            }

            contract = contract.ToUpper().Trim();
            if (contract == string.Empty)
            {
                contract = null;
            }

            return contract;
        }

        #endregion
    }
}