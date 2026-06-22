using System.ComponentModel.DataAnnotations;

namespace IssueBoard.Web.Models.Workspaces;

public sealed class CreateWorkspaceRequest
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}
