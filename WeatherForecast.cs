// <copyright file="WeatherForecast.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DIMS_Backend
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(this.TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}