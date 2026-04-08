using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class CompanyCustomer : Address
    {
        public int Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CustomerId { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        public string? MobilePhone { get; set; }
        [Phone]
        [MaxLength(32)]
        public string? SecondaryPhoneNumber { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.EmailAddress)]
        [MaxLength(256)]
        public string? SecondaryCcEmail { get; set; }
        public Guid CompanyId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Company? Company { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual List<ProjectData>? CustomerProjects { get; set; }
    }
}
