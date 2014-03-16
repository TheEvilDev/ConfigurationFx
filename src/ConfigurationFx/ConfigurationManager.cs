using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using ConfigurationFx.Sections;

namespace ConfigurationFx
{
    public class ConfigurationManager : DynamicObject
    {
        public static IDictionary<Type, Func<ConfigurationSection, Configuration, DynamicObject>> SectionWrappers =
            new Dictionary<Type, Func<ConfigurationSection, Configuration, DynamicObject>>();

        private readonly Configuration _configuration;

        public ConfigurationManager(Configuration configuration)
        {
            _configuration = configuration;
            SectionWrappers.Add(typeof (AppSettingsSection),
                (section, config) => new DynamicAppSettingsSection(section, config));
            SectionWrappers.Add(typeof(FreeFormConfigurationSection), (s, c) => new DynamicFreeFormConfigurationSection(s, c));
        }

        public static ConfigurationManager Current
        {
            get
            {
                var config = HttpContext.Current != null
                    ? WebConfigurationManager.OpenWebConfiguration("~")
                    : System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                return new ConfigurationManager(config);
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var section =
                _configuration.Sections.Cast<ConfigurationSection>()
                    .FirstOrDefault(s => s.SectionInformation.SectionName.EqualsIgnoreCase(binder.Name));

            if (section != null)
            {
                result = SectionWrappers.ContainsKey(section.GetType())
                    ? SectionWrappers[section.GetType()](section, _configuration)
                    : (dynamic) section;

                return true;
            }

            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var exists =
                _configuration.Sections.Cast<ConfigurationSection>()
                    .FirstOrDefault(s => s.SectionInformation.SectionName.EqualsIgnoreCase(binder.Name));

            if (exists != null)
            {
                _configuration.Sections.Remove(exists.SectionInformation.SectionName);
            }

            var section = value as ConfigurationSection ?? new FreeFormConfigurationSection(value);

            _configuration.Sections.Add(binder.Name, section);
            _configuration.Save(ConfigurationSaveMode.Modified);

            return true;
        }
    }
}