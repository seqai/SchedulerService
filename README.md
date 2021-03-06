# SchedulerService
## Running the Application
- Option A: `dotnet run` with default configuration. This will create a `schedule.db` file if not yet created
- Option B: build and run docker image. Please note that for data persistence it is recommended to create a volume for the sqlite database. Example command sequence (with superuser privileges):
```
docker build -f Dockerfile -t schedule-service .
docker volume create scheduledb
docker run --mount source=scheduledb,target=/db -p 7777:80 -e "PERSISTENCE__CONNECTIONSTRING=Data Source=/db/test.db" schedule-service
```
- In either way the application can be used either via simple UI on `http://localhost:[port]` or via swagger UI `http://localhost:[port]/swagger`

## Core Dependencies
- ASP.NET Core is used as a base framework for an API
- [Language Extentions](https://github.com/louthy/language-ext) library is used to provide basic functional types (Option, Either) and types to manage side-effects (e.g. TryAsync). 
- EF Core with Sqlite are used for persistence layer to provide code-first database definition
- xUnit with FsCheck are used to provide basic property testing on schedule calculation algorithm

## Implementation Notes
- While [Language Extentions](https://github.com/louthy/language-ext) allows a really interesting approach to deal with effects I wouldn't force this on other people, unless the team really would like to try it. Here I used it more to make the solution a bit more interesting.
- The basic architecture of the project is layered, but I intentionally did not create duplicated entities for each particular layer with a lot of (auto)mapping between them. Where an entity can be meaningfully reused, it was reused and if possible, aside from persistence layer, entities were defined as immutable
- Personally I am bit wary of ORMs, like EF Core, and always cautious about bringing it to the project, but it allowed to kickstart the assignment quite quick
- Complexity of handling the dates, time zones and other related problems is not covered within this implementation as it requires a lot of business input
- UI is implemented by using pure HTML/JS for simplicity. Please note that some of the features are quite recent, e.g. `input[type=date]` but should fallback graciously. Design and layout are very barebone
- Additional `studentId` discriminator is added to a schedule entity
- Multiple calculation methods were implemented, but because of the simplicity of student calendar and the weekly output they are not extremely useful for most cases. Potentially they can be reused with daily schedules and complex calendars
- Better error types and handling rather than just simple passing of `string` in `Either<string, T>` are expected to be used in real life application. The error messages then can be packed in proper json contract, but this was omitted for simplicity and time-constraints
