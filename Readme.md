In this repo, I followed CodeMaze's [ASP.NET Core Series:](https://code-maze.com/net-core-series/) to learn .NET.

## Chapter 1: 

## Chapter 2: 

## Chapter 3: 

## Chapter 4: 

## Chapter 5: Global Error Handling

- Add ConfigureExceptionHandler, reigister with Program
- With the handler, we can remove try-catch block in all classes

## Chapter 6: Getting Additional Resources

- Getting single record: Company
- Defining a customer exception: CompanyNotFoundException, register it in ExceptionMiddlewareExtensions
- If the return is null, throw CompanyNotFoundException
- Repeat for Employee repository, service, controller

## Chapter 7: Content Negotiation

- The default return format is json
- Add RespectBrowserAcceptHeader to allow content negotiation, return xml
- Add  ReturnHttpNotAcceptable to return 406 when reqeust media type is not available
- Implementing customer formatter: CsvOutputFormatter, register to service extension, Program

## Chapter 8: Method Safety and Method Idempotency

- Safety: the resources shouldn't be changed after the method is executed
- Idempotency: calling a method multiple times with the same result

## Chapter 9: Creating Resources

- Using POST method to create resources, CreatedAtRoute method
- [ApiController] attribute's behaviors
- To enable custom response, add 
  
  ` builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
`
- Creating children resources together with a parent
- Creating a collection of resources
- Model Binding
 
## Chapter 10: Working with DELETE Request

- Deleting a record
- Deleting a parent resource with its children

## Chapter 11: Working with PUT Request

- Updating a record
- Inserting children resources with parent

## Chapter 12: Working with PATCH Reqeust

- PATCH updates a record partially, PUT updates the whole record
- PATCH: [FromBody]JsonPatchDocument<Company>, PUT: [FromBody]Company
- PATCH request's media type: application/json-patch+json, PATCH request's media type: application/json
- PATCH request body: 
  ``` json
  [
    {
    "op": "replace",
    "path": "/name",
    "value": "new name"
    },
    {
    "op": "remove",
    "path": "/name"
    }
  ]
  ```
- There are six different operations for a PATCH request:
  ![](./img/PATCH.PNG)
- Configuring support for json patch using Newtonsoft.Json while leaving the other formatters unchanged
- With ReverseMap(), we can use map in reverse way. `CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap()`

## Chapter 13: Validation

- Model State, Rerun validation, `ModelState.ClearValidationState(), TryValidateModel(), UnprocessableEntity()`
- The most used built-in attributes: 
  ![](./img/built_in_validation.png)
  [Complete list](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=net-5.0)
- Creating custom attribute with `ValidationAttribute, IValidatableObject`
- Adding annotation in Dto to apply built-in validation, `Required, MaxLength, Range`

## Chapter 14: Asynchronous Code

- `async` keywoard is to enable the `await` within the method
- `await` performs an asynchronous wait on its argument. If a method needs time to finish, the `await` keyword will pause the method execution and return an incomplete task. The `await` keyword does three things:
  - It helps us extract the result from the async operation – we already
learned about that
  - Validates the success of the operation
  - Provides the Continuation for executing the rest of the code in the
async method
- `Task` represents an execution of the asynchronous method and not the result. The `Task` has several properties that indicate whether the operation was completed successfully or not (`Status, IsCompleted, IsCanceled, IsFaulted`). With these
properties, we can track the flow of our async operations. This is also called TAP (Task-based Asynchronous Pattern).
- In asynchronous programming, we have three return tyeps:
  - `Task<TResult>`, for an async method that returns a value
  - `Task`, for an async method that does not return a value
  - `void`, which we can use for an event handler. We should
use void only for the asynchronous event handlers which require
a void return type. Other than that, we should always return a Task
- Refactoring repository, service, controller to asychronous

## Chapter 15: Action Filters

- There are different filter types
  - **Authorization filters** – They run first to determine whether a user
is authorized for the current request
  - **Resource filters** – They run right after the authorization filters and
are very useful for caching and performance
  - **Action filters** – They run right before and after action method
execution
  - **Exception filters** – They are used to handle exceptions before the
response body is populated
  - **Result filters** – They run before and after the execution of the
action methods result
- Synchronous Action filter that runs before and after
action method execution:
  ``` c#
  namespace ActionFilters.Filters
  {
    public class ActionFilterExample : IActionFilter
    {
      public void OnActionExecuting(ActionExecutingContext context)
      {
      // our code before action executes
      }
      public void OnActionExecuted(ActionExecutedContext context)
      {
      // our code after action executes
      }
    }
  }
  ```
