using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminIntegratedSystemsExam.Models
{
    public class VaccinationCenter : BaseEntity
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int MaxCapacity { get; set; }
        public virtual ICollection<Vaccine>? Vaccines { get; set; }
    }
}
