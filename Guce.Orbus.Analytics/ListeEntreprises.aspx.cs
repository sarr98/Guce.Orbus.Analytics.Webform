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
using System.Web.UI.HtmlControls;

namespace Guce.Orbus.Analytics
{
    public partial class ListeEntreprises : System.Web.UI.Page
    {
        private String strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static string StrConn { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");

            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            txtNomEntreprise.Visible = false;
            EntrepriseList.Visible = true;

            StrConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            if (Session["NOMPOLE"] == null || Session["NOMPOLE"].ToString() == string.Empty)
            {
                Response.Redirect("Login.aspx", true);
            }
            else
            {
                if (!IsPostBack)
                {
                    ChargerDonnees();

                    if (typeActeurList != null && typeActeurList.SelectedValue != "-1")
                    {
                        ChargerEntrepriseList(typeActeurList.SelectedValue);
                    }
                    else
                    {
                        EntrepriseList.DataSource = new DataTable();
                        EntrepriseList.DataBind();
                        EntrepriseList.Items.Insert(0, new ListItem("Raison Sociale", "-1"));
                    }

                    //BindColumnsUsers();

                }
            }
        }

        protected void listActeur_SelectedIndexChanged(object sender, EventArgs e)
        {
            string acteur = typeActeurList.SelectedItem.Value;

            if (acteur != "-1" && acteur != "OC" && acteur != "ADM")
            {
                ChargerEntrepriseList(acteur);
                txtNomEntreprise.Visible = false;
                EntrepriseList.Visible = true;
                txtNomEntreprise.Text = "";
            }
            else
            {
                EntrepriseList.DataSource = new DataTable();
                EntrepriseList.DataBind();
                EntrepriseList.Items.Insert(0, new ListItem("Raison Sociale", "-1"));
                txtNomEntreprise.Visible = true;
                EntrepriseList.Visible = false;
            }

            if (acteur == "ADM")
            {
                txtNomEntreprise.Text = "Gainde 2000";
            }
        }

