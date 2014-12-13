// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using NUnit.Framework;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.ServiceLocation
{
    public sealed class ServiceLocatorTests : TestFixtureBase
    {
        #region CLASSES (4)

        private interface IService2
        {
        }

        [Export]
        private sealed class Service1
        {
        }

        [Export(typeof(IService2))]
        private sealed class Service2a : IService2
        {
        }

        [Export("B", typeof(IService2))]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        private sealed class Service2b : IService2
        {
        }

        #endregion CLASSES (4)

        #region Methods (1)

        [Test]
        public void ExportProviderServiceLocator_Test()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new TypeCatalog(typeof(Service1)));
            catalog.Catalogs.Add(new TypeCatalog(typeof(Service2a)));
            catalog.Catalogs.Add(new TypeCatalog(typeof(Service2b)));

            var container = new CompositionContainer(catalog);

            var sl = new ExportProviderServiceLocator(container);

            var obj1 = sl.GetInstance<Service1>();
            var obj2 = sl.GetInstance<IService2>();
            var obj3 = sl.GetInstance<IService2>("B");

            Assert.AreEqual(obj1.GetType(), typeof(Service1));
            Assert.AreEqual(obj2.GetType(), typeof(Service2a));
            Assert.AreEqual(obj3.GetType(), typeof(Service2b));

            var objList1 = sl.GetAllInstances<Service1>();
            var objList2 = sl.GetAllInstances<IService2>();
            var objList3 = sl.GetAllInstances<IService2>("B");

            Assert.AreEqual(objList1.Single().GetType(), typeof(Service1));
            Assert.AreEqual(objList2.Single().GetType(), typeof(Service2a));
            Assert.AreEqual(objList3.Single().GetType(), typeof(Service2b));

            Assert.AreSame(objList1.Single(), obj1);
            Assert.AreSame(objList2.Single(), obj2);
            Assert.AreNotSame(objList3.Single(), obj3);
        }

        #endregion Methods (1)
    }
}