# Worker Dependency Injection Demo

.NETcore gives us really cool out of the box dependency injection, but its is really only wired up for you with ASP.NET.
This repo will show you two ways to wire it up for service workers.

## What kinds of services will this approach work for
- Console Apps
- Custom Services
- Linux Services
- AWS Lambdas
- Azure WebJobs
- Azure Functions
- Windows Services
- Service Fabric Actors

## Two Types
There are really only two types of workers in the list ablove
### Run Once
  - Console apps
  - AWS Lambdas
  - Azure WebJobs
  - Azure Functions
### Hosted Service
  - Custom Services
  - Linux Services
  - Windows Services
  - Service Fabric Actors
  
 ## What's in Common between the two?
 The two share a lot of the same components
 - Configuration Builder
 - Service Provider
 
 ## But Hosted Services have an extra layer.
 For hosted services there is a Host Builder.
 