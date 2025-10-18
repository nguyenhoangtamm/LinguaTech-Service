using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IMenuService
{
    Task<Result<List<MenuResponse>>> GetAll(CancellationToken cancellationToken);
    Task<Result<MenuResponse>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<MenuTreeResponse>>> GetMenuTree(CancellationToken cancellationToken);
    Task<Result<List<MenuTreeResponse>>> GetMenuTreeByRole(int roleId, CancellationToken cancellationToken);
    Task<Result<List<UserMenuResponse>>> GetMenusByUserRoles(CancellationToken cancellationToken);
    Task<Result<int>> Create(CreateMenuRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateMenuRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<int>> AssignMenusToRole(AssignMenuToRoleRequest request, CancellationToken cancellationToken);
    Task<Result<List<RoleMenuResponse>>> GetRoleMenus(int roleId, CancellationToken cancellationToken);
}