using System.Collections.Generic;

using ASC.Core.Common.Configuration;
using ASC.Web.Studio.UserControls.Management;

namespace ASC.Web.Api.Models
{
    public class AuthServiceModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Instruction { get; set; }
        public bool CanSet { get; set; }
        public List<AuthKey> Props { get; set; }

        public AuthServiceModel()
        {

        }

        public AuthServiceModel(Consumer consumer)
        {
            var authService = new AuthService(consumer);

            Name = authService.Name;
            Title = authService.Title;
            Instruction = authService.Instruction;
            CanSet = authService.CanSet;

            if (consumer.CanSet)
            {
                Props = authService.Props;
                CanSet = authService.CanSet;
            }
        }
    }
}
