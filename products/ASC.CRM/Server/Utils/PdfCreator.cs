/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using ASC.Common.Logging;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Resources;
using ASC.Files.Core;
using ASC.Web.Core.Files;
using ASC.Web.CRM.Core;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Utils;
using Autofac;
using Ionic.Zip;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

using Microsoft.Extensions.DependencyInjection;
using ASC.Common;
using ASC.Web.Files.Classes;

namespace ASC.Web.CRM.Classes
{
    public class PdfCreator
    {
        public PdfCreator(IOptionsMonitor<ILog> logger,
                          Files.Classes.PathProvider filesPathProvider,
                          DocumentServiceConnector documentServiceConnector,
                          IServiceProvider serviceProvider,
                          OrganisationLogoManager organisationLogoManager,
                          DaoFactory daoFactory,
                          InvoiceFormattedData invoiceFormattedData)
        {
            FilesPathProvider = filesPathProvider;
            
            Logger = logger.Get("ASC.CRM");

            DocumentServiceConnector = documentServiceConnector;
            ServiceProvider = serviceProvider;
            OrganisationLogoManager = organisationLogoManager;
            DaoFactory = daoFactory;
            InvoiceFormattedData = invoiceFormattedData;
        }

        public InvoiceFormattedData InvoiceFormattedData { get; }
        
        public DaoFactory DaoFactory { get; }

        private Stream Template
        {
            get
            {
                var bytes = FileHelper.ReadBytesFromEmbeddedResource("ASC.Web.CRM.InvoiceTemplates.template.docx");
                return new MemoryStream(bytes);
            }
        }

        public IServiceProvider ServiceProvider { get; }
        public DocumentServiceConnector DocumentServiceConnector { get; }
        public OrganisationLogoManager OrganisationLogoManager { get; }
        public Files.Classes.PathProvider FilesPathProvider { get; }

        public ILog Logger { get; }

        private const string FormatPdf = ".pdf";
        private const string FormatDocx = ".docx";
        private const string DocumentXml = "word/document.xml";
        private const string DocumentLogoImage = "word/media/logo.jpeg";

