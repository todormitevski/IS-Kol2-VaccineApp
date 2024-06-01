using IntegratedSystems.Domain.Domain_Models;
using IntegratedSystems.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IntegratedSystems.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public AdminController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet("[action]")]
        public List<Patient> GetAllPatients()
        {
            return this._patientService.GetAllPatients();
        }
        [HttpPost("[action]")]
        public Patient GetDetails(BaseEntity id)
        {
            return this._patientService.GetDetailsForPatient(id);
        }

        [HttpPost("[action]")]
        public void ImportAllPatients(List<Patient> model)
        {
            foreach (var item in model)
            {
                var patient = new Patient
                {
                    Embg = item.Embg,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    PhoneNumber = item.PhoneNumber,
                    Email = item.Email,
                    VaccinationSchedule = null
                };

                this._patientService.CreateNewPatient(patient);
            }
        }

    }
}
