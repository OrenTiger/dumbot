using System.Collections.Generic;

namespace DumBot.Models.Forecast
{
    public class ForecastModel
    {
        public int Cod { get; set; }
        public List<ForecastItemModel> List { get; set; }
    }
}