- Asynchronous filter, we only have one method to implement the OnActionExecutionAsync.
  ``` c#
  namespace ActionFilters.Filters
  {
    public class AsyncActionFilterExample : IAsyncActionFilter
    {
      public async Task OnActionExecutionAsync(ActionExecutingContext context,
      ActionExecutionDelegate next)
      {
      // execute any code before the action executes
      var result = await next();
      // execute any code after the action executes
      }
    }
  }
  ```
- The action filter can be added to different scope levels: Global, Action, and Controller
  - If we want to use our filter globally, we need to register it inside the AddControllers() method in the Program class
    ``` c#
    builder.Services.AddControllers(config =>
    {
    config.Filters.Add(new GlobalFilterExample());
    });
    ```
  - On the Action or Controller level, we need to register it, but as a service in the IoC container:
    ``` c#
    builder.Services.AddScoped<ActionFilterExample>();
    builder.Services.AddScoped<ControllerFilterExample>();
    ```
  - To use a filter registered on the Action or Controller level, we need to place it on top of the Controller or Action as a ServiceType:
    ``` c#
    namespace AspNetCore.Controllers
    {
      [ServiceFilter(typeof(ControllerFilterExample))]
      [Route("api/[controller]")]
      [ApiController]
      public class TestController : ControllerBase
      {
        [HttpGet]
        [ServiceFilter(typeof(ActionFilterExample))]
        public IEnumerable<string> Get()
        {
          return new string[] { "example", "data" };
        }
      }
    }
    ```
- The order in which our filters are executed is as follows:
  ![](.img/../img/ActionFilterOrder.png)
  - we can change the order of invocation by adding the Order property to the invocation statement:
    ``` c#
    namespace AspNetCore.Controllers
    {
      [ServiceFilter(typeof(ControllerFilterExample), Order = 2)]
      [Route("api/[controller]")]
      [ApiController]
      public class TestController : ControllerBase
      {
        [HttpGet]
        [ServiceFilter(typeof(ActionFilterExample), Order = 1)]
        public IEnumerable<string> Get()
        {
          return new string[] { "example", "data" };
        }
      }
    }
    ```
    ``` c#
    [HttpGet]
    [ServiceFilter(typeof(ActionFilterExample), Order = 2)]
    [ServiceFilter(typeof(ActionFilterExample2), Order = 1)]
    public IEnumerable<string> Get()
    {
      return new string[] { "example", "data" };
    }
    ```

## Chapter 16: Paging

- Pageing means getting partial results from an API
  ``` c#
  public abstract class RequestParameters
      {
          const int maxPageSize = 50;
          public int PageNumber { get; set; } = 1;

          private int _pageSize = 10;
          public int PageSize 
          { 
              get 
              { 
                  return _pageSize;
              }
              set
              {
                  _pageSize = (value > maxPageSize) ? maxPageSize : value;
              }
          }
      }
  ```
- PagedList will inherit from the List class, we can also move the skip/take logic to the PagedList:
  ``` c#
  public class MetaData
  {
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }

		public bool HasPrevious => CurrentPage > 1;
		public bool HasNext => CurrentPage < TotalPages;
	}
  ```
  ``` c#
  public class PagedList<T> : List<T>
  {
    public MetaData MetaData { get; set; }
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        MetaData = new MetaData
        {
            TotalCount = count,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize),
        };

        AddRange(items);
    }

    public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
  }
  ```
- Implementing in repository, service, controller

## Chapter 17: Filtering

``` c#
public class EmployeeParameters : RequestParameters
    {
        public uint MinAge { get; set; }
        public uint MaxAge { get; set; } = int.MaxValue;
        public bool ValidAgeRange => MaxAge > MinAge;
    }
```
``` c#
var employees = await FindByCondition(e => e.CompanyId.Equals(companyId)
          && (e.Age >= employeeParameters.MinAge && 
          e.Age <= employeeParameters.MaxAge), trackChanges)
          .OrderBy(e => e.Name)
          .ToListAsync();
```

## Chapter 18: Searching

