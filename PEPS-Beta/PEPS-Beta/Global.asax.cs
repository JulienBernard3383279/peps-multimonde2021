using PEPS_Beta.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PEPS_Beta
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //IDatabaseInitializer<BddContext> init = new InitParamEtDonnees();

            //Database.SetInitializer(init);

            //init.InitializeDatabase(new BddContext());

            //add Sql.Client.SqlException catch => init
            // make a request, if request return empty => init
            // if it fails => init
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PEPS-Beta.Models.BddContext;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    conn.Open();
                    SqlCommand command = new SqlCommand("SELECT ASX FROM  IndexesAtDates WHERE Date = @DateParam", conn);
                    command.Parameters.Add(new SqlParameter("DateParam", DateTime.Parse("30/01/2008")));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read() == false)
                        {
                            conn.Close();
                            IDatabaseInitializer<BddContext> init = new InitParamEtDonnees();

                            Database.SetInitializer(init);

                            init.InitializeDatabase(new BddContext());
                        }
                    }
                }
            }
            catch (SqlException)
            {
                IDatabaseInitializer<BddContext> init = new InitParamEtDonnees();

                Database.SetInitializer(init);

                init.InitializeDatabase(new BddContext());
            }
        }
    }
}
