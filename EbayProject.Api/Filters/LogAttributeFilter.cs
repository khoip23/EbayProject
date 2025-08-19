using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class LogAttributeFilter : ActionFilterAttribute
{
    private readonly ILogger<LogAttributeFilter> _logger;
    public LogAttributeFilter(ILogger<LogAttributeFilter> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context) { }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        // //Sau khi xử lý sau khi action handler có kết quả
        // System.Console.WriteLine($@"OnActionExcuted: ");
        // context.Result = new ContentResult()
        // {
        //     StatusCode = 200,
        //     Content = "Không thể trả về kết quả"
        // };
    }
}
