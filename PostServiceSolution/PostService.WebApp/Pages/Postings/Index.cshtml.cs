using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PostService.Dtos;
using PostService.WebApp.Services;
namespace PostService.WebApp.Pages.Postings;
public sealed class IndexModel(PostingApiClient api) : PageModel
{
    public List<PostingGetDto> Postings { get; private set; } = [];
    public int Total { get; private set; }
    public string? Error { get; private set; }
    [BindProperty(SupportsGet = true)] public string? Q { get; set; }
    [TempData] public string? Success { get; set; }
    public async Task OnGetAsync(CancellationToken token)
    {
        try
        {
            var all = await api.GetAllAsync(token); Total = all.Count;
            var query = Q?.Trim();
            Postings = all.Where(p => string.IsNullOrEmpty(query) || p.Id.ToString().Contains(query)
                || p.From.Contains(query, StringComparison.OrdinalIgnoreCase) || p.To.Contains(query, StringComparison.OrdinalIgnoreCase)
                || p.Content.Contains(query, StringComparison.OrdinalIgnoreCase)).OrderByDescending(p => p.Id).ToList();
        }
        catch (PostingApiException ex) { Error = ex.Message; }
    }
}
