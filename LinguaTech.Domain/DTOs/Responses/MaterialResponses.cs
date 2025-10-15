using LinguaTech.Domain.Common.Mappings;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Domain.DTOs.Responses;

public class MaterialResponse
{
}

public class MaterialDto : IMapFrom<Material>
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long Size { get; set; }
}

public class GetMaterialDto : IMapFrom<Material>
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}

public class GetAllMaterialsDto : IMapFrom<Material>
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime? CreatedDate { get; set; }
}

public class GetMaterialsWithPaginationDto : IMapFrom<Material>
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime? CreatedDate { get; set; }
}