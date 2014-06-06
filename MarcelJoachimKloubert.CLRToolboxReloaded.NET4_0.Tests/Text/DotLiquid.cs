// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Text.Html;
using NUnit.Framework;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Text
{
    /// <summary>
    /// Tests DotLiquid templates
    /// </summary>
    // [Ignore]
    public class DotLiquid : TestFixtureBase
    {
        #region Methods (1)

        [Test]
        public void Test1()
        {
            var tpl = DotLiquidHtmlTemplate.Create(@"{% assign testVar1 = 'TM' %}
{% if testVar1 == 'MK' %}
<i>MK</i>
{% elsif testVar1 == 'TM' %}
<b>TM</b>
{% endif %}");

            Assert.AreEqual(tpl.Render(), @"

<b>TM</b>
");
        }

        #endregion Methods (1)
    }
}