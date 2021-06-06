

# Introduction
Sample Stroage project conducted by NIOC IT department. This project developed using ASP.NET Core as Backend and Blazor WebAssembly as Frontend and SQLServer as database.

- Also the Bit framework has been used both in Backend and Frontend for easy and fast development of the project.


# Server
All Nuget packages created by this project:

- **ATA.Bit.Helper**: *Utilities like extensions and helpers like DateTimeExtensions, EnumExtensions, JsonExtensions, LinqExtensions and StringExtensions*
- **ATA.Bit.Model**: *Common classes and interfaces between ATA Bit base apps like IATARepository, LoginJwtDto, IATAEntity, IUserInfoProvider, etc*
- **ATA.Bit.Server**: *Common services for Server side in ATA Bit base apps like BaseApiController, CamelCaseRoutesConvention and etc*
- **ATA.Bit.Data**: *Utilities for Data project like ModelBuilderExtensions including RegisterDbSets, RegisterIsArchivedGlobalQueryFilter, SetIsArchivedFilter, ConfigureDecimalPrecision, SetRestrictAsDefaultDeleteBehavior, ApplyConfigurations and UseJsonDbFunctions plus services for audit log entities (Needs registering with `services.AddATABitDataAccess();` in Startup.cs*
- **ATA.Bit.Shared**: *Shared codes between Blazor client and server like SelectListItem and UserDto models*
- **ATA.Broker.SSOSecurity**: *Calling to Security system (known as SSO client) never been easier ;-)*
- **ATA.Broker.Workflow**: *Calling to services inside Workflow system*
- **ATA.Broker.CDN**: *Upload files into CDN*
- **ATA.SMS**: *Comming soon..*


# Client
 To call ATA Security system (*SSO host*) endpoints do as following: 

1. Install **`ATA.Broker.SSOSecurity`** Nuget package

2. Register it in ConfigureService method:

	**`services.AddATASSOClient();`**

3. Inject **`IATASSOClientService`** in any class needed and enjoy :)

> Available methods are as below:
> - **GetUserByTokenAsync**: *Get user by SSOToken*
> - **GetUserRolesAsync**: *Get user roles defined in security system for the application*
> - **GetSSOLoginPageUrl**: *Get Security system login page address (SSOToken will be set there after successful login)*
> - **GetUserTokenByPersonnelCodeAsync**

# Shared
 It provides two important services: 
 1. **IATAOrgInfoProvider**: *Get ATA organizational information such as users, posts, chart, units and boxes*
 2. **IATAWorkflowService**: *Services related to Workflows of requests*


 Using the package:

1. Install **`ATA.Broker.Workflow`** Nuget package

2. Register it in ConfigureService method:

	**`services.AddATAWorkflowClient();`**

***IATAWorkflowService***: To call Workflow system endpoints do as following: 

 - Inject **`IATAWorkflowService`** in any class needed and enjoy :)

> Available ***methods*** are as below:
> - **SendWorkProcessOnStartAsync**: *Send a working process on the start of request. The only difference with normal SendWorkProcess is defining the OwnerId (main applicant) because it can be different with registrar*
> - **SendWorkProcessAsync**: *Send a working process done by an action (such as Confirm or Reject) from some permitted user to Workflow system*
> - **GetWorkHistoryAsync**: *Get History of a work but not ToDoes item*
> - **GetWorkListByWorkAsync**: *Get users currently are to-do for the work plus the work current state*
> - **GetUserPossibleActionsAsync**: *User possible actions on a work*
> - **GetCurrentStateTagAsync**: *Get work current state*
> - **DeleteWorkRecordsAsync**: *Soft delete records of a work saved on Workflow system*
> - **GetAllUserWorksAsync**: *Get user to-do works / or user related (involved) works. This method is deprecated due to using database View and OData*


> Available ***Dtos*** and ***Enums*** are as below:
> - *Dtos*: **SendWorkProcessDto, SendWorkProcessOnStartDto, UserWorkDto, WorkProcessResult**
> - *Enums*: **FlowType, ActionTag, WorkDashboardType**


***IATAOrgInfoProvider***: To call ATA Organizational endpoints do as following: 

 - Inject **`IATAOrgInfoProvider`** in any class needed and enjoy :)

> Available ***methods*** are as below:
> - **GetUserByIdAsync**: *Get User (ATA Employee) by Id Including his/her job position and unit (full box info)*
> - **GetUsersByUnitAsync**: *Get all employees working in a specific unit such as HR*
> - **GetUsersByBoxIdAsync**: *Get all employees in the same box (job)*


> Available ***Dtos*** and ***Enums*** are as below:
> - *Dtos*: **UserDto**
> - *Enums*: **ATAUnit**

# CDN (Upload files)
 To save files into CDN do as following: 

1. Install **`ATA.Broker.CDN`** Nuget package

2. Register it in ConfigureService method:

	**`services.AddATACDN();`**

3. Inject **`IATACDNService`** in any class needed and enjoy :)

> Available methods are as below:
> - **UploadFileAsync** ***(Using ByteArray)***: *Upload file in ATA CDN and get path Using Byte[] data*
> - **UploadFileAsync** ***(Using IFormFile)***: *Upload file in ATA CDN and get path Using IFormFile*
> - **GetDownloadUrl**: *Get full download link to a file e.g. "https://cdn.app.ataair.ir/portal/legal/41047c30-5404-4e33-abe2-b9de0c232c5a.jpg"*
> - **GetBaseUrl**: *Get uploaded file base-URL e.g. "https://cdn.app.ataair.ir/portal/legal"*
