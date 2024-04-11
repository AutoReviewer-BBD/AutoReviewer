# packages to add
- dotnet add package Microsoft.EntityFrameworkCore
- dotnet add package Microsoft.EntityFrameworkCore.Design
- dotnet add package Microsoft.EntityFrameworkCore.SqlServer
- dotnet add package Microsoft.EntityFrameworkCore.Tools
- dotnet add package Microsoft.Extensions.Configuration.UserSecrets
- dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
- dotnet add package System.IdentityModel.Tokens.Jwt

# Connection String
Maybe look into using secrets

## Live
Data Source=autoreviewerdb.cwnn1s7lxjgf.eu-west-1.rds.amazonaws.com,1433;User ID=AutoreviewerAdmin;Password=Password12345;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False  

## Local
Data Source=KATLEGOK\SQLEXPRESS;Initial Catalog=aradb;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False

# Scaffold
This command will make the model and db context for you.

## Live
`dotnet ef dbcontext scaffold "Data Source=autoreviewerdb.cwnn1s7lxjgf.eu-west-1.rds.amazonaws.com,1433;Initial Catalog=AutoReviewerDB;User ID=AutoreviewerAdmin;Password=Password12345;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False" Microsoft.EntityFrameworkCore.SqlServer --context-dir Data --output-dir Models --data-annotations`

## Local
`dotnet ef dbcontext scaffold "Data Source=KATLEGOK\SQLEXPRESS;Initial Catalog=aradb;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False" Microsoft.EntityFrameworkCore.SqlServer --context-dir Data --output-dir Models --data-annotations`