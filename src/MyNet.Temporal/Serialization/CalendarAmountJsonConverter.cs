// -----------------------------------------------------------------------
// <copyright file="CalendarAmountJsonConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyNet.Temporal.Serialization;

/// <summary>
/// Serializes <see cref="CalendarAmount"/> as <c>{ "years": n, "months": n, "timeSpan": "..." }</c>.
/// </summary>
public sealed class CalendarAmountJsonConverter : JsonConverter<CalendarAmount>
{
    public override CalendarAmount Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object for CalendarAmount.");

        var years = 0;
        var months = 0;
        var timeSpan = TimeSpan.Zero;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name.");

            var name = reader.GetString();
            reader.Read();

            switch (name)
            {
                case "years":
                    years = reader.GetInt32();
                    break;
                case "months":
                    months = reader.GetInt32();
                    break;
                case "timeSpan":
                    timeSpan = reader.TokenType == JsonTokenType.String
                        ? TimeSpan.Parse(reader.GetString()!, System.Globalization.CultureInfo.InvariantCulture)
                        : throw new JsonException("timeSpan must be a string.");
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        return new CalendarAmount { Years = years, Months = months, TimeSpan = timeSpan };
    }

    public override void Write(Utf8JsonWriter writer, CalendarAmount value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("years", value.Years);
        writer.WriteNumber("months", value.Months);
        writer.WriteString("timeSpan", value.TimeSpan.ToString());
        writer.WriteEndObject();
    }
}
