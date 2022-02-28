global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Reflection;
global using System.Text;
global using System.Threading;
global using System.Web;

global using ASC.Common;
global using ASC.Common.Mapping;
global using ASC.Core;
global using ASC.Core.Common;
global using ASC.Core.Common.Configuration;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.EF.Context;
global using ASC.Core.Common.EF.Model;
global using ASC.Core.Tenants;
global using ASC.VoipService.Dao;
global using ASC.VoipService.Mappings;
global using ASC.VoipService.Twilio;

global using AutoMapper;

global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;
global using Newtonsoft.Json.Serialization;

global using Twilio.Clients;
global using Twilio.Exceptions;
global using Twilio.Jwt;
global using Twilio.Jwt.Client;
global using Twilio.Rest.Api.V2010.Account;
global using Twilio.Rest.Api.V2010.Account.AvailablePhoneNumberCountry;
global using Twilio.Rest.Api.V2010.Account.Queue;
global using Twilio.TwiML;
global using Twilio.Types;
