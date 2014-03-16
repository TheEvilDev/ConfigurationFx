using System;
using System.Configuration;
using System.Dynamic;

namespace ConfigurationFx.Sections
{
    public class DynamicAppSettingsSection : DynamicObject
    {
        private readonly ConfigurationSection _section;
        private readonly Configuration _config;

        public DynamicAppSettingsSection(ConfigurationSection section, Configuration config)
        {
            _section = section;
            _config = config;
        }   
    }
}