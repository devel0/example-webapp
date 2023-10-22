global using System.Net;
global using System.Net.Http.Headers;
global using System.Net.Http.Json;
global using System.Data.Common;
global using System.IdentityModel.Tokens.Jwt;
global using System.Text;
global using System.Security.Claims;
global using System.Security.Cryptography;

global using Microsoft.Extensions.Hosting;
global using Microsoft.AspNetCore.Http.Json;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Logging;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Configuration;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.IdentityModel.Tokens;

global using Xunit;
global using Newtonsoft.Json;

global using ExampleWebApp.Backend.WebApi;
global using ExampleWebApp.Backend.WebApi.Types;
global using static ExampleWebApp.Backend.WebApi.Constants;
global using static ExampleWebApp.Backend.WebApi.Toolkit;
global using static ExampleWebApp.Backend.Tests.Integration.Toolkit;
global using static ExampleWebApp.Backend.Tests.Integration.Constants;