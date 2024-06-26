﻿using System.Text.Json.Serialization;
using CsvHelper.Configuration;

namespace Krake.Database.Models;

public sealed record PriceData
{
    [JsonIgnore] public Guid InstrumentId { get; set; } = Guid.Empty;
    [JsonPropertyName("date")] public required string Date { get; init; }
    [JsonPropertyName("open")] public required decimal Open { get; init; }
    [JsonPropertyName("high")] public required decimal High { get; init; }
    [JsonPropertyName("low")] public required decimal Low { get; init; }
    [JsonPropertyName("close")] public required decimal Close { get; init; }
    [JsonPropertyName("adjusted_close")] public required decimal AdjustedClose { get; init; }
    [JsonPropertyName("volume")] public required decimal Volume { get; init; }
}

public sealed class PriceDataMap : ClassMap<PriceData>
{
    public PriceDataMap()
    {
        Map(p => p.InstrumentId).Index(0).Name(nameof(PriceData.InstrumentId));
        Map(p => p.Date).Index(1).Name(nameof(PriceData.Date));
        Map(p => p.Open).Index(2).Name(nameof(PriceData.Open));
        Map(p => p.High).Index(3).Name(nameof(PriceData.High));
        Map(p => p.Low).Index(4).Name(nameof(PriceData.Low));
        Map(p => p.Close).Index(5).Name(nameof(PriceData.Close));
        Map(p => p.AdjustedClose).Index(6).Name(nameof(PriceData.AdjustedClose));
        Map(p => p.Volume).Index(7).Name(nameof(PriceData.Volume));
    }
}