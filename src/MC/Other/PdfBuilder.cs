using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Security;
using MC.Other;
using MC.Design;
using MC.Testing;
using MC.PaperTools;

namespace MC.Other
{
    public static class PdfBuilder
    {
        public static PdfDocument Create(Test test)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = test.Name;

            PageSize size = PageSize.A4;
            PageOrientation orientation;

            // Looks stupid, but PdfSharp has its own size classes
            switch (test.Paper.Template)
            {
                case (PaperTemplate.Letter):
                    size = PageSize.Letter;
                    break;
                case (PaperTemplate.GovernmentLetter):
                    size = PageSize.GovernmentLetter;
                    break;
                case (PaperTemplate.Legal):
                    size = PageSize.Legal;
                    break;
                case (PaperTemplate.Ledger):
                    size = PageSize.Ledger;
                    break;
                case (PaperTemplate.A0):
                    size = PageSize.A0;
                    break;
                case (PaperTemplate.A1):
                    size = PageSize.A1;
                    break;
                case (PaperTemplate.A2):
                    size = PageSize.A2;
                    break;
                case (PaperTemplate.A3):
                    size = PageSize.A3;
                    break;
                case (PaperTemplate.A4):
                    size = PageSize.A4;
                    break;
                case (PaperTemplate.A5):
                    size = PageSize.A5;
                    break;
            }

            if(test.Paper.Orientation == PaperOrientation.Portrait)
            {
                orientation = PageOrientation.Portrait;
            }
            else
            {
                orientation = PageOrientation.Landscape;
            }

            if (size == PageSize.GovernmentLetter || size == PageSize.Ledger)
            {
                orientation = orientation == PageOrientation.Portrait ? PageOrientation.Landscape : PageOrientation.Portrait;
            }

            foreach (Page page in test.Pages)
            {
                PdfPage pdfPage = document.AddPage();
                pdfPage.Size = size;
                pdfPage.Orientation = orientation;                
                XGraphics g = XGraphics.FromPdfPage(pdfPage);
                page.PdfDraw(g);
            }

            PdfSecuritySettings securitySettings = document.SecuritySettings;
            securitySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted128Bit;
            securitySettings.OwnerPassword = "BDVWM2vJtAtepS3NaykFL2rLdfh7EYD7cRaanZXx9XARk52W";
            securitySettings.PermitAccessibilityExtractContent = false;
            securitySettings.PermitAnnotations = false;
            securitySettings.PermitAssembleDocument = false;
            securitySettings.PermitExtractContent = false;
            securitySettings.PermitFormsFill = false;
            securitySettings.PermitFullQualityPrint = false;
            securitySettings.PermitModifyDocument = false;
            securitySettings.PermitPrint = true;
            securitySettings.PermitFullQualityPrint = true;

            return document;
        }        

    }
}
