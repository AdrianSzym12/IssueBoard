using IssueBoard.Application.Common.Errors;
using MediatR;

namespace IssueBoard.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
