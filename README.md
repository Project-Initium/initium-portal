# Stance

 A dotnet core authenticated web app with opinionated domain with CQRS, Entity Framework and Dapper.

 ## Build status

 ## Motivation

 After setting up a few sites with dotnet core and using the provided authentication provider, I found that  whilst it works, I could not get it to work well with a strong domain context.  This is where the idea of  Stance came from.  An example project to lay out an opinionate way to create a fully domain orientated  approached to a provide an authenticated system.  As I continued on I added MFA providers that works out of  the box whilst setting it up with modern security patterns.  

## Getting Started

This project is designed to follow the F5 Manifesto, it will run for the most part out of the box but
some prerequisites are still needed.

### Prerequisites

 - Node, for building the front end
 - DotNet Core 3.1
 - Visual Studio 2019 with SSDT
 - SQL Server, although there is a compose file supplied that will create a SQL Server

 ### Installing

  1. If you can I recommend using the included compose file to set up a SQL server as this will run a an instance with all the passwords set for quick running. For this open a shell at the root of the repository and run `docker-compose up -d` 
  1. The front end is generated using webpack, so install the components with the regular `npm install` and build with `npm run dev:build` from the web project directory.  There is also a watch mode that will monitor for changes in the files set out in the webpack config. This can be executed with the following `npm run dev:watch`.
  1. The database is controlled by a database project and can by deployed via Visual Studio with SSDT.
 ## Technical Overview

It is an DotNet Core 3.1 web app constructed in razor pages.

The is a strong domain implemented via the [CQRS](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs) pattern with the help for [MediatR](https://github.com/jbogard/MediatR). The domain uses a [aggregate root](https://www.martinfowler.com/bliki/DDD_Aggregate.html) pattern and data mutation is carefully controlled. Data is provided to the system via EF Core

Reads for presentation are either done via [Dapper](https://github.com/StackExchange/Dapper) and stored procedures or with [OData](https://www.odata.org/) via entity framework that has a read only context. Any stored procedures need to placed in the Portal schema of the database and the OData data needs to come from views in the `ReadAggregation` schema.

Front end is in the form of TypeScript and Sass with a Webpack task processing these files. This enables the `wwwroot` folder to never be placed in SCM.

 ## Configuration

So that sensitive configuration data is not included in source control, no configuration file is committed. To aid with this, a json file can be created called `appsettings.[MACHINENAME].json` and this will over ride the settings loaded.

It is important that any new configuration variables are added to the template with it's data made blank or generic, so that other developers have a valid configuration to reference.

 ### First Run

Once the database is published, the initial user can be created by navigating to https://localhost:44356/welcome/user-creation and by following the prompts.

> As this web app has always on MFA, the MFA code for email based MFA will currently be found in the logs.

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. 


## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details