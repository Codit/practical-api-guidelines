# Logging Guidance
Logging consists of several parts that should all be considered on design & development time.

- How should you structure your logs?
- How should you aggregate your logs?
- What are usefull logs (in each environment)?
- More technical: what framework should you use?

## Logging principals
Logging should be crystal clear and should give a direct insight in how the application is setup and should give a logical overview on what is happening.

In order to do this, it is mandatory to use a `correlationId`. A correlationId is an id which groups seperate logs together to a group. In the case of an api, each request will be one `correlationId`.

If the sink you are sending to supports this behavior, you can also create tree's in this correlation to distinct even more and make more clear what is actually happening in your code.

There are different types of logs that can be made, most importantly:

| Log Type | Description |
| -------- | ----------- |
| Request | Logging the incoming request, including response code, execution time etc... |
| Trace | Traces consists of usefull information in different verbosity levels (explained later) for a functional understanding of the API |
| Exception | On exception the details of the exceptions should be logged |
| Dependency | Tracking external dependencies like backend api's / database calls etc, with response code, execution time etc... |

All of this combined will give you a good understanding of how your application reacts in certain scenario's. It will be able to give you a result of load on the API, to detect and fix possible issues quickly and to give operations/support the ability to track the important measurements in a dashboard.

While doing so, try to avoid string concatenation as this might make your application less monitorable and make search queries more difficult.

The logs mentioned over here should be a combination of functional and technical logs. Every log should have an added value to functional and technical people, to operations & to developers.

In the end it might be good to have all these logs in one resultset on an acceptance environment, but it might not be needed on a production environment. 
That's why every log should have a `LogLevel`. The loglevel should be defined on every log. On every application there should a minimal loglevel be defined by configuration.

Trace logs can be set to different LogLevels.
List of log level from most detailed to most general.  
1. Verbose
1. Info
1. Warning
1. Error
1. Off  

ref: https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.tracelevel?view=netcore-2.2

Generally, Loglevels can be set on Application level. The values applicable here are: 
1. Trace
1. Debug
1. Warning
1. Error
1. Critical
1. None  

ref: https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=aspnetcore-2.2


# Logging sinks
By default, a console log should be added.

On Azure projects, by default, there should be logged to Application Insights.

# Logging frameworks
There are differnt ways to log your logs.

- If there are Application Insights specifics, such as:
    - Dependency logging
    - Custom Dimension logging
    - Tree correlations  

    Use the direct Application Insights logging.I specifics (for dependency logging etc)

    - If you don't have this requirement there are 2 possible ways to go for. [Microsoft.Extensions.Logging.ILogger](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger?view=aspnetcore-2.2) or [Serilog](https://serilog.net/)