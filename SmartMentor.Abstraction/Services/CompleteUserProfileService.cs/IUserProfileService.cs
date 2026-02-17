using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using SmartMentor.Abstraction.Dto.Requests.UserRequests;

namespace SmartMentor.Abstraction.Services.CompleteUserProfileService.cs
{
    public interface IUserProfileService
    {
        Task<Result> CompleteAsync(Guid userId, CompleteUserProfileRequest request, CancellationToken cancellationToken = default);
    }
}