        public void CreateAndSaveFile(int invoiceId)
        {           
            Logger.DebugFormat("PdfCreator. CreateAndSaveFile. Invoice ID = {0}", invoiceId);
            
            try
            {
                    var invoice = DaoFactory.GetInvoiceDao().GetByID(invoiceId);

                    if (invoice == null)
                    {
                        Logger.Warn(CRMErrorsResource.InvoiceNotFound + ". Invoice ID = " + invoiceId);
                     
                        return;
                    }

                    Logger.DebugFormat("PdfCreator. CreateAndSaveFile. Invoice ID = {0}. Convertation", invoiceId);

                    string urlToFile;
                   
                    using (var docxStream = GetStreamDocx(invoice))
                    {
                        urlToFile = GetUrlToFile(docxStream);
                    }

                    Logger.DebugFormat("PdfCreator. CreateAndSaveFile. Invoice ID = {0}. UrlToFile = {1}", invoiceId,
                        urlToFile);

                    var file = ServiceProvider.GetService<File<int>>();

                    file.Title = string.Format("{0}{1}", invoice.Number, FormatPdf);
                    file.FolderID = DaoFactory.GetFileDao().GetRoot();

                    var request = WebRequest.Create(urlToFile);

                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        file.ContentLength = response.ContentLength;

                        Logger.DebugFormat("PdfCreator. CreateAndSaveFile. Invoice ID = {0}. SaveFile", invoiceId);
                        file = DaoFactory.GetFileDao().SaveFile(file, stream);
                    }

                    if (file == null)
                    {
                        throw new Exception(CRMErrorsResource.FileCreateError);
                    }

                    invoice.FileID = Int32.Parse(file.ID.ToString());

                    Logger.DebugFormat("PdfCreator. CreateAndSaveFile. Invoice ID = {0}. UpdateInvoiceFileID. FileID = {1}", invoiceId, file.ID);

                    DaoFactory.GetInvoiceDao().UpdateInvoiceFileID(invoice.ID, invoice.FileID);

                    Logger.DebugFormat("PdfCreator. CreateAndSaveFile. Invoice ID = {0}. AttachFiles. FileID = {1}", invoiceId, file.ID);
                    
                    DaoFactory.GetRelationshipEventDao().AttachFiles(invoice.ContactID, invoice.EntityType, invoice.EntityID, new[] {invoice.FileID});
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public File<int> CreateFile(Invoice data, DaoFactory daoFactory)
        {
            try
            {
                using (var docxStream = GetStreamDocx(data))
                {
                    var urlToFile = GetUrlToFile(docxStream);

                    return SaveFile(data, urlToFile, daoFactory);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);

                throw;
            }
        }

        private string GetUrlToFile(Stream docxStream)
        {
            var externalUri = FilesPathProvider.GetTempUrl(docxStream, FormatDocx);

            externalUri = DocumentServiceConnector.ReplaceCommunityAdress(externalUri);

            Logger.DebugFormat("PdfCreator. GetUrlToFile. externalUri = {0}", externalUri);

            var revisionId = DocumentServiceConnector.GenerateRevisionId(Guid.NewGuid().ToString());
            
            string urlToFile;
            
            DocumentServiceConnector.GetConvertedUri(externalUri, FormatDocx, FormatPdf, revisionId, null, false, out urlToFile);

            Logger.DebugFormat("PdfCreator. GetUrlToFile. urlToFile = {0}", urlToFile);
            
            return urlToFile;

        }

        public ConverterData StartCreationFileAsync(Invoice data)
        {
            using (var docxStream = GetStreamDocx(data))
            {
                var externalUri = FilesPathProvider.GetTempUrl(docxStream, FormatDocx);

                externalUri = DocumentServiceConnector.ReplaceCommunityAdress(externalUri);

                var revisionId = DocumentServiceConnector.GenerateRevisionId(Guid.NewGuid().ToString());
            
                string urlToFile;
                
                DocumentServiceConnector.GetConvertedUri(externalUri, FormatDocx, FormatPdf, revisionId, null, true, out urlToFile);

                return new ConverterData
                    {
                        StorageUrl = externalUri,
                        RevisionId = revisionId,
                        InvoiceId = data.ID,
                    };
            }
        }

        public File<int> GetConvertedFile(ConverterData data, DaoFactory daoFactory)
        {
            if (string.IsNullOrEmpty(data.StorageUrl) || string.IsNullOrEmpty(data.RevisionId))
            {
                return null;
            }
            
            string urlToFile;

            DocumentServiceConnector.GetConvertedUri(data.StorageUrl, FormatDocx, FormatPdf, data.RevisionId, null, true, out urlToFile);

            if (string.IsNullOrEmpty(urlToFile))
            {
                return null;
            }

            var invoice = DaoFactory.GetInvoiceDao().GetByID(data.InvoiceId);

            return SaveFile(invoice, urlToFile, daoFactory);
        }

        private File<int> SaveFile(Invoice data, string url, DaoFactory daoFactory)
        {
            File<int> file = null;

            var request = (HttpWebRequest)WebRequest.Create(url);

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var document = ServiceProvider.GetService<File<int>>();

                        document.Title = string.Format("{0}{1}", data.Number, FormatPdf);
                        document.FolderID = DaoFactory.GetFileDao().GetRoot();
                        document.ContentLength = response.ContentLength;

                        if (data.GetInvoiceFile(daoFactory) != null)
                        {
                            document.ID = data.FileID;
                        }

                        file = DaoFactory.GetFileDao().SaveFile(document, stream);
                    }
                }
            }