``` c#
var employees = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
                .Search(employeeParameters.SearchTerm)
                .OrderBy(e => e.Name)
                .ToListAsync();
```
``` c#
public static class RepositoryEmployeeExtensions
  {
      public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee> employees,
          uint minAge, uint maxAge) =>
          employees.Where(e => e.Age >= minAge && e.Age <= maxAge);

      public static IQueryable<Employee> Search(this IQueryable<Employee> employees, string searchTerm)
      {
          if (string.IsNullOrWhiteSpace(searchTerm))
              return employees;

          var lowerCaseTerm = searchTerm.Trim().ToLower();

          return employees.Where(e => e.Name.ToLower().Contains(lowerCaseTerm));
      }
  }
```

## Chapter 19: Sorting

``` c#
public class EmployeeParameters : RequestParameters
{
    public EmployeeParameters() => OrderBy = "name";

    public uint MinAge { get; set; }
    public uint MaxAge { get; set; } = int.MaxValue;
    public bool ValidAgeRange => MaxAge > MinAge;

    public string? SearchTerm { get; set; }
}
```
``` c#
public static IQueryable<Employee> Sort(this IQueryable<Employee> employees, string orderByQueryString)
{
    if (string.IsNullOrWhiteSpace(orderByQueryString))
        return employees.OrderBy(e => e.Name);

    var orderParams = orderByQueryString.Trim().Split(',');
    var propertyInfos = typeof(Employee).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    var orderQueryBuilder = new StringBuilder();

    foreach (var param in orderParams)
    {
        if (string.IsNullOrWhiteSpace(param))
            continue;

        var propertyFromQueryName = param.Split(" ")[0];
        var objectProperty = propertyInfos.FirstOrDefault(pi =>
        pi.Name.Equals(propertyFromQueryName, StringComparison.CurrentCultureIgnoreCase));

        if (objectProperty == null)
            continue;

        var direction = param.EndsWith(" desc") ? "descending" : "ascending";
        orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction},");
    }

    var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

    if (string.IsNullOrWhiteSpace(orderQuery))
        return employees.OrderBy(e => e.Name);

    return employees.OrderBy(orderQuery);
}
```

## *Chapter 20: Data Shaping

- Data shaping is a great way to reduce the amount of traffic sent from the API to the client. It enables the consumer of the API to select (shape) the data by choosing the fields through the query string. For example: https://localhost:5001/api/companies/companyId/employees?fields=name,age
- DataShaper, ExpandoObject, XML
  ``` c#
  using Contracts;
  using System.Dynamic;
  using System.Reflection;

  namespace Service.DataShaping
  {
      public class DataShaper<T> : IDataShaper<T> where T : class
      {
          public PropertyInfo[] Properties { get; set; }
          
          public DataShaper()
          {
              Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
          }

          public IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString)
          {
              var requiredProperties = GetRequiredProperties(fieldsString);
              
              return FetchData(entities, requiredProperties);
          }

          public ExpandoObject ShapeData(T entity, string fieldsString)
          {
              var requiredProperties = GetRequiredProperties(fieldsString);

              return FetchDataForEntity(entity, requiredProperties);
          }

          private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
          {
              var requiredProperties = new List<PropertyInfo>();

              if (!string.IsNullOrWhiteSpace(fieldsString))
              {
                  var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

                  foreach ( var field in fields)
                  {
                      var property = Properties.FirstOrDefault(pi =>
                      pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));

                      if (property == null)
                          continue;

                      requiredProperties.Add(property);
                  }
              }
              else
              {
                  requiredProperties = Properties.ToList();
              }

              return requiredProperties;
          }

          private IEnumerable<ExpandoObject> FetchData(IEnumerable<T> entities, 
              IEnumerable<PropertyInfo> requiredProperties)
          {
              var shapedData = new List<ExpandoObject>();

              foreach (var entity in entities)
              {
                  var shapedObject = FetchDataForEntity(entity, requiredProperties);
                  shapedData.Add(shapedObject);
              }

              return shapedData;
          }

          private ExpandoObject FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
          {
              var shapedObject = new ExpandoObject();

              foreach (var property in requiredProperties)
              {
                  var objectPropertyValue = property.GetValue(entity);
                  shapedObject.TryAdd(property.Name, objectPropertyValue);
              }

              return shapedObject;
          }
      }
  }
  ```

## *Chapter 21: Supporting HATEOAS

- Whta is HATEOAS
- Links
- We can create a custom media type. A custom media type should look something like this:
`application/vnd.codemaze.hateoas+json`.
  - vnd – vendor prefix; it’s always there
  - codemaze – vendor identifier
  - hateoas – media type name
  - json – suffix; we can use it to describe if we want json or an XML response

## Chapter 22: Working with OPTIONS and HEAD Requests

