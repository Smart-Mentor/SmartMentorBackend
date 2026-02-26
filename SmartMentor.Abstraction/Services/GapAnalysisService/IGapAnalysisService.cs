using SmartMentor.Abstraction.Dto.Responses.GapAnalysisResponse;

namespace SmartMentor.Abstraction.Services.GapAnalysisService
{
    public interface IGapAnalysisService
    {
        Task<GapAnalysisResponse> AnalyzeGapAsync(Guid userId, CancellationToken cancellationToken);
    }
}