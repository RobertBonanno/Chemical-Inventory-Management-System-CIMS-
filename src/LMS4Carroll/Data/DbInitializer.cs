using LMS4Carroll.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LMS4Carroll.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
           
            try
            {
                //Incement and seed has been set within datatable itself by dropping and re-creating the table
                /*
                context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('ChemicalEquipments', RESEED, 2000);");
                context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('BioEquipments', RESEED, 1000);");
                context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('ChemInventory', RESEED, 10000);");
                context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('PhyEquipments', RESEED, 10000);");
                 */
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            context.SaveChanges();
        }
    }
}