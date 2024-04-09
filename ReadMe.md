# Scaffold
This command will make the model and db context for you.


`dotnet ef dbcontext scaffold "Data Source=autoreviewerdb.cwnn1s7lxjgf.eu-west-1.rds.amazonaws.com,1433;Initial Catalog=AutoReviewerDB;User ID=AutoreviewerAdmin;Password=Password12345;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False" Microsoft.EntityFrameworkCore.SqlServer --context-dir Data --output-dir Models --data-annotations`

Please do run thsi script to use the database. We will replace this with a secure way of connecting later!

# packages to add
- dotnet add package Microsoft.EntityFrameworkCore
- dotnet add package Microsoft.EntityFrameworkCore.Design
- dotnet add package Microsoft.EntityFrameworkCore.SqlServer
- dotnet add package Microsoft.EntityFrameworkCore.Tools
- dotnet add package Microsoft.Extensions.Configuration.UserSecrets

# Connection String
Data Source=autoreviewerdb.cwnn1s7lxjgf.eu-west-1.rds.amazonaws.com,1433;User ID=AutoreviewerAdmin;Password=Password12345;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False  
Maybe look into using secrets