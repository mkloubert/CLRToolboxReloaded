﻿// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// NoItemConstraint applies another constraint to each
    /// item in a collection, failing if any of them succeeds.
    /// </summary>
    public class NoItemConstraint : PrefixConstraint
    {
        /// <summary>
        /// Construct a NoItemConstraint on top of an existing constraint
        /// </summary>
        /// <param name="itemConstraint"></param>
        public NoItemConstraint(Constraint itemConstraint)
            : base(itemConstraint)
        {
            this.DisplayName = "none";
        }

        /// <summary>
        /// Apply the item constraint to each item in the collection,
        /// failing if any item fails.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        public override bool Matches(object actual)
        {
            this.actual = actual;

            if (!(actual is IEnumerable))
                throw new ArgumentException("The actual value must be an IEnumerable", "actual");

            foreach (object item in (IEnumerable)actual)
                if (baseConstraint.Matches(item))
                    return false;

            return true;
        }

        /// <summary>
        /// Write a description of this constraint to a MessageWriter
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("no item");
            baseConstraint.WriteDescriptionTo(writer);
        }
    }
}
