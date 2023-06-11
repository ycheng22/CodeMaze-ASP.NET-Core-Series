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


