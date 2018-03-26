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

            // make a request, if request return empty => init
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PEPS-Beta.Models.BddContext;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT ASX FROM  IndexesAtDates", conn);
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
    }
}
