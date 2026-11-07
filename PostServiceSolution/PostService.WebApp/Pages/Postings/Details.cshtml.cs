using Microsoft.AspNetCore.Mvc.RazorPages;
using PostService.Dtos;
using PostService.WebApp.Services;
namespace PostService.WebApp.Pages.Postings;
public sealed class DetailsModel(PostingApiClient api) : PageModel
{
    public PostingGetDto? Posting { get; private set; }
    public string? Error { get; private set; }
    public async Task OnGetAsync(int id, CancellationToken token)
    {
        try { Posting = await api.GetAsync(id, token); }
        catch (PostingApiException ex) { Error = ex.Message; }
    }
}
