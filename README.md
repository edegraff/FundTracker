# FundTracker
ECE 493 Capstone Project

## Setup
1. Install Visual Studio 2013 Ultimate: https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx
2. Update Visual Studio to update 4 (if you don't have it already): http://www.microsoft.com/en-us/download/details.aspx?id=44921
3. Install Azure Sdk: http://go.microsoft.com/fwlink/p/?linkid=323510&clcid=0x409
4. Open project in Visual Studio
5. Build the solution (F6), this will download all needed packages

## Running
#### Production
1. Go to https://ece493.azurewebsites.net/

#### Debug Website
1. Open Solution in Visual Studio
2. Press play button

#### Service
1. Open Solution in Visual Studio
2. Right click on "FundCloudService" in solution explorer
3. Select Debug>Start New Instance

#### Tests
1. Open Solution in Visual Studio
2. Select Test > Windows > Test Explorer
3. Select Run All in the newly opened window.

## Maintenance

#### Database Migrations
To create a database migration automatically after changing some model code

1. Open the Package Manager View
2. ensure the "default project" box is set to Common (or the project you want to migrate)
3. type "Add-Migration migrationName"
4. Open the class called migrationName under the common/migrations folder
5. Modify the up/down methods however you want to make sure the migration is doing what you want. The autogenerated migrations will not always be correct.
6. In package manager view type "Update-Database"

For more information https://msdn.microsoft.com/en-ca/data/jj591621.aspx

#### Publishing to Azure
