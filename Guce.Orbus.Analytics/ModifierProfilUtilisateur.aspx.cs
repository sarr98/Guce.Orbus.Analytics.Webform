using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Globalization;
using System.Web.UI.DataVisualization.Charting;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Web.Services;
using System.Web.Script.Services;
using System.Xml;
using System.Web.Script.Serialization;

namespace Guce.Orbus.Analytics
{
    public partial class ModifierProfilUtilisateur : System.Web.UI.Page
    {

        private String strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private String strNumEntreprise = "";
        private String profilUser = "";
        public static string StrConn { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            StrConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            if (Session["PROFILAGENT"] != null && Session["PROFILAGENT"].ToString() != string.Empty)
            {
                profilUser = Session["PROFILAGENT"].ToString();
            }

            if (HttpContext.Current.Request.QueryString["Ent"] != null && HttpContext.Current.Request.QueryString["Ent"].ToString() != "")
            {
                strNumEntreprise = HttpContext.Current.Request.QueryString["Ent"].ToString();
                getNomEntreprise(strNumEntreprise);
            }
            else if (Session["NumENTREPRISE"] != null)
            {
                strNumEntreprise = Session["NumENTREPRISE"].ToString();
                getNomEntreprise(strNumEntreprise);
            }

            

            if (Session["NOMPOLE"] == null || Session["NOMPOLE"].ToString() == string.Empty)
            {
                Response.Redirect("Login.aspx", true);
            }
            else
            {
                if (!IsPostBack)
                {
                    ChargerDonnees();
                }
            }       
        }

        private void ChargerDonnees(string sortExpression = null)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            SqlConnection con = new SqlConnection(strConnString);
            String str = "";
            con.Open();

            if (profilUser == "Administrateur")
            {
                str = "Select U.Id, U.IdEntreprise, U.Prenom, U.Nom, U.userLogin, U.Profil, " +
                         "Case When U.Etat = '1' then 'Actif' When U.Etat = '0' THEN 'Désactivé' End As Etat " +
                         "FROM ANA_UTILISATEURS U  INNER JOIN ANA_ENTREPRISES E on (U.IdEntreprise = E.id)  order by U.Prenom ";
            }
            else
            {
                str = "Select U.Id, U.IdEntreprise, U.Prenom, U.Nom, U.userLogin, U.Profil, " +
                         "Case When U.Etat = '1' then 'Actif' When U.Etat = '0' THEN 'Désactivé' End As Etat " +
                         "FROM ANA_UTILISATEURS U  INNER JOIN ANA_ENTREPRISES E on (U.IdEntreprise = E.id)  where U.IdEntreprise = " + strNumEntreprise + " order by U.Prenom ";
            }           

            using (cmd = new SqlCommand(str, con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    ListeUsers.DataSource = dt;
                    ListeUsers.DataBind();
                }
                else
                {
                    ListeUsers.DataSource = new DataTable();
                    ListeUsers.DataBind();
                }
            }

        }

        protected void ListeUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ListeUsers.Columns[0].Visible = false;
            ListeUsers.Columns[1].Visible = false;
            ListeUsers.Columns[6].Visible = false;
            

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView dr = (DataRowView)e.Row.DataItem;

                String leProfil = dr["Profil"].ToString();
                
