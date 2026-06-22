using IssueBoard.Web.Models.Common;
using IssueBoard.Web.Models.Issues;

namespace IssueBoard.Web.Services;

public interface IIssueService
{
    Task<ApiResult<PagedList<IssueDto>>> SearchAsync(
        Guid projectId,
        SearchIssuesRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult<IssueDto>> CreateAsync(
        Guid projectId,
        CreateIssueRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult<IssueDto>> ChangeStatusAsync(
        Guid issueId,
        ChangeIssueStatusRequest request,
        CancellationToken cancellationToken = default);
}
