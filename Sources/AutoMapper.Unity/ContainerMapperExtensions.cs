using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoMapper.Mappers;

using Microsoft.Practices.Unity;

namespace AutoMapper.Unity
{
	public static class ContainerMapperExtensions
	{
		private static IUnityContainer RegisterMappingProfile(IUnityContainer container, Type profileType)
		{
			return container.RegisterType(typeof(Profile), profileType, profileType.FullName, new ContainerControlledLifetimeManager());
		}

		public static IUnityContainer RegisterMappingProfilesFromAssembly(this IUnityContainer container, Assembly assembly)
		{
			foreach (var type in GetAccessibleTypes(assembly)
				.Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition && typeof(Profile).IsAssignableFrom(t)))
			{
				RegisterMappingProfile(container, type);
			}
			return container;
		}

		public static IUnityContainer RegisterMappingProfile<T>(this IUnityContainer container)
			where T : Profile
		{
			return RegisterMappingProfile(container, typeof(T));
		}

		public static IUnityContainer RegisterMapper(this IUnityContainer container)
		{
			container.RegisterType<IConfigurationProvider>(new ContainerControlledLifetimeManager(), new InjectionFactory(c =>
			{
				var configuration = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
				configuration.ConstructServicesUsing(t => container.Resolve(t));
				foreach (var profile in c.ResolveAll<Profile>())
					configuration.AddProfile(profile);	
				return configuration;
			}
			));
			container.RegisterType<IMappingEngine, MappingEngine>(new ContainerControlledLifetimeManager(), new InjectionConstructor(typeof(IConfigurationProvider)));
			return container;
		}

		private static IEnumerable<Type> GetAccessibleTypes(this Assembly assembly)
		{
			try
			{
				return assembly.DefinedTypes.Select(t => t.AsType());
			}
			catch (ReflectionTypeLoadException ex)
			{
				return ex.Types.Where(t => t != null);
			}
		}

	}
}
