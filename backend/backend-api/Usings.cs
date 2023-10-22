global using System.Reflection;
global using System.Net;
global using System.Text.Json.Serialization;
global using System.Diagnostics;
global using System.Security.Claims;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Cryptography;
global using System.ComponentModel;
global using System.Net.Http.Headers;
global using System.Web;
global using System.Data.Common;

global using Microsoft.EntityFrameworkCore.Migrations;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.OpenApi.Models;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.Data.Sqlite;
global using Microsoft.Net.Http.Headers;
global using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

global using Serilog;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

global using static ExampleWebApp.Backend.WebApi.Constants;
global using static ExampleWebApp.Backend.WebApi.Toolkit;
global using ExampleWebApp.Backend.WebApi;
global using ExampleWebApp.Backend.WebApi.Types;
global using ExampleWebApp.Backend.Data;
global using ExampleWebApp.Backend.Data.Types;