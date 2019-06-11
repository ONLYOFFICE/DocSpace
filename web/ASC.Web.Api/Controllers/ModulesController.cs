using System.Collections.Generic;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.WebZones;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Api.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        // GET: api/Modules
        [FormatIndexRoute()]
        [FormatIndexRoute(false)]
        public IEnumerable<Module> GetAll()
        {
            var result = new List<Module>(){
                new Module {
                    Title = "Documents",
                    Link = "/products/files/",
                    ImageUrl = "images/documents240.png",
                    Description = "Create, edit and share documents. Collaborate on them in real-time. 100% compatibility with MS Office formats guaranteed.",
                    IsPrimary = true
                },
                new Module {
                    Title = "Projects",
                    Link = "products/projects/",
                    ImageUrl = "images/projects_logolarge.png"
                },
                new Module {
                    Title = "Crm",
                    Link = "/products/crm/",
                    ImageUrl = "images/crm_logolarge.png"
                },
                new Module {
                    Title = "Mail",
                    Link = "/products/mail/",
                    ImageUrl = "images/mail_logolarge.png"
                },
                new Module {
                    Title = "Community",
                    Link = "products/community/",
                    ImageUrl = "images/community_logolarge.png"
                }
            };

            foreach (var a in WebItemManager.Instance.GetItems(WebZoneType.StartProductList))
            {
                result.Add(new Module() {
                    Title = a.Name,
                    Description = a.Description,
                    ImageUrl = a.Context.LargeIconFileName,
                    Link = a.StartURL
                });
            }


            return result;
        }
    }
}
