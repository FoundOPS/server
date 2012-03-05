set mydate=%date:~4,2%-%date:~7,2%-%date:~10,4%
set mytime=%time:~0,2%%time:~3,2%
set mydatetime=%mydate%-%mytime%

cd Desktop

RedGate.SQLAzureBackupCommandLine.exe  /AzureServer:f77m2u3n4m.database.windows.net  /AzureDatabase:Core /AzureUserName:perladmin /AzurePassword:QOI1m7DzVUJiNPMofFkk /CreateCopy /StorageAccount:fstoresql /AccessKey:fm+httX1f6/ew8njDRsswz6JRdpbb6EcYYwUSVop4Lyq30SE44eTV1egjEKptLz1cN2fUFQGMzvLk1XLBxZxZg==  /Container:backups /Filename:%mydatetime%



