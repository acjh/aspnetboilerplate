using System;
using System.Collections.Generic;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Modules;
using Abp.Reflection;
using AutoMapper;
using Castle.MicroKernel.Registration;

namespace Abp.AutoMapper
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpAutoMapperModule : AbpModule
    {
        private readonly ITypeFinder _typeFinder;

        public AbpAutoMapperModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            IocManager.Register<IAbpAutoMapperConfiguration, AbpAutoMapperConfiguration>();

            Configuration.ReplaceService<ObjectMapping.IObjectMapper, AutoMapperObjectMapper>();

            Configuration.Modules.AbpAutoMapper().Configurators.Add(CreateCoreMappings);
        }

        public override void PostInitialize()
        {
            CreateMappings();
        }

        private void CreateMappings()
        {
            Action<IMapperConfigurationExpression> configurer = configuration =>
            {
                FindAndAutoMapTypes(configuration);
                foreach (var configurator in Configuration.Modules.AbpAutoMapper().Configurators)
                {
                    configurator(configuration);
                }
            };

            var config = new MapperConfiguration(configurer);
            IocManager.IocContainer.Register(
                Component.For<IConfigurationProvider>().Instance(config).LifestyleSingleton()
            );

            var mapper = config.CreateMapper();
            IocManager.IocContainer.Register(
                Component.For<IMapper>().Instance(mapper).LifestyleSingleton()
            );
            AbpEmulateAutoMapper.Mapper = mapper;
        }

        private void FindAndAutoMapTypes(IMapperConfigurationExpression configuration)
        {
            var types = _typeFinder.Find(type =>
                {
                    var typeInfo = type.GetTypeInfo();
                    return typeInfo.IsDefined(typeof(AutoMapAttribute)) ||
                           typeInfo.IsDefined(typeof(AutoMapFromAttribute)) ||
                           typeInfo.IsDefined(typeof(AutoMapToAttribute));
                }
            );

            Logger.DebugFormat("Found {0} classes define auto mapping attributes", types.Length);

            foreach (var type in types)
            {
                Logger.Debug(type.FullName);
                configuration.CreateAutoAttributeMaps(type);
            }
        }

        private void CreateCoreMappings(IMapperConfigurationExpression configuration)
        {
            var localizationContext = IocManager.Resolve<ILocalizationContext>();

            configuration.CreateMap<ILocalizableString, string>().ConvertUsing((ils, s, context) =>
            {
                if (ils.GetType() == typeof(LocalizableString))
                {
                    var ls = (LocalizableString)ils;
                    var source = GetSource(context.Items, ls.SourceName, localizationContext.LocalizationManager);
                    return source.GetString(ls.Name);
                }

                return ils.Localize(localizationContext);
            });

            configuration.CreateMap<LocalizableString, string>().ConvertUsing((ls, s, context) =>
            {
                var source = GetSource(context.Items, ls.SourceName, localizationContext.LocalizationManager);
                return source.GetString(ls.Name);
            });
        }

        private ILocalizationSource GetSource(IDictionary<string, object> items, string sourceName, ILocalizationManager localizationManager)
        {
            var tmp = localizationManager.GetAllSources();

            IDictionary<string, ILocalizationSource> sources;
            ILocalizationSource source;

            if (items.TryGetValue("LocalizationSources", out object o))
            {
                sources = (IDictionary<string, ILocalizationSource>)o;
                if (sources.TryGetValue(sourceName, out source))
                {
                    return source;
                }
            }
            else
            {
                sources = new Dictionary<string, ILocalizationSource>();
                items["LocalizationSources"] = sources;
            }

            source = localizationManager.GetSource(sourceName);
            sources[sourceName] = source;
            return source;
        }
    }
}
