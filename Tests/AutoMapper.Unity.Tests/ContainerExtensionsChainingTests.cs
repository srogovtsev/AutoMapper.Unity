using System;

using AutoMapper.Unity.TestProfiles;

using FluentAssertions;

using Microsoft.Practices.Unity;

using Xunit;

namespace AutoMapper.Unity.Tests
{
	public class ContainerExtensionsChainingTests: IDisposable
	{
		private readonly IUnityContainer _container;

		public ContainerExtensionsChainingTests()
		{
			_container = new UnityContainer();
		}

		void IDisposable.Dispose()
		{
			_container.Dispose();
		}

		[Fact]
		public void RegisterMappingProfile()
		{
			_container
				.RegisterMappingProfile<MinimalProfile>()
				.Should().BeSameAs(_container);
		}

		[Fact]
		public void RegisterMappingProfilesFromAssembly()
		{
			_container
				.RegisterMappingProfilesFromAssembly(typeof(MinimalProfile).Assembly)
				.Should().BeSameAs(_container);
		}

		[Fact]
		public void RegisterMapper()
		{
			_container
				.RegisterMapper()
				.Should().BeSameAs(_container);
		}

	}
}
