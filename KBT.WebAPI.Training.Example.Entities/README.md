## Install Entityframework Core
```dotnet tool install --global dotnet-ef```

## NuGet Packages
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.Relational
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.EntityFrameworkCore.SqlServer

## Scaffold Entiry Command (Database first)
```dotnet ef dbcontext scaffold "data source=localhost;initial catalog=demo;persist security info=True;user id=sa;password=KBTtestdb123;MultipleActiveResultSets=True;" Microsoft.EntityFrameworkCore.SqlServer -o Demo -c DemoDbContext -f -v```
