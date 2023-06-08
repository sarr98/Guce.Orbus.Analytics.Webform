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
    public partial class StatsGlobales : System.Web.UI.Page
    {
        private String strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private String strNumInscription = "";
        private String strIdPole = "";
        private String str = "";
        public static string StrConn { get; set; }
        public static DataTable tempDtPays = new DataTable();
        public static DataTable tempDtProduits = new DataTable();
        public static DataTable tempDtBeneficiaire = new DataTable();


        //private string SortDirection
        //{
        //    get { return ViewState["SortDirection"] != null ? ViewState["SortDirection"].ToString() : "ASC"; }
        //    set { ViewState["SortDirection"] = value; }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {

            StrConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            //ChargerListeDevise();

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");

            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            resultatStatsGlobales.Visible = false;

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
                    ChargerListeDevise();
                    ChargerTempBeneficiaire();
                    tempDtBeneficiaire = (DataTable)TempGridBeneficiaire.DataSource;

                    if ((Drop_Operation_Rech.Value == "2") || (Drop_Operation_Rech.Value == "4"))
                    {
                        txtPaysProvenance.Attributes.Add("placeholder", "Pays Destination");
                    }
                    else
                    {
                        txtPaysProvenance.Attributes.Add("placeholder", "Pays Provenance");
                    }
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

            //String groupage = GroupageList.SelectedValue + "";

            String datePar = DateParList.Value + "";

            String paysProvenance = txtPaysProvenance.Text;
            paysProvenance = paysProvenance.Trim();

            String paysOrigine = txtPaysOrigine.Text;
            paysOrigine = paysOrigine.Trim();

            String beneficiaire = txtBeneficiaire.Text;
            beneficiaire = beneficiaire.Trim();


            String debut = Text_DebutPeriode_Rech.Text;
            debut = debut.Trim();

            String fin = Text_FinPeriode_Rech.Text;
            fin = fin.Trim();

            String devise = DeviseList.Value + "";
            devise = devise.Trim();

            String produit = txtProduit.Text;

            String strInterne = "";

            String reqParamPays = "";

            String strParamPays = "";

            String strPartages = "";

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

            if (datePar == "1")
            {
                if (devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "" && produit == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation],  SUM(fa.VALEURTOTALECFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                      " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                }

                if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "" && produit == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], fa.CODEDEVISE [DescDevise], SUM(fa.VALEURTOTALECFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                }

                if (paysProvenance != "" && devise == "-1" && paysOrigine == "" && beneficiaire == "" && produit == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE], SUM(fa.VALEURTOTALECFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                }

                if (paysOrigine != "" && devise == "-1" && paysProvenance == "" && beneficiaire == "" && produit == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                }

                if (beneficiaire != "" && devise == "-1" && paysOrigine == "" && paysProvenance == "" && produit == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(fa.VALEURTOTALECFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit != "" && devise == "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit == "" && devise != "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], fa.CODEDEVISE [DescDevise], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , SUM(fa.VALEURTOTALECFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                }

                if (produit == "" && devise != "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], fa.CODEDEVISE [DescDevise], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                }

                if (produit == "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], fa.CODEDEVISE [DescDevise], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(fa.VALEURTOTALECFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS"+
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (produit != "" && devise != "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit != "" && devise != "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (produit != "" && devise != "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit != "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit != "" && devise == "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }
                if (produit != "" && devise == "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }
                if (produit != "" && devise == "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit == "" && devise != "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], fa.PAYSPROVENANCE , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                }
                if (produit == "" && devise != "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , fa.CODEDEVISE [DescDevise], SUM(fa.VALEURTOTALECFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }
                if (produit == "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }
                if (produit == "" && devise == "-1" && paysOrigine != "" && paysProvenance != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                }
                if (produit == "" && devise == "-1" && beneficiaire != "" && paysProvenance != "" && paysOrigine == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(fa.VALEURTOTALECFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }
                if (produit == "" && devise == "-1" && beneficiaire != "" && paysOrigine != "" && paysProvenance == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit == "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance != "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit != "" && devise == "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance != "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres], DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + Right(DateName( Year, do.DATEDOSSIERTPS ) , 2) [Lettres] , DATENAME(YEAR,do.DATEDOSSIERTPS) [Annee] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

            }
            else
            {
                if (devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "" && produit == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation],  SUM(fa.VALEURTOTALECFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                      " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                }

                if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "" && produit == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], fa.CODEDEVISE [DescDevise], SUM(fa.VALEURTOTALECFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                }

                if (paysProvenance != "" && devise == "-1" && paysOrigine == "" && beneficiaire == "" && produit == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , SUM(fa.VALEURTOTALECFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                }

                if (paysOrigine != "" && devise == "-1" && paysProvenance == "" && beneficiaire == "" && produit == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                }

                if (beneficiaire != "" && devise == "-1" && paysOrigine == "" && paysProvenance == "" && produit == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(fa.VALEURTOTALECFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit != "" && devise == "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }
                if (produit == "" && devise != "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], fa.CODEDEVISE [DescDevise], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , SUM(fa.VALEURTOTALECFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                }

                if (produit == "" && devise != "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], fa.CODEDEVISE [DescDevise], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                }

                if (produit == "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], fa.CODEDEVISE [DescDevise], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(fa.VALEURTOTALECFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (produit != "" && devise != "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit != "" && devise != "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }

                if (produit != "" && devise != "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit != "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit != "" && devise == "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
                }
                if (produit != "" && devise == "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }
                if (produit != "" && devise == "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit == "" && devise != "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                }
                if (produit == "" && devise != "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , fa.CODEDEVISE [DescDevise], SUM(fa.VALEURTOTALECFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }
                if (produit == "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }
                if (produit == "" && devise == "-1" && paysOrigine != "" && paysProvenance != "" && beneficiaire == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], fa.PAYSPROVENANCE , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                }
                if (produit == "" && devise == "-1" && beneficiaire != "" && paysProvenance != "" && paysOrigine == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }
                if (produit == "" && devise == "-1" && beneficiaire != "" && paysOrigine != "" && paysProvenance == "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit == "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance != "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , UPPER(co.PAYSORIGINE) [PAYSORIGINE] , UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , fa.CODEDEVISE [DescDevise], SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";
                }

                if (produit != "" && devise == "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance != "")
                {
                    str = " SELECT do.IMPORTATIONOUEXPORTATION [Operation], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit], UPPER(co.PAYSORIGINE) [PAYSORIGINE] , UPPER(fa.PAYSPROVENANCE) [PAYSPROVENANCE] , ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE , SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR " ;
                }

            }

            strPartages = str; 

            if (numIncsriptionClient > 0)
            {
                if (!str.Contains("inner join OPERATEUR op"))
                {
                    strPartages = strPartages + " inner join OPERATEUR op on do.TRANSITAIRE = op.IDTPSOPERATEUR " +
                                  " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient +
                                  " and not exists(select * from OPERATEUR op2 where op2.IDTPSOPERATEUR=do.IDTPSOPERATEUR and op2.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient + ")";

                    str = str + " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR ";

                    
                }
            }
            else if (idPole > 0)
            {
                //if (!str.Contains("select * from JOINDRE"))
                //{
                //    strInterne = strInterne + " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS =do.NUMERODOSSIERTPS) ";
                //}

            }

            str = str + " where 1=1 ";


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

                    strInterne = strInterne + " and co.NUMEROTARIFDOUANE in (" + strProduit + ")";

                    strPartages = strPartages + " and co.NUMEROTARIFDOUANE in (" + strProduit + ")";

                }
                else
                {
                    str = str + " and co.NUMEROTARIFDOUANE = '" + produit + "'";
                    strInterne = strInterne + " and co.NUMEROTARIFDOUANE = '" + produit + "'";
                    strPartages = strPartages + " and co.NUMEROTARIFDOUANE = '" + produit + "'";

                }

            }

            if (natureOperation != "-1")
            {
                if (natureOperation == "1")
                {
                    str = str + " and do.IMPORTATIONOUEXPORTATION = 'I' ";
                    strInterne = strInterne + " and do.IMPORTATIONOUEXPORTATION = 'I' ";
                    strPartages = strPartages + " and do.IMPORTATIONOUEXPORTATION = 'I' ";
                }
                else if (natureOperation == "2")
                {
                    str = str + " and do.IMPORTATIONOUEXPORTATION = 'E' ";
                    strInterne = strInterne + " and do.IMPORTATIONOUEXPORTATION = 'E' ";
                    strPartages = strPartages + " and do.IMPORTATIONOUEXPORTATION = 'E' ";
                }
                else if (natureOperation == "3")
                {
                    str = str + " and do.IMPORTATIONOUEXPORTATION = 'S' ";
                    strInterne = strInterne + " and do.IMPORTATIONOUEXPORTATION = 'S' ";
                    strPartages = strPartages + " and do.IMPORTATIONOUEXPORTATION = 'S' ";
                }
                else if (natureOperation == "4")
                {
                    str = str + " and do.IMPORTATIONOUEXPORTATION = 'R' ";
                    strInterne = strInterne + " and do.IMPORTATIONOUEXPORTATION = 'R' ";
                    strPartages = strPartages + " and do.IMPORTATIONOUEXPORTATION = 'R' ";
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

                    str = str + " and fa.PAYSPROVENANCE in (" + strPays + ")";
                    strInterne = strInterne + " and fa.PAYSPROVENANCE in (" + strPays + ")";
                    strPartages = strPartages + " and fa.PAYSPROVENANCE in (" + strPays + ")";

                }
                else
                {
                    str = str + " and fa.PAYSPROVENANCE = '" + paysProvenance + "'";
                    strInterne = strInterne + " and fa.PAYSPROVENANCE = '" + paysProvenance + "'";
                    strPartages = strPartages + " and fa.PAYSPROVENANCE = '" + paysProvenance + "'";
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
                    strInterne = strInterne + " and co.PAYSORIGINE in (" + strPays + ")";
                    strPartages = strPartages + " and co.PAYSORIGINE in (" + strPays + ")";

                }
                else
                {
                    str = str + " and co.PAYSORIGINE = '" + paysOrigine + "'";
                    strInterne = strInterne + " and co.PAYSORIGINE = '" + paysOrigine + "'";
                    strPartages = strPartages + " and co.PAYSORIGINE = '" + paysOrigine + "'";
                }

            }

            if (beneficiaire != "")
            {
                if (beneficiaire.Contains(';'))
                {

                    string[] val = beneficiaire.Split(';');
                    string strBeneficiaire = "'" + val[0] + "'";
                    int i = 1;

                    for (i = 1; i < val.Length; i++)
                    {
                        strBeneficiaire = strBeneficiaire + ",'" + val[i] + "'";
                    }

                    if (numIncsriptionClient > 0)
                    {
                        str = str + " and do.CODEPPM in (" + strBeneficiaire + ") and op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                        strInterne = strInterne + " and do.CODEPPM in (" + strBeneficiaire + ") ";
                        strPartages = strPartages + " and do.CODEPPM in (" + strBeneficiaire + ") and op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                    }
                    else
                    {
                        str = str + " and do.CODEPPM in (" + strBeneficiaire + ") and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS) ";
                        strInterne = strInterne + " and do.CODEPPM in (" + strBeneficiaire + ") ";
                        strPartages = strPartages + " and do.CODEPPM in (" + strBeneficiaire + ") and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS) ";
                    }
                    

                }
                else
                {
                    if (numIncsriptionClient > 0)
                    {
                        str = str + " and do.CODEPPM = '" + beneficiaire + "' and op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                        strInterne = strInterne + " and do.CODEPPM = '" + beneficiaire + "' ";
                        strPartages = strPartages + " do.CODEPPM = '" + beneficiaire + "' and op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                    }
                    else
                    {
                        str = str + " and do.CODEPPM = '" + beneficiaire + "' and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS) ";
                        strInterne = strInterne + " and do.CODEPPM = '" + beneficiaire + "' ";
                        strPartages = strPartages + " do.CODEPPM = '" + beneficiaire + "' and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS) ";
                    }

                }

            }

            if (devise != "-1")
            {

                str = str + " and fa.CODEDEVISE like '" + devise + "%'";
                strInterne = strInterne + " and fa.CODEDEVISE like '" + devise + "%'";
                strPartages = strPartages + " and fa.CODEDEVISE like '" + devise + "%'";

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
                strInterne = strInterne + " and do.DATEDOSSIERTPS  >= '" + debut + "' ";
                strPartages = strPartages + " and do.DATEDOSSIERTPS  >= '" + debut + "' ";
            }

            if (fin != "")
            {
                // Isdate de datedebut    
                DateTime d;
                if (DateTime.TryParseExact(fin, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d))
                {
                    fin = d.ToString("dd/MM/yyyy");
                }
                DateTime dateFin = Convert.ToDateTime(fin).AddDays(1);
                str = str + " and do.DATEDOSSIERTPS  <= '" + dateFin.ToString() + "' ";
                strInterne = strInterne + " and do.DATEDOSSIERTPS  <= '" + dateFin.ToString() + "' ";
                strPartages = strPartages + " and do.DATEDOSSIERTPS  <= '" + dateFin.ToString() + "' ";
            }

            Boolean verifChamps = true;

            if (debut != "" && fin != "")
            {
                DateTime dateDebut = Convert.ToDateTime(debut);
                DateTime dateFin = Convert.ToDateTime(fin);

                if (dateDebut >= dateFin.AddDays(1))
                {
                    msgPopup.Text = "La date de fin doit être supérieure à la date de début !!!";
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(@"<script language='javascript'>");
                    sb.Append(@"$('#mymodal-dialog').modal('show');");
                    sb.Append(@"</script>");

                    ClientScript.RegisterStartupScript(this.GetType(), "JSScript", sb.ToString());

                    verifChamps = false;
                }
                else if (datePar == "1")
                {
                    if (dateDebut.AddMonths(12) < dateFin)
                    {
                        msgPopup.Text = "Un intervalle d'au plus une année est autorisé pour les dates !!!";
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

            }

            string strGroup = "";

            if (verifChamps)
            {
                //if (idPole > 0)
                //{
                //    str = str + " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS) ";
                //}

                if (datePar == "1")
                {
                    if (devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "" && produit == "")
                    {
                        strGroup = " GROUP BY do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS),DATEPART(MONTH,datedossiertps),DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps) ";
                    }
                    else
                    {
                        if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "" && produit == "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS),DATEPART(MONTH,datedossiertps),DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION ,fa.CODEDEVISE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (paysProvenance != "" && devise == "-1" && paysOrigine == "" && beneficiaire == "" && produit == "")
                        {
                            strGroup = " GROUP BY fa.PAYSPROVENANCE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS),DATEPART(MONTH,datedossiertps),DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.PAYSPROVENANCE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (paysOrigine != "" && devise == "-1" && paysProvenance == "" && beneficiaire == "" && produit == "")
                        {
                            strGroup = " GROUP BY co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS),DATEPART(MONTH,datedossiertps),DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.PAYSORIGINE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (beneficiaire != "" && devise == "-1" && paysOrigine == "" && paysProvenance == "" && produit == "")
                        {
                            strGroup = " GROUP BY do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS),DATEPART(MONTH,datedossiertps),DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit != "" && devise == "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , co.PAYSORIGINE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit == "" && devise != "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  fa.PAYSPROVENANCE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  fa.PAYSPROVENANCE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit == "" && devise != "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  co.PAYSORIGINE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit == "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit != "" && devise != "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  co.PAYSORIGINE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }


                        if (produit != "" && devise != "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, co.PAYSORIGINE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit != "" && devise != "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit != "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit != "" && devise == "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE, co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE ,  fa.PAYSPROVENANCE, co.PAYSORIGINE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }
                        if (produit != "" && devise == "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit != "" && devise == "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit == "" && devise != "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  fa.PAYSPROVENANCE, co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, co.PAYSORIGINE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit == "" && devise != "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit == "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE, do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit == "" && devise == "-1" && paysOrigine != "" && paysProvenance != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY fa.PAYSPROVENANCE, co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.PAYSPROVENANCE, co.PAYSORIGINE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit == "" && devise == "-1" && beneficiaire != "" && paysProvenance != "" && paysOrigine == "")
                        {
                            strGroup = " GROUP BY fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit == "" && devise == "-1" && beneficiaire != "" && paysOrigine != "" && paysProvenance == "")
                        {
                            strGroup = " GROUP BY co.PAYSORIGINE , do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , co.PAYSORIGINE , do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        /// ajout 24022021

                        if (produit == "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance != "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  fa.PAYSPROVENANCE , co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  fa.PAYSPROVENANCE , co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }

                        if (produit != "" && devise == "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance != "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE , co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR,do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE , co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , DATENAME(YEAR,do.DATEDOSSIERTPS) , DATEPART(MONTH,datedossiertps)  ";
                        }


                        /// Fin ajout 24022021

                    }

                    //strInterne = strInterne + " GROUP BY DATENAME(MONTH, do.DATEDOSSIERTPS), DATEPART(MONTH,datedossiertps), DATENAME(YEAR, do.DATEDOSSIERTPS) ,do.IMPORTATIONOUEXPORTATION " +
                    //                    " ORDER BY DATEPART(MONTH,datedossiertps) , do.IMPORTATIONOUEXPORTATION ";
                }
                else
                {
                    if (devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "" && produit == "")
                    {
                        strGroup = " GROUP BY do.IMPORTATIONOUEXPORTATION , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                    }
                    else
                    {
                        if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , do.IMPORTATIONOUEXPORTATION , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "" && produit == "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION ,fa.CODEDEVISE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (paysProvenance != "" && devise == "-1" && paysOrigine == "" && beneficiaire == "" && produit == "")
                        {
                            strGroup = " GROUP BY fa.PAYSPROVENANCE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.PAYSPROVENANCE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (paysOrigine != "" && devise == "-1" && paysProvenance == "" && beneficiaire == "" && produit == "")
                        {
                            strGroup = " GROUP BY co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.PAYSORIGINE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (beneficiaire != "" && devise == "-1" && paysOrigine == "" && paysProvenance == "" && produit == "")
                        {
                            strGroup = " GROUP BY do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , do.NOMOURAISONSOCIALEBENEFICIAIRE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit != "" && devise == "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE , do.IMPORTATIONOUEXPORTATION , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS)  ";
                        }

                        if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , co.PAYSORIGINE , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS)  ";
                        }

                        if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , do.NOMOURAISONSOCIALEBENEFICIAIRE , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS)  ";
                        }

                        if (produit == "" && devise != "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  fa.PAYSPROVENANCE , do.IMPORTATIONOUEXPORTATION , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  fa.PAYSPROVENANCE , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS)  ";
                        }

                        if (produit == "" && devise != "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  co.PAYSORIGINE , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS)  ";
                        }

                        if (produit == "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  do.NOMOURAISONSOCIALEBENEFICIAIRE , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS)  ";
                        }

                        if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,   DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit != "" && devise != "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, do.IMPORTATIONOUEXPORTATION , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS)  ";
                        }

                        if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  co.PAYSORIGINE , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  do.NOMOURAISONSOCIALEBENEFICIAIRE , DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit != "" && devise != "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, co.PAYSORIGINE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit != "" && devise != "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit != "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.CODEDEVISE ,  co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit != "" && devise == "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE, co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                    " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE ,  fa.PAYSPROVENANCE, co.PAYSORIGINE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }
                        if (produit != "" && devise == "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit != "" && devise == "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit == "" && devise != "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  fa.PAYSPROVENANCE, co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, co.PAYSORIGINE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit == "" && devise != "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit == "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit == "" && devise == "-1" && paysOrigine != "" && paysProvenance != "" && beneficiaire == "")
                        {
                            strGroup = " GROUP BY fa.PAYSPROVENANCE, co.PAYSORIGINE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.PAYSPROVENANCE, co.PAYSORIGINE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit == "" && devise == "-1" && beneficiaire != "" && paysProvenance != "" && paysOrigine == "")
                        {
                            strGroup = " GROUP BY fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.PAYSPROVENANCE, do.NOMOURAISONSOCIALEBENEFICIAIRE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit == "" && devise == "-1" && beneficiaire != "" && paysOrigine != "" && paysProvenance == "")
                        {
                            strGroup = " GROUP BY co.PAYSORIGINE , do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , co.PAYSORIGINE , do.NOMOURAISONSOCIALEBENEFICIAIRE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        /// ajout 24022021

                        if (produit == "" && devise != "-1" && beneficiaire != "" && paysOrigine != "" && paysProvenance != "")
                        {
                            strGroup = " GROUP BY fa.CODEDEVISE ,  fa.PAYSPROVENANCE, co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , fa.CODEDEVISE ,  fa.PAYSPROVENANCE , co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        if (produit != "" && devise == "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance != "")
                        {
                            strGroup = " GROUP BY co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE , co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE , do.IMPORTATIONOUEXPORTATION ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                                     " ORDER BY do.IMPORTATIONOUEXPORTATION , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , fa.PAYSPROVENANCE , co.PAYSORIGINE, do.NOMOURAISONSOCIALEBENEFICIAIRE ,  DATEPART(YEAR,datedossiertps) , DATENAME(YEAR, do.DATEDOSSIERTPS) ";
                        }

                        /// Fin ajout 24022021

                    }

                    //strInterne = strInterne + " GROUP BY DATENAME(YEAR, do.DATEDOSSIERTPS),DATEPART(YEAR,datedossiertps), DATENAME(YEAR, do.DATEDOSSIERTPS) , do.IMPORTATIONOUEXPORTATION " +
                    //                    " ORDER BY DATEPART(YEAR,datedossiertps) , do.IMPORTATIONOUEXPORTATION ";

                }

                //strInterne = str + strInterne;

                str = str + strGroup;
                strPartages = strPartages + strGroup;

                //int position = str.IndexOf("where 1=1");

                //if (numIncsriptionClient > 0)
                //{
                //    strPartages = str.Substring(0, position) +
                //                  " where not exists(select * from OPERATEUR op2 where op2.IDTPSOPERATEUR=do.IDTPSOPERATEUR and op2.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient + ")" +
                //                  str.Substring(position + 9);

                //}

                if ((natureOperation == "2") || (natureOperation == "4"))
                {
                    str = str.Replace("fa.PAYSPROVENANCE", "fa.PAYSDESTINATION");
                    str = str.Replace("[PAYSPROVENANCE]", "[PAYSDESTINATION]");
                    strPartages = strPartages.Replace("fa.PAYSPROVENANCE", "fa.PAYSDESTINATION");
                    strPartages = strPartages.Replace("[PAYSPROVENANCE]", "[PAYSDESTINATION]");
                    reqParamPays = "PAYSDESTINATION";
                    strParamPays = "Destination";
                }
                else
                {
                    reqParamPays = "PAYSPROVENANCE";
                    strParamPays = "Provenance";
                }

                if (devise != "-1")
                {
                    str = str.Replace("fa.VALEURTOTALECFA", "fa.VALEURTOTALDEVISE");
                    str = str.Replace("co.VALEURCFA", "co.VALEURDEVISE");

                    strPartages = strPartages.Replace("fa.VALEURTOTALECFA", "fa.VALEURTOTALDEVISE");
                    strPartages = strPartages.Replace("co.VALEURCFA", "co.VALEURDEVISE");

                    strInterne = strInterne.Replace("fa.VALEURTOTALECFA", "fa.VALEURTOTALDEVISE");
                    strInterne = strInterne.Replace("co.VALEURCFA", "co.VALEURDEVISE");
                }

                
                decimal totalValeurCFA = 0;
                DataTable dtDepart = new DataTable();
                DataTable dtFinal = new DataTable();
                SqlCommand cmd1 = new SqlCommand();
                var maxLignes = 0;
                //storing total rows count to loop on each Record  
                string[] x = new string[dtDepart.Rows.Count];
                decimal[] y = new decimal[dtDepart.Rows.Count];
                decimal[] ord = new decimal[dtDepart.Rows.Count];
                var f = new NumberFormatInfo { NumberGroupSeparator = " " };
                SqlCommand cmdPoleOperateur = new SqlCommand();
                SqlCommand cmdOrbus = new SqlCommand();

                try
                {
                    ////////////////AJOUT DJOSSOU: Pour recadrer la requete///////////////////////////////
                    //////////////////////////////////////////////////////////////////////////////////////


                    if (produit != "")
                    {
                        str = str.Replace("SUM(fa.VALEURTOTALECFA)", "SUM(co.VALEURCFA)");
                        strPartages = strPartages.Replace("SUM(fa.VALEURTOTALECFA)", "SUM(co.VALEURCFA)");
                    }

                    if (numIncsriptionClient > 0)
                    {
                        str = str + ";" + strPartages;
                    }
                    
                    
                    if (devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "" && produit == "")
                    {

                        using (cmd1 = new SqlCommand(str, con))
                        {
                            SqlDataAdapter da = new SqlDataAdapter(cmd1);
                            da.Fill(dtDepart);

                            if (dtDepart.Rows.Count > 0)
                            {
                                resultatStatsGlobales.Visible = true;

                                var query = (from row in dtDepart.AsEnumerable()
                                             group row by new
                                             {
                                                 JoursLettres = row.Field<string>("Lettres")
                                             } into grp
                                             select new
                                             {
                                                 Periode = grp.Key.JoursLettres.ToString(),
                                                 NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                 Operation = grp.Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                 Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                 //SingleOperation = grp.Select(r => r.Field<string>("Operation")).FirstOrDefault()
                                                 //OpMontant = grp.Select(r => r.Field<string>("Operation") == "").Where(r => r.Field<double>("Total"))

                                             }).ToList();

                                maxLignes = query.Count();

                                dtFinal.Columns.Add("Opération", typeof(string));

                                int b = 1;

                                foreach (var resultat in query)
                                {
                                    if (datePar == "1")
                                    {
                                        dtFinal.Columns.Add(resultat.Periode.First().ToString().ToUpper() + resultat.Periode.Substring(1), typeof(string));
                                    }
                                    else
                                    {
                                        dtFinal.Columns.Add(resultat.Periode.ToString(), typeof(string));
                                    }

                                    while (dtFinal.Rows.Count < resultat.NbreNatureOperation)
                                    {
                                        dtFinal.Rows.Add();
                                    }

                                    int a = 0;

                                    foreach (var nature in resultat.Operation)
                                    {
                                        if (dtFinal.Rows[a][0].ToString() == "")
                                        {
                                            dtFinal.Rows[a][0] = nature;

                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                           where product.Field<string>("Operation") == nature && product.Field<string>("Lettres") == resultat.Periode.ToString()
                                                           select product).First();

                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                            {
                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                dtFinal.Rows[a][1] = valeur;
                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                            }
                                        }
                                        else if (dtFinal.Rows[a][0].ToString() == nature)
                                        {
                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                           where product.Field<string>("Operation") == nature && product.Field<string>("Lettres") == resultat.Periode.ToString()
                                                           select product).First();

                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                            {
                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                dtFinal.Rows[a][b] = valeur;
                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                            }
                                        }
                                        else
                                        {
                                            if (a < dtFinal.Rows.Count)
                                            {
                                                if (dtFinal.Rows[a + 1][0].ToString() == nature)
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Operation") == nature && product.Field<string>("Lettres") == resultat.Periode.ToString()
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[a + 1][b] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                            }

                                        }

                                        ++a;
                                    }

                                    ++b;
                                }

                                dtFinal.Columns.Add("Cumul", typeof(string));

                                for (int i = 0; i < dtFinal.Rows.Count; i++)
                                {
                                    decimal totalParOperation = 0;

                                    string typeOperation = dtFinal.Rows[i][0].ToString();

                                    switch (typeOperation)
                                    {
                                        case "E":
                                            dtFinal.Rows[i][0] = "Export ";
                                            break;
                                        case "I":
                                            dtFinal.Rows[i][0] = "Import  ";
                                            break;
                                        case "S":
                                            dtFinal.Rows[i][0] = "Transit ";
                                            break;
                                        case "R":
                                            dtFinal.Rows[i][0] = "RéExport ";
                                            break;
                                    }

                                    for (int j = 1; j < dtFinal.Columns.Count - 1; j++)
                                    {
                                        if (dtFinal.Rows[i][j].ToString() != "")
                                        {
                                            decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                            totalParOperation = totalParOperation + val;

                                            dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                        }

                                    }

                                    dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                }

                                ListeGrid.DataSource = dtFinal;
                                ListeGrid.DataBind();


                                for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                {
                                    ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                    ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                }

                                lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                            }
                            else
                            {
                                ListeGrid.DataSource = dtDepart;
                                ListeGrid.DataBind();
                            }
                        }
                    }
                    else
                    {
                        if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "")
                        {



                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 2;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }


                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString();

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 2] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString())
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 2] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString())
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 2] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 2; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "" && produit == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("DescDevise")
                                                 } into grp
                                                 select new
                                                 {
                                                     Devise = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 2;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }


                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString();

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 2] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString())
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 2] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString())
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 2] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 2; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (paysProvenance != "" && devise == "-1" && paysOrigine == "" && beneficiaire == "" && produit == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     Pays = row.Field<string>("PaysProvenance")
                                                 } into grp
                                                 select new
                                                 {
                                                     DescPays = grp.Key.Pays.ToString(),
                                                     Count = grp.Count(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 2;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>(reqParamPays) == resultat.DescPays.ToString()
                                                                        select product.Field<string>("Operation")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>(reqParamPays) == resultat.DescPays.ToString()
                                                                        select product.Field<string>("Operation")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }


                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>(reqParamPays) == resultat.DescPays.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString();

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>(reqParamPays) == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 2] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString())
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>(reqParamPays) == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 2] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString())
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>(reqParamPays) == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 2] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 2; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (paysOrigine != "" && devise == "-1" && paysProvenance == "" && beneficiaire == "" && produit == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     Pays = row.Field<string>("PaysOrigine")
                                                 } into grp
                                                 select new
                                                 {
                                                     DescPays = grp.Key.Pays.ToString(),
                                                     Count = grp.Count(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 2;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("PaysOrigine") == resultat.DescPays.ToString()
                                                                        select product.Field<string>("Operation")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("PaysOrigine") == resultat.DescPays.ToString()
                                                                        select product.Field<string>("Operation")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }


                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("PaysOrigine") == resultat.DescPays.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString();

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("PaysOrigine") == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 2] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString())
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("PaysOrigine") == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 2] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {
                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString())
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("PaysOrigine") == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 2] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 2; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (beneficiaire != "" && devise == "-1" && paysOrigine == "" && paysProvenance == "" && produit == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     Pays = row.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")
                                                 } into grp
                                                 select new
                                                 {
                                                     DescPays = grp.Key.Pays.ToString(),
                                                     Count = grp.Count(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 2;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == resultat.DescPays.ToString()
                                                                        select product.Field<string>("Operation")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == resultat.DescPays.ToString()
                                                                        select product.Field<string>("Operation")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }


                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == resultat.DescPays.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString();

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 2] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString())
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 2] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {
                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString())
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 2] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 2; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 3;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 3] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 3; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise == "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 3;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>(reqParamPays)).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>(reqParamPays)).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>(reqParamPays)).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 3] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 3; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }
                        if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 3;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("PaysOrigine")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("PaysOrigine")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("PaysOrigine")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("PaysOrigine") == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("PaysOrigine") == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("PaysOrigine") == strListe[2]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 3] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 3; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise == "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 3;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[2]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 3] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 3; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit == "" && devise != "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeDevise = row.Field<string>("DescDevise")
                                                 } into grp
                                                 select new
                                                 {
                                                     Devise = grp.Key.CodeDevise.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 4;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays)).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays)).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays)).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 3] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 3; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit == "" && devise != "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeDevise = row.Field<string>("DescDevise")
                                                 } into grp
                                                 select new
                                                 {
                                                     Devise = grp.Key.CodeDevise.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 4;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("PaysOrigine")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("PaysOrigine")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("PaysOrigine")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>("PaysOrigine") == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>("PaysOrigine") == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>("PaysOrigine") == strListe[2]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 3] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 3; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit == "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeDevise = row.Field<string>("DescDevise")
                                                 } into grp
                                                 select new
                                                 {
                                                     Devise = grp.Key.CodeDevise.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 4;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[2]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[2]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 3] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 3; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise != "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 4;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays)).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays)).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays)).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>(reqParamPays) == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>(reqParamPays) == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>(reqParamPays) == strListe[3]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 4] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 4; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 4;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("PaysOrigine")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("PaysOrigine")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("PaysOrigine")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 4] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 4; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine == "" && beneficiaire != "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 4;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 4] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 4; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise != "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 5;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>(reqParamPays) == strListe[3] && product.Field<string>("PaysOrigine") == strListe[4]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][4] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 5] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>(reqParamPays) == strListe[3] && product.Field<string>("PaysOrigine") == strListe[4]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][4] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 5] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>(reqParamPays) == strListe[3] && product.Field<string>("PaysOrigine") == strListe[4]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][4] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 5] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 5; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise != "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire != "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 5;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>(reqParamPays) == strListe[3] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[4]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][4] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 5] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>(reqParamPays) == strListe[3] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[4]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][4] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 5] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>(reqParamPays) == strListe[3] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[4]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][4] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 5] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 5; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise != "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire != "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 5;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[4]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][4] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 5] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[4]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][4] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 5] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("DescDevise") == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[4]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][4] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 5] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 5; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise == "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 4;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 4] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 4; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise == "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 4;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 4] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 4; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise == "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 4;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("PaysOrigine") == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("PaysOrigine") == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>("PaysOrigine") == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 4] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 4; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit == "" && devise != "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeDevise = row.Field<string>("DescDevise")
                                                 } into grp
                                                 select new
                                                 {
                                                     Devise = grp.Key.CodeDevise.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 4;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 4] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 4; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit == "" && devise != "-1" && paysProvenance != "" && beneficiaire != "" && paysOrigine == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeDevise = row.Field<string>("DescDevise")
                                                 } into grp
                                                 select new
                                                 {
                                                     Devise = grp.Key.CodeDevise.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 4;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 4] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 4; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit == "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeDevise = row.Field<string>("DescDevise")
                                                 } into grp
                                                 select new
                                                 {
                                                     Devise = grp.Key.CodeDevise.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 4;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>("PaysOrigine") == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>("PaysOrigine") == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 4] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {
                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>("PaysOrigine") == strListe[2] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[3]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 4] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 4; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit == "" && devise == "-1" && paysProvenance != "" && paysOrigine != "" && beneficiaire == "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     Pays = row.Field<string>("PaysProvenance")
                                                 } into grp
                                                 select new
                                                 {
                                                     DescPays = grp.Key.Pays.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 3;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>(reqParamPays) == resultat.DescPays.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("PaysOrigine")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>(reqParamPays) == resultat.DescPays.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("PaysOrigine")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>(reqParamPays) == resultat.DescPays.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("PaysOrigine")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>(reqParamPays) == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("PaysOrigine") == strListe[1]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>(reqParamPays) == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("PaysOrigine") == strListe[1]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {
                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>(reqParamPays) == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("PaysOrigine") == strListe[1]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 3] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 3; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit == "" && devise == "-1" && paysProvenance != "" && paysOrigine == "" && beneficiaire != "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     Pays = row.Field<string>("PaysProvenance")
                                                 } into grp
                                                 select new
                                                 {
                                                     DescPays = grp.Key.Pays.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 3;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>(reqParamPays) == resultat.DescPays.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>(reqParamPays) == resultat.DescPays.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>(reqParamPays) == resultat.DescPays.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>(reqParamPays) == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[1]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>(reqParamPays) == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[1]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {
                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>(reqParamPays) == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[1]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 3] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 3; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit == "" && devise == "-1" && paysProvenance == "" && paysOrigine != "" && beneficiaire != "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     Pays = row.Field<string>("PaysOrigine")
                                                 } into grp
                                                 select new
                                                 {
                                                     DescPays = grp.Key.Pays.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 3;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("PaysOrigine") == resultat.DescPays.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("PaysOrigine") == resultat.DescPays.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("PaysOrigine") == resultat.DescPays.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("PaysOrigine") == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[1]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("PaysOrigine") == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[1]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 3] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {
                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("PaysOrigine") == resultat.DescPays.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[1]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 3] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 3; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        //// ajout 24022021

                        if (produit == "" && devise != "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance != "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeDevise = row.Field<string>("DescDevise")
                                                 } into grp
                                                 select new
                                                 {
                                                     Devise = grp.Key.CodeDevise.ToString(),
                                                     Count = grp.Count(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Devise", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 5;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("DescDevise") == resultat.Devise.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("DescDevise") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[4]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][4] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 5] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[4]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescDevise"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][4] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 5] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {
                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("DescDevise") == resultat.Devise.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("DescDevise") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[4]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescDevise"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][4] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 5] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 5; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        if (produit != "" && devise == "-1" && paysOrigine != "" && beneficiaire != "" && paysProvenance != "")
                        {
                            using (cmd1 = new SqlCommand(str, con))
                            {
                                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                                da.Fill(dtDepart);

                                if (dtDepart.Rows.Count > 0)
                                {
                                    resultatStatsGlobales.Visible = true;

                                    var query = (from row in dtDepart.AsEnumerable()
                                                 group row by new
                                                 {
                                                     CodeTarif = row.Field<string>("Produit")
                                                 } into grp
                                                 select new
                                                 {
                                                     Produit = grp.Key.CodeTarif.ToString(),
                                                     Count = grp.Count(),
                                                     ////PeriodeEnCours = grp.Select(r => r.Field<string>("Lettres")).First(),
                                                     ////NbreNatureOperation = grp.Select(r => r.Field<string>("Operation")).ToList().Count(),
                                                     ////OperationProduit = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First() && r.Field<string>("Produit") == grp.Key.CodeTarif.ToString()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     //////Operation = grp.Where(r => r.Field<string>("Lettres") == grp.Select(a => a.Field<string>("Lettres")).First()).Select(r => r.Field<string>("Operation")).OrderBy(i => i).ToList(),
                                                     ////Montant = grp.Select(r => r.Field<double>("Total")).ToList(),
                                                     Periode = grp.Select(i => new { chiffre = i.Field<int>("Chiffres"), libelle = i.Field<string>("Lettres") }).Distinct().OrderBy(i => i.chiffre).ToList()

                                                 }).ToList();

                                    maxLignes = query.Count();

                                    var uniquePeriode = (from data in dtDepart.AsEnumerable()
                                                         select new
                                                         {
                                                             chiffre = data.Field<int>("Chiffres"),
                                                             Libelle = data.Field<string>("Lettres")
                                                         }).Distinct().OrderBy(i => i.chiffre).ToList();

                                    string[] indicateurPeriode = new string[uniquePeriode.Count()];

                                    dtFinal.Columns.Add("Opération", typeof(string));

                                    dtFinal.Columns.Add("Produit", typeof(string));

                                    dtFinal.Columns.Add(strParamPays, typeof(string));

                                    dtFinal.Columns.Add("Origine", typeof(string));

                                    dtFinal.Columns.Add("Bénéficiaire", typeof(string));

                                    int valIndicatteur = 0;

                                    foreach (var per in uniquePeriode)
                                    {
                                        if (datePar == "1")
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString().Substring(0, 1).ToUpper() + per.Libelle.ToString().Substring(1), typeof(string));
                                        }
                                        else
                                        {
                                            dtFinal.Columns.Add(per.Libelle.ToString(), typeof(string));
                                        }

                                        indicateurPeriode[valIndicatteur] = per.Libelle;

                                        ++valIndicatteur;
                                    }

                                    int b = 5;

                                    int numeroProduit = 0;

                                    foreach (var resultat in query)
                                    {
                                        int totalOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().Count();

                                        var listeOperationParProduit = (from product in dtDepart.AsEnumerable()
                                                                        where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                                        select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).Distinct().ToList();

                                        string[] indicateurOperation = new string[totalOperationParProduit];

                                        int q = 0;

                                        foreach (var unique in listeOperationParProduit)
                                        {
                                            indicateurOperation[q] = unique.ToString();
                                            ++q;
                                        }

                                        for (int z = 0; z < totalOperationParProduit; z++)
                                        {
                                            dtFinal.Rows.Add();
                                        }

                                        foreach (var courantMoisAnnee in resultat.Periode)
                                        {
                                            var dataCourantPeriode = (from product in dtDepart.AsEnumerable()
                                                                      where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString()
                                                                      select product.Field<string>("Operation") + "*" + product.Field<string>("Produit") + "*" + product.Field<string>(reqParamPays) + "*" + product.Field<string>("PaysOrigine") + "*" + product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE")).ToList();

                                            foreach (var nature in dataCourantPeriode)
                                            {
                                                string[] strListe = nature.Split('*');

                                                int PositionPeriode = Array.FindIndex(indicateurPeriode, m => m == courantMoisAnnee.libelle.ToString());

                                                int PositionOperation = Array.FindIndex(indicateurOperation, m => m == nature.ToString());

                                                int LigneReelle = PositionOperation + numeroProduit;

                                                if (dtFinal.Rows[LigneReelle][0].ToString() == "")
                                                {
                                                    dtFinal.Rows[LigneReelle][0] = nature.ToString().Substring(0, 1);

                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[4]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][4] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 5] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else if (dtFinal.Rows[LigneReelle][0].ToString() == nature.ToString().Substring(0, 1))
                                                {
                                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[4]
                                                                   select product).First();

                                                    if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                    {
                                                        double valeur = double.Parse(dataRow["Total"].ToString());
                                                        dtFinal.Rows[LigneReelle][1] = dataRow["DescProduit"].ToString();
                                                        dtFinal.Rows[LigneReelle][2] = dataRow[reqParamPays].ToString();
                                                        dtFinal.Rows[LigneReelle][3] = dataRow["PaysOrigine"].ToString();
                                                        dtFinal.Rows[LigneReelle][4] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                        dtFinal.Rows[LigneReelle][PositionPeriode + 5] = valeur;
                                                        totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                    }
                                                }
                                                else
                                                {
                                                    int bonus = 1;

                                                    while (LigneReelle < dtFinal.Rows.Count)
                                                    {

                                                        if (dtFinal.Rows[LigneReelle + bonus][0].ToString() == nature.ToString().Substring(0, 1))
                                                        {
                                                            var dataRow = (from product in dtDepart.AsEnumerable()
                                                                           where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Operation") == nature.ToString().Substring(0, 1) && product.Field<string>("Lettres") == courantMoisAnnee.libelle.ToString() && product.Field<string>("Produit") == strListe[1] && product.Field<string>(reqParamPays) == strListe[2] && product.Field<string>("PaysOrigine") == strListe[3] && product.Field<string>("NOMOURAISONSOCIALEBENEFICIAIRE") == strListe[4]
                                                                           select product).First();

                                                            if (dataRow["Total"].ToString() != null && dataRow["Total"].ToString() != "")
                                                            {
                                                                double valeur = double.Parse(dataRow["Total"].ToString());
                                                                dtFinal.Rows[LigneReelle + bonus][1] = dataRow["DescProduit"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][2] = dataRow[reqParamPays].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][3] = dataRow["PaysOrigine"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][4] = dataRow["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                                                                dtFinal.Rows[LigneReelle + bonus][PositionPeriode + 5] = valeur;
                                                                totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ++bonus;
                                                        }
                                                    }

                                                }

                                            }
                                        }

                                        numeroProduit = numeroProduit + totalOperationParProduit;

                                        ++b;
                                    }


                                    dtFinal.Columns.Add("Cumul", typeof(string));

                                    for (int i = 0; i < dtFinal.Rows.Count; i++)
                                    {
                                        decimal totalParOperation = 0;

                                        string typeOperation = dtFinal.Rows[i][0].ToString();

                                        switch (typeOperation)
                                        {
                                            case "E":
                                                dtFinal.Rows[i][0] = "Export ";
                                                break;
                                            case "I":
                                                dtFinal.Rows[i][0] = "Import  ";
                                                break;
                                            case "S":
                                                dtFinal.Rows[i][0] = "Transit ";
                                                break;
                                            case "R":
                                                dtFinal.Rows[i][0] = "RéExport ";
                                                break;
                                        }

                                        for (int j = 5; j < dtFinal.Columns.Count - 1; j++)
                                        {
                                            if (dtFinal.Rows[i][j].ToString() != "")
                                            {
                                                decimal val = Convert.ToDecimal(dtFinal.Rows[i][j].ToString());

                                                totalParOperation = totalParOperation + val;

                                                dtFinal.Rows[i][j] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                            }

                                        }

                                        dtFinal.Rows[i][dtFinal.Columns.Count - 1] = totalParOperation.ToString("n", f).Substring(0, totalParOperation.ToString("n", f).LastIndexOf("."));

                                    }

                                    ListeGrid.DataSource = dtFinal;
                                    ListeGrid.DataBind();


                                    for (int i = 0; i < ListeGrid.Rows.Count; i++)
                                    {
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].Font.Bold = true;
                                        ListeGrid.Rows[i].Cells[dtFinal.Columns.Count - 1].ForeColor = Color.OrangeRed;
                                    }

                                    lblTotal.Text = totalValeurCFA.ToString("n", f).Substring(0, totalValeurCFA.ToString("n", f).LastIndexOf("."));

                                }
                                else
                                {
                                    ListeGrid.DataSource = dtDepart;
                                    ListeGrid.DataBind();
                                }
                            }
                        }

                        //// Fin ajout 24022021

                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    lblTotal.Font.Bold = true;
                    lblTotal.ForeColor = Color.Green;

                    String strGlobalOrbus = "";
                    String strGlobalOrbusPoleOperateur = "";

                    strGlobalOrbus = " SELECT SUM(co.VALEURCFA) [Total] " +
                                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                                  " WHERE 1=1 " + strInterne;

                    if (strInterne != "")
                    {
                        if (numIncsriptionClient > 0)
                        {
                            if (produit != "" || paysOrigine != "")
                            {
                                strGlobalOrbusPoleOperateur = " SELECT SUM(co.VALEURCFA) [Total] " +
                                      " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                      " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                      " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                                      " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR " +
                                      " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient + " " + strInterne;
                            }
                            else
                            {
                                strGlobalOrbus = " SELECT SUM(fa.VALEURTOTALECFA) [Total] " +
                                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                  " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR " +
                                      " WHERE 1=1 " + strInterne;

                                strGlobalOrbusPoleOperateur = " SELECT SUM(fa.VALEURTOTALECFA) [Total] " +
                                      " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                      " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR " +
                                      " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient + " " + strInterne;
                            }
                            
                        }
                        else
                        {
                            if (produit != "" || paysOrigine != "")
                            {
                                strGlobalOrbusPoleOperateur = " SELECT SUM(co.VALEURCFA) [Total] " +
                                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                          " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                                          " WHERE 1=1 " + strInterne;

                                if (!strGlobalOrbusPoleOperateur.Contains("select * from JOINDRE"))
                                {
                                    strGlobalOrbusPoleOperateur = strGlobalOrbusPoleOperateur + " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS) ";
                                }    
                            }
                            else
                            {
                                strGlobalOrbus = " SELECT SUM(fa.VALEURTOTALECFA) [Total] " +
                                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                          " WHERE 1=1 " + strInterne;

                                strGlobalOrbusPoleOperateur = " SELECT SUM(fa.VALEURTOTALECFA) [Total] " +
                                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                          " WHERE 1=1 " + strInterne;

                                if (!strGlobalOrbusPoleOperateur.Contains("select * from JOINDRE"))
                                {
                                    strGlobalOrbusPoleOperateur = strGlobalOrbusPoleOperateur + " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS) ";
                                }      
                            }
                        }

                        if (devise != "-1")
                        {
                            strGlobalOrbus = strGlobalOrbus.Replace("fa.VALEURTOTALECFA", "fa.VALEURTOTALDEVISE");
                            strGlobalOrbus = strGlobalOrbus.Replace("co.VALEURCFA", "co.VALEURDEVISE");

                            strGlobalOrbusPoleOperateur = strGlobalOrbusPoleOperateur.Replace("fa.VALEURTOTALECFA", "fa.VALEURTOTALDEVISE");
                            strGlobalOrbusPoleOperateur = strGlobalOrbusPoleOperateur.Replace("co.VALEURCFA", "co.VALEURDEVISE");
                        }

                    }

                    decimal valOrbus = 0;

                    using (cmdOrbus = new SqlCommand(strGlobalOrbus, con))
                    {
                        SqlDataReader sqlReader = cmdOrbus.ExecuteReader();

                        if (sqlReader.Read())
                        {
                            if (!string.IsNullOrEmpty(sqlReader.GetValue(0).ToString()))
                            {
                                valOrbus = valOrbus + decimal.Parse(sqlReader.GetValue(0).ToString());

                            }
                        }

                        sqlReader.Dispose();

                    }

                    cmdOrbus.Dispose();

                    using (cmdPoleOperateur = new SqlCommand(strGlobalOrbusPoleOperateur, con))
                    {
                        SqlDataReader sqlReader = cmdPoleOperateur.ExecuteReader();

                        //lblTransactions.Text = " 0 ";

                        lblPourcentagePartMarche.Text = " 0 % ";

                        if (sqlReader.Read())
                        {
                            decimal courant = 0;

                            if (!string.IsNullOrEmpty(sqlReader.GetValue(0).ToString()))
                            {
                                decimal.Parse(sqlReader.GetValue(0).ToString());

                                courant = courant + decimal.Parse(sqlReader.GetValue(0).ToString());

                                decimal pourcentage = (Math.Abs((decimal)courant / valOrbus)) * 100;

                                if (idPole > 0)
                                {
                                    lblTransactions.Text = courant.ToString("n", f).Substring(0, courant.ToString("n", f).LastIndexOf("."));
                                }
                                else
                                {
                                    lblTransactions.Text = "";
                                }
                                

                                lblPourcentagePartMarche.Text = "  " + Math.Round(pourcentage, 2) + " % ";
                            }

                        }

                        if (idPole > 0)
                        {
                            lblTransactions.Font.Bold = true;
                            lblTransactions.ForeColor = Color.Green;
                        }

                        //lblTransactions.Font.Bold = true;
                        //lblTransactions.ForeColor = Color.Green;

                        lblPourcentagePartMarche.Font.Bold = true;
                        lblPourcentagePartMarche.ForeColor = Color.Green;
                    }

                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }
                finally
                {
                    cmd1.Dispose();
                    cmdPoleOperateur.Dispose();
                    con.Dispose();
                }

            }
        }

        //protected void ListeGrid_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    this.ChargerDonnees(e.SortExpression);
        //}

        protected void ListeGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = ListeGrid.SelectedRow;
        }

        protected void ListeGrid_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            ListeGrid.PageIndex = e.NewPageIndex;
            ChargerDonnees();
        }


        private void BindColumnPays()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("codePays");
            dt.Columns.Add("nomPays");
            dt.Rows.Add();
            PaysGrid.DataSource = dt;
            PaysGrid.DataBind();
        }

        private void BindColumnProduits()
        {
            //ProduitSearch = grdProd;
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[2] { new DataColumn("codeProduit"), new DataColumn("designation") });
            //dt.Columns.Add(new DataColumn("Action", typeof(bool)));
            //dt.Columns.Add("codeProduit", typeof(string));
            //dt.Columns.Add("designation", typeof(string));
            dt.Rows.Add();

            ProduitSearch.DataSource = dt;
            ProduitSearch.DataBind();

        }

        [System.Web.Services.WebMethod]
        public static List<ProduitListe> ChargerListeProduit(string codeProduit, string designation)
        {
            DataTable dt = new DataTable();
            List<ProduitListe> detailsProduits = new List<ProduitListe>();
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
                        ProduitListe benef = new ProduitListe();
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
                        ProduitListe benef = new ProduitListe();
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
                        ProduitListe benef = new ProduitListe();
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

        public class ProduitListe
        {
            public string codeProduit { get; set; }
            public string designation { get; set; }
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
                        benef.nomPays = resultat.nom_pays.ToString().ToUpper();
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
                        benef.nomPays = resultat.nom_pays.ToString().ToUpper();
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
                        benef.nomPays = resultat.nom_pays.ToString().ToUpper();
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

        protected void BeneficiaireGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = BeneficiaireGrid.SelectedRow;
        }

        protected void BeneficiaireGrid_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            BeneficiaireGrid.PageIndex = e.NewPageIndex;
            ChargerTempBeneficiaire();
        }

        protected void PaysGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = PaysGrid.SelectedRow;
        }

        protected void PaysGrid_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            PaysGrid.PageIndex = e.NewPageIndex;
            BindColumnPays();
        }
        protected void ProduitSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = ProduitSearch.SelectedRow;
        }

        protected void ProduitSearch_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            ProduitSearch.PageIndex = e.NewPageIndex;
            BindColumnProduits();
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
            String str = " select distinct codeppm, coalesce(nomouraisonsocialebeneficiaire, '') nomouraisonsocialebeneficiaire from clients where codeppm is not null order by nomouraisonsocialebeneficiaire ASC ";
            SqlCommand cmd = new SqlCommand(str, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            TempGridBeneficiaire.DataSource = dt;
            TempGridBeneficiaire.DataBind();

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
            String str = " select CODE_PAYS , NOM_PAYS from Pays order by nom_pays ASC ";
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
            ProduitSearch.DataSource = dt1;
            ProduitSearch.DataBind();

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

            //SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            //dt.Columns.Add(new DataColumn("CODEDEVISE", typeof(string)));
            //dt.Columns.Add(new DataColumn("NOMDEVISE", typeof(string)));
            //DataRow row = dt.NewRow();
            //row["CODEDEVISE"] = "-1";
            //row["NOMDEVISE"] = " ";
            //dt.Rows.Add(row);
            //DataRow blankRow = dt.NewRow();
            //blankRow["CODEDEVISE"] = "-1";
            //blankRow["NOMDEVISE"] = " ";
            //dt.Rows.InsertAt(blankRow, 0);
            //DeviseList.Items.Insert(0, new ListItem("", "-1"));
            //DeviseList.DataTextField = "NOMDEVISE";
            //DeviseList.DataValueField = "CODEDEVISE";
            
            DeviseList.Items.Insert(0, new ListItem(String.Empty,"-1"));

        }

        protected void VisualiserExcel_Click(object sender, EventArgs e)
        {
            Response.ClearContent();
            Response.AppendHeader("content-disposition", "attachment;filename=TestExtract.xls");
            Response.ContentType = "application/vnd.ms-excel";

            StringWriter stringwriter = new StringWriter();
            HtmlTextWriter htmtextwriter = new HtmlTextWriter(stringwriter);

            ListeGrid.HeaderRow.Style.Add("background-color", "#ffffff");

            foreach (TableCell tableCell in ListeGrid.HeaderRow.Cells)
            {
                tableCell.Style["background-color"] = "#ffffff";
            }

            foreach (GridViewRow gridviewrow in ListeGrid.Rows)
            {
                gridviewrow.BackColor = System.Drawing.Color.White;
                foreach (TableCell gridviewrowtablecell in gridviewrow.Cells)
                {
                    gridviewrowtablecell.Style["background-color"] = "#ffffff";
                }
            }

            ListeGrid.AllowPaging = false;
            this.ChargerDonnees();
            ListeGrid.RenderControl(htmtextwriter);
            Response.Write(stringwriter.ToString());
            Response.End();

        }

        protected void Annuler_Click(object sender, EventArgs e)
        {
            Drop_Operation_Rech.Value = "-1";

            DateParList.Value = "-1";

            txtPaysProvenance.Text = "";

            txtPaysOrigine.Text = "";

            txtBeneficiaire.Text = "";

            Text_DebutPeriode_Rech.Text = "";

            Text_FinPeriode_Rech.Text = "";

            //DeviseList.Value = "-1";

            Request.Form["txtProduit"] = "";

            ListeGrid.DataSource = null;
            ListeGrid.DataBind();

            lblTotal.Text = "";
            lblTransactions.Text = "";
        }

        public override void VerifyRenderingInServerForm(System.Web.UI.Control control) { }

    }
}