


namespace ASC.Core.Common.Resources
{
    public class Translation
    {
        private static JsonResourceManager JsonResourceManager { get; set; }
        static Translation()
        {
            JsonResourceManager = new JsonResourceManager("translation");
        }

        public static string Title
        {
            get
            {
                return JsonResourceManager.GetString("title");
            }
        }
        public static ModuleData Module 
        {
            get
            {
                return new ModuleData();
            }
        }
        public class ModuleData
        {
            public HomeData Home
            {
                get
                {
                    return new HomeData();
                }
            }
            public class HomeData
            {

            }
            public DocumentsData Documents
            {
                get
                {
                    return new DocumentsData();
                }
            }
            public class DocumentsData
            {
                public string Title
                {
                    get
                    {
                        return JsonResourceManager.GetString("module.documents.title");
                    }
                }
                public string Description
                {
                    get
                    {
                        return JsonResourceManager.GetString("module.documents.description");
                    }
                }

            }
            public ProjectsData Projects
            {
                get
                {
                    return new ProjectsData();
                }
            }
            public class ProjectsData
            {
                public string Title
                {
                    get
                    {
                        return JsonResourceManager.GetString("module.projects.title");
                    }
                }

            }
            public CrmData Crm
            {
                get
                {
                    return new CrmData();
                }
            }
            public class CrmData
            {
                public string Title
                {
                    get
                    {
                        return JsonResourceManager.GetString("module.crm.title");
                    }
                }

            }
            public MailData Mail
            {
                get
                {
                    return new MailData();
                }
            }
            public class MailData
            {
                public string Title
                {
                    get
                    {
                        return JsonResourceManager.GetString("module.mail.title");
                    }
                }

            }
            public PeopleData People
            {
                get
                {
                    return new PeopleData();
                }
            }
            public class PeopleData
            {
                public string Title
                {
                    get
                    {
                        return JsonResourceManager.GetString("module.People.title");
                    }
                }

            }
            public CommunityData Community
            {
                get
                {
                    return new CommunityData();
                }
            }
            public class CommunityData
            {
                public string Title
                {
                    get
                    {
                        return JsonResourceManager.GetString("module.community.title");
                    }
                }

            }

        }

        
    }
}
