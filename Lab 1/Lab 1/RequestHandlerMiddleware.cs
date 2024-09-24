using System.Text.Json;

namespace Lab_1;

public class RequestHandlerMiddleware
{
    private const string StackKey = "STACK";

    private readonly RequestDelegate _next;

    private static int _result = 0;

    public RequestHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/resource-vnd"))
        {
            switch (context.Request.Method)
            {
                case "GET":
                    await HandleGetRequest(context);
                    break;
                case "POST":
                    HandlePostRequest(context);
                    break;
                case "PUT":
                    HandlePutRequest(context);
                    break;
                case "DELETE":
                    HandleDeleteRequest(context);
                    break;
                default:
                    context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    break;
            }
        }
        else
        {
            await _next(context);
        }
    }

    private static async Task HandleGetRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        var stack = GetSesstionStack(context);

        int stackValue = 0;

        if (stack.Count > 0)
        {
            stackValue = stack.Last();
        }

        var result = _result + stackValue;

        var json = JsonSerializer.Serialize(new { Result = result });

        await context.Response.WriteAsync(json);
    }

    private static void HandlePostRequest(HttpContext context)
    {
        if (context.Request.Query.ContainsKey("result") && 
            int.TryParse(context.Request.Query["result"], out int result))
        {
            _result = result;

            context.Response.StatusCode = StatusCodes.Status200OK;
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static void HandlePutRequest(HttpContext context)
    {
        if (context.Request.Query.ContainsKey("add") && 
            int.TryParse(context.Request.Query["add"], out int value))
        {
            var stack = GetSesstionStack(context);

            stack.Add(value);

            SetSessionStack(context, stack);

            context.Response.StatusCode = StatusCodes.Status200OK;
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static void HandleDeleteRequest(HttpContext context)
    {
        var stack = GetSesstionStack(context);

        if (stack.Count > 0)
        {
            stack.RemoveAt(stack.Count - 1);

            SetSessionStack(context, stack);

            context.Response.StatusCode = StatusCodes.Status200OK;
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static List<int> GetSesstionStack(HttpContext context)
    {
        var stack = context.Session.GetObjectFromJson<List<int>>(StackKey);

        if (stack is null)
        {
            context.Session.SetObjectAsJson(StackKey, new List<int>());
        }

        return context.Session.GetObjectFromJson<List<int>>(StackKey)!;
    }

    private static void SetSessionStack(HttpContext context, List<int> stack)
    {
        context.Session.SetObjectAsJson(StackKey, stack);
    }
}
