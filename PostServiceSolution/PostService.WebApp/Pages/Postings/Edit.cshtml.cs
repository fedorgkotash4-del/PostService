using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PostService.WebApp.Models;
using PostService.WebApp.Services;
namespace PostService.WebApp.Pages.Postings;
public sealed class EditModel(PostingApiClient api) : PageModel
{
    [BindProperty] public PostingForm Input { get; set; } = new();
    public int? PostingId { get; private set; }
    public bool CanEdit { get; private set; } = true;
    public string Title => PostingId.HasValue ? $"Редагування відправлення №{PostingId}" : "Нове відправлення";
    [TempData] public string? Success { get; set; }
    public async Task OnGetAsync(int? id, CancellationToken token)
    {
        PostingId = id;
        if (!id.HasValue) return;
        try { Input = PostingForm.FromDto(await api.GetAsync(id.Value, token)); }
        catch (PostingApiException ex) { CanEdit = false; ModelState.AddModelError("", ex.Message); }
    }
    public async Task<IActionResult> OnPostAsync(int? id, CancellationToken token)
    {
        PostingId = id;
        foreach (var (field, error) in Input.ValidateNumbers(id.HasValue)) ModelState.AddModelError($"Input.{field}", error);
        if (!ModelState.IsValid) return Page();
        try
        {
            if (id.HasValue) await api.UpdateAsync(id.Value, Input.ToPut(id.Value), token);
            else await api.CreateAsync(Input.ToPost(), token);
            Success = id.HasValue ? $"Зміни відправлення №{id} збережено." : "Нове відправлення успішно створено.";
            return RedirectToPage("Index");
        }
        catch (PostingApiException ex)
        {
            ModelState.AddModelError("", ex.Message);
            if (ex.NotFound) CanEdit = false;
            return Page();
        }
    }
}
