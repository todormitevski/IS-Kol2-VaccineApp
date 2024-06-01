using AdminIntegratedSystemsExam.Models;
using ClosedXML.Excel;
using ExcelDataReader;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using System.Text;

namespace AdminIntegratedSystemsExam.Controllers
{
    public class PatientController : Controller
    {
        public PatientController()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }

        public IActionResult Index()
        {
            HttpClient client = new HttpClient();
            string URL = "https://localhost:7045/api/Admin/GetAllPatients";

            HttpResponseMessage response = client.GetAsync(URL).Result;
            var data = response.Content.ReadAsAsync<List<Patient>>().Result;
            return View(data);
        }

        public IActionResult Details(string id)
        {
            HttpClient client = new HttpClient();
            //added in next aud
            string URL = "https://localhost:7045/api/Admin/GetDetails";
            var model = new
            {
                Id = id
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<Patient>().Result;


            return View(result);

        }



        public FileContentResult CreateInvoice(string id)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:7045/api/Admin/GetDetails";
            var model = new
            {
                Id = id
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<Patient>().Result;

            /*
             * {{PatientEMBG}}
             * {{PatientFullName}}
             * {{VaccineList}}
            */

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{PatientEMBG}}", result.Embg.ToString());
            document.Content.Replace("{{PatientFullName}}", result.FirstName + " " + result.LastName);

            StringBuilder sb = new StringBuilder();
            var total = 0;

            if(result.VaccinationSchedule != null)
            {
                foreach (var item in result.VaccinationSchedule)
                {
                    sb.AppendLine("Vaccine " + item.Manufacturer);
                    total++;
                }

                document.Content.Replace("{{VaccineList}}", sb.ToString());
                document.Content.Replace("{{NumVaccines}}", total.ToString());
            }
            else
            {
                document.Content.Replace("{{VaccineList}}", "No vaccines");
                document.Content.Replace("{{NumVaccines}}", "0");
            }

            var stream = new MemoryStream();
            document.Save(stream, new PdfSaveOptions());
            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");

        }

        [HttpGet]
        public FileContentResult ExportAllPatients()
        {
            string fileName = "Patients.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("Patients");
                worksheet.Cell(1, 1).Value = "Patient EMBG";
                worksheet.Cell(1, 2).Value = "Patient Full Name";
                worksheet.Cell(1, 3).Value = "Total Vaccines";
                HttpClient client = new HttpClient();
                string URL = "https://localhost:7045/api/Admin/GetAllPatients";

                HttpResponseMessage response = client.GetAsync(URL).Result;
                var data = response.Content.ReadAsAsync<List<Patient>>().Result;

                for (int i = 0; i < data.Count(); i++)
                {
                    var item = data[i];
                    worksheet.Cell(i + 2, 1).Value = item.Embg.ToString();
                    worksheet.Cell(i + 2, 2).Value = item.FirstName + " " + item.LastName;
                    var total = 0;

                    if(item.VaccinationSchedule != null)
                    {
                        for (int j = 0; j < item.VaccinationSchedule.Count(); j++)
                        {
                            worksheet.Cell(1, 4 + j).Value = "Vaccine - " + (j + 1);
                            worksheet.Cell(i + 2, 4 + j).Value = item.VaccinationSchedule.ElementAt(j).Manufacturer;
                            total++;
                        }
                    }
                    
                    worksheet.Cell(i + 2, 3).Value = total;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }

        }

        public IActionResult ImportPatients(IFormFile file)
        {
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }

            List<Patient> patients = getAllPatientsFromFile(file.FileName);
            HttpClient client = new HttpClient();
            string URL = "https://localhost:7045/api/Admin/ImportAllPatients";

            HttpContent content = new StringContent(JsonConvert.SerializeObject(patients), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<bool>().Result;

            return RedirectToAction("Index", "Patient");

        }

        private List<Patient> getAllPatientsFromFile(string fileName)
        {
            List<Patient> patients = new List<Patient>();
            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        patients.Add(new Models.Patient
                        {
                            Embg = reader.GetValue(0).ToString(),
                            FirstName = reader.GetValue(1).ToString(),
                            LastName = reader.GetValue(2).ToString(),
                            PhoneNumber = reader.GetValue(3).ToString(),
                            Email = reader.GetValue(4).ToString(),
                            VaccinationSchedule = null
                        });
                    }

                }
            }
            return patients;

        }
    }
}
