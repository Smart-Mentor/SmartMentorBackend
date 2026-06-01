using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using SmartMentor.Abstraction.Dto.Requests.UserRequests;
using SmartMentor.Abstraction.Dto.Responses.UserResponse;

namespace SmartMentor.Abstraction.Services.CompleteUserProfileService
{
    public interface IUserProfileService
    {
        Task<Result> CompleteAsync(Guid userId, CompleteUserProfileRequest request, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(Guid userId, CompleteUserProfileRequest request, CancellationToken cancellationToken = default);
        Task<Result<string>> updateSkillLevel(Guid userId ,int skillId, CancellationToken cancellationToken = default);
        Task<Result<UserProfileResponseDto>> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
