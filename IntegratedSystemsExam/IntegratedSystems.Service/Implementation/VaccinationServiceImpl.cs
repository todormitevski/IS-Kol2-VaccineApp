using IntegratedSystems.Domain.Domain_Models;
using IntegratedSystems.Domain.DTO;
using IntegratedSystems.Repository.Interface;
using IntegratedSystems.Service.Interface;

namespace IntegratedSystems.Service.Implementation
{
    public class VaccinationServiceImpl : IVaccinationService
    {
        private readonly IRepository<VaccinationCenter> vaccinationCenterRepository;
        private readonly IRepository<Vaccine> vaccineRepository;

        public VaccinationServiceImpl(IRepository<VaccinationCenter> repository, IRepository<Vaccine> vaccineRepository)
        {
            vaccinationCenterRepository = repository;
            this.vaccineRepository = vaccineRepository;
        }

        public List<Vaccine> GetVaccinesForCenter(Guid? centerId)
        {
            return vaccineRepository.GetAll().Where(z => z.VaccinationCenter == centerId).ToList();
        }

        public List<Vaccine> GetVaccinesForPatient(Guid? patientId)
        {
            return vaccineRepository.GetAll().Where(z => z.PatientId == patientId).ToList();
        }

        public void ScheduleVaccine(VaccinationDTO dto)
        {
            Vaccine vaccine = new Vaccine();
            vaccine.Manufacturer = dto.manufacturer;
            vaccine.Certificate = Guid.NewGuid();
            vaccine.VaccinationCenter = dto.vaccCenterId;
            vaccine.PatientId = dto.patientId;
            vaccine.DateTaken = dto.vaccinationDate;
            vaccine.Center = vaccinationCenterRepository.Get(dto.vaccCenterId);
            vaccineRepository.Insert(vaccine);
        }
    }
}
