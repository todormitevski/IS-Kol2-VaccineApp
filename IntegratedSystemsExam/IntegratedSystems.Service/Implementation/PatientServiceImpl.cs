using IntegratedSystems.Domain.Domain_Models;
using IntegratedSystems.Repository.Interface;
using IntegratedSystems.Service.Interface;

namespace IntegratedSystems.Service.Implementation
{
    public class PatientServiceImpl : IPatientService
    {

        private readonly IRepository<Patient> patientRepository;
        private readonly IPatientRepository _patientRepository;

        public PatientServiceImpl(IRepository<Patient> patientRepository1, IPatientRepository patientRepository2)
        {
            patientRepository = patientRepository1;
            _patientRepository = patientRepository2;
        }

        public Patient CreateNewPatient(Patient patient)
        {
            return patientRepository.Insert(patient);

        }

        public Patient DeletePatient(Guid id)
        {
            var patient = this.GetPatientById(id);
            return patientRepository.Delete(patient);
        }

        // new
        public List<Patient> GetAllPatients()
        {
            return _patientRepository.GetAllPatients();
        }

        public Patient GetDetailsForPatient(BaseEntity id)
        {
            return _patientRepository.GetDetailsForPatient(id);
        }
        // ---

        public Patient GetPatientById(Guid? id)
        {
            return patientRepository.Get(id);
        }

        public List<Patient> GetPatients()
        {
            return patientRepository.GetAll().ToList();
        }

        public Patient UpdatePatient(Patient patient)
        {
            return patientRepository.Update(patient);
        }
    }
}
