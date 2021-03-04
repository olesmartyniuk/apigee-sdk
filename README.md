# apigee-sdk
.NET binding for Apigee Edge Management API 

It allows to manage developers, apis, applications, deployments and much more.

## Example of usage

This code shows how to lists all developers in an organization.

```c#
    var options = new ApigeeClientOptions(
        "your@email",
        "your_password",
        "your_organization_name",
        "your_environment");

    var client = new ApigeeClient(options);

    var developers = await client.GetDevelopers();

    Console.WriteLine("Developers list:");
    foreach (var developer in developers)
    {
        Console.WriteLine($"Developer name: {developer.FirstName} {developer.LastName}");
        Console.WriteLine($"Developer status: {developer.Status}");
    }
```

You can provide your own HttpClient instance while constructing the Apigee client like this.

```c#
    var client = new ApigeeClient(options, new HttpClient());
```

## Supported methods

* GetApplicationIds()
* GetApplications()
* GetApplication(string applicationId)
* GetApplication(string developerEmail, string applicationName)
* GetDeveloperApplicationNames(string developerEmail)
* GetDevelopersEmails()
* GetDevelopers()
* GetDeveloper(string developerEmail)
* GetCompanies()
* GetApiProductNames()
* GetApiProducts()
* GetApiProduct(string productName)
* ImportApiProxy(string name, string pathToBundle)        
* CreateApiProduct(NewApiProduct apiProduct)
* DeployApiProxy(string apiName, int revisionNumber)
* UndeployApiProxy(string apiName, int revisionNumber)
* ForceDeployApiProxy(string apiName, int revisionNumber)

## Run tests

Integration tests for Apigee SDK use Apigee API so to run these tests you need :
* valid account in Apigee
* set environment variables with your credentials

To register a new account you need to visit [registration page](https://login.apigee.com/sign__up) and specify data for yourself and your organization. You need to remember your email, password and your organization name as they will be needed for end-to-end tests. The organization name usually ends with `eval` for the evaluation account.

The next step is to set the environment variables with newly created credentials, so integration tests can read them in order to verify SDK work.

* APIGEE_EMAIL
* APIGEE_PASSWORD
* APIGEE_ORGNAME

In Windows Powershell you can do this like described below:

```powershell
> [System.Environment]::SetEnvironmentVariable('APIGEE_EMAIL','your.email@domain.com',[System.EnvironmentVariableTarget]::User)

> [System.Environment]::SetEnvironmentVariable('APIGEE_PASSWORD','yourpassword123!',[System.EnvironmentVariableTarget]::User)

> [System.Environment]::SetEnvironmentVariable('APIGEE_ORGNAME','yourorganization-eval',[System.EnvironmentVariableTarget]::User)
```

After that you can navigate to the folder with tests and run the command:

```
cd Tests\Integration
dotnet test
```
or 
```
cd Tests\Unit
dotnet test
```


