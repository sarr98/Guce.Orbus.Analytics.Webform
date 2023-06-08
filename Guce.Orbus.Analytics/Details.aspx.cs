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
    public partial class Details : System.Web.UI.Page
    {
        private String strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private String strNumInscription = "";
        private String strIdPole = "";
        public static DataTable tempDtPays = new DataTable();
        public static DataTable tempDtProduits = new DataTable();
        public static DataTable tempDtBeneficiaire = new DataTable();

        public static string StrConn { get; set; }
        private string SortDirection
        {
            get { return ViewState["SortDirection"] != null ? ViewState["SortDirection"].ToString() : "ASC"; }
            set { ViewState["SortDirection"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            StrConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");

            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            resultatDetails.Visible = false;

            if (Session["NUMEROINSCRIPTIONABONNE"] != null)
            {
                strNumInscription = Session["NUMEROINSCRIPTIONABONNE"].ToString();
            }
            else if (Session["IdPole"] != null)
            {
                strIdPole = Session["IdPole"].ToString();
            }

            if (Session["NOMPOLE"] == null || Session["NOMPOLE"].ToString() == string.Empty)
            {
                Response.Redirect("Login.aspx", true);
            }
            else
            {
                if (!IsPostBack)
                {
                    ChargerTempPays();
                    tempDtPays = (DataTable)TempGridPays.DataSource;
                    ChargerTempProduits();
                    tempDtProduits = (DataTable)TempGridProduits.DataSource;
                    ChargerTempBeneficiaire();
                    tempDtBeneficiaire = (DataTable)TempGridBeneficiaire.DataSource;

                    ChargerListeDevise();

                }
            }


        }

        protected void Rechercher_Click(object sender, EventArgs e)
        {
            ChargerDonnees();
        }

        private void ChargerDonnees(string sortExpression = null)
        {
            String natureOperation = Drop_Operation_Rech.Value + "";
            natureOperation = natureOperation.Trim();

            String beneficiaire = txtBeneficiaire.Text + "";
            beneficiaire = beneficiaire.Trim();

            String paysProvenance = txtPaysProvenance.Text + "";
            paysProvenance = paysProvenance.Trim();

            String paysOrigine = txtPaysOrigine.Text + "";
            paysOrigine = paysOrigine.Trim();

            String debut = Text_DebutPeriode_Rech.Text + "";
            debut = debut.Trim();

            String fin = Text_FinPeriode_Rech.Text + "";
            fin = fin.Trim();

            String devise = DeviseList.Value + "";

            String produit = txtProduit.Text + "";

            String str = "";

            SqlConnection con = new SqlConnection(strConnString);
            con.Open();

            int numIncsriptionClient = 0;
            int idPole = 0;

            if (!strNumInscription.Equals(""))
            {
                numIncsriptionClient = Int32.Parse(strNumInscription);
            }
            else
            {
                idPole = Int32.Parse(strIdPole);
            }

            if (numIncsriptionClient > 0)
            {
                str = " SELECT co.NUMERODOSSIERTPS [NumeroDossier], co.NUMEROTARIFDOUANE [CodeTarifDouane], co.DESIGNATIONCOMMERCIALE [Produit], co.PAYSPROVENANCE [Provenance], co.PAYSORIGINE [Origine], co.QUANTITEMESURE [PoidsNet], co.VALEURCFA [ValeurCfa],do.NOMOURAISONSOCIALEBENEFICIAIRE [Beneficiaire] " +
                      " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                      " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                      " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                      " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                      " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient ;
            }
            else
            {
                str = " SELECT co.NUMERODOSSIERTPS [NumeroDossier], co.NUMEROTARIFDOUANE [CodeTarifDouane], co.DESIGNATIONCOMMERCIALE [Produit], co.PAYSPROVENANCE [Provenance], co.PAYSORIGINE [Origine], co.QUANTITEMESURE [PoidsNet], co.VALEURCFA [ValeurCfa],do.NOMOURAISONSOCIALEBENEFICIAIRE [Beneficiaire] " +
                      " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                      " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                      " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                      " WHERE 1=1 ";

            }


            ///////////////AJOUT DJOSSOU: si banque alors prendre en compte le parametre domiciliation /////////////////////

            string typePole = "";
            string typeDossier = "";
            string strDom = "";

            //Si c'est un pole
            if ((numIncsriptionClient <= 0) && (idPole > 0))
            {
                try
                {
                    //récupération type pole
                    typePole = Session["TypeEntreprise"].ToString();
                    ////typePole = GetTypePole(idPole);

                    //si banque alors prendre en compte le parametre domiciliation
                    if ((!string.IsNullOrEmpty(typePole)) && (typePole.ToLowerInvariant().Equals("bnq")))
                    {
                        typeDossier = TypeDossierList.Value + "";

                        switch (typeDossier.Trim())
                        {
                            case "1":  //<--- dossiers domiciliés

                                if ((natureOperation == "1") || (natureOperation == "3")) //Si importation (import ou transit)
                                {
                                    strDom = string.Format(" and exists(select * from REGISTREDOMICILIATION re " +
                                                           " where re.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS AND re.IDPOLE = {0}) ", idPole);
                                }
                                else if ((natureOperation == "2") || (natureOperation == "4")) ////Si exportation (export ou réexport)
                                {
                                    strDom = string.Format(" and exists(select * from REGISTREDOMICILIATIONEXPORT re " +
                                                           " where re.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS AND re.IDPOLE = {0}) ", idPole);
                                }
                                else
                                {
                                    //Ensemble des dossiers domiciliés à l'import comme à l'export
                                    strDom = string.Format(" and ( exists(select * from REGISTREDOMICILIATION re " +
                                                           " where re.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS AND re.IDPOLE = {0}) " +
                                                           "  OR exists(select * from REGISTREDOMICILIATIONEXPORT reex " +
                                                           "  where reex.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS AND reex.IDPOLE = {0}) " +
                                                           ") ", idPole);
                                }

                                break;

                            case "2":     //<--- dossiers non domiciliés

                                if ((natureOperation == "1") || (natureOperation == "3")) //Si importation (import ou transit)
                                {
                                    strDom = string.Format(" and not exists(select * from REGISTREDOMICILIATION re " +
                                                           " where re.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS AND re.IDPOLE = {0}) ", idPole);
                                }
                                else if ((natureOperation == "2") || (natureOperation == "4")) ////Si exportation (export ou réexport)
                                {
                                    strDom = string.Format(" and not exists(select * from REGISTREDOMICILIATIONEXPORT re " +
                                                           " where re.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS AND re.IDPOLE = {0}) ", idPole);
                                }
                                else
                                {
                                    //Ensemble des dossiers non domiciliés à l'import comme à l'export
                                    strDom = string.Format(" and (not exists(select * from REGISTREDOMICILIATION re " +
                                                           " where re.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS AND re.IDPOLE = {0}) " +
                                                           "  AND not exists(select * from REGISTREDOMICILIATIONEXPORT reex " +
                                                           "  where reex.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS AND reex.IDPOLE = {0}) " +
                                                           ") ", idPole);
                                }

                                break;

                            default:
                                break;
                        }


                    }

                }
                catch (Exception ex)
                {

                }



            }

            //Ajout à la requête
            str = str + strDom;


            ////////////////////////////////////////////////////////////////////////////////////////////////




            if (beneficiaire != "")
            {
                beneficiaire = beneficiaire.Replace("'", "''");

                if (beneficiaire.Contains(';'))
                {

                    string[] val = beneficiaire.Split(';');
                    string strBeneficiaire = "'" + val[0] + "'";
                    int i = 1;

                    for (i = 1; i < val.Length; i++)
                    {
                        strBeneficiaire = strBeneficiaire + ",'" + val[i] + "'";
                    }

                    str = str + " and do.CODEPPM in (" + strBeneficiaire + ")";

                }
                else
                {
                    str = str + " and do.CODEPPM = '" + beneficiaire + "'";
                }

            }

            if (produit != "")
            {
                produit = produit.Replace("'", "''");

                if (produit.Contains(';'))
                {

                    string[] val = produit.Split(';');
                    string strProduit = "'" + val[0] + "'";
                    int i = 1;

                    for (i = 1; i < val.Length; i++)
                    {
                        strProduit = strProduit + ",'" + val[i] + "'";
                    }

                    str = str + " and co.NUMEROTARIFDOUANE in (" + strProduit + ")";

                }
                else
                {
                    str = str + " and co.NUMEROTARIFDOUANE = '" + produit + "'";
                }

            }

            if (natureOperation != "-1")
            {
                if (natureOperation == "1")
                {
                    str = str + " and do.IMPORTATIONOUEXPORTATION = 'I' ";
                }
                else if (natureOperation == "2")
                {
                    str = str + " and do.IMPORTATIONOUEXPORTATION = 'E' ";
                }
                else if (natureOperation == "3")
                {
                    str = str + " and do.IMPORTATIONOUEXPORTATION = 'S' ";
                }
                else if (natureOperation == "4")
                {
                    str = str + " and do.IMPORTATIONOUEXPORTATION = 'R' ";
                }
            }

            if (paysProvenance != "")
            {
                if (paysProvenance.Contains(';'))
                {

                    string[] val = paysProvenance.Split(';');
                    string strPays = "'" + val[0] + "'";
                    int i = 1;

                    for (i = 1; i < val.Length; i++)
                    {
                        strPays = strPays + ",'" + val[i] + "'";
                    }

                    str = str + " and co.PAYSPROVENANCE in (" + strPays + ")";

                }
                else
                {
                    str = str + " and co.PAYSPROVENANCE = '" + paysProvenance + "'";
                }

            }

            if (paysOrigine != "")
            {
                if (paysOrigine.Contains(';'))
                {

                    string[] val = paysOrigine.Split(';');
                    string strPays = "'" + val[0] + "'";
                    int i = 1;

                    for (i = 1; i < val.Length; i++)
                    {
                        strPays = strPays + ",'" + val[i] + "'";
                    }

                    str = str + " and co.PAYSORIGINE in (" + strPays + ")";

                }
                else
                {
                    str = str + " and co.PAYSORIGINE = '" + paysOrigine + "'";
                }

            }

            if (devise != "-1")
            {
                str = str + " and fa.CODEDEVISE = '" + devise + "'";
            }



            if (debut != "")
            {
                // Isdate de datedebut    
                DateTime d;
                if (DateTime.TryParseExact(debut, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d))
                {
                    debut = d.ToString("dd/MM/yyyy");
                }

                str = str + " and do.DATEDOSSIERTPS  >= '" + debut + "' ";
                //convert(datetime,'" + debut + "',103)";
            }

            if (fin != "")
            {
                DateTime d;
                if (DateTime.TryParseExact(fin, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d))
                {
                    fin = d.ToString("dd/MM/yyyy");
                }

                DateTime dateFin = Convert.ToDateTime(fin).AddDays(1);
                str = str + " and do.DATEDOSSIERTPS  <= '" + dateFin.ToString() + "' ";
            }

            Boolean verifChamps = true;

            if (debut != "" && fin != "")
            {
                DateTime dateDebut = Convert.ToDateTime(debut);
                DateTime dateFin = Convert.ToDateTime(fin);

                if (dateDebut >= dateFin)
                {
                    msgPopup.Text = "La date de fin doit être supérieure à la date de début !!!";
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(@"<script language='javascript'>");
                    sb.Append(@"$('#mymodal-dialog').modal('show');");
                    sb.Append(@"</script>");

                    ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());

                    verifChamps = false;
                }
                else
                {
                    verifChamps = true;
                }

            }

            if (verifChamps)
            {
                if (idPole > 0)
                {
                    str = str + " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS) ";
                }

                str = str + " ORDER BY 1 ASC ";




                ////////////////AJOUT DJOSSOU: Pour recadrer la requete et pour l'exportation///////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////

                if ((natureOperation == "2") || (natureOperation == "4"))
                {
                    str = str.Replace("co.PAYSPROVENANCE", "co.PAYSDESTINATION");
                }


                /////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////////////////////////////////////



                var f = new NumberFormatInfo { NumberGroupSeparator = " " };

                DataTable dtDepart = new DataTable();
                DataTable dtFinal = new DataTable();
                SqlCommand cmd1 = new SqlCommand();
                SqlCommand cmd2 = new SqlCommand();
                
                try
                {


                    using (cmd1 = new SqlCommand(str, con))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd1);
                        da.Fill(dtDepart);

                        if (dtDepart.Rows.Count > 0)
                        {
                            string paysDirection = "Provenance";

                            ////////////////AJOUT DJOSSOU: pour l'exportation///////////////////
                            ////////////////////////////////////////////////////////////////////////////////////////////////

                            if ((natureOperation == "2") || (natureOperation == "4"))
                                paysDirection = "Destination";

                            /////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////

                            resultatDetails.Visible = true;
                            dtFinal.Columns.Add("Dossier", typeof(string));
                            dtFinal.Columns.Add("Code Produit", typeof(string));
                            dtFinal.Columns.Add("Désignation", typeof(string));
                            dtFinal.Columns.Add(paysDirection, typeof(string));
                            dtFinal.Columns.Add("Origine", typeof(string));
                            dtFinal.Columns.Add("Bénéficiaire", typeof(string));
                            dtFinal.Columns.Add("Poids Net", typeof(decimal));
                            dtFinal.Columns.Add("Valeur (CFA)", typeof(decimal));

                            for (int i = 0; i < dtDepart.Rows.Count; i++)
                            {
                                decimal poidsNet = string.IsNullOrEmpty(dtDepart.Rows[i]["PoidsNet"].ToString()) ? 0 : Convert.ToDecimal(dtDepart.Rows[i]["PoidsNet"].ToString());
                                decimal valeurCfa = string.IsNullOrEmpty(dtDepart.Rows[i]["ValeurCfa"].ToString()) ? 0 : Convert.ToDecimal(dtDepart.Rows[i]["ValeurCfa"].ToString());

                                dtFinal.Rows.Add(dtDepart.Rows[i]["NumeroDossier"].ToString(), dtDepart.Rows[i]["CodeTarifDouane"].ToString(), dtDepart.Rows[i]["Produit"].ToString(), dtDepart.Rows[i]["Provenance"].ToString(), dtDepart.Rows[i]["Origine"].ToString(), dtDepart.Rows[i]["Beneficiaire"].ToString(), poidsNet, valeurCfa);

                            }

                            if (sortExpression != null)
                            {
                                DataView dv = dtFinal.AsDataView();
                                this.SortDirection = this.SortDirection == "ASC" ? "DESC" : "ASC";

                                dv.Sort = sortExpression + " " + this.SortDirection;
                                DetailsListeGridView.DataSource = dv;
                            }
                            else
                            {
                                DetailsListeGridView.DataSource = dtFinal;
                            }

                            //DetailsListeGridView.DataSource = dtFinal;
                            DetailsListeGridView.DataBind();

                        }
                        else
                        {
                            DetailsListeGridView.DataSource = dtDepart;
                            DetailsListeGridView.DataBind();
                        }
                    }

                    if (str != "")
                    {
                        //si export ce replace ne fonctionnera pas car la chaine contient "co.PAYSDESTINATION" et non "co.PAYSPROVENANCE"
                        str = str.Replace("co.NUMERODOSSIERTPS [NumeroDossier], co.NUMEROTARIFDOUANE [CodeTarifDouane], co.DESIGNATIONCOMMERCIALE [Produit], co.PAYSPROVENANCE [Provenance], co.PAYSORIGINE [Origine], co.QUANTITEMESURE [PoidsNet], co.VALEURCFA [ValeurCfa],do.NOMOURAISONSOCIALEBENEFICIAIRE [Beneficiaire]", " count(do.NUMERODOSSIERTPS), sum(fa.VALEURTOTALECFA) ");


                        //////////////////////////AJOUT DJOSSOU: Pour recadrer la requete //////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////

                        //si export ce replace ne fonctionnera pas car la chaine contient "co.PAYSDESTINATION" et non "co.PAYSPROVENANCE"
                        str = str.Replace("co.NUMERODOSSIERTPS [NumeroDossier], co.NUMEROTARIFDOUANE [CodeTarifDouane], co.DESIGNATIONCOMMERCIALE [Produit], co.PAYSPROVENANCE [Provenance], co.PAYSORIGINE [Origine], co.QUANTITEMESURE [PoidsNet], co.VALEURCFA [ValeurCfa],do.NOMOURAISONSOCIALEBENEFICIAIRE [Beneficiaire]", " count(distinct do.NUMERODOSSIERTPS), sum(co.VALEURCFA) ");

                        if ((natureOperation == "2") || (natureOperation == "4"))
                            str = str.Replace("co.NUMERODOSSIERTPS [NumeroDossier], co.NUMEROTARIFDOUANE [CodeTarifDouane], co.DESIGNATIONCOMMERCIALE [Produit], co.PAYSDESTINATION [Provenance], co.PAYSORIGINE [Origine], co.QUANTITEMESURE [PoidsNet], co.VALEURCFA [ValeurCfa],do.NOMOURAISONSOCIALEBENEFICIAIRE [Beneficiaire]", " count(distinct do.NUMERODOSSIERTPS), sum(co.VALEURCFA) ");

                        //str = str.Replace("inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS", "");
                        //str = str.Replace("inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE", "");

                        /////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////
                        
                    }
                    using (cmd2 = new SqlCommand(str, con))
                    {
                        SqlDataReader sqlReader = cmd2.ExecuteReader();
                        //var f = new NumberFormatInfo { NumberGroupSeparator = " " };
                        while (sqlReader.Read())
                        {
                            lblTotal.Text = sqlReader.GetValue(0).ToString();
                            lblTransactions.Text = sqlReader.GetValue(1).ToString();
                        }

                        if (lblTransactions.Text != null)
                        {
                            decimal totalValeurCFA = decimal.Parse(lblTransactions.Text);

                            if (totalValeurCFA > 0)
                            {
                                if (devise != "-1")
                                {
                                    decimal valeurEnDevise = ConvertirEnDevise(totalValeurCFA, devise);
                                    lblTransactions.Text = totalValeurCFA.ToString("n", f) + " " + devise + "  /  " + valeurEnDevise.ToString("n", f) + " CFA  ";
                                }
                                else
                                {
                                    lblTransactions.Text = totalValeurCFA.ToString("n", f) + " CFA  ";
                                }
                            }
                            else
                            {
                                lblTransactions.Text = " 0 ";
                            }
                        }                     

                        lblTotal.Font.Bold = true;
                        lblTotal.ForeColor = Color.OrangeRed;

                        lblTransactions.Font.Bold = true;
                        lblTransactions.ForeColor = Color.OrangeRed;
                    }

                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }
                finally
                {
                    cmd1.Dispose();
                    cmd2.Dispose();
                    con.Dispose();
                }

            }
        }

        public decimal ConvertirEnDevise(decimal valeur, string codeDevise)
        {
            SqlConnection con = new SqlConnection(strConnString);
            con.Open();
            String str = " select TAUXDEVISE from DEVISE where CODEDEVISE = '" + codeDevise + "'";
            SqlCommand cmd = new SqlCommand(str, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            decimal valeurDevise = 0;

            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                decimal taux = Convert.ToDecimal(dt.Rows[0]["TAUXDEVISE"].ToString());
                valeurDevise = valeur * taux;
            }

            return valeurDevise;
        }

        protected void DetailsListeGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.ChargerDonnees(e.SortExpression);
        }

        protected void DetailsListeGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = DetailsListeGridView.SelectedRow;
        }

        protected void DetailsListeGridView_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            DetailsListeGridView.PageIndex = e.NewPageIndex;
            ChargerDonnees();
        }


        [System.Web.Services.WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<PaysDetails> ChargerListePays(string codePays, string nomPays)
        {
            List<PaysDetails> detailsPays = new List<PaysDetails>();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            using (SqlConnection con = new SqlConnection(StrConn))
            {
                //string str = "";
                //PaysDetails benef = new PaysDetails();
                if (codePays != "" && nomPays != "")
                {
                    codePays = codePays.Replace("'", "''");
                    nomPays = nomPays.Replace("'", "''");
                    var paysData = from pays in tempDtPays.AsEnumerable()
                                   where pays.Field<string>("code_pays").ToUpperInvariant().StartsWith(codePays.ToUpperInvariant()) && pays.Field<string>("nom_pays").ToUpperInvariant().StartsWith(nomPays.ToUpperInvariant())
                                   select new
                                   {
                                       code_pays = pays.Field<string>("code_pays"),
                                       nom_pays = pays.Field<string>("nom_pays")
                                   };

                    foreach (var resultat in paysData)
                    {
                        PaysDetails benef = new PaysDetails();
                        benef.codePays = resultat.code_pays.ToString();
                        benef.nomPays = resultat.nom_pays.ToString();
                        detailsPays.Add(benef);
                    }


                    //str = "select * from Pays where code_pays like '%" + codePays + "%' and nom_pays like '%" + nomPays + "%'";
                }
                else if (codePays != "")
                {
                    codePays = codePays.Replace("'", "''");
                    var paysData = from pays in tempDtPays.AsEnumerable()
                                   where pays.Field<string>("code_pays").ToUpperInvariant().StartsWith(codePays.ToUpperInvariant())
                                   select new
                                   {
                                       code_pays = pays.Field<string>("code_pays"),
                                       nom_pays = pays.Field<string>("nom_pays")
                                   };

                    foreach (var resultat in paysData)
                    {
                        PaysDetails benef = new PaysDetails();
                        benef.codePays = resultat.code_pays.ToString();
                        benef.nomPays = resultat.nom_pays.ToString();
                        detailsPays.Add(benef);
                    }
                    //str = "select * from Pays where code_pays like '%" + codePays + "%' ";
                }//&& (m.nom_mat.IndexOf("a")<m.nom_mat.IndexOf("b"))
                else if (nomPays != "")
                {
                    nomPays = nomPays.Replace("'", "''");
                    var paysData = (from pays in tempDtPays.AsEnumerable()
                                    where pays.Field<string>("nom_pays").ToUpperInvariant().StartsWith(nomPays.ToUpperInvariant())
                                    select new
                                    {
                                        code_pays = pays.Field<string>("code_pays"),
                                        nom_pays = pays.Field<string>("nom_pays")
                                    }).ToList();

                    foreach (var resultat in paysData)
                    {
                        PaysDetails benef = new PaysDetails();
                        benef.codePays = resultat.code_pays.ToString();
                        benef.nomPays = resultat.nom_pays.ToString();
                        detailsPays.Add(benef);
                    }
                    //str = " select * from Pays where nom_pays like '%" + nomPays + "%'";
                }

                if (detailsPays.Count() <= 0)
                {
                    detailsPays.Clear();
                }

            }

            return detailsPays;
        }

        public class PaysDetails
        {
            public string codePays { get; set; }
            public string nomPays { get; set; }
        }


        protected void PaysGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = PaysGrid.SelectedRow;
        }

        protected void PaysGrid_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            PaysGrid.PageIndex = e.NewPageIndex;
            ChargerTempPays();
        }


        public void ChargerTempPays()
        {
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("codePays");
            dt1.Columns.Add("nomPays");
            dt1.Rows.Add();

            PaysGrid.DataSource = dt1;
            PaysGrid.DataBind();

            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(strConnString);
            con.Open();
            String str = " select * from Pays order by nom_pays ASC ";
            SqlCommand cmd = new SqlCommand(str, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            TempGridPays.DataSource = dt;
            TempGridPays.DataBind();

        }

        public void ChargerTempProduits()
        {

            DataTable dt1 = new DataTable();
            dt1.Columns.AddRange(new DataColumn[2] { new DataColumn("codeProduit"), new DataColumn("designation") });
            dt1.Rows.Add();
            ProduitGrid.DataSource = dt1;
            ProduitGrid.DataBind();

            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(strConnString);
            con.Open();
            String str = " select NUMEROTARIFDOUANE, LIBELLETARIFDOUANE from TARIFDOUANE order by LIBELLETARIFDOUANE ASC ";
            SqlCommand cmd = new SqlCommand(str, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            TempGridProduits.DataSource = dt;
            TempGridProduits.DataBind();

        }

        public void ChargerTempBeneficiaire()
        {

            DataTable dt1 = new DataTable();
            dt1.Columns.AddRange(new DataColumn[2] { new DataColumn("codePPM"), new DataColumn("beneficiaire") });
            dt1.Rows.Add();
            BeneficiaireGrid.DataSource = dt1;
            BeneficiaireGrid.DataBind();

            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(strConnString);
            con.Open();
            String str = " select codeppm, nomouraisonsocialebeneficiaire from clients where codeppm is not null order by nomouraisonsocialebeneficiaire ASC ";
            SqlCommand cmd = new SqlCommand(str, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            TempGridBeneficiaire.DataSource = dt;
            TempGridBeneficiaire.DataBind();

        }

        [System.Web.Services.WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<BeneficiaireDetails> ChargerListeBeneficiaire(string codePPM, string beneficiaire)
        {
            DataTable dt = new DataTable();
            List<BeneficiaireDetails> details = new List<BeneficiaireDetails>();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            using (SqlConnection con = new SqlConnection(StrConn))
            {
                //string str = "";

                if (codePPM != "" && beneficiaire != "")
                {
                    codePPM = codePPM.Replace("'", "''");
                    beneficiaire = beneficiaire.Replace("'", "''");

                    var benefData = from clients in tempDtBeneficiaire.AsEnumerable()
                                    where clients.Field<string>("codeppm").StartsWith(codePPM) && clients.Field<string>("nomouraisonsocialebeneficiaire").ToUpperInvariant().StartsWith(beneficiaire.ToUpperInvariant())
                                    select new
                                    {
                                        codeppm = clients.Field<string>("codeppm"),
                                        nomouraisonsocialebeneficiaire = clients.Field<string>("nomouraisonsocialebeneficiaire")
                                    };

                    foreach (var resultat in benefData)
                    {
                        BeneficiaireDetails benef = new BeneficiaireDetails();
                        benef.codePPM = resultat.codeppm.ToString();
                        benef.beneficiaire = resultat.nomouraisonsocialebeneficiaire.ToString();
                        details.Add(benef);
                    }

                    //str = "select codeppm, nomouraisonsocialebeneficiaire from clients where codeppm like '%" + codePPM + "%' and nomouraisonsocialebeneficiaire like '%" + beneficiaire + "%'";
                }
                else if (codePPM != "")
                {
                    codePPM = codePPM.Replace("'", "''");

                    var benefData = from clients in tempDtBeneficiaire.AsEnumerable()
                                    where clients.Field<string>("codeppm").StartsWith(codePPM)
                                    select new
                                    {
                                        codeppm = clients.Field<string>("codeppm"),
                                        nomouraisonsocialebeneficiaire = clients.Field<string>("nomouraisonsocialebeneficiaire")
                                    };

                    foreach (var resultat in benefData)
                    {
                        BeneficiaireDetails benef = new BeneficiaireDetails();
                        benef.codePPM = resultat.codeppm.ToString();
                        benef.beneficiaire = resultat.nomouraisonsocialebeneficiaire.ToString();
                        details.Add(benef);
                    }

                    //str = "select codeppm, nomouraisonsocialebeneficiaire from clients where codeppm like '%" + codePPM + "%' ";
                }
                else if (beneficiaire != "")
                {
                    beneficiaire = beneficiaire.Replace("'", "''");

                    var benefData = from clients in tempDtBeneficiaire.AsEnumerable()
                                    where clients.Field<string>("nomouraisonsocialebeneficiaire").ToUpperInvariant().StartsWith(beneficiaire.ToUpperInvariant())
                                    select new
                                    {
                                        codeppm = clients.Field<string>("codeppm"),
                                        nomouraisonsocialebeneficiaire = clients.Field<string>("nomouraisonsocialebeneficiaire")
                                    };

                    foreach (var resultat in benefData)
                    {
                        BeneficiaireDetails benef = new BeneficiaireDetails();
                        benef.codePPM = resultat.codeppm.ToString();
                        benef.beneficiaire = resultat.nomouraisonsocialebeneficiaire.ToString();
                        details.Add(benef);
                    }

                    //str = " select codeppm, nomouraisonsocialebeneficiaire from clients where nomouraisonsocialebeneficiaire like '%" + beneficiaire + "%'";
                }

                if (details.Count() <= 0)
                {
                    details.Clear();
                }

            }

            return details;
        }

        public class BeneficiaireDetails
        {
            public string codePPM { get; set; }
            public string beneficiaire { get; set; }
        }

        [System.Web.Services.WebMethod]
        public static List<ProduitDetails> ChargerListeProduit(string codeProduit, string designation)
        {
            DataTable dt = new DataTable();
            List<ProduitDetails> detailsProduits = new List<ProduitDetails>();
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            using (SqlConnection con = new SqlConnection(StrConn))
            {
                //string str = "";

                if (codeProduit != "" && designation != "")
                {
                    codeProduit = codeProduit.Replace("'", "''");
                    designation = designation.Replace("'", "''");

                    var produitData = from produit in tempDtProduits.AsEnumerable()
                                      where produit.Field<string>("NUMEROTARIFDOUANE").StartsWith(codeProduit) && produit.Field<string>("LIBELLETARIFDOUANE").ToUpperInvariant().Contains(designation.ToUpperInvariant())
                                      select new
                                      {
                                          codeProduit = produit.Field<string>("NUMEROTARIFDOUANE"),
                                          designation = produit.Field<string>("LIBELLETARIFDOUANE")
                                      };

                    foreach (var resultat in produitData)
                    {
                        ProduitDetails benef = new ProduitDetails();
                        benef.codeProduit = resultat.codeProduit.ToString();
                        benef.designation = resultat.designation.ToString();
                        detailsProduits.Add(benef);
                    }

                    //str = "select NUMEROTARIFDOUANE as codeProduit, LIBELLETARIFDOUANE as designation from TARIFDOUANE where NUMEROTARIFDOUANE like '%" + codeProduit + "%' and LIBELLETARIFDOUANE like '%" + designation + "%'";
                }
                else if (codeProduit != "")
                {
                    codeProduit = codeProduit.Replace("'", "''");

                    var produitData = from produit in tempDtProduits.AsEnumerable()
                                      where produit.Field<string>("NUMEROTARIFDOUANE").StartsWith(codeProduit)
                                      select new
                                      {
                                          codeProduit = produit.Field<string>("NUMEROTARIFDOUANE"),
                                          designation = produit.Field<string>("LIBELLETARIFDOUANE")
                                      };

                    foreach (var resultat in produitData)
                    {
                        ProduitDetails benef = new ProduitDetails();
                        benef.codeProduit = resultat.codeProduit.ToString();
                        benef.designation = resultat.designation.ToString();
                        detailsProduits.Add(benef);
                    }

                    //str = "select NUMEROTARIFDOUANE as codeProduit, LIBELLETARIFDOUANE as designation from TARIFDOUANE where NUMEROTARIFDOUANE like '%" + codeProduit + "%' ";
                }
                else if (designation != "")
                {
                    designation = designation.Replace("'", "''");

                    var produitData = from produit in tempDtProduits.AsEnumerable()
                                      where produit.Field<string>("LIBELLETARIFDOUANE").ToUpperInvariant().Contains(designation.ToUpperInvariant())
                                      select new
                                      {
                                          codeProduit = produit.Field<string>("NUMEROTARIFDOUANE"),
                                          designation = produit.Field<string>("LIBELLETARIFDOUANE")
                                      };

                    foreach (var resultat in produitData)
                    {
                        ProduitDetails benef = new ProduitDetails();
                        benef.codeProduit = resultat.codeProduit.ToString();
                        benef.designation = resultat.designation.ToString();
                        detailsProduits.Add(benef);
                    }

                    //str = " select NUMEROTARIFDOUANE as codeProduit, LIBELLETARIFDOUANE as designation from TARIFDOUANE where LIBELLETARIFDOUANE like '%" + designation + "%'";
                }

                if (detailsProduits.Count() <= 0)
                {
                    detailsProduits.Clear();
                }

            }

            return detailsProduits;
        }

        public class ProduitDetails
        {
            public string codeProduit { get; set; }
            public string designation { get; set; }
        }



        protected void BeneficiaireGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = BeneficiaireGrid.SelectedRow;
        }

        protected void BeneficiaireGrid_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            BeneficiaireGrid.PageIndex = e.NewPageIndex;
            ChargerTempBeneficiaire();
        }
        protected void ProduitGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = ProduitGrid.SelectedRow;
        }

        protected void ProduitGrid_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            ProduitGrid.PageIndex = e.NewPageIndex;
            ChargerTempProduits();
        }


        public void ChargerListeDevise()
        {
            SqlConnection con = new SqlConnection(strConnString);
            con.Open();
            String str = " select CODEDEVISE, NOMDEVISE from DEVISE where NOMDEVISE != '' order by NOMDEVISE ASC ";
            SqlCommand cmd = new SqlCommand(str, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            DeviseList.DataSource = dt;
            DeviseList.DataBind();
            DeviseList.Items.Insert(0, new ListItem(String.Empty, "-1"));
        }

        protected void VisualiserExcel_Click(object sender, EventArgs e)
        {
            Response.ClearContent();
            Response.AppendHeader("content-disposition", "attachment;filename=DetailsExtract.xls");
            Response.ContentType = "application/vnd.ms-excel";

            StringWriter stringwriter = new StringWriter();
            HtmlTextWriter htmtextwriter = new HtmlTextWriter(stringwriter);

            DetailsListeGridView.HeaderRow.Style.Add("background-color", "#ffffff");

            foreach (TableCell tableCell in DetailsListeGridView.HeaderRow.Cells)
            {
                tableCell.Style["background-color"] = "#ffffff";
            }

            foreach (GridViewRow gridviewrow in DetailsListeGridView.Rows)
            {
                gridviewrow.BackColor = System.Drawing.Color.White;
                foreach (TableCell gridviewrowtablecell in gridviewrow.Cells)
                {
                    gridviewrowtablecell.Style["background-color"] = "#ffffff";
                }
            }

            DetailsListeGridView.AllowPaging = false;
            this.ChargerDonnees();
            DetailsListeGridView.RenderControl(htmtextwriter);
            Response.Write(stringwriter.ToString());
            Response.End();


        }

        protected void Annuler_Click(object sender, EventArgs e)
        {
            Drop_Operation_Rech.Value = "-1";

            txtPaysProvenance.Text = "";

            txtPaysOrigine.Text = "";

            txtBeneficiaire.Text = "";

            Text_DebutPeriode_Rech.Text = "";

            Text_FinPeriode_Rech.Text = "";

            DeviseList.Value = "-1";

            txtProduit.Text = "";

            lblTotal.Text = "";
            lblTransactions.Text = "";

            DetailsListeGridView.DataSource = null;
            DetailsListeGridView.DataBind();

        }


        public override void VerifyRenderingInServerForm(System.Web.UI.Control control) { }

    }
}