                if (e.Row.Cells[7].Controls[0] is ImageButton)
                {
                    ImageButton _btn = e.Row.Cells[7].Controls[0] as ImageButton;
                    _btn.ImageUrl = "~/Images/MajProfil.png";

                    if (leProfil.Equals("Superviseur"))
                    {
                        _btn.ToolTip = "Changer en Agent";
                        e.Row.Cells[5].ForeColor = Color.LimeGreen;
                    }
                    else if (leProfil.Equals("Agent"))
                    {
                        _btn.ToolTip = "Changer en Superviseur ";
                        e.Row.Cells[5].ForeColor = Color.DarkGoldenrod;
                    }
                }
            }
        }

        protected void ListeUsers_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.ChargerDonnees(e.SortExpression);
        }

        protected void ListeUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = ListeUsers.SelectedRow;
            changerProfil_Click();
        }

        private void changerProfil_Click()
        {
            String x = ListeUsers.DataKeys[ListeUsers.SelectedIndex].Values["Id"].ToString();
            int idEntreprise = 0;
            String majQuery = "";
            SqlTransaction transaction = null;
            SqlCommand command = null;
            SqlConnection con = new SqlConnection(strConnString);
            String profilUtilisateurSelectionne = System.Convert.ToString(ListeUsers.DataKeys[ListeUsers.SelectedIndex].Values["Profil"]);
            string ReturnedValue = "";

            if (profilUtilisateurSelectionne == "Superviseur")
            {
                ReturnedValue = "Agent";
            }
            else if (profilUtilisateurSelectionne == "Agent")
            {
                ReturnedValue = "Superviseur";
            }           

            if ((x != null) && (!x.Equals("")))
            {
                idEntreprise = int.Parse(x);
            }
           
            String erreurMajEtat = " Une erreur est survenue !!!";

            majQuery = " update ANA_UTILISATEURS Set Profil ='" + ReturnedValue + "' where Id = " + idEntreprise;

            if (ReturnedValue != "")
            {
                try
                {
                    con.Open();
                    transaction = con.BeginTransaction();
                    command = con.CreateCommand();
                    command.Transaction = transaction;

                    if (UpdateDataT(majQuery, command) == 1)
                    {

                        transaction.Commit();
                        ChargerDonnees();
                    
                    }
                    else
                    {
                        msgPopup.Text = erreurMajEtat; msgPopup.ForeColor = System.Drawing.Color.Black;
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append(@"<script language='javascript'>");
                        sb.Append(@"$('#mymodal-dialog').modal('show');");
                        sb.Append(@"</script>");
                        ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());
                    }
                }
                catch (SqlException ex)
                {
                    Response.Write(ex.Message);
                }
                finally
                {
                    if (con != null)
                    {
                        con.Close();
                        transaction = null;
                        command = null;
                    }
                }
            }
        }

        public int UpdateDataT(String query, SqlCommand theCmd)
        {
            int value = 0;
            try
            {
                theCmd.CommandText = query;
                value = theCmd.ExecuteNonQuery();
            }
            catch (Exception ep)
            {
                Response.Write(ep.ToString());
                value = -2;
            }
            return value;
        }

      

        protected void ListeUsers_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            ListeUsers.PageIndex = e.NewPageIndex;
            ChargerDonnees();
        }

        public Boolean getNomEntreprise(String numeroInscription)
        {
            String sQuery = "";
            Boolean existe = false;
            string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            try
            {
                sQuery = "SELECT NomouRaisonSociale from ANA_ENTREPRISES where Id = " + numeroInscription + "";

                using (SqlConnection con = new SqlConnection(conString))
                {
                    DataTable dt = new DataTable();
                    SqlCommand cmd = new SqlCommand(sQuery, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        if (profilUser != "Administrateur")
                        {
                            lblNomEntreprise.Text = dt.Rows[0]["NomouRaisonSociale"].ToString();
                        } 

                        
                    }

                    con.Close();
                }

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return existe;
        }


        [System.Web.Services.WebMethod]
        public static Boolean ResetPassword(string id)
        {
            Boolean reinitialise = false;
            DataTable dt = new DataTable();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            using (SqlConnection con = new SqlConnection(StrConn))
            {
                DataTable dtUsers = new DataTable();
                SqlCommand cmd = new SqlCommand();
                con.Open();

                String mdp = "E1FD27C0C558CF076EAD9F1F1DF799572F69C763";

                String str = " update ANA_UTILISATEURS Set Signature = '" + mdp + "' where Id = " + id;

                using (cmd = new SqlCommand(str, con))
                {
                    int a = cmd.ExecuteNonQuery();

                    if (a > 0)
                    {
                        reinitialise = true;
                    }
                }
                return reinitialise;
            }
        }

        // ******************* Ajout Utilisateur **************** ////////////////

        private byte[] CalculateSHA1(string str)
        {
            SHA1 sha256 = SHA1Managed.Create();
            byte[] hashValue;
            ASCIIEncoding obj = new ASCIIEncoding();
            hashValue = sha256.ComputeHash(obj.GetBytes(str));

            return hashValue;
        }

        public Boolean getIfLoginExist(String login)
        {
            String sQuery = "";
            Boolean existe = false;
            string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            try
            {
                sQuery = "SELECT * from ANA_UTILISATEURS where userLogin = '" + login + "'";

                using (SqlConnection con = new SqlConnection(conString))
                {
                    DataTable dt = new DataTable();
                    SqlCommand cmd = new SqlCommand(sQuery, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        existe = true;
                    }

                    con.Close();
                }

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return existe;
        }

        public string replaceCode(string chaine)
        {
            string chaineTemp = string.Empty;
            char c = '\'';
            for (int i = 0; i < chaine.Length; i++)
            {
                if (chaine[i].Equals(c))
                {
                    chaineTemp += "''";
                }
                else
                {
                    chaineTemp += chaine[i];
                }
            }
            return chaineTemp;
        }

        
        

    }
}