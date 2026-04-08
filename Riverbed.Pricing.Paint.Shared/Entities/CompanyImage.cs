using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class CompanyImage
    {
        [Key]
        public int Id { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid ImageGuid { get; set; }
    }
}
