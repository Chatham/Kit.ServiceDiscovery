using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Chatham.Kit.ServiceDiscovery.Consul.Tests
{
    [ExcludeFromCodeCoverage]
    public class ChathamServiceRegistrarTests
    {
        [Fact]
        public async Task Register_SuccessfulRegistration_CallsConsulAndDoesNotThrowException()
        {
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var consulClient = Substitute.For<IConsulClient>();
            consulClient.Agent.ServiceRegister(Arg.Any<AgentServiceRegistration>())
                .Returns(new WriteResult {RequestTime = TimeSpan.Zero, StatusCode = HttpStatusCode.Created});

            var service = new AgentServiceRegistration();
            var registrar = new ConsulServiceRegistrar(loggerFactory, consulClient, service);

            await registrar.Register();
            await consulClient.Agent.Received(1).ServiceRegister(Arg.Any<AgentServiceRegistration>());
        }

        [Fact]
        public async Task Register_ConsulClientThrowsException_CallsConsulAndThrowException()
        {
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var consulClient = Substitute.For<IConsulClient>();

            var expectedException = new Exception();
            consulClient.Agent.ServiceRegister(Arg.Any<AgentServiceRegistration>())
                .Throws(expectedException);

            var service = new AgentServiceRegistration();
            var registrar = new ConsulServiceRegistrar(loggerFactory, consulClient, service);

            Exception actualException = null;
            try
            {
                await registrar.Register();
            }
            catch (Exception ex)
            {
                actualException = ex;
            }

            Assert.Same(expectedException, actualException);
            await consulClient.Agent.Received(1).ServiceRegister(Arg.Any<AgentServiceRegistration>());
        }

        [Fact]
        public async Task Register_ConsulReturnsNonSuccessfulHttpStatus_CallsConsulAndThrowException()
        {
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var consulClient = Substitute.For<IConsulClient>();

            consulClient.Agent.ServiceRegister(Arg.Any<AgentServiceRegistration>())
                .Returns(new WriteResult { RequestTime = TimeSpan.Zero, StatusCode = HttpStatusCode.InternalServerError });

            var service = new AgentServiceRegistration();
            var registrar = new ConsulServiceRegistrar(loggerFactory, consulClient, service);

            Exception exception = null;
            try
            {
                await registrar.Register();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            await consulClient.Agent.Received(1).ServiceRegister(Arg.Any<AgentServiceRegistration>());
        }

        [Fact]
        public async Task Register_ConsulReturnsNonSuccessfulHttpStatusBelow200_CallsConsulAndThrowException()
        {
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var consulClient = Substitute.For<IConsulClient>();

            consulClient.Agent.ServiceRegister(Arg.Any<AgentServiceRegistration>())
                .Returns(new WriteResult { RequestTime = TimeSpan.Zero, StatusCode = HttpStatusCode.Continue });

            var service = new AgentServiceRegistration();
            var registrar = new ConsulServiceRegistrar(loggerFactory, consulClient, service);

            Exception exception = null;
            try
            {
                await registrar.Register();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            await consulClient.Agent.Received(1).ServiceRegister(Arg.Any<AgentServiceRegistration>());
        }

        [Fact]
        public async Task Deregister_SuccessfulRegistration_CallsConsulAndDoesNotThrowException()
        {
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var consulClient = Substitute.For<IConsulClient>();
            consulClient.Agent.ServiceDeregister(Arg.Any<string>())
                .Returns(new WriteResult {RequestTime = TimeSpan.Zero, StatusCode = HttpStatusCode.Created});

            var service = new AgentServiceRegistration();
            var registrar = new ConsulServiceRegistrar(loggerFactory, consulClient, service);

            await registrar.Deregister();
            await consulClient.Agent.Received(1).ServiceDeregister(Arg.Any<string>());
        }

        [Fact]
        public async Task Deregister_ConsulClientThrowsException_CallsConsulAndThrowException()
        {
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var consulClient = Substitute.For<IConsulClient>();

            var expectedException = new Exception();
            consulClient.Agent.ServiceDeregister(Arg.Any<string>())
                .Throws(expectedException);

            var service = new AgentServiceRegistration();
            var registrar = new ConsulServiceRegistrar(loggerFactory, consulClient, service);

            Exception actualException = null;
            try
            {
                await registrar.Deregister();
            }
            catch (Exception ex)
            {
                actualException = ex;
            }

            Assert.Same(expectedException, actualException);
            await consulClient.Agent.Received(1).ServiceDeregister(Arg.Any<string>());
        }

        [Fact]
        public async Task Deregister_ConsulReturnsNonSuccessfulHttpStatus_CallsConsulAndThrowException()
        {
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var consulClient = Substitute.For<IConsulClient>();

            consulClient.Agent.ServiceRegister(Arg.Any<AgentServiceRegistration>())
                .Returns(new WriteResult { RequestTime = TimeSpan.Zero, StatusCode = HttpStatusCode.InternalServerError });

            var service = new AgentServiceRegistration();
            var registrar = new ConsulServiceRegistrar(loggerFactory, consulClient, service);

            Exception exception = null;
            try
            {
                await registrar.Deregister();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            await consulClient.Agent.Received(1).ServiceDeregister(Arg.Any<string>());
        }

        [Fact]
        public async Task Deregister_ConsulReturnsNonSuccessfulHttpStatusBelow200_CallsConsulAndThrowException()
        {
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var consulClient = Substitute.For<IConsulClient>();

            consulClient.Agent.ServiceDeregister(Arg.Any<string>())
                .Returns(new WriteResult { RequestTime = TimeSpan.Zero, StatusCode = HttpStatusCode.Continue });

            var service = new AgentServiceRegistration();
            var registrar = new ConsulServiceRegistrar(loggerFactory, consulClient, service);

            Exception exception = null;
            try
            {
                await registrar.Deregister();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            await consulClient.Agent.Received(1).ServiceDeregister(Arg.Any<string>());
        }
    }
}
