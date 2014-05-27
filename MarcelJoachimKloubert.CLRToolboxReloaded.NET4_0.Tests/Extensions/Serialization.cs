// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Serialization.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Extensions
{
    [TestFixture]
    public class Serialization
    {
        #region Methods (1)

        [Test]
        public void JsonParamResult()
        {
            var rand = new Random();

            var result = new JsonParamResult();
            result.code = rand.Next();
            result.tag = new Dictionary<string, object>()
            {
                { "mk", rand.Next() },
                { "TM", rand.Next() },
            };

            // make JSON
            var json = result.ToJson();
            // restore from JSON as dictionary
            var resultBack1 = json.FromJson();

            // check properties and their values
            foreach (var property in result.GetType()
                                           .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                           .Where(p => p.Name != "tag"))
            {
                var pn = property.Name;

                // contains key?
                Assert.IsTrue(resultBack1.ContainsKey(pn));
                // same values?
                Assert.AreEqual(property.GetValue(result, null),
                                resultBack1[pn]);
            }

            var resultBack1Tag = resultBack1["tag"] as IDictionary<string, JToken>;

            Assert.IsNotNull(resultBack1Tag);
            foreach (var item in result.tag)
            {
                var key = item.Key;

                // contains key?
                Assert.IsTrue(resultBack1Tag.ContainsKey(key));

                // same values? ... check as strings because of JToken
                Assert.AreEqual(resultBack1Tag[key].AsString(),
                                result.tag[key].AsString());
            }

            // restore from JSON as object
            var resultBack2 = json.FromJson<JsonParamResult>();

            // check if set
            Assert.IsNotNull(resultBack2);
            Assert.IsNotNull(resultBack2.tag);

            // check properties and values
            foreach (var item in result.tag)
            {
                var key = item.Key;

                // contains key?
                Assert.IsTrue(resultBack2.tag.ContainsKey(key));
                // same values?
                Assert.AreEqual(resultBack2.tag[key],
                                result.tag[key]);
            }
        }

        #endregion Methods (1)
    }
}