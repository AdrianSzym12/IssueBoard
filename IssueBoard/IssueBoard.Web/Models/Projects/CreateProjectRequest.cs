using System.ComponentModel.DataAnnotations;

namespace IssueBoard.Web.Models.Projects;

public sealed class CreateProjectRequest
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(12)]
    public string Key { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}
