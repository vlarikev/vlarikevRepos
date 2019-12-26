using NUnit.Framework;
using CCNetStore.Models;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace CCNetStore.Tests
{
    [TestFixture]
    public class Test
    {
        private CCStoreEntities db = new CCStoreEntities();
        [Test]
        public void TestCartToList()
        {
            List<cart> l = db.carts.Where(c => c.clientId == 3).ToList();
            Assert.AreEqual(4, l.Count());
        }
        [Test]
        public void TestCartSum()
        {
            var carts = db.carts.Include(c => c.client).Include(c => c.product);

            decimal i = carts.Where(c => c.client.clientLogin == "client@mail.com").Sum(p => p.product.productPrice).Value;
            Assert.AreEqual(38000.00, i);
        }
    }
}