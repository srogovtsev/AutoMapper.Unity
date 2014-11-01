using System;

using AutoMapper.Mappers;

using Microsoft.Practices.Unity;

namespace AutoMapper.Unity
{
	public static class ContainerMapperExtensions
	{
		public static void RegisterMappingProfile<T>(this IUnityContainer container)
			where T : Profile
		{
			container.RegisterType<Profile,T>(typeof(T).FullName, new ContainerControlledLifetimeManager());
		}

		public static void RegisterMapper(this IUnityContainer container)
		{
			container.RegisterType<IConfigurationProvider>(new ContainerControlledLifetimeManager(), new InjectionFactory(c =>
			{
				var configuration = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.AllMappers());
				configuration.ConstructServicesUsing(t => container.Resolve(t));
				foreach (var profile in c.ResolveAll<Profile>())
					configuration.AddProfile(profile);	
				return configuration;
			}
			));
			container.RegisterType<IMappingEngine, MappingEngine>(new ContainerControlledLifetimeManager(), new InjectionConstructor(typeof(IConfigurationProvider)));
		}
	}
}
