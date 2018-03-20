namespace DumBot.Models.Callback
{
    public class CallbackEventModel
    {
        public string Type { get; set; }
        public object Object { get; set; }
        public int Group_id { get; set; }
    }
}
