using Companies.Domain.Abstraction.Repositories;
using Companies.Domain.Abstraction.Services;
using Companies.Domain.Settings;
using Companies.Infrastructure.Models.PdfData;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services
{
    public class PdfSummaryService:IPdfSummaryService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly string _pdfFolderPath;
        private readonly string _pdfFileName;

        public PdfSummaryService(IOptions<PdfSettings> pdfSettings, ICompanyRepository companyRepository)
        {
            _pdfFileName = pdfSettings.Value.PdfFileName;
            _pdfFolderPath = pdfSettings.Value.PdfFolderPath;
            _companyRepository =companyRepository;
        }

        public async Task GeneratePdf(string organizationId)
        {
            try
            {
                string fileName = $"{_pdfFileName}{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string filePath = Path.Combine(_pdfFolderPath, fileName);

                using (var document = new iTextSharp.text.Document())
                {

                    using (var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create)))
                    {

                        document.Open();
                        PdfData pdfData = await WritePdfFile(organizationId);
                        document.Add(new Paragraph(pdfData.Title));
                        document.Add(new Paragraph(pdfData.Content));
                        document.Close();
                    }
                }
                Log.Information("pdf file was created successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "an error occurred while trying to create the pdf file.");
            }
        }

        private async Task<PdfData> WritePdfFile(string organizationId)
        {
            try
            {
                PdfData pdfData = new PdfData();
                var company = await _companyRepository.GetCompanyByOrganizationId(organizationId);
                if (company == null)
                {
                    pdfData.Title = $"There is no available information in the database" +
                        $"about a company with the OrganizationId: {organizationId}";
                    return pdfData;
                }
                pdfData.Title = $"--Summary Of The Company {company.Name}--";
                pdfData.Content =
                    $"Organization Id: {company.OrganizationId};\n"
                    + $"Company name: {company.Name};\n"
                    + $"Website: {company.Website};\n"
                    + $"Country: {company.Country};\n"
                    + $"Describtion: {company.Description};\n"
                    + $"Founded in: {company.Founded};\n"
                    + $"Number of employees: {company.Employees};\n";
                if (company.Industries != null && company.Industries.Any())
                {
                    pdfData.Content += "Industries: ";
                    foreach (var industry in company.Industries)
                    {
                        pdfData.Content += $"{industry.Name} /";
                    }
                }
                return pdfData;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"an error occured while trying to generate the pdf file content.");
                throw;
            }
        }
    }
}
