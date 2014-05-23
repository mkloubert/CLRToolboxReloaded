﻿// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System.Collections;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// UniqueItemsConstraint tests whether all the items in a 
    /// collection are unique.
    /// </summary>
    public class UniqueItemsConstraint : CollectionItemsEqualConstraint
    {
        /// <summary>
        /// Check that all items are unique.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool doMatch(IEnumerable actual)
        {
            ArrayList list = new ArrayList();

            foreach (object o1 in actual)
            {
                foreach (object o2 in list)
                    if (ItemsEqual(o1, o2))
                        return false;
                list.Add(o1);
            }

            return true;
        }

        /// <summary>
        /// Write a description of this constraint to a MessageWriter
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.Write("all items unique");
        }
    }
}
