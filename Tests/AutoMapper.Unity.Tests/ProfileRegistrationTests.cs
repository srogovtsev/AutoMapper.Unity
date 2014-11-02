using System;
using System.Linq;

using AutoMapper.Unity.TestProfiles;

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
			_container.RegisterMappingProfile<MinimalProfile>();
			_container.Registrations.Should()
				.ContainSingle(cr =>
					cr.Name == typeof (MinimalProfile).FullName
					&& cr.MappedToType == typeof (MinimalProfile)
					&& cr.RegisteredType == typeof (Profile));
		}

		[Fact]
		public void ShouldRegisterSingleton()
		{
			_container.RegisterMappingProfile<MinimalProfile>();

			var name = typeof (MinimalProfile).FullName;
			_container
				.Resolve<Profile>(name)
				.Should().BeSameAs(
					_container.Resolve<Profile>(name)
				);
		}

		[Fact]
		public void ShouldRegisterAllFromAssembly()
		{
			_container.RegisterMappingProfilesFromAssembly(typeof(MinimalProfile).Assembly);
			_container
				.ResolveAll<Profile>().Select(p => p.GetType())
				// ReSharper disable once CoVariantArrayConversion
				.Should().BeEquivalentTo(new[] {typeof (MinimalProfile), typeof (SecondProfile)});
		}
	}
}
