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

namespace Analytics
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private byte[] CalculateSHA1(string str)
        {
            SHA1 sha256 = SHA1Managed.Create();
            byte[] hashValue;
            ASCIIEncoding obj = new ASCIIEncoding();
            hashValue = sha256.ComputeHash(obj.GetBytes(str));

            return hashValue;
        }

        //public String isBollore()
        //{
        //    String strBollore = "";
        //    String sqlQuery = "";

        //    try
        //    {
        //        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        //        sqlQuery = "select distinct NUMEROINSCRIPTIONABONNE from OPERATEUR where NOMOURAISONSOCIALEOPERATEUR like '%BOLLORE AFRICA LOGISTIC%'";
        //        SqlCommand sqlCmd = new SqlCommand(sqlQuery, con);
        //        con.Open();

        //        using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
        //        {
        //            while (sqlReader.Read())
        //            {
        //                strBollore = strBollore + sqlReader.GetValue(0).ToString() + ",";

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write(ex.Message);
        //    }


        //    return strBollore;
        //}


        protected void Button1_Click(object sender, EventArgs e)
        {
            String pwd = Request.Form["TextBox2"];

            byte[] pwdHashed = CalculateSHA1(pwd);

            StringBuilder sb = new StringBuilder();

            foreach (Byte a in pwdHashed)
            {
                sb.Append(a.ToString("X2"));
            }

            pwd = sb.ToString();

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            string queryString = " SELECT uti.nom, uti.prenom, uti.userLogin, ent.IdOrbus, ent.NomouRaisonSociale, ent.TypeEntreprise , uti.Profil , ent.Id , ent.DateExpiration FROM ANA_UTILISATEURS uti " +
                                 " INNER JOIN ANA_ENTREPRISES  ent on uti.IdEntreprise = ent.Id " +
                                 " WHERE uti.userLogin = '" + Request.Form["TextBox1"] + "' AND uti.Signature = '" + pwd + "'";

            //string queryString = "SELECT NOMAGENT,PRENOMAGENT,POLEUSER,CODEAGENT FROM AGENT WHERE CODEAGENT='" + Request.Form["TextBox1"] + "' AND SIGNATURE='" + pwd + "' ; " +
            //                     "SELECT NUMEROINSCRIPTIONABONNE,NOMUTILISATEUR,PRENOMUTILISATEUR,NOMOURAISONSOCIALEOPERATEUR FROM OPERATEUR WHERE LOGINOPERATEUR='" + Request.Form["TextBox1"] + "' AND MOTDEPASSEOPERATEUR='" + pwd + "'";

            try
            {
                SqlCommand sqlCmd = new SqlCommand(queryString, con);
                con.Open();
                Boolean trouve = false;

                using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                {
                    while (sqlReader.Read())
                    {
                        Session["NOMAGENT"] = sqlReader.GetValue(0).ToString();
                        Session["PRENOMAGENT"] = sqlReader.GetValue(1).ToString();
                        Session["CODEAGENT"] = sqlReader.GetValue(2).ToString();
                        Session["NOMPOLE"] = sqlReader.GetValue(4).ToString();
                        Session["PROFILAGENT"] = sqlReader.GetValue(6).ToString();

                        Session["NumENTREPRISE"] = sqlReader.GetValue(7).ToString();

                        //Djoss
                        Session["TypeEntreprise"] = sqlReader.GetValue(5).ToString();

                        String typeEntreprise = sqlReader.GetValue(5).ToString();

                        DateTime jourDate = DateTime.Now;

                        DateTime dateExpiration = Convert.ToDateTime(sqlReader.GetValue(8).ToString());

                        if (dateExpiration < jourDate)
                        {
                            labelMsg.InnerText = " Votre souscription est expirée !!!";
                        }
                        else
                        {
                            if (typeEntreprise == "BNQ" || typeEntreprise == "ASS")
                            {
                                Session["IdPole"] = sqlReader.GetValue(3).ToString();
                            }
                            else if (typeEntreprise == "IA" || typeEntreprise == "CA" || typeEntreprise == "CC" || typeEntreprise == "AU")
                            {

                                Session["NUMEROINSCRIPTIONABONNE"] = sqlReader.GetValue(3).ToString();

                            }

                            trouve = true;
                        }

                    }

                    //sqlReader.NextResult();

                    //while (sqlReader.Read())
                    //{
                    //    Session["NOMAGENT"] = sqlReader.GetValue(1).ToString();
                    //    Session["PRENOMAGENT"] = sqlReader.GetValue(2).ToString();
                    //    Session["NOMPOLE"] = sqlReader.GetValue(3).ToString();
                    //    Session["NUMEROINSCRIPTIONABONNE"] = sqlReader.GetValue(0).ToString();
                    //    trouve = true;
                    //}

                    if (!trouve)
                    {
                        labelMsg.InnerText = " Utilisateur ou Mot de passe incorrect !!!";
                    }
                    else
                    {
                        labelMsg.InnerText = "";
                        Response.Redirect("Accueil.aspx");
                    }

                    sqlReader.Close();
                    sqlCmd.Dispose();
                    con.Close();
                }


            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }


        }

      


    }
}