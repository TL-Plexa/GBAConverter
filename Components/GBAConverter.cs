using LiveSplit.Model;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class GBAConverter : IComponent
    {
        protected InfoTextComponent InternalComponent { get; set; }
        public GBAConverterSettings Settings { get; set; }
        protected LiveSplitState CurrentState { get; set; }
        private const double CONVERSION_FACTOR = 60.0 / 59.7275;

        public string ComponentName => "GBA Converter";
        public float HorizontalWidth => InternalComponent.HorizontalWidth;
        public float MinimumWidth => InternalComponent.MinimumWidth;
        public float VerticalHeight => InternalComponent.VerticalHeight;
        public float MinimumHeight => InternalComponent.MinimumHeight;
        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;
        public IDictionary<string, Action> ContextMenuControls => null;

        public GBAConverter(LiveSplitState state)
        {
            Settings = new GBAConverterSettings();
            InternalComponent = new InfoTextComponent("GBA Time", "0:00.00");
            CurrentState = state;
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            // Apply text color override if enabled
            if (Settings.OverrideTextColor)
            {
                InternalComponent.NameLabel.ForeColor = Settings.TextColor;
            }
            else
            {
                InternalComponent.NameLabel.ForeColor = state.LayoutSettings.TextColor;
            }

            // Apply time color override if enabled
            if (Settings.OverrideTimeColor)
            {
                InternalComponent.ValueLabel.ForeColor = Settings.TimeColor;
            }
            else
            {
                InternalComponent.ValueLabel.ForeColor = state.LayoutSettings.TextColor;
            }

            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            InternalComponent.DisplayTwoRows = Settings.Display2Rows;
            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            // Apply text color override if enabled
            if (Settings.OverrideTextColor)
            {
                InternalComponent.NameLabel.ForeColor = Settings.TextColor;
            }
            else
            {
                InternalComponent.NameLabel.ForeColor = state.LayoutSettings.TextColor;
            }

            // Apply time color override if enabled
            if (Settings.OverrideTimeColor)
            {
                InternalComponent.ValueLabel.ForeColor = Settings.TimeColor;
            }
            else
            {
                InternalComponent.ValueLabel.ForeColor = state.LayoutSettings.TextColor;
            }

            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            Settings.Mode = mode;
            return Settings;
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        private string GetTimeFormat(TimeSpan time)
        {
            string baseFormat;
            string decimalFormat = "";

            // Determine base format (with or without hours)
            if (time.TotalHours >= 1)
            {
                baseFormat = @"h\:mm\:ss";
            }
            else
            {
                // Check if we should drop decimals for times over 1 minute
                if (Settings.DropDecimals && time.TotalMinutes >= 1)
                {
                    return time.ToString(@"m\:ss");
                }
                baseFormat = @"m\:ss";
            }

            // Add decimal precision based on accuracy setting
            switch (Settings.Accuracy)
            {
                case GBAConverterSettings.TimeAccuracy.Seconds:
                    // No decimals
                    break;
                case GBAConverterSettings.TimeAccuracy.Tenths:
                    decimalFormat = @"\.f";
                    break;
                case GBAConverterSettings.TimeAccuracy.Hundredths:
                    decimalFormat = @"\.ff";
                    break;
                case GBAConverterSettings.TimeAccuracy.Milliseconds:
                    decimalFormat = @"\.fff";
                    break;
            }

            return baseFormat + decimalFormat;
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            var currentTime = state.CurrentTime.RealTime ?? state.CurrentTime.GameTime ?? TimeSpan.Zero;
            var convertedTime = TimeSpan.FromTicks((long)(currentTime.Ticks * CONVERSION_FACTOR));

            string timeFormat = GetTimeFormat(convertedTime);
            string timeString = convertedTime.ToString(timeFormat);

            InternalComponent.InformationValue = timeString;
            InternalComponent.Update(invalidator, state, width, height, mode);
        }

        public void Dispose()
        {
        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
    }
}
