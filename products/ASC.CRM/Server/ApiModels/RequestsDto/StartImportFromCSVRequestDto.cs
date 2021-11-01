using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.ApiModels
{
    public class StartImportFromCSVRequestDto
    {
        public string CsvFileURI { get; set; }
        public string JsonSettings { get; set; }
    }
}