- The Options request can be used to request information on the communication options available upon a certain URI. Basically, Options should inform us whether we can Get a resource or execute any other action (POST, PUT, or DELETE). All of the options should be returned in the Allow header of the response as a commaseparated list of methods.
``` c#
[HttpOptions]
public IActionResult GetCompaniesOptions()
{
    Response.Headers.Add("Allow", "GET, OPTIONS, POST");

    return Ok();
}
```
- The Head is identical to Get but without a response body. This type of request could be used to obtain information about validity, accessibility, and recent modifications of the resource. All we have to do is add the HttpHead attribute below HttpGet.
``` c#
using Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;

        public RootController(LinkGenerator linkGenerator) =>_linkGenerator = linkGenerator;

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
        {
            if (mediaType.Contains("application/vnd.codemaze.apiroot"))
            {
				var list = new List<Link>
				{
					new Link
					{
						Href = _linkGenerator.GetUriByName(HttpContext, nameof(GetRoot), new {}),
						Rel = "self",
						Method = "GET"
					},
					new Link
					{
						Href = _linkGenerator.GetUriByName(HttpContext, "GetCompanies", new {}),
						Rel = "companies",
						Method = "GET"
					},
					new Link
					{
						Href = _linkGenerator.GetUriByName(HttpContext, "CreateCompany", new {}),
						Rel = "create_company",
						Method = "POST"
					}
				};

				return Ok(list);
			}

			return NoContent();
        }

    }
}
```

## Chapter 24: Versioning APIs

- ConfigureVersioning() with packege `Microsoft.AspNetCore.Mvc.Versioning`:
  ``` c#
  public static void ConfigureVersioning(this IServiceCollection services)
  {
      services.AddApiVersioning(opt =>
      {
          opt.ReportApiVersions = true;
          opt.AssumeDefaultVersionWhenUnspecified = true;
          opt.DefaultApiVersion = new ApiVersion(1, 0);
      });
  }
  ```
- `[ApiVersion("2.0")]`, request in uri: https://localhost:5001/api/companies?api-version=2.0
- Using URL versioning: 
  ``` c#
  [ApiVersion("2.0")]
  [Route("api/{v:apiversion}/companies")]
  ```
  https://localhost:5001/api/2.0/companies
- If we don’t want to change the URI of the API, we can send the version in the HTTP Header. `opt.ApiVersionReader = new HeaderApiVersionReader("api-version");`
- If we want to deprecate version of an API, but don’t want to remove it completely, we can use the Deprecated property for that purpose: `[ApiVersion("2.0", Deprecated = true)]`
- If we have a lot of versions of a single controller, we can assign these versions in the configuration instead. We can remove the `[ApiVersion]` attribute from the controllers.
  ``` c#
  opt.Conventions.Controller<CompaniesController>()
  .HasApiVersion(new ApiVersion(1, 0));
  opt.Conventions.Controller<CompaniesV2Controller>()
  .HasDeprecatedApiVersion(new ApiVersion(2, 0));
  ```

## Chapter 25: Caching

- There are three types of caches: 
  - Client Cache: lives on the client (browser); thus, it is a private cache. It is private because it is related to a single client.
  - Gateway Cache:  lives on the server and is a shared cache.
  - Proxy Cache: is also a shared cache, but it doesn’t live on the server nor the client side. It lives on the network.
- To cache some resources, we have to know whether or not it’s cacheable. The response header helps us with that. The one that is used most often is `Cache-Control: Cache-Control: max-age=180`.
- `[ResponseCache(Duration = 60)]`
- ConfigureResponseCaching()
  ``` c#
  public static void ConfigureResponseCaching(this IServiceCollection services) =>
              services.AddResponseCaching();
  ```
  ``` c#
  builder.Services.ConfigureResponseCaching();
  ```
  ``` c#
  app.UseCors("CorsPolicy");
  app.UseResponseCaching();
  ```
- `CacheProfiles`:
  ``` c#
  builder.Services.AddControllers(config => {
    ...
    config.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120});
  })...
  ```
  Adding `[ResponseCache(CacheProfileName = "120SecondsDuration")]` on top of the Companies controller. This cache rule will apply to all the actions inside the controller except the ones that already have the ResponseCache attribute applied.
- Validation Model: the value is caches, the second reqeust get value from cache store, but we don't know if the cased value is the latest value (someone could modified the value), so the cache server communicate with API server, if not modified, API server returns 304 Not Modified status, then the cached the value is served.
  ![](./img/caching_validation.png)
