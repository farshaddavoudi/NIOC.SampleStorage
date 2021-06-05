using System;

namespace NIOC.SampleStorage.Client.Service
{
    public class AppData
    {
        //private List<WeatherForecast>? _categories;
        //public List<WeatherForecast> Categories
        //{
        //    get => _categories ?? new List<WeatherForecast>();
        //    set
        //    {
        //        _categories = value;
        //        NotifyDataChanged();
        //    }
        //}

        public string? UserProfileImageUrl { get; set; }



        public event Action? OnChange;
        private void NotifyDataChanged() => OnChange?.Invoke();

    }
}