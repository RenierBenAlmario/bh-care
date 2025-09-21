using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Barangay.Models;
using Microsoft.Extensions.Logging;

namespace Barangay.Services
{
    public interface IConsultationPdfService
    {
        Task<byte[]> GenerateConsultationPdfAsync(MedicalRecord record, string patientName, string doctorName, int? prescriptionId = null);
    }

    public class ConsultationPdfService : IConsultationPdfService
    {
        private readonly ILogger<ConsultationPdfService> _logger;

        public ConsultationPdfService(ILogger<ConsultationPdfService> logger)
        {
            _logger = logger;
        }

        public Task<byte[]> GenerateConsultationPdfAsync(MedicalRecord record, string patientName, string doctorName, int? prescriptionId = null)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using var writer = new PdfWriter(memoryStream);
                using var pdf = new PdfDocument(writer);
                using var document = new Document(pdf);

                // Add header
                var header = new Paragraph("CITY GOVERNMENT OF CALOOCAN")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12);
                document.Add(header);

                var subHeader = new Paragraph("CITY HEALTH DEPARTMENT")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12);
                document.Add(subHeader);

                var centerHeader = new Paragraph("BAESA HEALTH CENTER")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12);
                document.Add(centerHeader);

                var addressHeader = new Paragraph("356 RCBC")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12);
                document.Add(addressHeader);

                // Add spacing
                document.Add(new Paragraph("\n"));

                // Add consultation title and date
                var consultationTitle = new Paragraph($"Consultation Summary - {record.Date:MM/dd/yyyy}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(14);
                document.Add(consultationTitle);

                // Add date and time
                var dateTime = new Paragraph($"{record.Date:MM/dd/yy, h:mm tt}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(10);
                document.Add(dateTime);

                // Add spacing
                document.Add(new Paragraph("\n"));

                // Patient Information Section
                var patientInfoTitle = new Paragraph("PATIENT INFORMATION")
                    .SetFontSize(12);
                document.Add(patientInfoTitle);

                // Patient details table
                var patientTable = new Table(2).UseAllAvailableWidth();
                patientTable.AddCell(new Cell().Add(new Paragraph("PATIENT NAME:")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                patientTable.AddCell(new Cell().Add(new Paragraph(patientName)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                patientTable.AddCell(new Cell().Add(new Paragraph("DOCTOR:")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                patientTable.AddCell(new Cell().Add(new Paragraph(doctorName)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                patientTable.AddCell(new Cell().Add(new Paragraph("DATE:")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                patientTable.AddCell(new Cell().Add(new Paragraph(record.Date.ToString("MM/dd/yyyy"))).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                document.Add(patientTable);

                // Add spacing
                document.Add(new Paragraph("\n"));

                // Consultation Details Section
                var consultationTitle2 = new Paragraph("CONSULTATION DETAILS")
                    .SetFontSize(12);
                document.Add(consultationTitle2);

                // Chief Complaint
                if (!string.IsNullOrEmpty(record.ChiefComplaint))
                {
                    document.Add(new Paragraph("Chief Complaint:"));
                    document.Add(new Paragraph(record.ChiefComplaint));
                    document.Add(new Paragraph("\n"));
                }

                // Diagnosis
                if (!string.IsNullOrEmpty(record.Diagnosis))
                {
                    document.Add(new Paragraph("Diagnosis:"));
                    document.Add(new Paragraph(record.Diagnosis));
                    document.Add(new Paragraph("\n"));
                }

                // Treatment Plan
                if (!string.IsNullOrEmpty(record.Treatment))
                {
                    document.Add(new Paragraph("Treatment Plan:"));
                    document.Add(new Paragraph(record.Treatment));
                    document.Add(new Paragraph("\n"));
                }

                // Additional Notes
                if (!string.IsNullOrEmpty(record.Notes))
                {
                    document.Add(new Paragraph("Additional Notes:"));
                    document.Add(new Paragraph(record.Notes));
                    document.Add(new Paragraph("\n"));
                }

                // Medications
                if (!string.IsNullOrEmpty(record.Medications) && record.Medications != "No medications prescribed")
                {
                    document.Add(new Paragraph("Medications Prescribed:"));
                    document.Add(new Paragraph(record.Medications));
                    document.Add(new Paragraph("\n"));
                }

                // Prescription Information (if available)
                if (prescriptionId.HasValue)
                {
                    document.Add(new Paragraph("PRESCRIPTION INFORMATION")
                        .SetFontSize(12));
                    
                    var prescriptionTable = new Table(2).UseAllAvailableWidth();
                    prescriptionTable.AddCell(new Cell().Add(new Paragraph("Prescription ID:")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    prescriptionTable.AddCell(new Cell().Add(new Paragraph(prescriptionId.Value.ToString())).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    prescriptionTable.AddCell(new Cell().Add(new Paragraph("Prescription Date:")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    prescriptionTable.AddCell(new Cell().Add(new Paragraph(record.Date.ToString("MM/dd/yyyy"))).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    prescriptionTable.AddCell(new Cell().Add(new Paragraph("Valid Until:")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    prescriptionTable.AddCell(new Cell().Add(new Paragraph(record.Date.AddDays(30).ToString("MM/dd/yyyy"))).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    document.Add(prescriptionTable);
                    
                    document.Add(new Paragraph("\n"));
                }

                // Add spacing for signature area
                document.Add(new Paragraph("\n\n"));

                // Signature lines
                var signatureTable = new Table(2).UseAllAvailableWidth();
                signatureTable.AddCell(new Cell().Add(new Paragraph("PHYSICIAN-IN-CHARGE")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                signatureTable.AddCell(new Cell().Add(new Paragraph("LICENSE NO.")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                signatureTable.AddCell(new Cell().Add(new Paragraph("_________________________")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                signatureTable.AddCell(new Cell().Add(new Paragraph("_________________________")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                document.Add(signatureTable);

                // Add footer
                document.Add(new Paragraph("\n\n"));
                var footer = new Paragraph("This consultation summary is for your records. Please keep it safe.")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(10);
                document.Add(footer);

                document.Close();
                return Task.FromResult(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate consultation PDF for patient {PatientName}", patientName);
                throw;
            }
        }
    }
}
