using IntegratedSystems.Domain.Domain_Models;
using IntegratedSystems.Domain.DTO;

namespace IntegratedSystems.Service.Interface
{
    public interface IVaccinationService
    {
        public void ScheduleVaccine(VaccinationDTO dto);

        public List<Vaccine> GetVaccinesForCenter(Guid? centerId);

        public List<Vaccine> GetVaccinesForPatient(Guid? patientId);

    }
}
