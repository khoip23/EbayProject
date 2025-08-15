using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class DemoFilter : ActionFilterAttribute
{
    public string name { get; set; }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        Console.WriteLine($@"OnActionExecuting {name}");
        //Trước khi xử lý action 
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        //Sau khi xử lý sau khi action handler có kết quả
        System.Console.WriteLine($@"OnActionExcuted: ");
        context.Result = new ContentResult()
        {
            StatusCode = 200,
            Content = "Không thể trả về kết quả"
        };
    }
}