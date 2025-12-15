using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IEnrollmentService
{
    Task<Result<int>> Create(CreateEnrollmentRequest request, CancellationToken cancellationToken);
    Task<Result<List<UserEnrollmentType>>> GetUserEnrollments(CancellationToken cancellationToken);
    Task<Result<CheckEnrollmentResType>> CheckEnrollment(int courseId, CancellationToken cancellationToken);
    Task<Result<UpdateProgressResType>> UpdateProgress(int courseId, UpdateProgressRequest request, CancellationToken cancellationToken);
    Task<Result<List<UserEnrollmentType>>> GetContinueCourses(CancellationToken cancellationToken);
}