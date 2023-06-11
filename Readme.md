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


## Chapter 9: 

## Chapter 10: 