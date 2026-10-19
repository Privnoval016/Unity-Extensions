using System;
using System.ComponentModel;
using System.Globalization;

namespace Extensions.Ids
{
    /**
     * <summary>Converts an <see cref="AssetId" /> to and from its hex text, so serializers that honour type converters
     * (Newtonsoft does) write it as text without needing to know about it.</summary>
     */
    public sealed class AssetIdTypeConverter : TypeConverter
    {
        /** <inheritdoc /> */
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        /** <inheritdoc /> */
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        /** <inheritdoc /> <remarks>Text that isn't an id reads as an empty id.</remarks> */
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            value is string text ? (AssetId.TryParse(text, out var id) ? id : default) : base.ConvertFrom(context, culture, value);

        /** <inheritdoc /> */
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
            destinationType == typeof(string) && value is AssetId id ? id.Value : base.ConvertTo(context, culture, value, destinationType);
    }
}
