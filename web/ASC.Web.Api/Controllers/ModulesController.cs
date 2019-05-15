using System.Collections.Generic;
using ASC.Web.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        // GET: api/Modules
        [HttpGet]
        public IEnumerable<Module> Get()
        {
            return new List<Module> {
                new Module {
                    Title = "People",
                    Link = "/products/people/",
                    Image = "people_logolarge.png",
                    Description = "Manage portal employees.",
                    IsPrimary = true
                }
            };

            /*return new List<Module> {
                new Module {
                    Title = "Documents",
                    Link = "/products/files/",
                    Image = "documents240.png",
                    Description = "Create, edit and share documents. Collaborate on them in real-time. 100% compatibility with MS Office formats guaranteed.",
                    IsPrimary = true
                },
                new Module {
                    Title = "Projects",
                    Link = "products/projects/",
                    Image = "projects_logolarge.png"
                },
                new Module {
                    Title = "Crm",
                    Link = "/products/crm/",
                    Image = "crm_logolarge.png"
                },
                new Module {
                    Title = "Mail",
                    Link = "/products/mail/",
                    Image = "mail_logolarge.png"
                },
                new Module {
                    Title = "People",
                    Link = "/products/people/",
                    Image = "people_logolarge.png"
                },
                new Module {
                    Title = "Community",
                    Link = "products/community/",
                    Image = "community_logolarge.png"
                }
            };*/
        }
    }
}
