using System;
using System.Collections.Generic;
using Seadora.Content.Domain.Enums;

namespace Seadora.Content.Application.DTOs
{
    public class ConciergeChatResponseDto
    {
        public string ReplyText { get; set; } = string.Empty;
        public List<Guid>? SuggestedTours { get; set; }
        public List<TourSummaryDto>? SuggestedTourDetails { get; set; }
        public List<string>? QuickReplies { get; set; }
        public ConciergeIntent Intent { get; set; }
    }
}
