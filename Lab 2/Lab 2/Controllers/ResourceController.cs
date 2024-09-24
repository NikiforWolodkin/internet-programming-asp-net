using Lab_2.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Lab_2.Controllers;

[ApiController]
[Route("[controller]")]
public class ResourceController : ControllerBase
{
    private const string StackKey = "STACK";

    private static int _result = 0;

    [HttpGet]
    public async Task<IActionResult> GetResourceAsync()
    {
        var stack = GetSesstionStack(HttpContext);

        int stackValue = 0;

        if (stack.Count > 0)
        {
            stackValue = stack.Last();
        }

        var result = _result + stackValue;

        return Ok(new { Result = result });
    }

    [HttpPost]
    public async Task<IActionResult> SetResultAsync(int result)
    {
        _result = result;

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> PushToStackAsync(int add)
    {
        var stack = GetSesstionStack(HttpContext);

        stack.Add(add);

        SetSessionStack(HttpContext, stack);

        return Ok();
    }

    [HttpDelete]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PopStackAsync()
    {
        var stack = GetSesstionStack(HttpContext);

        if (stack.Count > 0)
        {
            stack.RemoveAt(stack.Count - 1);

            SetSessionStack(HttpContext, stack);

            return Ok();
        }
        else
        {
            return BadRequest("Stack is empty.");
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
