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
    public partial class ListeUtilisateurs : System.Web.UI.Page
    {

        private String strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private String strNumEntreprise = "";
        public static string StrConn { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            StrConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

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
            con.Open();
            String str = "Select U.Id, U.IdEntreprise, U.Prenom, U.Nom, U.userLogin, U.Profil, " +
                         "Case When U.Etat = '1' then 'Actif' When U.Etat = '0' THEN 'Désactivé' End As Etat " +
                         "FROM ANA_UTILISATEURS U  INNER JOIN ANA_ENTREPRISES E on (U.IdEntreprise = E.id)  where U.IdEntreprise = " + strNumEntreprise + " order by U.Prenom ";

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

                //e.Row.Attributes.Add("data-userEntrepriseId", dr["Id"].ToString());

                String etat = dr["Etat"].ToString();

                if (e.Row.Cells[7].Controls[0] is ImageButton)
                {
                    if (etat.Equals("Actif"))
                    {
                        ImageButton _btn = e.Row.Cells[7].Controls[0] as ImageButton;
                        _btn.ImageUrl = "~/Images/actif.jpg";
                        _btn.ToolTip = "Désactiver";
                    }
                    else if (etat.Equals("Désactivé"))
                    {
                        ImageButton _btn = e.Row.Cells[7].Controls[0] as ImageButton;
                        _btn.ImageUrl = "~/Images/inactif.jpg";
                        _btn.ToolTip = "Activer";
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
            activer_desactiver_Click();
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

        private void activer_desactiver_Click()
        {
            String x = ListeUsers.DataKeys[ListeUsers.SelectedIndex].Values["Id"].ToString();
            int idEntreprise = 0;
            String majQuery = "";
            SqlTransaction transaction = null;
            SqlCommand command = null;
            SqlConnection con = new SqlConnection(strConnString);
            String etat = System.Convert.ToString(ListeUsers.DataKeys[ListeUsers.SelectedIndex].Values["Etat"]);
            string ReturnedValue = etat == "Actif" ? "0" : "1";

            if ((x != null) && (!x.Equals("")))
            {
                idEntreprise = int.Parse(x);
            }

            String modificationEtatOKScript = etat == "Actif" ? "Utilisateur désactivé avec succès !!!" : "Utilisateur activé avec succès !!!";
            String erreurMajEtat = " Mise à jour Statut Utilisateur non effectuée !!!";

            majQuery = " update ANA_UTILISATEURS Set Etat ='" + ReturnedValue + "' where Id = " + idEntreprise;

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
                    msgPopup.Text = modificationEtatOKScript;
                    msgPopup.ForeColor = System.Drawing.Color.Black;
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(@"<script language='javascript'>");
                    sb.Append(@"$('#mymodal-dialog').modal('show');");
                    sb.Append(@"</script>");
                    ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());
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
                        lblNomEntreprise.Text = dt.Rows[0]["NomouRaisonSociale"].ToString();
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

        protected void Profil_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "utilisateur-dialog", "openpopup();", true);
        }

        //protected void Profil_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    string profil = txtProfil.SelectedItem.Value;

        //}

        protected void Valider_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(StrConn))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;

                    string numeroEntreprise = "";
                    string nom = txtNom.Value;
                    string prenom = txtPrenom.Value;
                    string telephone = txtTelephone.Value;
                    string profil = txtProfil.Value;
                    string email = txtEmail.Value;
                    string login = txtLogin.Value;

                    byte[] pwdHashed = CalculateSHA1("P@sser123");

                    StringBuilder sbr = new StringBuilder();

                    foreach (Byte a in pwdHashed)
                    {
                        sbr.Append(a.ToString("X2"));
                    }

                    string mdp = sbr.ToString();


                    if (HttpContext.Current.Request.QueryString["Ent"] != null && HttpContext.Current.Request.QueryString["Ent"].ToString() != "")
                    {
                        numeroEntreprise = HttpContext.Current.Request.QueryString["Ent"].ToString();
                    }

                    try
                    {
                        connection.Open();

                        Boolean existeLogin = getIfLoginExist(login);

                        if (existeLogin)
                        {
                            labelErrorMsg.Text = "Un utilisateur est déjà enregistré avec cet email !!!";
                        }
                        else
                        {
                            int resultat = -1;

                            command.CommandText = "INSERT INTO ANA_UTILISATEURS (userLogin,Nom,Prenom,Email,Profil,Telephone,IdEntreprise,Etat,DateCreation,Signature) " +
                        "VALUES('" + login + "', '" + replaceCode(nom) + "', '" + replaceCode(prenom) + "', '" + email + "', '" + profil + "', " + telephone + "," + numeroEntreprise + ", '1','" + DateTime.Now + "', '" + mdp + "')";

                            resultat = command.ExecuteNonQuery();

                            if (resultat > 0)
                            {
                                ChargerDonnees();
                            }

                        }

                    }
                    catch (SqlException ex)
                    {
                        Response.Write(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

    }
}