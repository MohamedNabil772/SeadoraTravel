using System;

namespace Seadora.Content.Application.DTOs
{
    public class ConciergeChatRequestDto
    {
        public string Message { get; set; } = string.Empty;
        public string Language { get; set; } = "en";
        public DateTime? SelectedDate { get; set; }
        public int? TourId { get; set; }
        public string? WorkflowStep { get; set; }
        public string? ActionKey { get; set; }
    }
}