- Supporting Validation with package `Marvin.Cache.Headers`:
  ``` c#
  public static void ConfigureHttpCacheHeaders(this IServiceCollection services) => 
  services.AddHttpCacheHeaders();
  ```
  ```c#
  builder.Services.ConfigureHttpCacheHeaders();
  ```
  ```c#
  app.UseHttpCacheHeaders();
  ```
- Configuring our expiration and validation headers globally.
  ```c#
  public static void ConfigureHttpCacheHeaders(this IServiceCollection services) =>
      services.AddHttpCacheHeaders(
          (expirationOpt) =>
          {
              expirationOpt.MaxAge = 65;
              expirationOpt.CacheLocation = CacheLocation.Private;
          },
          (validationOpt) =>
          {
              validationOpt.MustRevalidate = true;
          }
          );
  ```
- Other than global configuration, we can apply it on the resource level (on action or controller). The overriding rules are the same. Configuration on the action level will override the configuration on the controller or global level. Also, the configuration on the controller level will override the global
level configuration.
- Configuring resource level configuration:
  ```c#
  [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
  [HttpCacheValidation(MustRevalidate = false)]
  ```
-  The `ResponseCaching` library doesn’t correctly implement the validation model, alternatives:
   -  Varnish - https://varnish-cache.org/
   -  Apache Traffic Server - https://trafficserver.apache.org/
   -  Squid - http://www.squid-cache.org/


## Chapter 26: Rate Limiting and Throttling

- To provide information about rate limiting, we use the response headers. They are separated between Allowed requests, which all start with the XRate-Limit and Disallowed requests. The Allowed requests header contains the following information :
  - X-Rate-Limit-Limit – rate limit period.
  - X-Rate-Limit-Remaining – number of remaining requests.
  - X-Rate-Limit-Reset – date/time information about resetting the request limit.
  - 
- For the disallowed requests, we use a 429 status code; that stands for too many requests.
- Implementing rate limiting with package `AspNetCoreRateLimit`.
  ```c#
  public static void ConfigureRateLimitingOptions(this IServiceCollection services)
  {
      var rateLimitRules = new List<RateLimitRule>
      {
          new RateLimitRule
          {
              Endpoint = "*",
              Limit = 3,
              Period = "5m"
          }
      };

      services.Configure<IpRateLimitOptions>(opt => { opt.GeneralRules = rateLimitRules; });
      services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
      services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
      services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
      services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
  }
  ```
  ```c#
  builder.Services.AddMemoryCache();
  builder.Services.ConfigureRateLimitingOptions();
  builder.Services.AddHttpContextAccessor();
  ```
  ```c#
  app.UseIpRateLimiting();
  ```

## Chapter 27: JWT, Identity, and Refresh Token

- Implementing Identity with package `Microsoft.AspNetCore.Identity.EntityFrameworkCore`.
  ```c#
  public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName {  get; set; }
    }
  ```
  Our class now inherits from the IdentityDbContext class and not DbContext because we want to integrate our context with Identity.
  ```c#
  public class RepositoryContext : IdentityDbContext<User>
    {
        public RepositoryContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        }

        public DbSet<Company>? Companies { get; set; }
        public DbSet<Employee>? Employees { get; set; }
    }
  ```
  ```c#
  public static void ConfigureIdentity(this IServiceCollection services)
  {
      var builder = services.AddIdentity<User, IdentityRole>(o =>
      {
          o.Password.RequireDigit = true;
          o.Password.RequireLowercase = false;
          o.Password.RequireUppercase = false;
          o.Password.RequireNonAlphanumeric = false;
          o.Password.RequiredLength = 10;
          o.User.RequireUniqueEmail = true;
      })
      .AddEntityFrameworkStores<RepositoryContext>()
      .AddDefaultTokenProviders();
  }
  ```
  ```c#
  builder.Services.AddAuthentication();
  builder.Services.ConfigureIdentity();
  ```
  ```c#
  app.UseAuthentication();
  ```
- Add-Migration CreatingIdentityTables, Update-Database
- RoleConfiguration
  ```c#
  public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
  {
      public void Configure(EntityTypeBuilder<IdentityRole> builder)
      {
          builder.HasData(
              new IdentityRole
              {
                  Name = "Manager",
                  NormalizedName = "MANAGER"
              },
              new IdentityRole
              {
                  Name = "Administrator",
                  NormalizedName = "ADMINISTRATOR"
              }
          );
      }
  }
  ```
