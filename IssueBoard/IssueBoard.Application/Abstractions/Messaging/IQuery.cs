using IssueBoard.Application.Common.Errors;
using MediatR;

namespace IssueBoard.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
