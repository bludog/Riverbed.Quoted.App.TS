using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Data
{
    public sealed record ProjectDto(
    Guid ProjectId,
    string ProjectName,
    int? StatusCodeId,
    String CreatedDate,
    String? CompletedDate,
    decimal? BaseAmount,
    string? Summary);

    public sealed record CustomerDto(
        Guid CustomerId,
        string CustomerName,
        string? CustomerEmail,
        String? CreatedDate,
        string? CustomerLocation,
        ProjectDto[] Projects);

    //public sealed record CompanyDto(
    //    Guid CompanyId,
    //    string CompanyName,
    //    string? PhoneNumber,
    //    string? Email,
    //    string? CompanyLocation,
    //    CustomerDto[] Customers);

    public sealed record CompanyProjectsDto(
          //Guid CompanyId,
          string CompanyName,
          //Guid? CustomerId,
          string? CustomerName,
          //Guid? ProjectId,
          string? ProjectName,
          int? StatusCodeId,
          DateTime? CreatedDate,
          DateTime? CompletedDate,
          decimal? BaseAmount);
}
