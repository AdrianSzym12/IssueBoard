using IssueBoard.Application.Common.Errors;
using MediatR;

namespace IssueBoard.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
