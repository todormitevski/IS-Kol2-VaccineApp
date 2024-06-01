using IntegratedSystems.Domain.Domain_Models;
using IntegratedSystems.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedSystems.Repository.Implementation
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _db;
        private DbSet<Patient> entities;

        public PatientRepository(ApplicationDbContext context)
        {
            this._db = context;
            entities = _db.Set<Patient>();
        }
        public List<Patient> GetAllPatients()
        {
            return entities
                .Include(z => z.VaccinationSchedule)
                .ToList();
        }

        public Patient GetDetailsForPatient(BaseEntity id)
        {
            return entities
                .Include(z => z.VaccinationSchedule)
                .SingleOrDefaultAsync(z => z.Id == id.Id).Result;
        }
    }
}
