using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using DevLab.JmesPath;
using DevLab.JmesPath.Utils;

public class Functions
{
    private readonly IConfiguration configuration_;
    private readonly JmesPath jp_;

    public Functions(IConfiguration configuration, JmesPath jp)
    {
        configuration_ = configuration;
        jp_ = jp;
    }

    [FunctionName("jmespath")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "search")] string content,
        ILogger log
    )
    {
        // handle JMESPath request

        using var reader = new StringReader(content);
        var expression = reader.ReadLine();
        var document = reader.ReadToEnd();

        log.LogDebug($"JSON: {document}");
        log.LogDebug($"JMESPath: {expression}");

        var json = JmesPath.ParseJson(document);

        try
        {
            var result = jp_.Transform(json, expression)
                .AsString()
                ;

            log.LogDebug($"JSON: {result}");

            return new OkObjectResult(result);
        }
        catch (Exception e)
        {
            log.LogCritical(e.Message);
            return new OkObjectResult(null);
        }
    }
}
