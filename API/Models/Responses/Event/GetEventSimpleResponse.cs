using API.Models.Responses.Activity;

namespace API.Models.Responses.Event
{
    public class GetEventSimpleResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public GetActivityResponse Activity { get; set; }
    }
}
