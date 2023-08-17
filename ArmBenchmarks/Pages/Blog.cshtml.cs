using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArmBenchmarks.Pages;

public class BlogModel : PageModel
{
    [FromQuery]
    public string? Author { get; set; }
    [FromQuery]
    public string? Tag { get; set; }
}