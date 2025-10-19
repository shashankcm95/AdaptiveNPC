using System.Threading.Tasks;

namespace AdaptiveNPC
{
    public interface IResponseProvider
    {
        Task<string> GenerateResponse(ResponseRequest request);
    }
    
    public class ResponseRequest
    {
        public string Action { get; set; }
        public string Context { get; set; }
        public Pattern Pattern { get; set; }
        public string Memory { get; set; }
        public string Personality { get; set; }
        public string CompanionName { get; set; }
    }
}
