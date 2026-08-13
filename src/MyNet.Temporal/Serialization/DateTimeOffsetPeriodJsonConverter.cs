// -----------------------------------------------------------------------
// <copyright file="DateTimeOffsetPeriodJsonConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MyNet.Temporal.Intervals;

namespace MyNet.Temporal.Serialization;

/// <summary>
/// Serializes <see cref="DateTimeOffsetPeriod"/> as <c>{ "start": "...", "end": "..." }</c> (ISO-8601 with offset).
/// </summary>
public sealed class DateTimeOffsetPeriodJsonConverter : JsonConverter<DateTimeOffsetPeriod>
{
    public override DateTimeOffsetPeriod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object for DateTimeOffsetPeriod.");

        DateTimeOffset? start = null;
        DateTimeOffset? end = null;

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
                case "start":
                    start = reader.GetDateTimeOffset();
                    break;
                case "end":
                    end = reader.GetDateTimeOffset();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        if (!start.HasValue || !end.HasValue)
            throw new JsonException("DateTimeOffsetPeriod requires start and end.");

        return new DateTimeOffsetPeriod(start.Value, end.Value);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffsetPeriod value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("start", value.Start!.Value.Value);
        writer.WriteString("end", value.End!.Value.Value);
        writer.WriteEndObject();
    }
}
