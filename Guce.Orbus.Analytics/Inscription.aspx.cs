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
    public partial class Inscription : System.Web.UI.Page
    {
        private String strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private String strNumInscription = "";
        private String strIdPole = "";
        public static string StrConn { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            StrConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            //ChargerListeDevise();

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            //
            // Set the thread's CurrentCulture the same as CurrentUICulture.
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            if (Session["NOMPOLE"] == null || Session["NOMPOLE"].ToString() == string.Empty)
            {
                Response.Redirect("Login.aspx", true);
            }
            else
            {
                if (!IsPostBack)
                {
                    if (typeActeurList != null && typeActeurList.SelectedValue != "-1" && typeActeurList.SelectedValue != "ADM")
                    {
                        ChargerEntrepriseList(typeActeurList.SelectedValue);
                    }
                    else
                    {
                        EntrepriseList.DataSource = new DataTable();
                        EntrepriseList.DataBind();
                        EntrepriseList.Items.Insert(0, new ListItem("Choisir", "-1"));
                    }

                }
            }


        }

        protected void listActeur_SelectedIndexChanged(object sender, EventArgs e)
        {
            string acteur = typeActeurList.SelectedItem.Value;

            if (acteur != "-1" && acteur != "OC" && acteur != "ADM")
            {
                ChargerEntrepriseList(acteur);
             
                EntrepriseList.Visible = true;
                txtNomEntreprise.Visible = false;
            }
            else
            {
                if (acteur == "-1")
                {
                    EntrepriseList.DataSource = null;
                    EntrepriseList.Items.Clear();
                    EntrepriseList.Visible = true;
                    txtNomEntreprise.Visible = false;
                }
                else
                {
                    EntrepriseList.Visible = false;
                    txtNomEntreprise.Visible = true;
                    txtNomEntreprise.Text = "Gainde 2000";
                }
            }
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

        public Boolean verifierIfEntrepriseExist(String numeroInscription)
        {
            String sQuery = "";
            Boolean existe = false;
            string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            try
            {
                sQuery = "SELECT * from ANA_ENTREPRISES where IdOrbus = '" + numeroInscription + "'";

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


        private void ChargerEntrepriseList(string acteur)
        {
            if (acteur != "-1" && acteur != null)
            {
                string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                string str = "";

                if (acteur == "IA" || acteur == "CA" || acteur == "CC" || acteur == "AU")
                {
                    str = " select NOMOURAISONSOCIALEOPERATEUR as Entreprise,NumeroInscriptionOperateur as NumeroAbonne from ENTREPRISES where CodeTypeClient = '" + acteur + "' order by NOMOURAISONSOCIALEOPERATEUR ASC ";
                }
                else if (acteur == "ASS" || acteur == "BNQ")
                {
                    str = " select NOMPOLE as Entreprise,STR(IDPOLE) as NumeroAbonne from POLES where type like '%" + acteur + "%' order by NOMPOLE ASC ";
                }

                SqlCommand cmd = new SqlCommand(str);

                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        con.Open();
                        EntrepriseList.DataSource = cmd.ExecuteReader();
                        EntrepriseList.DataTextField = "Entreprise";
                        EntrepriseList.DataValueField = "NumeroAbonne";
                        EntrepriseList.DataBind();
                        EntrepriseList.Items.Insert(0, new ListItem(" ", "-1"));
                        con.Close();
                    }
                }
            }
            else
            {
                EntrepriseList.DataSource = null;
            }


        }


        public class EntrepriseDetails
        {
            public string NumeroAbonne { get; set; }
            public string NOMOURAISONSOCIALEOPERATEUR { get; set; }
        }

        public class Entreprise
        {
            public string NumeroAbonne { get; set; }
            public string NOMOURAISONSOCIALEOPERATEUR { get; set; }
            public string Telephone { get; set; }
            public string Adresse { get; set; }
        }

        [System.Web.Services.WebMethod]
        public static List<EntrepriseDetails> ChargerListeEntreprise(string acteur)
        {
            List<EntrepriseDetails> details = new List<EntrepriseDetails>();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            using (SqlConnection con = new SqlConnection(StrConn))
            {
                String str = "";

                if (acteur == "IA" || acteur == "CA" || acteur == "CC" || acteur == "AU")
                {
                    str = " select NOMOURAISONSOCIALEOPERATEUR as Entreprise,NumeroInscriptionOperateur as NumeroAbonne from ENTREPRISES where CodeTypeClient = '" + acteur + "' order by NOMOURAISONSOCIALEOPERATEUR ASC ";
                }
                else if (acteur == "ASS" || acteur == "BNQ")
                {
                    str = " select NOMPOLE as Entreprise,STR(IDPOLE) as NumeroAbonne from POLES where type like '%" + acteur + "%' order by NOMPOLE ASC ";
                }

                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    details.Clear();

                    var data = from clients in dt.AsEnumerable()
                               select new
                               {
                                   NumeroInscriptionOperateur = clients.Field<string>("NumeroAbonne"),
                                   NOMOURAISONSOCIALEOPERATEUR = clients.Field<string>("Entreprise")

                               };

                    foreach (var resultat in data)
                    {
                        EntrepriseDetails client = new EntrepriseDetails();
                        client.NumeroAbonne = resultat.NumeroInscriptionOperateur.ToString();
                        client.NOMOURAISONSOCIALEOPERATEUR = resultat.NOMOURAISONSOCIALEOPERATEUR.ToString();

                        details.Add(client);
                    }
                }

            }

            return details;
        }


        [System.Web.Services.WebMethod]
        public static List<Entreprise> ChargerDonneesEntreprise(string acteur, string numero)
        {
            List<Entreprise> details = new List<Entreprise>();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            using (SqlConnection con = new SqlConnection(StrConn))
            {
                String str = "";
                Boolean indicateur = false;
                if (acteur == "IA" || acteur == "CA" || acteur == "CC" || acteur == "AU")
                {
                    indicateur = true;
                    str = " select NOMOURAISONSOCIALEOPERATEUR as Entreprise,NumeroInscriptionOperateur as NumeroAbonne,ADRESSEOPERATEUR as Adresse,TELEPHONEOPERATEUR as Telephone from ENTREPRISES where NumeroInscriptionOperateur = '" + numero + "' order by NOMOURAISONSOCIALEOPERATEUR ASC ";
                }
                else if (acteur == "ASS" || acteur == "BNQ")
                {
                    str = " select NOMPOLE as Entreprise,STR(IDPOLE) as NumeroAbonne from POLES where IDPOLE = " + numero + " order by NOMPOLE ASC ";
                }

                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {

                    var data = from clients in dt.AsEnumerable()
                               select new
                               {
                                   NumeroInscriptionOperateur = clients.Field<string>("NumeroAbonne"),
                                   NOMOURAISONSOCIALEOPERATEUR = clients.Field<string>("Entreprise"),
                                   Telephone = indicateur ? clients.Field<string>("Telephone") : "",
                                   Adresse = indicateur ? clients.Field<string>("Adresse") : ""
                               };

                    foreach (var resultat in data)
                    {
                        Entreprise client = new Entreprise();
                        client.NumeroAbonne = resultat.NumeroInscriptionOperateur.ToString();
                        client.NOMOURAISONSOCIALEOPERATEUR = resultat.NOMOURAISONSOCIALEOPERATEUR.ToString();
                        client.Telephone = resultat.Telephone;
                        client.Adresse = resultat.Adresse;
                        details.Add(client);
                    }
                }

            }

            return details;
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

        private byte[] CalculateSHA1(string str)
        {
            SHA1 sha256 = SHA1Managed.Create();
            byte[] hashValue;
            ASCIIEncoding obj = new ASCIIEncoding();
            hashValue = sha256.ComputeHash(obj.GetBytes(str));

            return hashValue;
        }

        protected void Valider_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(StrConn))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;

                    string nomEntreprise = "";
                    string numeroOrbus = "";
                    string adresseEntreprise = txtAdresseEntreprise.Text;
                    string telEntreprise = txtTelEntreprise.Text;
                    string typeActeur = typeActeurList.SelectedValue;
                    string libelleTypeActeur = typeActeurList.SelectedItem.ToString();

                    if (EntrepriseList.SelectedItem.Value != "-1")
                    {
                        numeroOrbus = EntrepriseList.SelectedItem.Value;
                        nomEntreprise = EntrepriseList.SelectedItem.Text;
                    }
                    else
                    {
                        nomEntreprise = txtNomEntreprise.Text;
                    }

                    byte[] pwdHashed = CalculateSHA1("P@sser123");

                    StringBuilder sbr = new StringBuilder();

                    foreach (Byte a in pwdHashed)
                    {
                        sbr.Append(a.ToString("X2"));
                    }

                    string mdp = sbr.ToString();

                    numeroOrbus = numeroOrbus != "" ? numeroOrbus.Trim() : numeroOrbus;
                    telEntreprise = telEntreprise != "" ? telEntreprise.Trim() : telEntreprise;

                    command.CommandText = "INSERT into ANA_ENTREPRISES (idOrbus, NomouRaisonSociale, Adresse, Telephone, TypeEntreprise, LibelleTypeEntreprise, Etat, DateInscription, DateExpiration) " +
                                           " VALUES ('" + numeroOrbus + "', '" + nomEntreprise + "', '" + adresseEntreprise + "', '" + telEntreprise + "', '" + typeActeur + "', '" + libelleTypeActeur + "', '1', '" + DateTime.Now + "', '" + DateTime.Now.AddYears(1) + "')";

                    try
                    {
                        connection.Open();

                        Boolean trouverEntreprise = verifierIfEntrepriseExist(numeroOrbus);

                        if (trouverEntreprise)
                        {
                            msgPopup.Text = "Cette entreprise est déjà enregistrée !!!";
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            sb.Append(@"<script language='javascript'>");
                            sb.Append(@"$('#mymodal-dialog').modal('show');");
                            sb.Append(@"</script>");

                            ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());
                        }
                        else
                        {
                            int recordsAffected = command.ExecuteNonQuery();

                            if (recordsAffected > 0)
                            {
                                int numeroEntreprise = 0;
                                command.CommandText = "SELECT @@IDENTITY";
                                numeroEntreprise = Convert.ToInt32(command.ExecuteScalar());
                                string nomAdmin = txtNomAdmin.Text;
                                string prenomAdmin = txtPrenomSuperviseur.Text;
                                string emailAdmin = txtEmailSuperviseur.Text;
                                string loginAdmin = txtLoginAdmin.Text;
                                string telephoneAdmin = txtTelAdmin.Text;

                                Boolean existeEmail = getIfLoginExist(loginAdmin);

                                if (existeEmail)
                                {
                                    txtIdentificationOrbus.Value = "" + numeroEntreprise;
                                    msgPopup.Text = "Un utilisateur est déjà enregistré avec cet email !!!";
                                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                    sb.Append(@"<script language='javascript'>");
                                    sb.Append(@"$('#mymodal-dialog').modal('show');");
                                    sb.Append(@"</script>");

                                    ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());
                                }
                                else
                                {
                                    txtIdentificationOrbus.Value = "";
                                    string insertQuery = "INSERT INTO ANA_UTILISATEURS (userLogin,Nom,Prenom,Email,Profil,Telephone,IdEntreprise,Etat,DateCreation,Signature) " +
                                "VALUES('" + loginAdmin + "', '" + replaceCode(nomAdmin) + "', '" + replaceCode(prenomAdmin) + "', '" + emailAdmin + "', 'Superviseur', " + telephoneAdmin + "," + numeroEntreprise + ", '1','" + DateTime.Now + "', '" + mdp + "')";

                                    int resultat = -1;
                                    //command.Connection = connection;
                                    command.CommandText = insertQuery;
                                    resultat = command.ExecuteNonQuery();

                                    if (resultat > 0)
                                    {
                                        msgPopup.Text = "L'inscription a été validée avec succès !!!";
                                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                        sb.Append(@"<script language='javascript'>");
                                        sb.Append(@"$('#mymodal-dialog').modal('show');");
                                        sb.Append(@"</script>");

                                        ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());

                                    }
                                    else
                                    {
                                        command.CommandText = " delete from ANA_ENTREPRISES where id = " + numeroEntreprise;

                                        resultat = command.ExecuteNonQuery();

                                        msgPopup.Text = "Une erreur a été rencontrée !!!";
                                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                        sb.Append(@"<script language='javascript'>");
                                        sb.Append(@"$('#mymodal-dialog').modal('show');");
                                        sb.Append(@"</script>");

                                        ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());

                                    }

                                    if ((recordsAffected > 0) && (resultat > 0))
                                    {
                                        Response.Redirect("ListeEntreprises.aspx");
                                    }
                                }
                            }
                        }

                        //if (txtIdentificationOrbus.Value != "")
                        //{

                        //        int numeroEntreprise = Convert.ToInt32(txtIdentificationOrbus.Value);
                        //        string nomAdmin = txtNomAdmin.Text;
                        //        string prenomAdmin = txtPrenomSuperviseur.Text;
                        //        string emailAdmin = txtEmailSuperviseur.Text;
                        //        string loginAdmin = txtEmailSuperviseur.Text;
                        //        string telephoneAdmin = txtTelAdmin.Text;

                        //        Boolean existeEmail = getIfEmailExist(emailAdmin);

                        //        if (existeEmail)
                        //        {
                        //            txtIdentificationOrbus.Value = "" + numeroEntreprise;
                        //            msgPopup.Text = "Un utilisateur est déjà enregistré avec cet email !!!";
                        //            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        //            sb.Append(@"<script language='javascript'>");
                        //            sb.Append(@"$('#myModal').modal('show');");
                        //            sb.Append(@"</script>");

                        //            ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());

                        //        }
                        //        else
                        //        {
                        //            txtIdentificationOrbus.Value = "";

                        //            String insertQuery = "INSERT INTO ANA_UTILISATEURS (userLogin,Nom,Prenom,Email,Profil,Telephone,IdEntreprise,Etat,DateCreation) " +
                        //        "VALUES('" + loginAdmin + "', '" + replaceCode(nomAdmin) + "', '" + replaceCode(prenomAdmin) + "', '" + emailAdmin + "', 'Superviseur', " + telephoneAdmin + "," + numeroEntreprise + ", '1','" + DateTime.Now + "')";

                        //            int resultat = -1;
                        //            //command.Connection = connection;
                        //            command.CommandText = insertQuery;
                        //            resultat = command.ExecuteNonQuery();

                        //            if (resultat > 0)
                        //            {
                        //                msgPopup.Text = "L'inscription a été validée avec succès !!!";
                        //                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        //                sb.Append(@"<script language='javascript'>");
                        //                sb.Append(@"$('#myModal').modal('show');");
                        //                sb.Append(@"</script>");

                        //                ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());
                        //            }
                        //            else
                        //            {
                        //                command.CommandText = " delete from ANA_ENTREPRISES where id = " + numeroEntreprise;

                        //                resultat = command.ExecuteNonQuery();

                        //                msgPopup.Text = "Une erreur a été rencontrée !!!";
                        //                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        //                sb.Append(@"<script language='javascript'>");
                        //                sb.Append(@"$('#myModal').modal('show');");
                        //                sb.Append(@"</script>");

                        //                ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());

                        //            }
                        //        }
                        //    //}
                        //}
                        //else
                        //{

                        //}




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

        protected void Annuler_Click(object sender, EventArgs e)
        {

        }

    }
}