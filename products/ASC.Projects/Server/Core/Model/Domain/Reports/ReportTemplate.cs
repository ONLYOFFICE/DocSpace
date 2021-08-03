/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.IO;

using ASC.Common;
using ASC.Core.Common.Settings;
using ASC.Projects.Engine;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Utils;

namespace ASC.Projects.Core.Domain.Reports
{
    public class ReportTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ReportType ReportType { get; set; }
        public TaskFilter Filter { get; set; }
        public string Cron { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public bool AutoGenerated { get; set; }
        public int Tenant { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            var t = obj as ReportTemplate;
            return t != null && Id.Equals(t.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    [Scope]
    public class ReportTemplateHelper { 
        private SettingsManager SettingsManager { get; set; }
        private FileUploader FileUploader { get; set; }
        private GlobalFolderHelper GlobalFolderHelper { get; set; }
        private ReportTemplate ReportTemplate { get; set; }
        private EngineFactory EngineFactory { get; set; }

        public ReportTemplateHelper(SettingsManager settingsManager, FileUploader fileUploader, GlobalFolderHelper globalFolderHelper, EngineFactory engineFactory)
        {
            SettingsManager = settingsManager;
            FileUploader = fileUploader;
            GlobalFolderHelper = globalFolderHelper;
            EngineFactory = engineFactory;
        }
        public ReportTemplate GetReportTemplate(ReportType reportType)
        {
            if (ReportTemplate == null) 
            { 
                ReportTemplate = new ReportTemplate();
                ReportTemplate.ReportType = reportType;
                ReportTemplate.AutoGenerated = false;
                return ReportTemplate;
            }
            else
            {
                return ReportTemplate;
            }
        }

        internal void SaveDocbuilderReport(ReportState state, string url)
        {
            if (ReportTemplate == null) throw new NullReferenceException();
            var data = new System.Net.WebClient().DownloadData(url);

            using (var memStream = new MemoryStream(data))
            {
                Action<Stream> action = stream =>
                {
                    var file = FileUploader.Exec(SettingsManager.Load<ProjectsCommonSettings>().FolderId, state.FileName, stream.Length, stream, true);
                    state.FileId = (int)file.ID;
                };

                try
                {
                    action(memStream);
                }
                catch (DirectoryNotFoundException)
                {
                    var settings = SettingsManager.LoadForCurrentUser<ProjectsCommonSettings>();
                    settings.FolderId = GlobalFolderHelper.FolderMy;
                    SettingsManager.SaveForCurrentUser(settings);

                    action(memStream);
                }
            }

            EngineFactory.GetReportEngine().Save(new ReportFile
            {
                FileId = state.FileId,
                Name = ReportTemplate.Name,
                ReportType = ReportTemplate.ReportType
            });
        }

        internal void SaveDocbuilderReport(ReportState state, string url, ReportTemplate report)
        {
            var data = new System.Net.WebClient().DownloadData(url);

            using (var memStream = new MemoryStream(data))
            {
                Action<Stream> action = stream =>
                {
                    var file = FileUploader.Exec(SettingsManager.Load<ProjectsCommonSettings>().FolderId, state.FileName, stream.Length, stream, true);
                    state.FileId = (int)file.ID;
                };

                try
                {
                    action(memStream);
                }
                catch (DirectoryNotFoundException)
                {
                    var settings = SettingsManager.LoadForCurrentUser<ProjectsCommonSettings>();
                    settings.FolderId = GlobalFolderHelper.FolderMy;
                    SettingsManager.SaveForCurrentUser(settings);

                    action(memStream);
                }
            }

            EngineFactory.GetReportEngine().Save(new ReportFile
            {
                FileId = state.FileId,
                Name = report.Name,
                ReportType = report.ReportType
            });
        }
    }
}