- AuthenticationController
  ```c#
  [Route("api/authentication")]
  [ApiController]
  public class AuthenticationController : ControllerBase
  {
      private readonly IServiceManager _service;

      public AuthenticationController(IServiceManager service) => _service = service;

      [HttpPost]
      [ServiceFilter(typeof(ValidationFilterAttribute))]
      public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
      {
          var result = await _service.AuthenticationService.RegisterUser(userForRegistration);
          if (!result.Succeeded)
          {
              foreach(var error in result.Errors)
              {
                  ModelState.TryAddModelError(error.Code, error.Description);
              }
              return BadRequest(ModelState);
          }

          return StatusCode(201);
      }
  }
  ```
- JWT configuration
  - appsettings.json
    ```json
    "JwtSettings": {
      "validIssuer": "CodeMazeAPI",
      "validAudience":  "https://localhost:5001",
      "expires":  5
    },
    ```
    - Creating an environment variable: `setx SECRET "CodeMazeSecretKey" /M`
    - ConfigureJWT() 
    ```c#
    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = Environment.GetEnvironmentVariable("SECRET");

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });
    }
    ```
    ```c#
    builder.Services.ConfigureJWT(builder.Configuration);
    ```
- Adding `[Authorize]` above GetCompanies action, calling this api will receive 401
- Implimenting authentication
  ```c#
  public record UserForAuthenticationDto
  {
      [Required(ErrorMessage = "User name is required")]
      public string? UserName { get; init; }
      [Required(ErrorMessage = "Password name is required")]
      public string? Password { get; init; }
  }
  ```
  ```c#
  public interface IAuthenticationService
  {
      Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration);
      Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
      Task<string> CreateToken();
  }
  ```
  ```c#
  public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
  {
      _user = await _userManager.FindByNameAsync(userForAuth.UserName);

      var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuth.Password));
      if (!result)
          _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. Wrong user name or password.");

      return result;
  }

  public async Task<string> CreateToken()
  {
      var signingCredentials = GetSigningCredentials();
      var claims = await GetClaims();
      var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

      return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
  }

  private SigningCredentials GetSigningCredentials()
  {
      var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET");
      var secret = new SymmetricSecurityKey(key);

      return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
  }

  private async Task<List<Claim>> GetClaims()
  {
      var claims = new List<Claim>
      {
          new Claim(ClaimTypes.Name, _user.UserName)
      };

      var roles = await _userManager.GetRolesAsync(_user);
      foreach(var role in roles)
      {
          claims.Add(new Claim(ClaimTypes.Role, role));
      }

      return claims;
  }

  private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials,
      List<Claim> claims)
  {
      var jwtSettings = _configuration.GetSection("jwtSettings");

      var tokenOptions = new JwtSecurityToken
      (
          issuer: jwtSettings["validIssuer"],
          audience: jwtSettings["validAudience"],
          claims: claims,
          expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
          signingCredentials: signingCredentials
      );

      return tokenOptions;
  }
  ```
  ```c#
  [HttpPost("login")]
  [ServiceFilter(typeof(ValidationFilterAttribute))]
  public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
  {
      if (!await _service.AuthenticationService.ValidateUser(user))
          return Unauthorized();

      return Ok(new { Token = await _service.AuthenticationService.CreateToken() });
  }
  ```
- Role-based authorization：adding `[Authorize]` or `[Authorize(Roles = "...")]` on controller level or action level, we can control the authorization requirement for controllers and actions
  
## Chapter 28: Refresh Token

