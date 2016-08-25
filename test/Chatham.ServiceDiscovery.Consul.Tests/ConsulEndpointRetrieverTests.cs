using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chatham.ServiceDiscovery.Consul;
using NSubstitute.ExceptionExtensions;

namespace Chatham.ServiceDiscovery.Consul.Tests
{
    [TestClass]
    public class ConsulEndpointRetrieverTests
    {
        [TestMethod]
        public void FetchEndPoints_consulThrowsException_throwsException()
        {
            //var expectedException = new Exception();

            //var fixture = new ConsulSubscriberFixture();
            //fixture.ServiceName = Guid.NewGuid().ToString();

            //fixture.ClientQueryResult = new QueryResult<ServiceEntry[]>();

            //fixture.Client.Health.Throws(expectedException);
            //var subscriber = fixture.CreateSut();

            //Action action = async () => await FetchEndpoints(); ;
            //Assert.ThrowsException<Exception>(action);
        }



    }
}
