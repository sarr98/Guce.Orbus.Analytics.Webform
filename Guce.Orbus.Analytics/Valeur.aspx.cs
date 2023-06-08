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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Guce.Orbus.Analytics
{
    public partial class Valeur : System.Web.UI.Page
    {
        private String strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private String strNumInscription = "";
        private String strIdPole = "";
        public static string StrConn { get; set; }
        public static DataTable tempDtPays = new DataTable();
        public static DataTable tempDtProduits = new DataTable();
        public static DataTable tempDtBeneficiaire = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {

            StrConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            ChartSeries.Visible = false;
            ChartLine.Visible = false;
            resultatValeur.Visible = false;

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


        private void ChargerDonnees()
        {
            String natureOperation = Drop_Operation_Rech.Value + "";
            natureOperation = natureOperation.Trim();

            String beneficiaire = txtBeneficiaire.Text + "";
            beneficiaire = beneficiaire.Trim();

            String groupage = GroupageList.Value + "";

            String datePar = DateParList.Value + "";

            String paysProvenance = txtPaysProvenance.Text + "";
            paysProvenance = paysProvenance.Trim();

            String paysOrigine = txtPaysOrigine.Text + "";
            paysOrigine = paysOrigine.Trim();

            String debut = Text_DebutPeriode_Rech.Text + "";
            debut = debut.Trim();

            String fin = Text_FinPeriode_Rech.Text + "";
            fin = fin.Trim();

            String devise = DeviseList.Value + "";
            devise = devise.Trim();

            String produit = txtProduit.Text + "";


            SqlConnection con = new SqlConnection(strConnString);
            con.Open();
            String str = "";
            String strGlobalBeneficiaire = "";
            String reqPercentBeneficiaire = "";

            int numIncsriptionClient = 0;
            int idPole = 0;

            if (!strNumInscription.Equals(""))
            {
                numIncsriptionClient = Int32.Parse(strNumInscription);

                //String numInscriptionBollore = isBollore();

                //if (numInscriptionBollore.Contains(strNumInscription))
                //{
                //    String[] val = numInscriptionBollore.Split(',');
                //    String strBollore = "'" + val[0] + "'";
                //    int i = 1;

                //    for (i = 1; i < val.Length; i++)
                //    {
                //        strBollore = strBollore + ",'" + val[i] + "'";
                //    }

                //}
            }
            else
            {
                idPole = Int32.Parse(strIdPole);
            }

            if (groupage == "1")
            {
                if (datePar == "1")
                {
                    if (numIncsriptionClient > 0)
                    {
                        if (produit != "" || paysOrigine != "")
                        {
                            str = " SELECT SUM(co.VALEURCFA) [Total], FORMAT (datedossiertps, 'dd/MM/yyyy') [Chiffres],DATENAME(WEEKDAY, do.DATEDOSSIERTPS) [Lettres] , DATEPART(month,DATEDOSSIERTPS) [Mois], DATEPART(WEEKDAY, do.DATEDOSSIERTPS) [Jour] " +
                              " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                              " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                              " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                              " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(co.VALEURCFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                           " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) ";
                                           //" WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                        }
                        else
                        {
                            str = " SELECT SUM(fa.VALEURTOTALECFA) [Total], FORMAT (datedossiertps, 'dd/MM/yyyy') [Chiffres],DATENAME(WEEKDAY, do.DATEDOSSIERTPS) [Lettres] , DATEPART(month,DATEDOSSIERTPS) [Mois], DATEPART(WEEKDAY, do.DATEDOSSIERTPS) [Jour] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(fa.VALEURTOTALECFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " ;
                                          //" WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                        }
                        
                    }
                    else
                    {
                        if (produit != "" || paysOrigine != "")
                        {
                            str = " SELECT SUM(co.VALEURCFA) [Total], FORMAT (datedossiertps, 'dd/MM/yyyy') [Chiffres],DATENAME(WEEKDAY, do.DATEDOSSIERTPS) [Lettres] , DATEPART(month,DATEDOSSIERTPS) [Mois], DATEPART(WEEKDAY, do.DATEDOSSIERTPS) [Jour] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " WHERE  1=1 ";

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(co.VALEURCFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                            //" WHERE 1=1 ";
                        }
                        else
                        {
                            str = " SELECT SUM(fa.VALEURTOTALECFA) [Total], FORMAT (datedossiertps, 'dd/MM/yyyy') [Chiffres],DATENAME(WEEKDAY, do.DATEDOSSIERTPS) [Lettres] , DATEPART(month,DATEDOSSIERTPS) [Mois], DATEPART(WEEKDAY, do.DATEDOSSIERTPS) [Jour] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " WHERE  1=1 ";

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(fa.VALEURTOTALECFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                            //" WHERE 1=1 ";
                        }
                        
                    }

                }
                else if (datePar == "2")
                {
                    if (numIncsriptionClient > 0)
                    {
                        if (produit != "" || paysOrigine != "")
                        {
                            str = " SELECT SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres],DATENAME(MONTH, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(co.VALEURCFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                           " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " ;
                                           //" WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                        }
                        else
                        {
                            str = " SELECT SUM(fa.VALEURTOTALECFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres],DATENAME(MONTH, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(fa.VALEURTOTALECFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " ;
                                           //" WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                        }
                        
                    }
                    else
                    {
                        if (produit != "" || paysOrigine != "")
                        {
                            str = " SELECT SUM(co.VALEURCFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres],DATENAME(MONTH, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " WHERE 1=1 ";

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(co.VALEURCFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                            //" WHERE 1=1 ";
                        }
                        else
                        {
                            str = " SELECT SUM(fa.VALEURTOTALECFA) [Total], DATEPART(MONTH,datedossiertps) [Chiffres],DATENAME(MONTH, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " WHERE 1=1 ";

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(fa.VALEURTOTALECFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                            //" WHERE 1=1 ";
                        }
                        
                    }

                }
                else if (datePar == "3")
                {
                    if (numIncsriptionClient > 0)
                    {
                        if (produit != "" || paysOrigine != "")
                        {
                            str = " SELECT SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(co.VALEURCFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                           " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " ;
                                           //" WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                        }
                        else
                        {
                            str = " SELECT SUM(fa.VALEURTOTALECFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(fa.VALEURTOTALECFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " ;
                                           //" WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                        }
                        
                    }
                    else
                    {
                        if (produit != "" || paysOrigine != "")
                        {
                            str = " SELECT SUM(co.VALEURCFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " WHERE  1=1 ";

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(co.VALEURCFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                            //" WHERE 1=1 ";
                        }
                        else
                        {
                            str = " SELECT SUM(fa.VALEURTOTALECFA) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " WHERE  1=1 ";

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(fa.VALEURTOTALECFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                            //" WHERE 1=1 ";
                        }
                        
                    }

                }
            }
            else if (groupage == "2")
            {
                if (numIncsriptionClient > 0)
                {
                    if (produit != "" || paysOrigine != "")
                    {
                        str = " SELECT SUM(co.VALEURCFA) [Total], fa.PAYSPROVENANCE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(co.VALEURCFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                           " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " ;
                                           //" WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                    }
                    else
                    {
                        str = " SELECT SUM(fa.VALEURTOTALECFA) [Total], fa.PAYSPROVENANCE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(fa.VALEURTOTALECFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " ;
                                           //" WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                    }
                    
                }
                else
                {
                    if (produit != "" || paysOrigine != "")
                    {
                        str = " SELECT SUM(co.VALEURCFA) [Total], fa.PAYSPROVENANCE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " WHERE  1=1 ";

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(co.VALEURCFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                        //" WHERE 1=1 ";
                    }
                    else
                    {
                        str = " SELECT SUM(fa.VALEURTOTALECFA) [Total], fa.PAYSPROVENANCE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " WHERE  1=1 ";

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(fa.VALEURTOTALECFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                        //" WHERE 1=1 ";
                    }
                    
                }

            }
            else
            {
                if (numIncsriptionClient > 0)
                {
                    if (produit != "" || paysOrigine != "")
                    {
                        str = " SELECT SUM(co.VALEURCFA) [Total], do.NOMOURAISONSOCIALEBENEFICIAIRE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(co.VALEURCFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                           " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " ;
                                           //" WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                    }
                    else
                    {
                        str = " SELECT SUM(fa.VALEURTOTALECFA) [Total], do.NOMOURAISONSOCIALEBENEFICIAIRE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(fa.VALEURTOTALECFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " ;
                                           //" WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                    }
                    
                }
                else
                {
                    if (produit != "" || paysOrigine != "")
                    {
                        str = " SELECT SUM(co.VALEURCFA) [Total], do.NOMOURAISONSOCIALEBENEFICIAIRE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " WHERE  1=1 ";

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(co.VALEURCFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                           " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                        //" WHERE 1=1 ";
                    }
                    else
                    {
                        str = " SELECT SUM(fa.VALEURTOTALECFA) [Total], do.NOMOURAISONSOCIALEBENEFICIAIRE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " WHERE  1=1 ";

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,SUM(coalesce(fa.VALEURTOTALECFA, 0)) Total " +
                                           " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                                           //" WHERE 1=1 ";
                    }
                    
                }

            }

            ///////////////AJOUT DJOSSOU: si banque alors prendre en compte le parametre domiciliation/////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
            strGlobalBeneficiaire = strGlobalBeneficiaire + " WHERE 1=1 " + strDom;
            reqPercentBeneficiaire = strGlobalBeneficiaire ;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


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
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and do.CODEPPM in (" + strBeneficiaire + ")";

                }
                else
                {
                    str = str + " and do.CODEPPM = '" + beneficiaire + "'";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and do.CODEPPM = '" + beneficiaire + "'";
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
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and co.NUMEROTARIFDOUANE in (" + strProduit + ")";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and co.NUMEROTARIFDOUANE in (" + strProduit + ")";

                }
                else
                {
                    str = str + " and co.NUMEROTARIFDOUANE = '" + produit + "'";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and co.NUMEROTARIFDOUANE = '" + produit + "'";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and co.NUMEROTARIFDOUANE = '" + produit + "'";
                }

            }

            if (natureOperation != "-1")
            {
                if (natureOperation == "1")
                {
                    str = str + " and do.IMPORTATIONOUEXPORTATION = 'I' ";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and do.IMPORTATIONOUEXPORTATION = 'I' ";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and do.IMPORTATIONOUEXPORTATION = 'I' ";
                }
                else if (natureOperation == "2")
                {
                    str = str + " and do.IMPORTATIONOUEXPORTATION = 'E' ";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and do.IMPORTATIONOUEXPORTATION = 'E' ";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and do.IMPORTATIONOUEXPORTATION = 'E' ";
                }
                else if (natureOperation == "3")
                {
                    str = str + " and do.IMPORTATIONOUEXPORTATION = 'S' ";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and do.IMPORTATIONOUEXPORTATION = 'S' ";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and do.IMPORTATIONOUEXPORTATION = 'S' ";
                }
                else if (natureOperation == "4")
                {
                    str = str + " and do.IMPORTATIONOUEXPORTATION = 'R' ";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and do.IMPORTATIONOUEXPORTATION = 'R' ";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and do.IMPORTATIONOUEXPORTATION = 'R' ";
                }
            }

            if (devise != "-1")
            {
                if (devise.Contains("EUR"))
                {
                    str = str + " and fa.CODEDEVISE like 'EUR%'";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and fa.CODEDEVISE like 'EUR%'";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and fa.CODEDEVISE like 'EUR%'";
                }
                else
                {
                    str = str + " and fa.CODEDEVISE = '" + devise + "'";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and fa.CODEDEVISE like '" + devise + "%'";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and fa.CODEDEVISE like '" + devise + "%'";
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
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and fa.PAYSPROVENANCE in (" + strPays + ")";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and fa.PAYSPROVENANCE in (" + strPays + ")";

                }
                else
                {
                    str = str + " and fa.PAYSPROVENANCE = '" + paysProvenance + "'";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and fa.PAYSPROVENANCE = '" + paysProvenance + "'";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and fa.PAYSPROVENANCE = '" + paysProvenance + "'";
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
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and co.PAYSORIGINE in (" + strPays + ")";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and co.PAYSORIGINE in (" + strPays + ")";

                }
                else
                {
                    str = str + " and co.PAYSORIGINE = '" + paysOrigine + "'";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and co.PAYSORIGINE = '" + paysOrigine + "'";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and co.PAYSORIGINE = '" + paysOrigine + "'";
                }

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
                //str = str + " and do.DATEDOSSIERTPS  < '" + fin + "' ";
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
                else
                {
                    if (datePar == "1")
                    {
                        if (dateDebut.AddMonths(1) < dateFin)
                        {
                            msgPopup.Text = "Un intervalle d'au plus un mois est autorisé pour les dates !!!";
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
                    else if (datePar == "2")
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

            }

            //var diffMonths = (end.Month + end.Year * 12) - (start.Month + start.Year * 12);

            if (verifChamps)
            {
                if (idPole > 0)
                {
                    str = str + " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS =do.NUMERODOSSIERTPS) ";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS =do.NUMERODOSSIERTPS) ";
                }
                else if (numIncsriptionClient > 0)
                {
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                }

                if (groupage == "1")
                {
                    if (datePar == "1")
                    {
                        str = str + " GROUP BY  DATEPART(WEEKDAY, do.DATEDOSSIERTPS) , FORMAT (datedossiertps, 'dd/MM/yyyy'), DATEPART(month,DATEDOSSIERTPS), DATENAME(WEEKDAY,DATEDOSSIERTPS) " +
                                    " ORDER BY 5,4,2 ASC ";
                    }
                    else if (datePar == "2")
                    {
                        str = str + " GROUP BY  DATENAME(MONTH, do.DATEDOSSIERTPS),DATEPART(MONTH,datedossiertps) " +
                                    " ORDER BY DATEPART(MONTH,datedossiertps) ";
                    }
                    else if (datePar == "3")
                    {
                        
                        str = str + " GROUP BY  DATENAME(YEAR, do.DATEDOSSIERTPS),DATEPART(YEAR,datedossiertps) " +
                                    " ORDER BY DATEPART(YEAR,datedossiertps) ";
                    }
                }
                else if (groupage == "2")
                {
                    
                    str = str + " GROUP BY fa.PAYSPROVENANCE " +
                                " ORDER BY SUM(fa.VALEURTOTALECFA) DESC ";
                }
                else
                {
                    
                    str = str + " GROUP BY do.NOMOURAISONSOCIALEBENEFICIAIRE " +
                                " ORDER BY SUM(fa.VALEURTOTALECFA) DESC ";
                }

                ////////////////AJOUT DJOSSOU: Pour recadrer la requete///////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////

                if (produit != "" || paysOrigine != "")
                {
                    str = str.Replace("SUM(fa.VALEURTOTALECFA)", "SUM(co.VALEURCFA)");
                }

                if ((natureOperation == "2") || (natureOperation == "4"))
                {
                    str = str.Replace("fa.PAYSPROVENANCE", "fa.PAYSDESTINATION");
                }

                if (devise != "-1")
                {
                    str = str.Replace("fa.VALEURTOTALECFA", "fa.VALEURTOTALDEVISE");
                    str = str.Replace("co.VALEURCFA", "co.VALEURDEVISE");

                    reqPercentBeneficiaire = reqPercentBeneficiaire.Replace("fa.VALEURTOTALECFA", "fa.VALEURTOTALDEVISE");
                    reqPercentBeneficiaire = reqPercentBeneficiaire.Replace("co.VALEURCFA", "co.VALEURDEVISE");

                    strGlobalBeneficiaire = strGlobalBeneficiaire.Replace("fa.VALEURTOTALECFA", "fa.VALEURTOTALDEVISE");
                    strGlobalBeneficiaire = strGlobalBeneficiaire.Replace("co.VALEURCFA", "co.VALEURDEVISE");
                }

                /////////////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////

                try
                {
                    decimal totalValeurCFA = 0;
                    DataTable dtDepart = new DataTable();
                    DataTable dtFinal = new DataTable();
                    SqlCommand cmd = new SqlCommand(str, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtDepart);
                    var maxLignes = 0;
                    //storing total rows count to loop on each Record  
                    string[] x = new string[dtDepart.Rows.Count];
                    decimal[] y = new decimal[dtDepart.Rows.Count];
                    decimal[] ord = new decimal[dtDepart.Rows.Count];
                    decimal[] ordPerCent = new decimal[dtDepart.Rows.Count];
                    var f = new NumberFormatInfo { NumberGroupSeparator = " " };

                    if (dtDepart.Rows.Count > 0)
                    {
                        ChartSeries.Visible = true;
                        ChartLine.Visible = true;
                        resultatValeur.Visible = true;

                        if (groupage == "1")
                        {
                            if (datePar == "1")
                            {
                                var query = (from row in dtDepart.AsEnumerable()
                                             group row by new
                                             {
                                                 JoursLettres = row.Field<string>("Lettres")
                                             } into grp
                                             select new
                                             {
                                                 Jours = grp.Key.JoursLettres.ToString(),
                                                 Count = grp.Count(),
                                                 DateDossier = grp.Select(r => r.Field<string>("Chiffres")).ToList(),
                                                 Total = grp.Select(r => r.Field<double>("Total")).ToList()
                                             }).ToList();

                                int a = 0;
                                maxLignes = query.Max(r => r.Count);
                                this.ChartSeries.Series.RemoveAt(0);

                                foreach (var resultat in query)
                                {
                                    dtFinal.Columns.Add(resultat.Jours.First().ToString().ToUpper() + resultat.Jours.Substring(1), typeof(string));
                                    ChartSeries.Series.Add(resultat.Jours.First().ToString().ToUpper() + resultat.Jours.Substring(1));
                                    x[a] = resultat.Jours.First().ToString().ToUpper() + resultat.Jours.Substring(1);

                                    while (dtFinal.Rows.Count <= maxLignes)
                                    {
                                        dtFinal.Rows.Add();
                                    }

                                    int b = 0;
                                    decimal totalParJours = 0;

                                    foreach (var nbre in resultat.Total)
                                    {
                                        decimal YVal = (decimal)nbre;
                                        dtFinal.Rows[b][a] = nbre;
                                        ChartSeries.Series[resultat.Jours.First().ToString().ToUpper() + resultat.Jours.Substring(1)].Points.AddXY("Semaine " + (int)(b + 1), YVal);
                                        totalValeurCFA = totalValeurCFA + (decimal)nbre;
                                        totalParJours = totalParJours + (decimal)nbre;

                                        dtFinal.Rows[b][a] = nbre + "#" + resultat.DateDossier[b].ToString();

                                        dtFinal.Rows[maxLignes][a] = totalParJours;
                                        y[b] = (decimal)nbre;
                                        ++b;
                                    }
                                    ord[a] = Convert.ToDecimal(dtFinal.Rows[maxLignes][a]);
                                    ++a;
                                }

                                x = x.Where(c => c != null).ToArray();
                                ord = ord.Where(c => c != 0).ToArray();

                                /////// Affichage pourcentage Valeur Chart Line ////////////
                                decimal pourcentage2 = 0;

                                for (int i = 0; i < ord.Count(); i++)
                                {
                                    decimal courant = decimal.Parse(ord[i].ToString());
                                    pourcentage2 = (Math.Abs((decimal)courant / totalValeurCFA)) * 100;
                                    ordPerCent[i] = Math.Round(pourcentage2, 2);
                                }

                                ordPerCent = ordPerCent.Where(c => c != 0).ToArray();

                                /////// Fin Affichage pourcentage Valeur Chart Line ////////////
                            }
                            else
                            {
                                if (datePar == "2")
                                {
                                    for (int i = 0; i < dtDepart.Rows.Count; i++)
                                    {
                                        String courant = dtDepart.Rows[i]["Lettres"].ToString();
                                        dtFinal.Columns.Add(courant.Substring(0, 1).ToUpper() + courant.Substring(1), typeof(string));
                                        //storing Values for X axis  
                                        x[i] = courant.Substring(0, 1).ToUpper() + courant.Substring(1);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < dtDepart.Rows.Count; i++)
                                    {
                                        dtFinal.Columns.Add(dtDepart.Rows[i]["Lettres"].ToString().ToUpper(), typeof(string));
                                        //storing Values for X axis  
                                        x[i] = dtDepart.Rows[i]["Lettres"].ToString();
                                    }
                                }

                                DataRow newRow = dtFinal.NewRow();

                                for (int i = 0; i < dtDepart.Rows.Count; i++)
                                {
                                    newRow[i] = dtDepart.Rows[i]["Total"].ToString();
                                    //storing values for Y Axis  
                                    y[i] = Convert.ToDecimal(dtDepart.Rows[i]["Total"]);
                                    totalValeurCFA = totalValeurCFA + decimal.Parse(dtDepart.Rows[i]["Total"].ToString());
                                }

                                dtFinal.Rows.Add(newRow);

                                x = x.Where(c => c != null).ToArray();
                                y = y.Where(c => c != 0).ToArray();

                                /////// Affichage pourcentage Valeur Chart Line ////////////

                                decimal pourcentage2 = 0;

                                for (int i = 0; i < y.Count(); i++)
                                {
                                    decimal courant = decimal.Parse(y[i].ToString());
                                    pourcentage2 = (Math.Abs((decimal)courant / totalValeurCFA)) * 100;
                                    ordPerCent[i] = Math.Round(pourcentage2, 2);
                                }

                                ordPerCent = ordPerCent.Where(c => c != 0).ToArray();

                                /////// Fin Affichage pourcentage Valeur Chart Line ////////////

                                //binding chart control  
                                ChartSeries.Series[0].Points.DataBindXY(x, ordPerCent); // x
                                ChartSeries.Series[0].ChartType = SeriesChartType.Column;
                            }
                        }
                        else if (groupage == "2")
                        {
                            if ((natureOperation == "2") || (natureOperation == "4"))
                            {
                                dtFinal.Columns.Add(" Pays Destination ", typeof(string));
                            }
                            else
                            {
                                dtFinal.Columns.Add(" Pays Provenance ", typeof(string));
                            }

                            dtFinal.Columns.Add(" Valeur Totale ", typeof(string));
                            //dtFinal.Columns.Add(" Part Pays ", typeof(string));

                            decimal totalValeursAutres = 0;

                            for (int i = 0; i < dtDepart.Rows.Count; i++)
                            {
                                decimal valeurTotaleFormat = Convert.ToDecimal(dtDepart.Rows[i]["Total"]);

                                dtFinal.Rows.Add(dtDepart.Rows[i]["Lettres"].ToString(), valeurTotaleFormat.ToString("n", f).Substring(0, valeurTotaleFormat.ToString("n", f).LastIndexOf(".")));

                                totalValeurCFA = totalValeurCFA + decimal.Parse(dtDepart.Rows[i]["Total"].ToString());

                                if (i < 5)
                                {
                                    x[i] = dtDepart.Rows[i]["Lettres"].ToString();
                                    y[i] = Convert.ToDecimal(dtDepart.Rows[i]["Total"]);
                                }
                                else
                                {
                                    x[5] = "Autres";
                                    y[5] = totalValeursAutres = totalValeursAutres + decimal.Parse(dtDepart.Rows[i]["Total"].ToString());
                                }

                            }

                            x = x.Where(c => c != null).ToArray();
                            y = y.Where(c => c != 0).ToArray();

                            decimal pourcentage3 = 0;

                            for (int i = 0; i < y.Count(); i++)
                            {
                                decimal courantY = decimal.Parse(y[i].ToString());
                                pourcentage3 = (Math.Abs((decimal)courantY / totalValeurCFA)) * 100;
                                ordPerCent[i] = Math.Round(pourcentage3, 2);
                            }

                            ordPerCent = ordPerCent.Where(c => c != 0).ToArray();

                            ChartSeries.Series[0].Points.DataBindXY(x, ordPerCent); // y
                            ChartSeries.Series[0].ChartType = SeriesChartType.Column;

                        }
                        else
                        {
                            dtFinal.Columns.Add(" Client ", typeof(string));
                            dtFinal.Columns.Add(" Valeur Totale ", typeof(decimal));
                            dtFinal.Columns.Add(" Part Client ", typeof(string));

                            decimal totalValeursAutres = 0;

                            for (int i = 0; i < dtDepart.Rows.Count; i++)
                            {
                                dtFinal.Rows.Add(dtDepart.Rows[i]["Lettres"].ToString(), dtDepart.Rows[i]["Total"], null);

                                totalValeurCFA = totalValeurCFA + decimal.Parse(dtDepart.Rows[i]["Total"].ToString());

                                if (i < 5)
                                {
                                    x[i] = dtDepart.Rows[i]["Lettres"].ToString();
                                    y[i] = Convert.ToDecimal(dtDepart.Rows[i]["Total"]);
                                }
                                else
                                {
                                    x[5] = "Autres";
                                    y[5] = totalValeursAutres = totalValeursAutres + decimal.Parse(dtDepart.Rows[i]["Total"].ToString());
                                }

                            }

                            x = x.Where(c => c != null).ToArray();
                            y = y.Where(c => c != 0).ToArray();

                            decimal pourcentage4 = 0;

                            for (int i = 0; i < y.Count(); i++)
                            {
                                decimal courantY = decimal.Parse(y[i].ToString());
                                pourcentage4 = (Math.Abs((decimal)courantY / totalValeurCFA)) * 100;
                                ordPerCent[i] = Math.Round(pourcentage4, 2);
                            }

                            ordPerCent = ordPerCent.Where(c => c != 0).ToArray();

                            ChartSeries.Series[0].Points.DataBindXY(x, ordPerCent); // y
                            ChartSeries.Series[0].ChartType = SeriesChartType.Column;

                        }

                        if (totalValeurCFA > 0)
                        {
                            if (devise != "-1")
                            {
                                decimal valeurEnDevise = ConvertirEnDevise(totalValeurCFA, devise);

                                if (devise != "CFA" && devise != "XAF")
                                {
                                    lblValeurTotale.Text = totalValeurCFA.ToString("n", f) + " " + devise + "  /  " + valeurEnDevise.ToString("n", f) + " CFA  ";
                                }
                                else
                                {
                                    lblValeurTotale.Text = valeurEnDevise.ToString("n", f) + " " + devise ;
                                }
                               
                            }
                            else
                            {
                                lblValeurTotale.Text = totalValeurCFA.ToString("n", f) + " CFA  ";
                            }
                        }
                        else
                        {
                            lblValeurTotale.Text = " 0 ";
                        }

                        lblValeurTotale.Font.Bold = true;
                        lblValeurTotale.ForeColor = Color.OrangeRed;

                        decimal pourcentage = 0;

                        string dateDeb = debut;
                        string dateFin = Convert.ToDateTime(fin).AddDays(1).ToString();

                        if (groupage == "1")
                        {
                            dtFinal.Rows.Add();

                            for (int i = 0; i < dtFinal.Columns.Count; i++)
                            {
                                decimal courant = decimal.Parse(dtFinal.Rows[dtFinal.Rows.Count - 2][i].ToString());
                                pourcentage = (Math.Abs((decimal)courant / totalValeurCFA)) * 100;
                                dtFinal.Rows[dtFinal.Rows.Count - 1][i] = Math.Round(pourcentage, 2) + " % ";
                            }


                            ValeurListeGridView.DataSource = dtFinal;

                            //////AJOUT DJOSSOU: Changement de la structure de la datasource du gridview//////////
                            //////////////////////////////////////////////////////////////////////////////////////

                            if (datePar == "2")
                            {
                                DataTable dtFinal2 = ChangeDataTableDirection(dtFinal);
                                ValeurListeGridView.DataSource = dtFinal2;
                            }

                            /////////////////////////////////////////////////////////////////////////////////////
                            //////////////////////////////////////////////////////////////////////////////////////



                            ValeurListeGridView.DataBind();



                            /*
                            ValeurListeGridView.Rows[ValeurListeGridView.Rows.Count - 2].Font.Bold = true;
                            ValeurListeGridView.Rows[ValeurListeGridView.Rows.Count - 2].ForeColor = Color.OrangeRed;

                            ValeurListeGridView.Rows[ValeurListeGridView.Rows.Count - 1].Font.Bold = true;
                            ValeurListeGridView.Rows[ValeurListeGridView.Rows.Count - 1].ForeColor = Color.Olive;
                            */

                        }
                        else
                        {
                            if (groupage == "3")
                            {
                                Dictionary<string, decimal> listValeurPercentBeneficiaire = GetListValeurGlobaleBeneficiaire(reqPercentBeneficiaire, dateDeb, dateFin);

                                decimal sommeBeneficiaire = 0;

                                sommeBeneficiaire = listValeurPercentBeneficiaire.Sum(item => item.Value);

                                for (int i = 0; i < dtFinal.Rows.Count; i++)
                                {
                                    string nomBeneficiaire = dtFinal.Rows[i][dtFinal.Columns.Count - 3].ToString();
                                    decimal courantActuel = listValeurPercentBeneficiaire.Where(k => k.Key == nomBeneficiaire).FirstOrDefault().Value;
                                    pourcentage = (Math.Abs((decimal)courantActuel / sommeBeneficiaire)) * 100;
                                    dtFinal.Rows[i][dtFinal.Columns.Count - 1] = Math.Round(pourcentage, 2) + " % ";
                                }

                                ValeurListeGridView.DataSource = dtFinal;

                                DataTable dtFinal3 = new DataTable();
                                dtFinal3.Columns.Add(" Client ", typeof(string));
                                dtFinal3.Columns.Add(" Valeur Totale ", typeof(string));
                                dtFinal3.Columns.Add(" Part Client ", typeof(string));
                                dtFinal3.Columns.Add(" Global Orbus ", typeof(string));

                                Dictionary<string, decimal> listValeurGlobaleBeneficiaire = GetListValeurGlobaleBeneficiaire(strGlobalBeneficiaire, dateDeb, dateFin);

                                for (int i = 0; i < dtFinal.Rows.Count; i++)
                                {

                                    try
                                    {
                                        //if (debut != "" && fin != "")
                                        decimal pourcentageGlobale = 0;
                                        decimal valeurGobaleCFA = 0;
                                        string nomBeneficiaire = dtFinal.Rows[i][0].ToString().Trim();

                                        //valeurGobaleCFA = GetValeurGlobaleBeneficiaire(nomBeneficiaire, debut, fin);
                                        valeurGobaleCFA = listValeurGlobaleBeneficiaire.Where(k => k.Key == nomBeneficiaire).FirstOrDefault().Value;
                                        decimal courant2 = decimal.Parse(dtFinal.Rows[i][dtFinal.Columns.Count - 2].ToString());
                                        if (valeurGobaleCFA > 0)
                                            pourcentageGlobale = (Math.Abs((decimal)courant2 / valeurGobaleCFA)) * 100;

                                        dtFinal3.Rows.Add();
                                        dtFinal3.Rows[i][0] = dtFinal.Rows[i][0].ToString();
                                        decimal valeurFormat = Convert.ToDecimal(dtFinal.Rows[i][1].ToString());
                                        dtFinal3.Rows[i][1] = valeurFormat.ToString("n", f).Substring(0, valeurFormat.ToString("n", f).LastIndexOf("."));//dtFinal.Rows[i][1].ToString();
                                        dtFinal3.Rows[i][2] = dtFinal.Rows[i][2].ToString();
                                        dtFinal3.Rows[i][3] = Math.Round(pourcentageGlobale, 2) + " % ";

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }

                                ValeurListeGridView.DataSource = dtFinal3;
                            }
                            else
                            {
                                ValeurListeGridView.DataSource = dtFinal;
                            }
                            
                            

                            //////AJOUT DJOSSOU: Changement de la structure de la datasource du gridview//////////
                            //////////////////////////////////////////////////////////////////////////////////////

                            //////////////////////////////////////////////////////////////////////////////////////
                            //////////////////////////////////////////////////////////////////////////////////////
                            ///chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{0:0,}K";
                            ///Chart4.ChartAreas[0].AxisX.LabelStyle.Enabled.


                            ValeurListeGridView.DataBind();

                        }

                        ChartSeries.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                        ChartSeries.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;

                        if (groupage == "1")
                        {

                            ChartLine.ChartAreas["ChartArea1"].AxisX.Maximum = x.Count();
                            ChartLine.ChartAreas[0].AxisX.Title = "Jours";
                            ChartLine.ChartAreas[0].AxisY.Title = "Nombre Dossiers (%)";
                            ChartLine.ChartAreas["ChartArea1"].AxisX.IsLabelAutoFit = true;
                            ChartLine.Series[0].MarkerStyle = MarkerStyle.Circle;
                            ChartLine.Series[0].MarkerColor = Color.Gold;
                            ChartLine.Series[0].ChartType = SeriesChartType.Line;
                            ChartLine.Series[0].IsVisibleInLegend = false;
                            ChartLine.Series[0].IsValueShownAsLabel = true;
                            ChartLine.Series[0].BorderWidth = 3;
                            ChartLine.Legends[0].Enabled = true;
                            ChartLine.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                            ChartLine.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
                            ChartLine.ChartAreas["ChartArea1"].AxisX.IsStartedFromZero = false;

                            if (datePar == "1")
                            {
                                ChartLine.Series[0].Points.DataBindXY(x, ordPerCent);
                                //ChartLine.Series[0].Points.DataBindXY(x, ord);

                                Color[] myPalette = { Color.Green, Color.LightGreen, Color.Coral, Color.LightBlue, Color.DeepSkyBlue, Color.Goldenrod, Color.Aquamarine, Color.Aqua, Color.MediumOrchid, Color.Lavender, Color.Thistle, Color.Beige };

                                ChartSeries.Palette = ChartColorPalette.None;
                                ChartSeries.PaletteCustomColors = myPalette;

                                for (int q = 0; q < x.Count(); q++)
                                {
                                    ChartSeries.Series[q].Points[0].BackGradientStyle = GradientStyle.Center;
                                    ChartSeries.Series[q].Points[0].Color = myPalette[q];
                                    ChartSeries.Series[q].Points[0].BackSecondaryColor = ControlPaint.Light(myPalette[q]);
                                }
                            }
                            else
                            {
                                if (datePar == "2")
                                {
                                    ChartLine.ChartAreas[0].AxisX.Title = "Mois";
                                }
                                else
                                {
                                    ChartLine.ChartAreas[0].AxisX.Title = "Années";
                                }

                                ChartLine.Series[0].Points.DataBindXY(x, ordPerCent);
                                //ChartLine.Series[0].Points.DataBindXY(x, y);

                                //binding chart control  
                                ChartSeries.Series[0].Points.DataBindXY(x, ordPerCent); //x
                                ChartSeries.Series[0].ChartType = SeriesChartType.Column;
                                ChartSeries.Series[0].IsValueShownAsLabel = true;

                                Color[] myPalette = { Color.Green, Color.LightGreen, Color.Coral, Color.LightBlue, Color.DeepSkyBlue, Color.Goldenrod, Color.Aquamarine, Color.Aqua, Color.MediumOrchid, Color.Lavender, Color.Thistle, Color.Beige };

                                ChartSeries.Palette = ChartColorPalette.None;
                                ChartSeries.PaletteCustomColors = myPalette;

                                for (int i = 0; i < x.Count(); i++)
                                {
                                    ChartSeries.Series[0].Points[i].BackGradientStyle = GradientStyle.None;
                                    ChartSeries.Series[0].Points[i].Color = myPalette[i];
                                    ChartSeries.Series[0].Points[i].BackSecondaryColor = ControlPaint.Light(myPalette[i]);
                                }

                                ChartSeries.Legends.Clear();

                            }

                        }
                        else
                        {
                            x = x.Where(c => c != null).ToArray();
                            y = y.Where(c => c != 0).ToArray();

                            Color[] myPalette1 = { Color.Aquamarine, Color.Aqua, Color.MediumOrchid, Color.Lavender, Color.Thistle, Color.Beige };
                            Color[] myPalette2 = { Color.Green, Color.LightGreen, Color.Coral, Color.LightBlue, Color.DeepSkyBlue, Color.Goldenrod };

                            ChartLine.Palette = ChartColorPalette.None;
                            ChartLine.PaletteCustomColors = myPalette1;

                            ChartSeries.Palette = ChartColorPalette.None;
                            ChartLine.PaletteCustomColors = myPalette1;

                            //////////////var maxValue = Convert.ToDecimal(y.Max());
                            //////////////double maxScale = 0;

                            //////////////if (maxValue < 1000000)
                            //////////////{
                            //////////////    ChartLine.ChartAreas["ChartArea1"].AxisY.LabelStyle.Format = "#,##0,K";

                            //////////////    // set maximum scale to the nearest interval by 500,000
                            //////////////    maxScale = Math.Ceiling(Convert.ToDouble(maxValue) / 500000d) * 500000;
                            //////////////}
                            //////////////else if (maxValue >= 1000000 && maxValue < 1000000000)
                            //////////////{
                            //////////////    ChartLine.ChartAreas["ChartArea1"].AxisY.LabelStyle.Format = "#,##0,,M";

                            //////////////    // set maximum scale to the nearest interval by 5 million
                            //////////////    maxScale = Math.Ceiling(Convert.ToDouble(maxValue) / 5000000d) * 5000000;
                            //////////////}

                            // set maximum scale
                            ////////////ChartLine.ChartAreas["ChartArea1"].AxisY.Maximum = maxScale;

                            ChartLine.Series[0].Points.DataBindXY(x, y);
                            ChartLine.Series[0].ChartType = SeriesChartType.Pie;
                            ChartLine.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                            ChartLine.Legends[0].Enabled = true;
                            //ChartLine.Series[0]["PieLabelStyle"] = "Outside";
                            ChartLine.ChartAreas[0].Area3DStyle.Inclination = 0;
                            ChartLine.Series[0].BorderWidth = 1;
                            ChartLine.Series[0].BorderDashStyle = ChartDashStyle.Dash;
                            ChartLine.Series[0].Label = "#PERCENT{P2}"; //"#PERCENT\n#VALX"; //"#PERCENT{P2}";
                            ChartLine.Series[0].LegendText = "#VALX";
                            ChartSeries.Legends[0].Enabled = false;
                            ChartSeries.Series[0].Label = "#PERCENT{P2}";
                            //ChartSeries.Series[0].LegendText = "#VALX";
                            ChartSeries.Series[0].IsValueShownAsLabel = true;

                            for (int i = 0; i < x.Count(); i++)
                            {
                                ChartLine.Series[0].Points[i].BackGradientStyle = GradientStyle.Center;
                                ChartLine.Series[0].Points[i].Color = myPalette1[i];
                                ChartLine.Series[0].Points[i].BackSecondaryColor = ControlPaint.Light(myPalette1[i]);

                                ChartSeries.Series[0].Points[i].BackGradientStyle = GradientStyle.Center;
                                ChartSeries.Series[0].Points[i].Color = myPalette2[i];
                                ChartSeries.Series[0].Points[i].BackSecondaryColor = ControlPaint.Light(myPalette2[i]);
                            }

                            ChartSeries.Legends.Clear();

                        }

                    }
                    else
                    {
                        ValeurListeGridView.DataSource = dtDepart;
                        ValeurListeGridView.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }
            }
        }

        protected void Rechercher_Click(object sender, EventArgs e)
        {
            this.ChargerDonnees();

        }

        protected void ValeurListeGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = ValeurListeGridView.SelectedRow;

        }
        protected void ValeurListeGridView_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            ValeurListeGridView.PageIndex = e.NewPageIndex;
            ChargerDonnees();
        }
        

        protected void ValeurListeGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                String groupage = GroupageList.Value + "";
                var f = new NumberFormatInfo { NumberGroupSeparator = " " };
                if (groupage == "1")
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        foreach (TableCell cell in e.Row.Cells)
                        {
                            string[] a = cell.Text.Split('#');
                            if (cell.Text.Contains('#'))
                            {
                                decimal val = Convert.ToDecimal(a[0]);
                                var str = "<span style='color: rgb(119,156,200);'>" + a[1] + "</span>" + "#" + val.ToString("n", f);
                                cell.Text = str.Replace("#", "<br>");
                            }
                        }
                    }
                }
                else if (groupage == "3")
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        foreach (TableCell cell in e.Row.Cells)
                        {
                            e.Row.Cells[2].Font.Bold = true;
                            e.Row.Cells[2].ForeColor = Color.OrangeRed;
                        }
                    }
                }


            }
            catch (Exception ex)
            {
            }


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
            String str = " select distinct codeppm, coalesce(nomouraisonsocialebeneficiaire, '') nomouraisonsocialebeneficiaire from clients where codeppm is not null order by nomouraisonsocialebeneficiaire ASC ";
            SqlCommand cmd = new SqlCommand(str, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            TempGridBeneficiaire.DataSource = dt;
            TempGridBeneficiaire.DataBind();

        }

        //private void BindColumnPays()
        //{

        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("codePays");
        //    dt.Columns.Add("nomPays");
        //    dt.Rows.Add();

        //    PaysGrid.DataSource = dt;
        //    PaysGrid.DataBind();

        //}


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

        //private void BindColumnBeneficiaire()
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("codePPM");
        //    dt.Columns.Add("beneficiaire");
        //    dt.Rows.Add();

        //    BeneficiaireGrid.DataSource = dt;
        //    BeneficiaireGrid.DataBind();
        //}

        //private void BindColumnProduits()
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("codeProduit");
        //    dt.Columns.Add("designation");
        //    dt.Rows.Add();

        //    ProduitGrid.DataSource = dt;
        //    ProduitGrid.DataBind();
        //}

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
                else
                    details = details.Take(50).ToList();
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
                else
                    detailsProduits = detailsProduits.Take(100).ToList();
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

                if (taux != 1)
                {
                    valeurDevise = valeur * taux;
                }
                else
                {
                    valeurDevise = valeur / taux;
                }

                
            }

            return valeurDevise;
        }

        //protected void VisualiserExcel_Click(object sender, EventArgs e)
        //{
        //    Response.Clear();
        //    Response.AddHeader("content-disposition", "attachement;filename=ExportData.xls");
        //    Response.Charset = "";
        //    Response.ContentType = "application/vnd.ms-excel";
        //    StringWriter UnstringWriter = new System.IO.StringWriter();
        //    HtmlTextWriter unhtmltextWriter = new HtmlTextWriter(UnstringWriter);
        //    ValeurListeGridView.RenderControl(unhtmltextWriter);
        //    Response.Write(UnstringWriter.ToString());
        //}

        //protected void VisualiserExcel_Click(object sender, EventArgs e)
        //{
        //    var doc = new Document();
        //    var pdf = Server.MapPath("PDF/Chart.PDF");

        //    PdfWriter.GetInstance(doc, new FileStream(pdf, FileMode.Create));
        //    doc.Open();

        //    doc.Add(new Paragraph("Dashboard"));
        //    var image = iTextSharp.text.Image.GetInstance(chartByte);
        //    image.ScalePercent(75f);
        //    doc.Add(image);
        //    doc.Close();

        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=Chart.PDF");
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.Write(pdf);
        //    Response.End();

        //}





        protected void VisualiserExcel_Click(object sender, EventArgs e)
        {
            String groupage = GroupageList.Value + "";
            Response.ClearContent();
            Response.AppendHeader("content-disposition", "attachment;filename=ValeurExtract.xls");
            Response.ContentType = "application/vnd.ms-excel";

            StringWriter stringwriter = new StringWriter();
            HtmlTextWriter htmtextwriter = new HtmlTextWriter(stringwriter);

            ValeurListeGridView.HeaderRow.Style.Add("background-color", "#ffffff");

            foreach (TableCell tableCell in ValeurListeGridView.HeaderRow.Cells)
            {
                tableCell.Style["background-color"] = "#df5015";
            }

            foreach (GridViewRow gridviewrow in ValeurListeGridView.Rows)
            {
                gridviewrow.BackColor = System.Drawing.Color.White;

                foreach (TableCell gridviewrowtablecell in gridviewrow.Cells)
                {
                    gridviewrowtablecell.Style["background-color"] = "#ffffff";
                }
            }

            ValeurListeGridView.AllowPaging = false;
            this.ChargerDonnees();
            ValeurListeGridView.RenderControl(htmtextwriter);
            Response.Write(stringwriter.ToString());
            Response.Flush();
            Response.End();

        }

        protected void Annuler_Click(object sender, EventArgs e)
        {
            Drop_Operation_Rech.Value = "-1";

            DateParList.Value = "1";

            GroupageList.Value = "1";

            txtPaysProvenance.Text = "";

            txtPaysOrigine.Text = "";

            txtBeneficiaire.Text = "";

            Text_DebutPeriode_Rech.Text = "";

            Text_FinPeriode_Rech.Text = "";

            DeviseList.Value = "-1";

            txtProduit.Text = "";

            lblValeurTotale.Text = "";

            ValeurListeGridView.DataSource = null;
            ValeurListeGridView.DataBind();

        }


        public override void VerifyRenderingInServerForm(System.Web.UI.Control control) { }

        //// export pdf
        //    Response.ContentType = "application/pdf";
        //Response.AddHeader("content-disposition", 
        //    "attachment;filename=Customers.pdf");
        //Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //StringWriter sw = new StringWriter();
        //HtmlTextWriter hw = new HtmlTextWriter(sw);

        //this.Page.RenderControl(hw);            

        // GridView gv = this.Existinggv;

        //gv.DataSource = 'your datasource with all required data'

        //gv.DataBind();
        //gv.RenderControl(hw);       

        //StringReader sr = new StringReader
        //    (sw.ToString().Replace("\r", "")
        //    .Replace("\n", "").Replace("  ", ""));

        //Document pdfDoc = 
        //    new Document(iTextSharp.text.PageSize.A4,
        //                 10f, 10f, 10f, 0.0f);

        //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //pdfDoc.Open();
        //htmlparser.Parse(sr);
        //pdfDoc.Close();
        //Response.Write(pdfDoc);
        //Response.End();



        ///////////////////////////AJOUT DJOSSOU: Récuperation du type pole//////////////////////


        private DataTable ChangeDataTableDirection(DataTable dtFinal)
        {

            DataTable dtFinal2 = new DataTable();

            try
            {

                dtFinal2.Columns.Add("Mois", typeof(string));
                dtFinal2.Columns.Add("Valeur", typeof(string));
                dtFinal2.Columns.Add("Part Mensuelle", typeof(string));

                //for (int i = 5; i < 10; i++)
                for (int i = 0; i < dtFinal.Columns.Count; i++)
                {
                    DataRow newRow2 = dtFinal2.NewRow();
                    string mois = dtFinal.Columns[i].ColumnName;
                    newRow2[0] = mois.Replace("é", "e").Replace("û", "u");

                    var f = new NumberFormatInfo { NumberGroupSeparator = " " };
                    decimal valeur = Convert.ToDecimal(dtFinal.Rows[0][mois].ToString());
                    
                    newRow2[1] = valeur.ToString("n", f);
                    newRow2[2] = dtFinal.Rows[1][mois].ToString();

                    dtFinal2.Rows.Add(newRow2);
                }

            }
            catch (Exception ex)
            { }

            return dtFinal2;

        }
        
        private Dictionary<string, decimal> GetListValeurGlobaleBeneficiaire(string str,string dateDebut, string dateFin)
        {
            Dictionary<string, decimal> listValeurGlobaleBeneficiaire = new Dictionary<string, decimal>();
            
            try
            {

                 str = string.Format("" + str + 
                                           " and  do.DATEDOSSIERTPS >= '{0}'  and do.DATEDOSSIERTPS <= '{1}' " +
                                           " group by ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) "
                                           , dateDebut, dateFin);


                SqlConnection con = new SqlConnection(strConnString);
                con.Open();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {

                    foreach(DataRow row in dt.Rows)
                    {
                        string beneficiaire = row["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                        decimal valeurGlogaleCFA = decimal.Parse(row["Total"].ToString());
                        listValeurGlobaleBeneficiaire.Add(beneficiaire, valeurGlogaleCFA);
                    }
                }
                   

            }
            catch (Exception ex)
            {

            }

            return listValeurGlobaleBeneficiaire;

        }


        private void OpenMSWordFileByBrowser()
        {
            Context.Response.Clear();
            FileInfo file = new FileInfo(Server.MapPath("~/GUIDE.docx"));
            Context.Response.ContentType = "Application/msword";
            Context.Response.AppendHeader("Content-Disposition", "inline; filename=" + file.Name);
            Context.Response.AppendHeader("Content-Length", file.Length.ToString());
            Context.Response.WriteFile(file.FullName);
            Context.Response.End();
        }
        public String isBollore()
        {
            String strBollore = "";
            String sqlQuery = "";

            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
                sqlQuery = "select distinct NUMEROINSCRIPTIONABONNE from OPERATEUR where NOMOURAISONSOCIALEOPERATEUR like '%BOLLORE AFRICA LOGISTIC%'";
                SqlCommand sqlCmd = new SqlCommand(sqlQuery, con);
                con.Open();

                using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                {
                    while (sqlReader.Read())
                    {
                        strBollore = strBollore + sqlReader.GetValue(0).ToString() + ",";

                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }


            return strBollore;
        }


        
        //////////////////////////////////////////////////////////////////////////////////////////



    }
}