- After the token expires, the user has to login again to obtain a new token. Instead of forcing our users to log in every single time the token expires. For that, we can use a refresh token. Refresh tokens are credentials that can be used to acquire new access tokens. The lifetime of a refresh token is usually set much longer compared to the lifetime of an access token.
- Adding RefreshToken, RefreshTokenExpiryTime into User class. After Add-Migration, check the migration file, keep only the AddColumn code, go to RepositoryContextModelSnapshot, revert the id to previous id.
  ```c#
  namespace Shared.DataTransferObjects;
  public record TokenDto(string AccessToken, string RefreshToken);
  ```
  ```c#
  Task<TokenDto> CreateToken(bool populateExp);
  ```
  ```c#
  public async Task<TokenDto> CreateToken(bool populateExp)
  {
      var signingCredentials = GetSigningCredentials();
      var claims = await GetClaims();
      var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

      var refreshToken = GenerateRefreshToken();
      
      _user.RefreshToken = refreshToken;

      if (populateExp)
          _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

      await _userManager.UpdateAsync(_user);

      var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

      return new TokenDto(accessToken, refreshToken);
  }

  private string GenerateRefreshToken()
  {
      var randomNumber = new byte[32];
      using (var rng = RandomNumberGenerator.Create())
      {
          rng.GetBytes(randomNumber);
          return Convert.ToBase64String(randomNumber);
      }
  }

  private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
  {
      var jwtSettings = _configuration.GetSection("JwtSettings");

      var tokenValidationParameters = new TokenValidationParameters
      {
          ValidateAudience = true,
          ValidateIssuer = true,
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
              Environment.GetEnvironmentVariable("SECRET"))),
          ValidateLifetime = true,
          ValidIssuer = jwtSettings["validIssuer"],
          ValidAudience = jwtSettings["validAudience"]
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      SecurityToken securityToken;
      var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

      var jwtSecurityToken = securityToken as JwtSecurityToken;
      if (jwtSecurityToken == null || 
          !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
          StringComparison.InvariantCultureIgnoreCase))
      {
          throw new SecurityTokenException("Invalid token");
      }

      return principal;
  }
  ```
  ```c#
  [HttpPost("login")]
  [ServiceFilter(typeof(ValidationFilterAttribute))]
  public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
  {
      if (!await _service.AuthenticationService.ValidateUser(user))
          return Unauthorized();

      var tokenDto = await _service.AuthenticationService.CreateToken(populateExp: true);

      return Ok(tokenDto);
  }
  ```
  ```c#
  public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
  {
      var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

      var user = await _userManager.FindByNameAsync(principal.Identity.Name);
      if (user == null || user.RefreshToken != tokenDto.RefreshToken ||
          user.RefreshTokenExpiryTime <= DateTime.Now)
          throw new RefreshTokenBadRequest();

      _user = user;

      return await CreateToken(populateExp: false);
  }
  ```
  ```c#
  [Route("api/token")]
  [ApiController]
  public class TokenController : ControllerBase
  {
      private readonly IServiceManager _service;

      public TokenController(IServiceManager service) => _service = service;

      [HttpPost("refresh")]
      [ServiceFilter(typeof(ValidationFilterAttribute))]
      public async Task<IActionResult> Refresh([FromBody]TokenDto tokenDto)
      {
          var tokenDtoToReturn = await _service.AuthenticationService.RefreshToken(tokenDto);

          return Ok(tokenDtoToReturn);
      }
  }
  ```

## Chapter 29: Binding Configuration and Options Pattern

- In previos chapter, we fetch value from appsettings.json. We can bind the configuration data to strongly
typed objects. To do that, we can use the Bind method.
  ```c#
  public class JwtConfiguration
  {
      public string Section { get; set; } = "JwtSettings";
      public string? ValidIssuer { get; set; }
      public string? ValidAudience { get; set; }
      public string? Expires { get; set; }
  }
  ```
  Replacing `var jwtSettings = configuration.GetSection("JwtSettings");` with
  ```c#
  var jwtConfiguration = new JwtConfiguration();
  configuration.Bind(jwtConfiguration.Section, jwtConfiguration);
  ```
  Replacing `jwtSettings["validIssuer"]` with `_jwtConfiguration.ValidIssuer`
- Options pattern
  ```c#
  public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));
  ```
  ```c#
  builder.Services.AddJwtConfiguration(builder.Configuration);
  ```
  Replace `IConfiguration configuration` with `IOptions<JwtConfiguration> configuration`
- Using `IOptionSnapshot` or `IOptionMonitor` if we don't want to restart system after modifying variable.
  The main **difference** between these two interfaces is that the IOptionsSnapshot service is registered as a scoped service and thus can’t be injected inside the singleton service. On the other hand, IOptionsMonitor is registered as a singleton service and can be injected into any service lifetime.
- **IOptions<T>**:
  - Is the original Options interface and it’s better than binding the whole Configuration
  - Does not support configuration reloading
  - Is registered as a singleton service and can be injected anywhere
  - Binds the configuration values only once at the registration, and returns the same values every time
  - Does not support named options
- **IOptionsSnapshot<T>**:
  - Registered as a scoped service
  - Supports configuration reloading
  - Cannot be injected into singleton services
  - Values reload per request
  - Supports named options
- **IOptionsMonitor<T>**:
  - Registered as a singleton service
  - Supports configuration reloading
  - Can be injected into any service lifetime
  - Values are cached and reloaded immediately
  - Supports named options
  
## Chapter 30: Documenting API with Swagger

