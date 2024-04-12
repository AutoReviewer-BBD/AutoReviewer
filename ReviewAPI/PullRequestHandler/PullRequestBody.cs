
namespace Api.PullRequestHandler;
public class PullRequestBody
{
    public string title { get; set; }
    public string head { get; set; }
    public string Base { get; set; }
    public string body { get; set; }
}