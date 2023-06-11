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
  ```
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
  ![](./img/built_in_validation.PNG)
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

