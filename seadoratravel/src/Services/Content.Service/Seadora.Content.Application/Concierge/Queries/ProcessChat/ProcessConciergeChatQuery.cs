using MediatR;
using Seadora.Content.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace Seadora.Content.Application.Concierge.Queries.ProcessChat;

public class ProcessConciergeChatQuery : IRequest<ConciergeChatResponseDto>
{
    public ConciergeChatRequestDto Request { get; set; }
    
    public ProcessConciergeChatQuery(ConciergeChatRequestDto request)
    {
        Request = request;
    }
}

public class ProcessConciergeChatQueryHandler : IRequestHandler<ProcessConciergeChatQuery, ConciergeChatResponseDto>
{
    private readonly IConciergeService _conciergeService;

    public ProcessConciergeChatQueryHandler(IConciergeService conciergeService)
    {
        _conciergeService = conciergeService;
    }

    public async Task<ConciergeChatResponseDto> Handle(ProcessConciergeChatQuery request, CancellationToken cancellationToken)
    {
        return await _conciergeService.ProcessChatAsync(request.Request);
    }
}
