using System.Globalization;
using System.Linq;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;
using Abp.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Tests.Localization.Json
{
    public class JsonEmbeddedFileLocalizationDictionaryProvider_Tests
    {
        private readonly JsonEmbeddedFileLocalizationDictionaryProvider _dictionaryProvider;

        public JsonEmbeddedFileLocalizationDictionaryProvider_Tests()
        {
            _dictionaryProvider = new JsonEmbeddedFileLocalizationDictionaryProvider(
                typeof(JsonEmbeddedFileLocalizationDictionaryProvider_Tests).GetAssembly(),
                "Abp.Tests.Localization.Json.JsonSources"
                );

            _dictionaryProvider.Initialize("Lang");
        }

        [Fact]
        public void Should_Get_Dictionaries()
        {
            var dictionaries = _dictionaryProvider.Dictionaries.Values.ToList();

            dictionaries.Count.ShouldBe(2);

            var enDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "en");
            enDict.ShouldNotBe(null);
            enDict["Apple"].ShouldBe("Apple");
            enDict["Banana"].ShouldBe("Banana");

            var zhHansDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "zh-Hans");
            zhHansDict.ShouldNotBe(null);
            zhHansDict["Apple"].ShouldBe("苹果");
            zhHansDict["Banana"].ShouldBe("香蕉");
        }

        [Fact]
        public void Should_Get_Dictionaries_Rebuilded()
        {
            var localizationConfiguration = new LocalizationConfiguration();
            var dictionaryProvider = new CustomLocalizationProvider();
            var localizationSource = new DictionaryBasedLocalizationSource("name", dictionaryProvider);
            localizationSource.Initialize(localizationConfiguration, IocManager.Instance);
            var allStrings = localizationSource.GetAllStrings();
            localizationSource.Initialize(localizationConfiguration, IocManager.Instance);
            var allStrings2 = localizationSource.GetAllStrings();
            allStrings2.ShouldNotBe(allStrings);
        }
    }

    public class CustomLocalizationProvider : LocalizationDictionaryProviderBase
    {
        protected int IterationNo = 0;

        protected override void InitializeDictionaries()
        {
            Dictionaries.Clear();

            IterationNo += 1;

            var deDict = new LocalizationDictionary(new CultureInfo("de-DE"));
            deDict["HelloWorld"] = $"Hallo Welt Nummer {IterationNo}";
            Dictionaries.Add("de-DE", deDict);

            var enDict = new LocalizationDictionary(new CultureInfo("en"));
            enDict["HelloWorld"] = $"Hello World number {IterationNo}";
            Dictionaries.Add("en", enDict);
        }
    }
}