            return file;
        }

        private Stream GetStreamDocx(Invoice data)
        {
            var invoiceData = InvoiceFormattedData.GetData(data, 0, 0);
            var logo = new byte[] {};

            if (!string.IsNullOrEmpty(invoiceData.LogoBase64))
            {
                logo = Convert.FromBase64String(invoiceData.LogoBase64);
            }
            else if (invoiceData.LogoBase64Id != 0)
            {
                logo = Convert.FromBase64String(OrganisationLogoManager.GetOrganisationLogoBase64(invoiceData.LogoBase64Id));
            }

            using (var zip = ZipFile.Read(Template))
            {
                var documentXmlStream = new MemoryStream();
                foreach (var entry in zip.Entries.Where(entry => entry.FileName == DocumentXml))
                {
                    entry.Extract(documentXmlStream);
                }
                documentXmlStream.Position = 0;
                zip.RemoveEntry(DocumentXml);
                var document = new XmlDocument();
                document.Load(documentXmlStream);
                var documentStr = GenerateDocumentXml(document, invoiceData, logo);
                zip.AddEntry(DocumentXml, documentStr, Encoding.UTF8);

                if (logo.Length > 0)
                {
                    zip.UpdateEntry(DocumentLogoImage, logo);
                }

                var streamZip = new MemoryStream();
                zip.Save(streamZip);
                streamZip.Seek(0, SeekOrigin.Begin);
                streamZip.Flush();
                return streamZip;
            }

        }

        private string GenerateDocumentXml(XmlDocument xDocument, InvoiceFormattedData data, byte[] logo)
        {
            XmlNodeList nodeList;
            XmlNode parent;
            XmlNode child;


            #region Seller

            nodeList = xDocument.SelectNodes("//*[@ascid='seller']");
            parent = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
            if (parent != null)
            {
                if (data.Seller == null)
                {
                    parent.RemoveAll();
                }
                else
                {
                    var newText = parent.CloneNode(true).OuterXml;
                    newText = newText
                        .Replace("${label}", EncodeAndReplaceLineBreaks(data.Seller.Item1))
                        .Replace("${value}", EncodeAndReplaceLineBreaks(data.Seller.Item2));
                    var newEl = new XmlDocument();
                    newEl.LoadXml(newText);
                    if (parent.ParentNode != null)
                    {
                        if (newEl.DocumentElement != null)
                        {
                            parent.ParentNode.InsertBefore(xDocument.ImportNode(newEl.DocumentElement, true), parent);
                        }
                        parent.ParentNode.RemoveChild(parent);
                    }
                }
            }

            #endregion


            #region Logo

            nodeList = xDocument.SelectNodes("//*[@ascid='logo']");
            parent = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
            if (parent != null)
            {
                if (logo.Length <= 0)
                {
                    parent.RemoveAll();
                }
                else
                {
                    using (var stream = new MemoryStream(logo))
                    using (var img = System.Drawing.Image.FromStream(stream))
                    {
                        var cx = img.Width*9525; //1px =  9525emu
                        var cy = img.Height*9525; //1px =  9525emu

                        var newText = parent.CloneNode(true).OuterXml;
                        newText = newText
                            .Replace("${width}", cx.ToString(CultureInfo.InvariantCulture))
                            .Replace("${height}", cy.ToString(CultureInfo.InvariantCulture));
                        var newEl = new XmlDocument();
                        newEl.LoadXml(newText);
                        if (parent.ParentNode != null)
                        {
                            if (newEl.DocumentElement != null)
                            {
                                parent.ParentNode.InsertBefore(xDocument.ImportNode(newEl.DocumentElement, true),
                                                               parent);
                            }
                            parent.ParentNode.RemoveChild(parent);
                        }
                    }
                }
            }

            #endregion


            #region Number

            nodeList = xDocument.SelectNodes("//*[@ascid='number']");
            parent = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
            if (parent != null)
            {
                if (data.Number == null)
                {
                    parent.RemoveAll();
                }
                else
                {
                    var newText = parent.CloneNode(true).OuterXml;
                    newText = newText
                        .Replace("${label}", EncodeAndReplaceLineBreaks(data.Number.Item1))
                        .Replace("${value}", EncodeAndReplaceLineBreaks(data.Number.Item2));
                    var newEl = new XmlDocument();
                    newEl.LoadXml(newText);
                    if (parent.ParentNode != null)
                    {
                        if (newEl.DocumentElement != null)
                        {
                            parent.ParentNode.InsertBefore(xDocument.ImportNode(newEl.DocumentElement, true), parent);
                        }
                        parent.ParentNode.RemoveChild(parent);
                    }
                }
            }

            #endregion


            #region Invoice

            nodeList = xDocument.SelectNodes("//*[@ascid='invoice']");
            parent = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
            if (parent != null)
            {
                nodeList = xDocument.SelectNodes("//*[@ascid='invoice']//*[@ascid='row']");
                child = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
                if (child != null)
                {
                    if (data.Invoice == null || data.Invoice.Count <= 0)
                    {
                        if (parent.ParentNode != null)
                        {
                            parent.ParentNode.RemoveChild(parent);
                        }
                    }
                    else
                    {
                        foreach (var line in data.Invoice)
                        {
                            var newText = child.CloneNode(true).OuterXml;
                            newText = newText
                                .Replace("${label}", EncodeAndReplaceLineBreaks(line.Item1))
                                .Replace("${value}", EncodeAndReplaceLineBreaks(line.Item2));
                            var newEl = new XmlDocument();
                            newEl.LoadXml(newText);
                            if (newEl.DocumentElement != null)
                            {
                                parent.InsertBefore(xDocument.ImportNode(newEl.DocumentElement, true), child);
                            }
                        }
                        parent.RemoveChild(child);
                    }
                }
            }

            #endregion


            #region Customer

            nodeList = xDocument.SelectNodes("//*[@ascid='customer']");
            parent = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
            if (parent != null)
            {
                if (data.Customer == null)
                {
                    if (parent.ParentNode != null)
                    {
                        parent.ParentNode.RemoveChild(parent);
                    }
                }
                else
                {
                    var newText = parent.CloneNode(true).OuterXml;
                    newText = newText
                        .Replace("${label}", EncodeAndReplaceLineBreaks(data.Customer.Item1))
                        .Replace("${value}", EncodeAndReplaceLineBreaks(data.Customer.Item2));
                    var newEl = new XmlDocument();
                    newEl.LoadXml(newText);
                    if (parent.ParentNode != null)
                    {
                        if (newEl.DocumentElement != null)
                        {
                            parent.ParentNode.InsertBefore(xDocument.ImportNode(newEl.DocumentElement, true), parent);
                        }
                        parent.ParentNode.RemoveChild(parent);
                    }
                }
            }

            #endregion


            nodeList = xDocument.SelectNodes("//*[@ascid='table']");
            parent = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
            if (parent != null)
            {
                #region TableHeaderRow

                nodeList = xDocument.SelectNodes("//*[@ascid='table']//*[@ascid='headerRow']");
                child = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
                if (child != null)
                {
                    if (data.TableHeaderRow == null || data.TableHeaderRow.Count <= 0)
                    {
                        if (parent.ParentNode != null)
                            parent.ParentNode.RemoveChild(parent);
                    }
                    else
                    {
                        var newText = child.CloneNode(true).OuterXml;
                        for (var i = 0; i < data.TableHeaderRow.Count; i++)
                        {
                            newText = newText
                                .Replace("${label" + i + "}", EncodeAndReplaceLineBreaks(data.TableHeaderRow[i]));
                        }
                        var newEl = new XmlDocument();
                        newEl.LoadXml(newText);
                        if (newEl.DocumentElement != null)
                        {
                            parent.InsertBefore(xDocument.ImportNode(newEl.DocumentElement, true), child);
                        }
                        parent.RemoveChild(child);
                    }
                }

                #endregion


                #region TableBodyRows

                nodeList = xDocument.SelectNodes("//*[@ascid='table']//*[@ascid='bodyRow']");
                child = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
                if (child != null)
                {
                    if (data.TableBodyRows == null || data.TableBodyRows.Count <= 0)
                    {
                        if (parent.ParentNode != null)
                            parent.ParentNode.RemoveChild(parent);
                    }
                    else
                    {
                        foreach (var line in data.TableBodyRows)
                        {
                            var newText = child.CloneNode(true).OuterXml;
                            for (var i = 0; i < line.Count; i++)
                            {
                                newText = newText
                                    .Replace("${value" + i + "}", EncodeAndReplaceLineBreaks(line[i]));
                            }
                            var newEl = new XmlDocument();
                            newEl.LoadXml(newText);
                            if (newEl.DocumentElement != null)
                            {
                                parent.InsertBefore(xDocument.ImportNode(newEl.DocumentElement, true), child);
                            }
                        }
                        parent.RemoveChild(child);
                    }
                }

                #endregion


                #region TableFooterRows

                nodeList = xDocument.SelectNodes("//*[@ascid='table']//*[@ascid='footerRow']");
                child = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
                if (child != null)
                {
                    if (data.TableFooterRows == null || data.TableFooterRows.Count <= 0)
                    {
                        if (parent.ParentNode != null)
                            parent.ParentNode.RemoveChild(parent);
                    }
                    else
                    {
                        foreach (var line in data.TableFooterRows)
                        {
                            var newText = child.CloneNode(true).OuterXml;
                            newText = newText
                                .Replace("${label}", EncodeAndReplaceLineBreaks(line.Item1))
                                .Replace("${value}", EncodeAndReplaceLineBreaks(line.Item2));
                            var newEl = new XmlDocument();
                            newEl.LoadXml(newText);
                            if (newEl.DocumentElement != null)
                            {
                                parent.InsertBefore(xDocument.ImportNode(newEl.DocumentElement, true), child);
                            }
                        }
                        parent.RemoveChild(child);
                    }
                }

                #endregion


                #region TableTotalRow

                nodeList = xDocument.SelectNodes("//*[@ascid='table']//*[@ascid='totalRow']");
                child = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
                if (child != null)
                {
                    if (data.TableTotalRow == null)
                    {
                        if (parent.ParentNode != null)
                            parent.ParentNode.RemoveChild(parent);
                    }
                    else
                    {
                        var newText = child.CloneNode(true).OuterXml;
                        newText = newText
                            .Replace("${label}", EncodeAndReplaceLineBreaks(data.TableTotalRow.Item1))
                            .Replace("${value}", EncodeAndReplaceLineBreaks(data.TableTotalRow.Item2));
                        var newEl = new XmlDocument();
                        newEl.LoadXml(newText);
                        if (newEl.DocumentElement != null)
                        {
                            parent.InsertBefore(xDocument.ImportNode(newEl.DocumentElement, true), child);
                        }
                        parent.RemoveChild(child);
                    }
                }

                #endregion
            }


            #region Terms

            nodeList = xDocument.SelectNodes("//*[@ascid='terms']");
            parent = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
            if (parent != null)
            {
                if (data.Terms == null)
                {
                    if (parent.ParentNode != null)
                        parent.ParentNode.RemoveChild(parent);
                }
                else
                {
                    var newText = parent.CloneNode(true).OuterXml;
                    newText = newText
                        .Replace("${label}", EncodeAndReplaceLineBreaks(data.Terms.Item1))
                        .Replace("${value}", EncodeAndReplaceLineBreaks(data.Terms.Item2));
                    var newEl = new XmlDocument();
                    newEl.LoadXml(newText);
                    if (parent.ParentNode != null)
                    {
                        if (newEl.DocumentElement != null)
                        {
                            parent.ParentNode.InsertBefore(xDocument.ImportNode(newEl.DocumentElement, true), parent);
                        }
                        parent.ParentNode.RemoveChild(parent);
                    }
                }
            }

            #endregion


            #region Notes

            nodeList = xDocument.SelectNodes("//*[@ascid='notes']");
            parent = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
            if (parent != null)
            {
                if (data.Notes == null)
                {
                    if (parent.ParentNode != null)
                        parent.ParentNode.RemoveChild(parent);
                }
                else
                {
                    var newText = parent.CloneNode(true).OuterXml;
                    newText = newText
                        .Replace("${label}", EncodeAndReplaceLineBreaks(data.Notes.Item1))
                        .Replace("${value}", EncodeAndReplaceLineBreaks(data.Notes.Item2));
                    var newEl = new XmlDocument();
                    newEl.LoadXml(newText);
                    if (parent.ParentNode != null)
                    {
                        if (newEl.DocumentElement != null)
                        {
                            parent.ParentNode.InsertBefore(xDocument.ImportNode(newEl.DocumentElement, true), parent);
                        }
                        parent.ParentNode.RemoveChild(parent);
                    }
                }
            }

            #endregion


            #region Consignee

            nodeList = xDocument.SelectNodes("//*[@ascid='consignee']");
            parent = nodeList != null && nodeList.Count > 0 ? nodeList[0] : null;
            if (parent != null)
            {
                if (data.Consignee == null)
                {
                    if (parent.ParentNode != null)
                    {
                        parent.ParentNode.RemoveChild(parent);
                    }
                }
                else
                {
                    var newText = parent.CloneNode(true).OuterXml;
                    newText = newText
                        .Replace("${label}", EncodeAndReplaceLineBreaks(data.Consignee.Item1))
                        .Replace("${value}", EncodeAndReplaceLineBreaks(data.Consignee.Item2));
                    var newEl = new XmlDocument();
                    newEl.LoadXml(newText);
                    if (parent.ParentNode != null)
                    {
                        if (newEl.DocumentElement != null)
                        {
                            parent.ParentNode.InsertBefore(xDocument.ImportNode(newEl.DocumentElement, true), parent);
                        }
                        parent.ParentNode.RemoveChild(parent);
                    }
                }
            }

            #endregion


            return xDocument.InnerXml;
        }

        private string EncodeAndReplaceLineBreaks(string str)
        {
            return str
                .Replace("&", "&amp;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("\r\n", "</w:t><w:br/><w:t xml:space=\"preserve\">")
                .Replace("\n", "</w:t><w:br/><w:t xml:space=\"preserve\">")
                .Replace("\r", "</w:t><w:br/><w:t xml:space=\"preserve\">");
        }
    }

    public class ConverterData
    {
        public string StorageUrl { get; set; }
        public string RevisionId { get; set; }
        public int InvoiceId { get; set; }
        public int FileId { get; set; }
    }


    public static class PdfCreatorExtention
    {
        public static DIHelper AddPdfCreatorService(this DIHelper services)
        {
            services.TryAddScoped<PdfCreator>();

            return services.AddPathProviderService()
                           .AddDocumentServiceConnectorService()
                           .AddOrganisationLogoManagerService()
                         //  .AddDaoFactoryService()
                           .AddInvoiceFormattedDataService();
        }
    }
}    