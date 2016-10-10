using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Practices.Unity;

namespace AutoMapper.Unity
{
	public static class ContainerMapperExtensions
	{
	    private static readonly TypeInfo ProfileTypeInfo = typeof(Profile).GetTypeInfo();

	    private static IUnityContainer RegisterMappingProfile(IUnityContainer container, Type profileType)
		{
			return container.RegisterType(typeof(Profile), profileType, profileType.FullName, new ContainerControlledLifetimeManager());
		}

		public static IUnityContainer RegisterMappingProfilesFromAssembly(this IUnityContainer container, Assembly assembly)
		{
			foreach (var type in GetAccessibleTypes(assembly)
				.Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition && ProfileTypeInfo.IsAssignableFrom(t)))
			{
				RegisterMappingProfile(container, type.AsType());
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
			return container
            .RegisterType<MapperConfiguration>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(c =>
                    new MapperConfiguration(configuration =>
                    {
                        configuration.ConstructServicesUsing(t => container.Resolve(t));
                        foreach (var profile in c.ResolveAll<Profile>())
                            configuration.AddProfile(profile);
                    })))
		    .RegisterType<IConfigurationProvider>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(c => c.Resolve<MapperConfiguration>()))
			.RegisterType<IMapper>(
                new InjectionFactory(c => c.Resolve<MapperConfiguration>().CreateMapper()));
		}

		private static IEnumerable<TypeInfo> GetAccessibleTypes(this Assembly assembly)
		{
			try
			{
				return assembly.DefinedTypes;
			}
			catch (ReflectionTypeLoadException ex)
			{
			    return ex.Types.Select(t => t.GetTypeInfo());
			}
		}

	}
}