        private void ChargerEntrepriseList(string acteur)
        {
            string str = "";

            if (acteur == "IA" || acteur == "CA" || acteur == "CC" || acteur == "AU")
            {
                str = " select NOMOURAISONSOCIALEOPERATEUR as Entreprise,NumeroInscriptionOperateur as NumeroAbonne from ENTREPRISES where CodeTypeClient = '" + acteur + "' order by NOMOURAISONSOCIALEOPERATEUR ASC ";
            }
            else if (acteur == "ASS" || acteur == "BNQ")
            {
                str = " select DESIGNATIONPOLE as Entreprise,STR(IDPOLE) as NumeroAbonne from POLES where type like '%" + acteur + "%' order by NOMPOLE ASC ";
            }

            SqlCommand cmd = new SqlCommand(str);

            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    con.Open();
                    EntrepriseList.DataSource = cmd.ExecuteReader();
                    EntrepriseList.DataTextField = "Entreprise";
                    EntrepriseList.DataValueField = "NumeroAbonne";
                    EntrepriseList.DataBind();
                    EntrepriseList.Items.Insert(0, new ListItem("Raison Sociale", "-1"));
                    con.Close();
                }
            }

        }

        private void ChargerDonnees(string sortExpression = null)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            SqlConnection con = new SqlConnection(strConnString);
            con.Open();
            String str = "Select U.Id, U.IdEntreprise, U.Prenom, U.Nom, U.userLogin, U.Profil, E.NomouRaisonSociale, E.LibelleTypeEntreprise, " +
                         "Case When U.Etat = '1' then 'Actif' When U.Etat = '0' THEN 'Inactif' End As Etat " +
                         "FROM ANA_UTILISATEURS U  INNER JOIN ANA_ENTREPRISES E on (U.IdEntreprise = E.id) order by U.Prenom ";

            String numeroORBUS = txtNumeroOrbus.Text;
            String etat = EtatList.SelectedValue;
            String acteur = typeActeurList.SelectedValue;
            String nomEntreprise = "";
            String dateInscription = txtDateInscription.Text;
            String dateExpiration = txtDateExpiration.Text;

            if (acteur != "-1" && EntrepriseList.SelectedValue != "-1")
            {
                nomEntreprise = EntrepriseList.SelectedItem.Text;
            }
            else
            {
                nomEntreprise = txtNomEntreprise.Text;
            }

            if ((numeroORBUS.Equals("")) && (etat.Equals("-1")) && (acteur.Equals("-1")) && (nomEntreprise.Equals("-1") || nomEntreprise.Equals("")) && (dateInscription.Equals("")) && (dateExpiration.Equals("")))
            {
                str = "Select E.Id, E.IdOrbus, E.NomouRaisonSociale, E.LibelleTypeEntreprise, E.DateInscription, E.DateExpiration, " +
                      "Case When E.Etat = '1' then 'Actif' When E.Etat = '0' THEN 'Désactivé' End As Etat " +
                      "FROM ANA_ENTREPRISES E  order by E.NomouRaisonSociale ";
            }
            else
            {
                str = "Select E.Id, E.IdOrbus, E.NomouRaisonSociale, E.LibelleTypeEntreprise, E.DateInscription, E.DateExpiration, " +
                      "Case When E.Etat = '1' then 'Actif' When E.Etat = '0' THEN 'Désactivé' End As Etat " +
                      "FROM ANA_ENTREPRISES E  where 1=1 ";

                if (!numeroORBUS.Equals(""))
                {
                    str = str + " And E.IdOrbus LIKE '%" + numeroORBUS.Trim() + "%'";
                }

                if (!etat.Equals("-1"))
                {
                    str = str + " And E.Etat = '" + etat + "'"; //etat.Equals("-1") ? true_expression : false_expression;
                }

                if (!acteur.Equals("-1"))
                {
                    str = str + " And E.TypeEntreprise = '" + acteur + "'";
                }

                if ((!nomEntreprise.Equals("-1")) && (!nomEntreprise.Equals("")))
                {
                    str = str + " And E.NomouRaisonSociale = '" + nomEntreprise + "'";
                }

                if (dateInscription != "")
                {
                    DateTime d;
                    if (DateTime.TryParseExact(dateInscription, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d))
                    {
                        dateInscription = d.ToString("dd/MM/yyyy");
                    }

                    str = str + " and E.DateInscription  >= '" + dateInscription + "' ";
                }

                if (dateExpiration != "")
                {
                    DateTime d;
                    if (DateTime.TryParseExact(dateExpiration, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d))
                    {
                        dateExpiration = d.ToString("dd/MM/yyyy");
                    }
                    DateTime dateFin = Convert.ToDateTime(dateExpiration).AddDays(1);
                    str = str + " and E.DateExpiration  <= '" + dateFin.ToString() + "' ";
                }

                if (dateExpiration != "" && dateInscription != "")
                {
                    DateTime inscription = Convert.ToDateTime(dateInscription);
                    DateTime expiration = Convert.ToDateTime(dateExpiration);

                    if (inscription >= expiration)
                    {
                        msgPopup.Text = "La date Expiration doit être supérieure à la date Inscription !!!";
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append(@"<script language='javascript'>");
                        sb.Append(@"$('#mymodal-dialog').modal('show');");
                        sb.Append(@"</script>");
                        ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());

                    }
                }

                str = str + " order by E.NomouRaisonSociale ";
            }

            using (cmd = new SqlCommand(str, con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    GridEntreprises.DataSource = dt;
                    GridEntreprises.DataBind();
                }
                else
                {
                    GridEntreprises.DataSource = new DataTable();
                    GridEntreprises.DataBind();
                }
            }

            txtEntreprise.Value = "";

        }

        protected void Rechercher_Click(object sender, EventArgs e)
        {
            ChargerDonnees();
        }
        protected void GridEntreprises_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.ChargerDonnees(e.SortExpression);
        }

        protected void GridEntreprises_SelectedIndexChanged(object sender, EventArgs e)
        {
            //GridViewRow gr = GridEntreprises.SelectedRow;
            //string name = GridEntreprises.SelectedRow.Cells[3].Text;
            //Response.Write(name);
            //TextBox1.Text = GridEntreprises.SelectedRow.Cells[3].Text;

            txtEntreprise.Value = GridEntreprises.DataKeys[GridEntreprises.SelectedIndex].Values["Id"].ToString();

            activer_desactiver_Click();
        }

        private void activer_desactiver_Click()
        {

            String x = GridEntreprises.DataKeys[GridEntreprises.SelectedIndex].Values["Id"].ToString();
            int idEntreprise = 0;
            String majQuery = "";
            SqlTransaction transaction = null;
            SqlCommand command = null;
            SqlConnection con = new SqlConnection(strConnString);
            String etat = System.Convert.ToString(GridEntreprises.DataKeys[GridEntreprises.SelectedIndex].Values["Etat"]);
            string ReturnedValue = etat == "Actif" ? "0" : "1";

            if ((x != null) && (!x.Equals("")))
            {
                idEntreprise = int.Parse(x);
            }

            String modificationEtatOKScript = etat == "Actif" ? "Entreprise désactivée avec succès !!!" : "Entreprise activée avec succès !!!";
            String erreurMajEtat = " Mise à jour Statut Entreprise non effectuée !!!";

            majQuery = " update ANA_ENTREPRISES Set Etat ='" + ReturnedValue + "' where Id = " + idEntreprise;

            try
            {
                con.Open();
                transaction = con.BeginTransaction();
                command = con.CreateCommand();
                command.Transaction = transaction;

                if (UpdateDataT(majQuery, command) == 1)
                {
                    if (ReturnedValue.Equals("0"))
                    {
                        String majUsers = " update ANA_UTILISATEURS Set Etat ='" + ReturnedValue + "' where IdEntreprise = " + idEntreprise;
                        SqlCommand command2 = null;
                        command2 = con.CreateCommand();
                        command2.Transaction = transaction;

                        if (UpdateDataT(majUsers, command2) >= 1)
                        {
                            transaction.Commit();
                            ChargerDonnees();
                            msgPopup.Text = modificationEtatOKScript;
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            sb.Append(@"<script language='javascript'>");
                            sb.Append(@"$('#mymodal-dialog').modal('show');");
                            sb.Append(@"</script>");
                            ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());
                        }
                    }
                    else
                    {
                        transaction.Commit();
                        ChargerDonnees();
                        msgPopup.Text = modificationEtatOKScript;
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append(@"<script language='javascript'>");
                        sb.Append(@"$('#mymodal-dialog').modal('show');");
                        sb.Append(@"</script>");
                        ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());
                    }
                }
                else
                {
                    msgPopup.Text = erreurMajEtat;
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


        protected void GridEntreprises_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            GridEntreprises.PageIndex = e.NewPageIndex;
            ChargerDonnees();
        }

        protected void GridEntreprises_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridEntreprises.Columns[0].Visible = false;
            GridEntreprises.Columns[6].Visible = false;

            //int index = int.Parse(e.CommandArgument.ToString());
            //ImageButton imageButton = ((GridView)e.CommandSource).Rows[index].Cells[0].Controls[0] as ImageButton;
            //imageButton.ImageUrl = "/images/rbullet.gif";

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

    }
}