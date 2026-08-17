using System.Threading.Tasks;
using Seadora.Content.Application.DTOs;

namespace Seadora.Content.Application.Concierge
{
    public interface IConciergeService
    {
        Task<ConciergeChatResponseDto> ProcessChatAsync(ConciergeChatRequestDto request);
    }
}
