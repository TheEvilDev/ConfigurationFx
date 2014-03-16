using System.Configuration;
using System.Dynamic;
using System.Linq;

namespace ConfigurationFx.Sections
{
    public class DynamicFreeFormConfigurationSection : DynamicObject
    {
        private readonly FreeFormConfigurationSection _section;
        private readonly Configuration _config;

        public DynamicFreeFormConfigurationSection(FreeFormConfigurationSection section, Configuration config)
        {
            _section = section;
            _config = config;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            var attribute = _section.Node.Attributes.FirstOrDefault(a => a.Name.LocalName.EqualsIgnoreCase(binder.Name));

            if (attribute != null)
            {
                result = attribute.Value;
                return true;
            }

            var elements = _section.Node.Elements.Where(a => a.Name.EqualsIgnoreCase(binder.Name));

            if (elements.Any())
            {
                if (elements.Count() == 1)
                {
                    result = elements.First();
                    return true;
                }
                result = elements;
                return true;
            }

            return false;
        }
    }
}