- There are three main components in the `Swashbuckle` package:
  - `Swashbuckle.AspNetCore.Swagger`: This contains the Swagger object model and the middleware to expose SwaggerDocument objects as JSON.
  - `Swashbuckle.AspNetCore.SwaggerGen`: A Swagger generator that builds SwaggerDocument objects directly from our routes, controllers, and models.
  - `Swashbuckle.AspNetCore.SwaggerUI`: An embedded version of the Swagger UI tool. It interprets Swagger JSON to build a rich, customizable experience for describing web API functionality.
- `Install-Package Swashbuckle.AspNetCore`
- ConfigureSwagger()
  ```c#
  public static void ConfigureSwagger(this IServiceCollection services)
  {
      services.AddSwaggerGen(s =>
      {
          s.SwaggerDoc("v1", new OpenApiInfo { Title = "Code Maze API", Version = "v1" });
          s.SwaggerDoc("v2", new OpenApiInfo { Title = "Code Maze API", Version = "v2" });
      });
  }
  ```
  ```c#
  builder.Services.ConfigureSwagger();
  ```
  ```c#
  app.UseSwagger();
  app.UseSwaggerUI(s =>
  {
      s.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Maze API v1");
      s.SwaggerEndpoint("/swagger/v2/swagger.json", "Code Maze API v2");
  });
  ```
  Adding `[ApiExplorerSettings(GroupName = "v1")]` above CompaniesController, `[ApiExplorerSettings(GroupName = "v2")]` above CompaniesV2Controller. All the other controllers will be included in both groups because they are not versioned.
- https://localhost:5001/swagger/index.html
- Adding authorization support: 
  ```c#
  s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
      In = ParameterLocation.Header,
      Description = "Place to add JWT with Bearer",
      Name = "Authorization",
      Type = SecuritySchemeType.ApiKey,
      Scheme = "Bearer"
  });

  s.AddSecurityRequirement(new OpenApiSecurityRequirement()
  {
      {
          new OpenApiSecurityScheme
          {
              Reference = new OpenApiReference
              {
                  Type = ReferenceType.SecurityScheme,
                  Id = "Bearer"
              },
              Name = "Bearer",
          },
          new List<string>()
      }
  });
  ```
- Adding information: 
  ```c#
  s.SwaggerDoc("v1", new OpenApiInfo 
  { 
      Title = "Code Maze API", 
      Version = "v1",
      Description = "CompanyEmployees API by CodeMaze",
      TermsOfService = new Uri("https://example.com/terms"),
      Contact = new OpenApiContact
      { 
          Name = "John Doe",
          Email = "John.Doe@gmail.com",
          Url = new Uri("https://twitter.com/johndoe"),
      },
      License = new OpenApiLicense
      {
          Name = "CompanyEmployees API LICX",
          Url = new Uri("https://example.com/license"),
      }
  });
  ```
- xml comments:
  ```c#
  var xmlFile = $"{typeof(Presentation.AssemblyReference).Assembly.GetName().Name}.xml";
  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
  s.IncludeXmlComments(xmlPath);
  ```
  Adding the following to CreateCompany:
  ```xml
  /// <summary>
  /// Creates a newly created company
  /// </summary>
  /// <param name="company"></param>
  /// <returns>A newly created company</returns>
  /// <response code="201">Returns the newly created item</response>
  /// <response code="400">If the item is null</response>
  /// <response code="422">If the model is invalid</response>
  [HttpPost(Name = "CreateCompany")]
  [ProducesResponseType(201)]
  [ProducesResponseType(400)]
  [ProducesResponseType(422)]
  [ServiceFilter(typeof(ValidationFilterAttribute))]
  ```

## Chapter 31: Deployment to IIS

- Creating publish files: 
  - Creating a folder: Publish
  - Right click on the main project, select publish
  - Select Folder, select the created folder
  - Click Finish, Publish
- Installing the [.NET Core Windows Server Hosting bundle](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-6.0.0-windows-hosting-bundle-installer). Adding `127.0.0.1 www.companyemployees.codemaze` to the end of file: C:\Windows\System32\drivers\etc\hosts
- IIS installation: 
  - Control panel -> Turn Windows features o or off -> check Internet Information Services
  - Win + R -> `inetmgr`
  - <img src="./img/ch31_1.png" width=200 height=200>
  - <img src="./img/ch31_2.png" width=400 height=500>
  - <img src="./img/ch31_3.png" width=1200 height=350>
  - Setting the .NET CLR version to No Managed Code is optional but recommended.
- We have to provide to IIS the name of that key and the value.
- 