using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Guce.Orbus.Analytics
{
    public partial class Site1 : System.Web.UI.MasterPage
    {

        public static string StrConn { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            StrConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            int IdPole = 0;

            if (Session["IdPole"] != null)
            {
                IdPole = int.Parse(Session["IdPole"].ToString());

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT DESIGNATIONPOLE, NOMQUEUEPEC,NOMPOLE,TYPE,CHEMINDOCUMENTORIGINAL,CHEMINDOCUMENTSIGNE,URLDOCUMENTSIGNE,NOMTABLEJOINDRE,URLNETDOCUMENTSIGNE FROM POLES WHERE IDPOLE = " + IdPole + " ", con);

                try
                {
                    using (SqlDataReader sqlReader = cmd.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            Session["NOMPOLE"] = sqlReader.GetValue(2).ToString();
                        }

                        sqlReader.Close();
                        cmd.Dispose();
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    IdPole = 0;
                    Response.Write("Erreur Connexion à la base Applications :" + ex.Message);
                }
            }
        }

        public Boolean getIfMotDePasseExist(String mdp)
        {
            String sQuery = "";
            Boolean existe = false;
            string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            try
            {
                byte[] pwdHashed = CalculateSHA1(mdp);

                StringBuilder sbr = new StringBuilder();

                foreach (Byte a in pwdHashed)
                {
                    sbr.Append(a.ToString("X2"));
                }

                string ancienmdp = sbr.ToString();

                sQuery = "SELECT * from ANA_UTILISATEURS where Signature = '" + ancienmdp + "'";

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

        private byte[] CalculateSHA1(string str)
        {
            SHA1 sha256 = SHA1Managed.Create();
            byte[] hashValue;
            ASCIIEncoding obj = new ASCIIEncoding();
            hashValue = sha256.ComputeHash(obj.GetBytes(str));

            return hashValue;
        }

        protected void ChangerMdp_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(StrConn))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    labelErrorMsg.Text = "";

                    string numeroEntreprise = "";
                    string ancienMdp = txtAncienMdp.Value;
                    string nouveauMdp = txtNouveauMdp.Value;
                    string confirmMdp = txtConfirmationMdp.Value;

                    if (!confirmMdp.Equals(nouveauMdp))
                    {
                        labelErrorMsg.Text = "Les mots de passe ne sont pas identiques !!!";
                        labelErrorMsg.ForeColor = System.Drawing.Color.Red;
                        
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append(@"<script language='javascript'>");
                        sb.Append(@"$('#mdp-dialog').modal('show');");
                        sb.Append(@"</script>");
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());
                        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Show", strJsSuccess, true);
                    }
                    else
                    {
                        labelErrorMsg.Text = "";
                        byte[] pwdHashedNouveau = CalculateSHA1(nouveauMdp);
                        byte[] pwdHashedConfirm = CalculateSHA1(confirmMdp);

                        StringBuilder sbr1 = new StringBuilder();
                        foreach (Byte a1 in pwdHashedNouveau)
                        {
                            sbr1.Append(a1.ToString("X2"));
                        }
                        string mdpNouveau = sbr1.ToString();

                        StringBuilder sbr2 = new StringBuilder();
                        foreach (Byte a2 in pwdHashedConfirm)
                        {
                            sbr2.Append(a2.ToString("X2"));
                        }
                        string mdpConfirm = sbr2.ToString();


                        if (HttpContext.Current.Request.QueryString["Ent"] != null && HttpContext.Current.Request.QueryString["Ent"].ToString() != "")
                        {
                            numeroEntreprise = HttpContext.Current.Request.QueryString["Ent"].ToString();
                        }

                        try
                        {
                            connection.Open();
                            Boolean mdpExiste = getIfMotDePasseExist(ancienMdp);

                            if (!mdpExiste)
                            {
                                labelErrorMsg.Text = "L'ancien mot de passe n'est pas valide !!!";
                                labelErrorMsg.ForeColor = System.Drawing.Color.Red;

                                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                sb.Append(@"<script language='javascript'>");
                                sb.Append(@"$('#mdp-dialog').modal('show');");
                                sb.Append(@"</script>");
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());
                            }
                            else
                            {
                                int resultat = -1;
                                command.CommandText = "UPDATE ANA_UTILISATEURS SET Signature = '" + mdpConfirm + "' where userLogin = '" + Session["CODEAGENT"].ToString() + "' ";
                                resultat = command.ExecuteNonQuery();
                                if (resultat > 0)
                                {
                                    labelErrorMsg.Text = "Mot de passe modifié avec succès !!!";
                                    labelErrorMsg.ForeColor = System.Drawing.Color.Green;

                                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                    sb.Append(@"<script language='javascript'>");
                                    sb.Append(@"$('#mdp-dialog').modal('show');");
                                    sb.Append(@"</script>");
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());
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
}