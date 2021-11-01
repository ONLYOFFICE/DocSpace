using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JSIStudios.SimpleRestServices.Testing.UnitTests.Client.Json
{
    [TestClass]
    public class JsonStringSerializerTests
    {
        [TestMethod]
        public void DeserializeNullStringToNull()
        {
            IStringSerializer serializer = new JsonStringSerializer();
            Assert.IsNull(serializer.Deserialize<object>(null));
        }

        [TestMethod]
        public void DeserializeEmptyStringToNull()
        {
            IStringSerializer serializer = new JsonStringSerializer();
            Assert.IsNull(serializer.Deserialize<object>(string.Empty));
        }


        [TestMethod]
        public void SerializeNullObjectToNullString()
        {
            IStringSerializer serializer = new JsonStringSerializer();
            Assert.IsNull(serializer.Serialize<object>(null));
        }
    }
}
