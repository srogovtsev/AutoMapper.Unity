using System;
using FluentAssertions;
using Microsoft.Practices.Unity;
using Xunit;

namespace AutoMapper.Unity.Tests
{
    public class ResolutionTests
    {
        private readonly IUnityContainer _container;

        public ResolutionTests()
        {
            _container = new UnityContainer();
            _container.RegisterMapper();
        }

        [Theory]
        [InlineData(typeof(IConfigurationProvider))]
        [InlineData(typeof(IMapper))]
        public void ShouldResolverIConfigurationProvider(Type type)
        {
            _container
                .Resolve(type)
                .Should()
                .NotBeNull().And.Match(b => type.IsInstanceOfType(b));
        }
    }
}
