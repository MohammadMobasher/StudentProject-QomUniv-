//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Service
//{
//    public class DbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
//    {
//        public DatabaseContext CreateDbContext(string[] args)
//        {
//            var optionBuilder = new DbContextOptionsBuilder();

//            //optionBuilder.UseSqlServer("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=LiftBazarDb2;Data Source=.");
//            optionBuilder.UseSqlServer("Password=umA6n7%9;Persist Security Info=True;User ID=SysAdminME;Initial Catalog=LiftBazarDb;Data Source=89.32.251.8,9992\\MSSQLSERVER2014");

//            return new DatabaseContext(optionBuilder.Options);
//        }
//    }
//}
