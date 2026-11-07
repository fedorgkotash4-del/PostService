using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PostService.Dtos;
using PostService.WebApp.Services;
namespace PostService.WebApp.Pages.Postings;
public sealed class DeleteModel(PostingApiClient api) : PageModel
{
    public PostingGetDto? Posting { get; private set; }
    public string? Error { get; private set; }
    [TempData] public string? Success { get; set; }
    public async Task OnGetAsync(int id, CancellationToken token)
    {
        try { Posting = await api.GetAsync(id, token); }
        catch (PostingApiException ex) { Error = ex.Message; }
    }
    public async Task<IActionResult> OnPostAsync(int id, CancellationToken token)
    {
        try { await api.DeleteAsync(id, token); Success = $"Відправлення №{id} видалено."; return RedirectToPage("Index"); }
        catch (PostingApiException ex)
        {
            Error = ex.Message;
            try { Posting = await api.GetAsync(id, token); } catch (PostingApiException) { }
            return Page();
        }
    }
}
