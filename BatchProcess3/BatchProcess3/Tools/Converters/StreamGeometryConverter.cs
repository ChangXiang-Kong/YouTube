using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Media;
using BatchProcess3.Tools.Extensions;

namespace BatchProcess3.Tools.Converters;

/// <summary>
/// JsonConverter 示例。
/// 该示例有问题，不能直接使用，只是作为参考
/// </summary>
public class StreamGeometryConverter : JsonConverter<StreamGeometry>
{
    public override StreamGeometry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var geometryString = reader.GetString();
        if (string.IsNullOrEmpty(geometryString))
        {
            return null;
        }
        return StreamGeometry.Parse(geometryString);
    }

    public override void Write(Utf8JsonWriter writer, StreamGeometry value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }
        var a = JsonSerializer.Serialize(value);
        // 使用 ToString 方法获取 StreamGeometry 的字符串表示
        writer.WriteStringValue(value.ToString());
    }
}