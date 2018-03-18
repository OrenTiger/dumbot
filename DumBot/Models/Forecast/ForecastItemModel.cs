using System.Collections.Generic;

namespace DumBot.Models.Forecast
{
    public class ForecastItemModel
    {
        public MainModel Main { get; set; }
        public List<WeatherItemModel> Weather { get; set; }
        public WindModel Wind { get; set; }
        public string Dt_txt { get; set; }
    }
}
