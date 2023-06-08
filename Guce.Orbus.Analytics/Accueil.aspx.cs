using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace Guce.Orbus.Analytics
{
    public partial class Accueil : System.Web.UI.Page
    {
        private String strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private String strNumEntreprise = "";
        public static string StrConn { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["NOMPOLE"] == null || Session["NOMPOLE"].ToString() == string.Empty)
            {
                Response.Redirect("Login.aspx", true);
            }

            String nbUsers = getTotalUtilisateurs();
            totalUsers.Text = nbUsers;

            if (Session["NUMEROINSCRIPTIONABONNE"] != null || Session["IdPole"] != null)
            {
                String nbDossiers = getTotalDossiers();
                totalDossiers.Text = nbDossiers;

                decimal montantJour = getTotalValeur();
                totalTransactions.Text = montantJour.ToString();
            }
            else
            {
                totalDossiers.Text = "0";
                totalTransactions.Text = "0";
            }
            

            
        }

        public String getTotalUtilisateurs()
        {
            String sQuery = "";
            String nb = "0";
            string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            try
            {
                sQuery = "SELECT count(*) as NbUsers from ANA_UTILISATEURS where etat = '1' and idEntreprise = '" + Session["NumENTREPRISE"].ToString() + "'";

                using (SqlConnection con = new SqlConnection(conString))
                {
                    DataTable dt = new DataTable();
                    SqlCommand cmd = new SqlCommand(sQuery, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        nb = dt.Rows[0]["NbUsers"].ToString();
                    }

                    con.Close();
                }

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return nb;
        }

        public String getTotalDossiers()
        {
            String sQuery = "";
            String nb = "0";
            string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            int numIncsriptionClient = 0;
            int idPole = 0;

            try
            {

                if (Session["NUMEROINSCRIPTIONABONNE"] != null)
                {
                    numIncsriptionClient = Int32.Parse(Session["NUMEROINSCRIPTIONABONNE"].ToString());

                    sQuery = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) as NbDossiers from DOSSIERTPS do " +
                             " inner join FACTURE fa on (do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS) " +
                             " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                             " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient + " and do.DATEDOSSIERTPS >= CAST( GETDATE() AS DATE)";
                }
                else if (Session["IdPole"] != null)
                {
                    idPole = Int32.Parse(Session["IdPole"].ToString());

                    sQuery = "SELECT COUNT(distinct do.NUMERODOSSIERTPS) as NbDossiers from DOSSIERTPS do " +
                             " inner join FACTURE fa on (do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS) " +
                             " WHERE exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS and  jo.NIVEAUEXECUTION = 'EnCoursDouane' and jo.DATERETOUR  >= CAST( GETDATE() AS DATE)) ";

                }
                
                using (SqlConnection con = new SqlConnection(conString))
                {
                    DataTable dt = new DataTable();
                    SqlCommand cmd = new SqlCommand(sQuery, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        nb = dt.Rows[0]["NbDossiers"].ToString();
                    }

                    con.Close();
                }

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return nb;
        }

        public decimal getTotalValeur()
        {
            String sQuery = "";
            decimal valeurTotale = 0;
            string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            int numIncsriptionClient = 0;
            int idPole = 0;

            try
            {

                if (Session["NUMEROINSCRIPTIONABONNE"] != null)
                {
                    numIncsriptionClient = Int32.Parse(Session["NUMEROINSCRIPTIONABONNE"].ToString());

                    sQuery = " SELECT SUM(co.VALEURCFA) as Total from DOSSIERTPS do " +
                             " inner join FACTURE fa on (do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS) " +
                             " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                             " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                             " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient + " and do.DATEDOSSIERTPS >= CAST( GETDATE() AS DATE)";
                }
                else if (Session["IdPole"] != null)
                {
                    idPole = Int32.Parse(Session["IdPole"].ToString());

                    sQuery = "SELECT SUM(co.VALEURCFA) as Total from DOSSIERTPS do " +
                             " inner join FACTURE fa on (do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS) " +
                             " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                             " WHERE exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS and  jo.NIVEAUEXECUTION = 'EnCoursDouane' and jo.DATERETOUR  >= CAST( GETDATE() AS DATE)) ";

                }

                using (SqlConnection con = new SqlConnection(conString))
                {
                    DataTable dt = new DataTable();
                    SqlCommand cmd = new SqlCommand(sQuery, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["Total"].ToString() != null && dt.Rows[0]["Total"].ToString() != "" && dt.Rows[0]["Total"].ToString() != "0")
                        {
                            valeurTotale = Convert.ToDecimal(dt.Rows[0]["Total"].ToString());
                        }                    
                    }

                    con.Close();
                }

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return valeurTotale;
        }

    }
}