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

