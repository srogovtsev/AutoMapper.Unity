using System;

using FluentAssertions;

using Microsoft.Practices.Unity;

using Xunit;

namespace AutoMapper.Unity.Tests
{
	public class ProfileRegistrationTests: IDisposable
	{
		private readonly UnityContainer _container;

		public ProfileRegistrationTests()
		{
			_container = new UnityContainer();
		}

		void IDisposable.Dispose()
		{
			_container.Dispose();
		}

		[Fact]
		public void ShouldRegisterWithNameFromType()
		{
			_container.RegisterMappingProfile<TestProfile>();
			_container.Registrations.Should()
				.ContainSingle(cr =>
					cr.Name == typeof (TestProfile).FullName
					&& cr.MappedToType == typeof (TestProfile)
					&& cr.RegisteredType == typeof (Profile));
		}

		[Fact]
		public void ShouldRegisterSingleton()
		{
			_container.RegisterMappingProfile<TestProfile>();

			var name = typeof (TestProfile).FullName;
			_container
				.Resolve<Profile>(name)
				.Should().BeSameAs(
					_container.Resolve<Profile>(name)
				);

		}

		private class TestProfile : Profile
		{
			
		}
	}
}
