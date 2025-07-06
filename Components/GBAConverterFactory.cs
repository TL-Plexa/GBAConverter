using LiveSplit.Model;

using System;

namespace LiveSplit.UI.Components
{
    public class GBAConverterFactory : IComponentFactory
    {
        public string ComponentName => "GBA Converter";
        public string Description => "Converts timer from 60Hz to 59.7275Hz to show true GBA time";
        public ComponentCategory Category => ComponentCategory.Information;
        public IComponent Create(LiveSplitState state) => new GBAConverter(state);
        public string UpdateName => ComponentName;
        public string UpdateURL => "";
        public string XMLURL => "";
        public Version Version => Version.Parse("1.0.0");
    }
}
