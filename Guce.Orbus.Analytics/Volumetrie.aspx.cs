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
    public partial class Volumetrie : System.Web.UI.Page
    {
        private String strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private String strNumInscription = "";
        private String strIdPole = "";
        public static DataTable tempDtPays = new DataTable();
        public static DataTable tempDtProduits = new DataTable();
        public static DataTable tempDtBeneficiaire = new DataTable();


        public static string StrConn { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            StrConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            //
            // Set the thread's CurrentCulture the same as CurrentUICulture.
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            ChartSeries.Visible = false;
            ChartLine.Visible = false;
            resultatVolumetrie.Visible = false;

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

            String groupage = GroupageList.Value + "";

            String datePar = DateParList.Value + "";

            String paysProvenance = txtPaysProvenance.Text + "";
            paysProvenance = paysProvenance.Trim();

            String paysOrigine = txtPaysOrigine.Text + "";
            paysOrigine = paysOrigine.Trim();

            String paysDestination = txtPaysDestination.Text + "";
            paysDestination = paysDestination.Trim();

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
                            str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], FORMAT (datedossiertps, 'dd/MM/yyyy') [Chiffres], DATENAME(WEEKDAY,DATEDOSSIERTPS) [Lettres], DATEPART(month,DATEDOSSIERTPS) [Mois], DATEPART(WEEKDAY, do.DATEDOSSIERTPS) [Jour] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS " +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                               " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) ";

                        }
                        else
                        {
                            str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], FORMAT (datedossiertps, 'dd/MM/yyyy') [Chiffres], DATENAME(WEEKDAY,DATEDOSSIERTPS) [Lettres], DATEPART(month,DATEDOSSIERTPS) [Mois], DATEPART(WEEKDAY, do.DATEDOSSIERTPS) [Jour] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS " +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) ";

                           
                        }
                        
                    }
                    else
                    {
                        if (produit != "" || paysOrigine != "")
                        {
                            str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], FORMAT (datedossiertps, 'dd/MM/yyyy') [Chiffres], DATENAME(WEEKDAY,DATEDOSSIERTPS) [Lettres], DATEPART(month,DATEDOSSIERTPS) [Mois], DATEPART(WEEKDAY, do.DATEDOSSIERTPS) [Jour] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS " +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " WHERE 1=1 ";

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                        }
                        else
                        {
                            str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], FORMAT (datedossiertps, 'dd/MM/yyyy') [Chiffres], DATENAME(WEEKDAY,DATEDOSSIERTPS) [Lettres], DATEPART(month,DATEDOSSIERTPS) [Mois], DATEPART(WEEKDAY, do.DATEDOSSIERTPS) [Jour] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS " +
                          " WHERE 1=1 ";

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                        }
                        
                    }

                }
                else if (datePar == "2")
                {
                    if (numIncsriptionClient > 0)
                    {
                        if (produit != "" || paysOrigine != "")
                        {
                            str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], DATEPART(MONTH,datedossiertps) [Chiffres],DATENAME(MONTH, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                               " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) ";

                        }
                        else
                        {
                            str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], DATEPART(MONTH,datedossiertps) [Chiffres],DATENAME(MONTH, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) ";

                        }
                        
                    }
                    else
                    {
                        if (produit != "" || paysOrigine != "")
                        {
                            str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], DATEPART(MONTH,datedossiertps) [Chiffres],DATENAME(MONTH, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " WHERE 1=1 ";

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                        }
                        else
                        {
                            str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], DATEPART(MONTH,datedossiertps) [Chiffres],DATENAME(MONTH, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " WHERE 1=1 ";

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                        }
                        
                    }

                }
                else if (datePar == "3")
                {
                    if (numIncsriptionClient > 0)
                    {
                        if (produit != "" || paysOrigine != "")
                        {
                            str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                               " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) ";


                        }
                        else
                        {
                            str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) ";


                        }
                        
                    }
                    else
                    {
                        if (produit != "" || paysOrigine != "")
                        {
                            str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                                                      " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                                      " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                                      " WHERE 1=1 ";

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                        }
                        else
                        {
                            str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], DATEPART(YEAR,datedossiertps) [Chiffres],DATENAME(YEAR, do.DATEDOSSIERTPS) [Lettres] " +
                                                      " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                                      " WHERE 1=1 ";

                            strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
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
                        str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], fa.PAYSPROVENANCE [Lettres] " +
                                                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                                  " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                                                  " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                               " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) ";


                    }
                    else
                    {
                        str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], fa.PAYSPROVENANCE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) ";

                    }
                    
                }
                else
                {
                    if (produit != "" || paysOrigine != "")
                    {
                        str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], fa.PAYSPROVENANCE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " WHERE 1=1 ";

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                    }
                    else
                    {
                        str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], fa.PAYSPROVENANCE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " WHERE 1=1 ";

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                    } 
                }

            }
            else
            {
                if (numIncsriptionClient > 0)
                {
                    if (produit != "" || paysOrigine != "")
                    {
                        str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], do.NOMOURAISONSOCIALEBENEFICIAIRE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                               " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) ";
                    }
                    else
                    {
                        str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], do.NOMOURAISONSOCIALEBENEFICIAIRE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) " +
                          " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join OPERATEUR op on (do.IDTPSOPERATEUR = op.IDTPSOPERATEUR  or do.TRANSITAIRE = op.IDTPSOPERATEUR) ";

                    }
                    
                }
                else
                {
                    if (produit != "" || paysOrigine != "")
                    {
                        str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], do.NOMOURAISONSOCIALEBENEFICIAIRE [Lettres] " +
                                                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                                  " WHERE 1=1 ";

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                               " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS ";
                    }
                    else
                    {
                        str = " SELECT COUNT(distinct do.NUMERODOSSIERTPS) [Total], do.NOMOURAISONSOCIALEBENEFICIAIRE [Lettres] " +
                          " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                          " WHERE 1=1 ";

                        strGlobalBeneficiaire = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                                               " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS";
                    }
                    
                }

            }


            ///////////////AJOUT DJOSSOU: si banque alors prendre en compte le parametre domiciliation/////////////////////

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
            str = str + strDom ;
            strGlobalBeneficiaire = strGlobalBeneficiaire + " WHERE 1=1 " + strDom ;
            reqPercentBeneficiaire = strGlobalBeneficiaire;

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

            if (paysDestination != "")
            {
                if (paysDestination.Contains(';'))
                {

                    string[] val = paysDestination.Split(';');
                    string strPays = "'" + val[0] + "'";
                    int i = 1;

                    for (i = 1; i < val.Length; i++)
                    {
                        strPays = strPays + ",'" + val[i] + "'";
                    }

                    str = str + " and fa.paysDestination in (" + strPays + ")";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and fa.paysDestination in (" + strPays + ")";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and fa.paysDestination in (" + strPays + ")";

                }
                else
                {
                    str = str + " and fa.paysDestination = '" + paysDestination + "'";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and fa.paysDestination = '" + paysDestination + "'";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and fa.paysDestination = '" + paysDestination + "'";

                }

            }

            if (devise != "-1")
            {
                if (devise.Contains("EUR"))
                {
                    str = str + " and fa.CODEDEVISE like '" + devise + "%'";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and fa.CODEDEVISE like 'EUR%'";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and fa.CODEDEVISE like 'EUR%'";
                }
                else
                {
                    str = str + " and fa.CODEDEVISE = '" + devise + "'";
                    strGlobalBeneficiaire = strGlobalBeneficiaire + " and fa.CODEDEVISE = '" + devise + "'";
                    reqPercentBeneficiaire = reqPercentBeneficiaire + " and fa.CODEDEVISE = '" + devise + "'";
                }
            }

            //if (produit != "")
            //{
            //    str = str + " and co.NUMEROTARIFDOUANE = '" + produit + "'";
            //}


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

            if (verifChamps)
            {

                if (groupage == "1")
                {
                    if (datePar == "1")
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

                        str = str + " GROUP BY  DATEPART(WEEKDAY, do.DATEDOSSIERTPS) ,FORMAT (datedossiertps, 'dd/MM/yyyy'),DATEPART(month,DATEDOSSIERTPS),DATENAME(WEEKDAY,DATEDOSSIERTPS)  " +
                                    " ORDER BY 5,4,2 ASC ";

                    }
                    else if (datePar == "2")
                    {
                        if (idPole > 0)
                        {
                            str = str + " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS =do.NUMERODOSSIERTPS) ";
                        }
                        else if (numIncsriptionClient > 0)
                        {
                            reqPercentBeneficiaire = reqPercentBeneficiaire + " and op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                        }

                        str = str + " GROUP BY DATENAME(MONTH, do.DATEDOSSIERTPS),DATEPART(MONTH,datedossiertps) " +
                                    " ORDER BY DATEPART(MONTH,datedossiertps) ";

                    }
                    else if (datePar == "3")
                    {

                        if (idPole > 0)
                        {
                            str = str + " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS =do.NUMERODOSSIERTPS) ";
                        }
                        else if (numIncsriptionClient > 0)
                        {
                            reqPercentBeneficiaire = reqPercentBeneficiaire + " and op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                        }

                        str = str + " GROUP BY DATENAME(YEAR, do.DATEDOSSIERTPS),DATEPART(YEAR,datedossiertps) " +
                                    " ORDER BY DATEPART(YEAR,datedossiertps) ";

                    }
                }
                else if (groupage == "2")
                {
                    if (idPole > 0)
                    {
                        str = str + " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS =do.NUMERODOSSIERTPS) ";
                    }
                    else if (numIncsriptionClient > 0)
                    {
                        reqPercentBeneficiaire = reqPercentBeneficiaire + " and op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                    }

                    str = str + " GROUP BY fa.PAYSPROVENANCE " +
                                " ORDER BY COUNT(*) DESC ";

                }
                else
                {
                    if (idPole > 0)
                    {
                        str = str + " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS =do.NUMERODOSSIERTPS) ";
                    }
                    else if (numIncsriptionClient > 0)
                    {
                        reqPercentBeneficiaire = reqPercentBeneficiaire + " and op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
                    }

                    str = str + " GROUP BY do.NOMOURAISONSOCIALEBENEFICIAIRE " +
                                " ORDER BY COUNT(*) DESC ";

                }

                ////////////////AJOUT DJOSSOU: Pour recadrer la requete et pour l'exportation/////////
                //////////////////////////////////////////////////////////////////////////////////////

                if ((natureOperation == "2") || (natureOperation == "4"))
                {
                    str = str.Replace("fa.PAYSPROVENANCE", "fa.PAYSDESTINATION");

                }

                /////////////////////////////////////////////////////////////////////////////////////

                try
                {

                    int totalDossiers = 0;
                    DataTable dtDepart = new DataTable();
                    DataTable dtFinal = new DataTable();
                    SqlCommand cmd = new SqlCommand(str, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtDepart);
                    var maxLignes = 0;
                    //storing total rows count to loop on each Record  
                    string[] x = new string[dtDepart.Rows.Count];
                    int[] y = new int[dtDepart.Rows.Count];
                    double[] ord = new double[dtDepart.Rows.Count];

                    if (dtDepart.Rows.Count > 0)
                    {
                        ChartSeries.Visible = true;
                        ChartLine.Visible = true;
                        resultatVolumetrie.Visible = true;

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
                                                 Total = grp.Select(r => r.Field<int>("Total")).ToList()
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
                                    int totalParJours = 0;

                                    foreach (var nbre in resultat.Total)
                                    {
                                        int YVal = (int)nbre;
                                        dtFinal.Rows[b][a] = nbre;
                                        ChartSeries.Series[resultat.Jours.First().ToString().ToUpper() + resultat.Jours.Substring(1)].Points.AddXY("Semaine " + (int)(b + 1), YVal);
                                        totalDossiers = totalDossiers + nbre;
                                        totalParJours = totalParJours + nbre;
                                        dtFinal.Rows[b][a] = nbre + "#" + resultat.DateDossier[b].ToString();
                                        dtFinal.Rows[maxLignes][a] = totalParJours;
                                        y[b] = nbre;
                                        ++b;

                                    }

                                    ord[a] = Convert.ToDouble(dtFinal.Rows[maxLignes][a]);
                                    ++a;
                                }

                                x = x.Where(c => c != null).ToArray();
                                ord = ord.Where(c => c != 0).ToArray();


                            }
                            else if (datePar == "3")
                            {
                                for (int i = 0; i < dtDepart.Rows.Count; i++)
                                {
                                    dtFinal.Columns.Add(dtDepart.Rows[i]["Chiffres"].ToString().ToUpper(), typeof(string));
                                    //storing Values for X axis  
                                    x[i] = dtDepart.Rows[i]["Chiffres"].ToString();
                                }

                                DataRow newRow = dtFinal.NewRow();

                                for (int i = 0; i < dtDepart.Rows.Count; i++)
                                {
                                    newRow[i] = dtDepart.Rows[i]["Total"].ToString();
                                    //storing values for Y Axis  
                                    y[i] = Convert.ToInt32(dtDepart.Rows[i]["Total"]);
                                    totalDossiers = totalDossiers + int.Parse(dtDepart.Rows[i]["Total"].ToString());
                                }

                                dtFinal.Rows.Add(newRow);

                                x = x.Where(c => c != null).ToArray();
                                y = y.Where(c => c != 0).ToArray();

                                //binding chart control  
                                ChartSeries.Series[0].Points.DataBindXY(x, y);
                                ChartSeries.Series[0].ChartType = SeriesChartType.Column;

                            }
                            else
                            {
                                for (int i = 0; i < dtDepart.Rows.Count; i++)
                                {
                                    String courant = dtDepart.Rows[i]["Lettres"].ToString();
                                    dtFinal.Columns.Add(courant.Substring(0, 1).ToUpper() + courant.Substring(1), typeof(string));
                                    //storing Values for X axis  
                                    x[i] = courant.Substring(0, 1).ToUpper() + courant.Substring(1);
                                }

                                DataRow newRow = dtFinal.NewRow();

                                for (int i = 0; i < dtDepart.Rows.Count; i++)
                                {
                                    newRow[i] = dtDepart.Rows[i]["Total"].ToString();
                                    //storing values for Y Axis  
                                    y[i] = Convert.ToInt32(dtDepart.Rows[i]["Total"]);
                                    totalDossiers = totalDossiers + int.Parse(dtDepart.Rows[i]["Total"].ToString());
                                }

                                dtFinal.Rows.Add(newRow);

                                x = x.Where(c => c != null).ToArray();
                                y = y.Where(c => c != 0).ToArray();

                                //binding chart control  
                                ChartSeries.Series[0].Points.DataBindXY(x, y);
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

                            dtFinal.Columns.Add(" Nombre de Dossiers ", typeof(int));
                            dtFinal.Columns.Add(" Part Pays ", typeof(string));

                            int totalDossiersAutres = 0;

                            for (int i = 0; i < dtDepart.Rows.Count; i++)
                            {
                                String courant = dtDepart.Rows[i]["Lettres"].ToString();

                                dtFinal.Rows.Add(courant, dtDepart.Rows[i]["Total"], null);

                                totalDossiers = totalDossiers + int.Parse(dtDepart.Rows[i]["Total"].ToString());

                                if (i < 5)
                                {
                                    x[i] = courant;
                                    y[i] = Convert.ToInt32(dtDepart.Rows[i]["Total"]);
                                }
                                else
                                {
                                    x[5] = "Autres";
                                    y[5] = totalDossiersAutres = totalDossiersAutres + int.Parse(dtDepart.Rows[i]["Total"].ToString());
                                }

                            }

                            x = x.Where(c => c != null).ToArray();
                            y = y.Where(c => c != 0).ToArray();

                            ChartSeries.Series[0].Points.DataBindXY(x, y);
                            ChartSeries.Series[0].ChartType = SeriesChartType.Column;

                        }
                        else
                        {
                            dtFinal.Columns.Add(" Nom ou Raison Sociale Bénéficiaire ", typeof(string));
                            dtFinal.Columns.Add(" Nombre de Dossiers ", typeof(int));
                            dtFinal.Columns.Add(" Part Client ", typeof(string));

                            int totalDossiersAutres = 0;

                            for (int i = 0; i < dtDepart.Rows.Count; i++)
                            {
                                String courant = dtDepart.Rows[i]["Lettres"].ToString();

                                dtFinal.Rows.Add(courant, dtDepart.Rows[i]["Total"], null);

                                totalDossiers = totalDossiers + int.Parse(dtDepart.Rows[i]["Total"].ToString());

                                if (i < 5)
                                {
                                    x[i] = courant;
                                    y[i] = Convert.ToInt32(dtDepart.Rows[i]["Total"]);
                                }
                                else
                                {
                                    x[5] = "Autres";
                                    y[5] = totalDossiersAutres = totalDossiersAutres + int.Parse(dtDepart.Rows[i]["Total"].ToString());
                                }

                            }

                            x = x.Where(c => c != null).ToArray();
                            y = y.Where(c => c != 0).ToArray();



                            ChartSeries.Series[0].Points.DataBindXY(x, y);
                            ChartSeries.Series[0].ChartType = SeriesChartType.Column;

                        }

                        if (totalDossiers > 0)
                        {
                            lblTotal.Text = totalDossiers.ToString();
                        }
                        else
                        {
                            lblTotal.Text = "0";
                        }

                        lblTotal.Font.Bold = true;
                        lblTotal.ForeColor = Color.OrangeRed;


                        double pourcentage = 0;

                        if (groupage == "1")
                        {
                            dtFinal.Rows.Add();

                            for (int i = 0; i < dtFinal.Columns.Count; i++)
                            {
                                int courant = int.Parse(dtFinal.Rows[dtFinal.Rows.Count - 2][i].ToString());
                                pourcentage = (Math.Abs((double)courant / totalDossiers)) * 100;
                                dtFinal.Rows[dtFinal.Rows.Count - 1][i] = Math.Round(pourcentage, 2) + " % ";
                                
                            }
                            

                            VolumetrieListeGridView.DataSource = dtFinal;


                            //////AJOUT DJOSSOU: Changement de la structure de la datasource du gridview//////////
                            //////////////////////////////////////////////////////////////////////////////////////

                            if (datePar == "2")
                            {
                                DataTable dtFinal2 = ChangeDataTableVolumeDirection(dtFinal);
                                VolumetrieListeGridView.DataSource = dtFinal2;
                            }

                            /////////////////////////////////////////////////////////////////////////////////////
                            //////////////////////////////////////////////////////////////////////////////////////


                            VolumetrieListeGridView.DataBind();

                            /*
                            VolumetrieListeGridView.Rows[VolumetrieListeGridView.Rows.Count - 2].Font.Bold = true;
                            VolumetrieListeGridView.Rows[VolumetrieListeGridView.Rows.Count - 2].ForeColor = Color.OrangeRed;

                            VolumetrieListeGridView.Rows[VolumetrieListeGridView.Rows.Count - 1].Font.Bold = true;
                            VolumetrieListeGridView.Rows[VolumetrieListeGridView.Rows.Count - 1].ForeColor = Color.Olive;
                            */

                        }
                        else
                        {
                            for (int i = 0; i < dtFinal.Rows.Count; i++)
                            {
                                int courant = int.Parse(dtFinal.Rows[i][dtFinal.Columns.Count - 2].ToString());
                                pourcentage = (Math.Abs((double)courant / totalDossiers)) * 100;
                                dtFinal.Rows[i][dtFinal.Columns.Count - 1] = Math.Round(pourcentage, 2) + " % ";
                                
                            }

                            
                            VolumetrieListeGridView.DataSource = dtFinal;

                            //////AJOUT DJOSSOU: Changement de la structure de la datasource du gridview//////////
                            //////////////////////////////////////////////////////////////////////////////////////

                            if (groupage == "3")
                            {
                                string dateDeb = debut;
                                string dateFin = Convert.ToDateTime(fin).AddDays(1).ToString();

                                Dictionary<string, int> listVolumePercentBeneficiaire = GetListVolumeGlobaleBeneficiaire(reqPercentBeneficiaire, dateDeb, dateFin);

                                double sommeBeneficiaire = 0;

                                sommeBeneficiaire = listVolumePercentBeneficiaire.Sum(item => item.Value);

                                for (int i = 0; i < dtFinal.Rows.Count; i++)
                                {
                                    string nomBeneficiaire = dtFinal.Rows[i][dtFinal.Columns.Count - 3].ToString();
                                    double courantActuel = listVolumePercentBeneficiaire.Where(k => k.Key == nomBeneficiaire).FirstOrDefault().Value;
                                    pourcentage = (Math.Abs((double)courantActuel / sommeBeneficiaire)) * 100;
                                    dtFinal.Rows[i][dtFinal.Columns.Count - 1] = Math.Round(pourcentage, 2) + " % ";
                                }

                                VolumetrieListeGridView.DataSource = dtFinal;

                                DataTable dtFinal3 = new DataTable();
                                dtFinal3.Columns.Add(" Nom ou Raison Sociale Bénéficiaire ", typeof(string));
                                dtFinal3.Columns.Add(" Nombre de Dossiers ", typeof(int));
                                dtFinal3.Columns.Add(" Part Client ", typeof(string));
                                dtFinal3.Columns.Add(" Global Orbus", typeof(string));
                               
                                Dictionary<string, int> listVolumeGlobaleBeneficiaire = GetListVolumeGlobaleBeneficiaire(strGlobalBeneficiaire,dateDeb, dateFin);
                          
                                for (int i = 0; i < dtFinal.Rows.Count; i++)
                                {
                                    try
                                    {
                                        dtFinal3.Rows.Add();

                                        decimal pourcentageGlobale = 0;
                                        int volumeGobale = 0;
                                        string nomBeneficiaire = dtFinal.Rows[i][0].ToString().Trim();

                                        volumeGobale = listVolumeGlobaleBeneficiaire.Where(k => k.Key == nomBeneficiaire).FirstOrDefault().Value;
                                        int courant2 = int.Parse(dtFinal.Rows[i][dtFinal.Columns.Count - 2].ToString());

                                        if (volumeGobale > 0)
                                            pourcentageGlobale = (Math.Abs((decimal)courant2 / volumeGobale)) * 100;
                                  
                                        dtFinal3.Rows[i][0] = dtFinal.Rows[i][0].ToString();
                                        dtFinal3.Rows[i][1] = dtFinal.Rows[i][1].ToString();
                                        dtFinal3.Rows[i][2] = dtFinal.Rows[i][2].ToString();
                                        dtFinal3.Rows[i][3] = Math.Round(pourcentageGlobale, 2) + " % ";

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }

                                VolumetrieListeGridView.DataSource = dtFinal3;
                            }

                            /////////////////////////////////////////////////////////////////////////////////////
                            //////////////////////////////////////////////////////////////////////////////////////


                            VolumetrieListeGridView.DataBind();

                        }

                        ChartSeries.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                        ChartSeries.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;



                        if (groupage == "1")
                        {

                            ChartLine.ChartAreas["ChartArea1"].AxisX.Maximum = x.Count();
                            
                            ChartLine.ChartAreas[0].AxisY.Title = "Nombre Dossiers";
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

                            //ChartSeries.Series[0].IsVisibleInLegend = true;


                            if (datePar == "1")
                            {

                                ChartLine.Series[0].Points.DataBindXY(x, ord);
                                ChartLine.ChartAreas[0].AxisX.Title = "Jours";
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

                                ChartLine.Series[0].Points.DataBindXY(x, y);

                                //binding chart control
                                ChartSeries.Series[0].Points.DataBindXY(x, y);
                                ChartSeries.Series[0].ChartType = SeriesChartType.Column;
                                ChartSeries.Series[0].IsValueShownAsLabel = true;

                                Color[] myPalette = { Color.Green, Color.LightGreen, Color.Coral, Color.LightBlue, Color.DeepSkyBlue, Color.Goldenrod, Color.Aquamarine, Color.Aqua, Color.MediumOrchid, Color.Lavender, Color.Thistle, Color.Beige };

                                ChartSeries.Palette = ChartColorPalette.None;
                                ChartSeries.PaletteCustomColors = myPalette;

                                for (int i = 0; i < x.Count(); i++)
                                {
                                    ChartSeries.Series[0].Points[i].BackGradientStyle = GradientStyle.Center;
                                    ChartSeries.Series[0].Points[i].Color = myPalette[i];
                                    ChartSeries.Series[0].Points[i].BackSecondaryColor = ControlPaint.Light(myPalette[i]);

                                    //ChartSeries.Series[0].Points[i].LegendText = "kkk";
                                }

                                //ChartSeries.Series["Default"].Label = "#PERCENT";
                                //ChartSeries.Series[0].LegendText = "#VALX";
                                //ChartSeries.Series[0].LegendText = "#AXISLABEL";

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

                            ChartSeries.Legends[0].Enabled = true;
                            //ChartSeries.Series[0].Label = "#PERCENT{P2}";
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
                        VolumetrieListeGridView.DataSource = dtDepart;
                        VolumetrieListeGridView.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }

            }
        }


        protected void VolumetrieListeGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = VolumetrieListeGridView.SelectedRow;
        }

        protected void VolumetrieListeGridView_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            VolumetrieListeGridView.PageIndex = e.NewPageIndex;
            ChargerDonnees();
        }



        protected void VolumetrieListeGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            String groupage = GroupageList.Value + "";

            if (groupage == "1")
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        string[] a = cell.Text.Split('#');
                        if (cell.Text.Contains('#'))
                        {
                            var str = a[0] + "#" + "<span style='color: rgb(119,156,200);'>" + a[1] + "</span>";
                            cell.Text = str.Replace("#", "<br>");
                        }
                    }
                }
            }
            else
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

        protected void Annuler_Click(object sender, EventArgs e)
        {
            Drop_Operation_Rech.Value = "-1";

            DateParList.Value = "1";

            GroupageList.Value = "1";

            txtPaysProvenance.Text = "";

            txtPaysOrigine.Text = "";

            txtPaysDestination.Text = "";

            txtBeneficiaire.Text = "";

            Text_DebutPeriode_Rech.Text = "";

            Text_FinPeriode_Rech.Text = "";

            DeviseList.Value = "-1";

            txtProduit.Text = "";

            lblTotal.Text = "";

            VolumetrieListeGridView.DataSource = null;
            VolumetrieListeGridView.DataBind();

        }


        //protected void BeneficiaireGrid_RowCreated(object sender, GridViewRowEventArgs e)
        //{
        //    //if (e.Row.RowType == DataControlRowType.DataRow)
        //    //{
        //    //    e.Row.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink((System.Web.UI.Control)sender, "Select$" + e.Row.RowIndex);
        //    //}
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.backgroundColor='yellow'");
        //        e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='white'");
        //    }
        //}  

        //protected void BeneficiaireGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    //if (e.Row.RowType == DataControlRowType.DataRow)
        //    //{
        //    //    e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(BeneficiaireGrid, "Select$" + e.Row.RowIndex);
        //    //    e.Row.Attributes["style"] = "cursor:pointer";
        //    //}

        //    if (e.Row.RowType == DataControlRowType.Header)
        //    {
        //        //add the thead and tbody section programatically
        //        e.Row.TableSection = TableRowSection.TableHeader;
        //    }
        //}

        //protected void BeneficiaireGrid_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    int index = BeneficiaireGrid.SelectedRow.RowIndex;
        //    string name = BeneficiaireGrid.SelectedRow.Cells[0].Text;
        //    string country = BeneficiaireGrid.SelectedRow.Cells[1].Text;
        //    string message = "Row Index: " + index + "\\nName: " + name + "\\nCountry: " + country;
        //    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + message + "');", true);
        //    txtBeneficiaire.Text = name;
        //}

        //protected void BeneficiaireGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='aquamarine';";
        //        e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='white';";
        //        e.Row.ToolTip = "Click last column for selecting this row.";
        //    }

        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(BeneficiaireGrid, "Select$" + e.Row.RowIndex);
        //        e.Row.ToolTip = "Click to select this row.";
        //    }
        //}




        protected void VisualiserExcel_Click(object sender, EventArgs e)
        {
            Response.ClearContent();
            Response.AppendHeader("content-disposition", "attachment;filename=VolumetrieExtract.xls");
            Response.ContentType = "application/vnd.ms-excel";
            String groupage = GroupageList.Value + "";

            StringWriter stringwriter = new StringWriter();
            HtmlTextWriter htmtextwriter = new HtmlTextWriter(stringwriter);

            VolumetrieListeGridView.HeaderRow.Style.Add("background-color", "#ffffff");

            foreach (TableCell tableCell in VolumetrieListeGridView.HeaderRow.Cells)
            {
                tableCell.Style["background-color"] = "#ffffff";
            }

            foreach (GridViewRow gridviewrow in VolumetrieListeGridView.Rows)
            {
                gridviewrow.BackColor = System.Drawing.Color.White;
                foreach (TableCell gridviewrowtablecell in gridviewrow.Cells)
                {
                    gridviewrowtablecell.Style["background-color"] = "#ffffff";
                }
            }

            VolumetrieListeGridView.AllowPaging = false;
            this.ChargerDonnees();
            VolumetrieListeGridView.RenderControl(htmtextwriter);
            Response.Write(stringwriter.ToString());
            Response.End();


        }

        protected void CopierGraphe_Click(object sender, EventArgs e)
        {
            // Créer un memory stream 
            MemoryStream stream = new MemoryStream();

            // Entregistrer l'image du chart dans le  stream 
            ChartLine.SaveImage(stream, ChartImageFormat.Png);

            // Créer un BitMap et le remplir avec le stream  
            Bitmap bmp = new Bitmap(stream);

            //System.Drawing.Image img = new Bitmap(this.ChartLine.Width, this.ChartLine.Height);

            //this.ChartLine.Draw(img);

            //Clipboard.SetDataObject(img);

            // Mettre le bitmap dans le  clipboard   
            Clipboard.SetImage(bmp);

        }

        public override void VerifyRenderingInServerForm(System.Web.UI.Control control) { }

        //protected void btnExport_Click(object sender, EventArgs e)
        //{
        //    DataSet ds = new DataSet();
        //    ds = GetDataForGridView();
        //    string attachment = attachment = "attachment; filename=MyExcelSheetName_" + DateTime.Now.ToString() + ".xls";
        //    if (ds.Tables.Count > 0)
        //    {
        //        DataTable dt = ds.Tables[0];
        //        Response.ClearContent();
        //        Response.AddHeader("content-disposition", attachment);
        //        Response.ContentType = "application/vnd.ms-excel";
        //        string tab = "";
        //        //This will give you the number of columns present in gridview
        //        for (int coulumns = 0; coulumns < GridView1.Columns.Count; coulumns++)
        //        {
        //            Response.Write(tab + GridView1.Columns[coulumns].HeaderText);
        //            tab = "\t";
        //        }
        //        Response.Write("\n");

        //        //Here we will visit each row of datatable and bind the corresponding column data.
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            tab = "";
        //            Response.Write(tab + dr["ColumnName1"].ToString());
        //            tab = "\t";
        //            Response.Write(tab + dr["ColumnName2"].ToString());
        //            tab = "\t";
        //            Response.Write(tab + dr["ColumnName3"].ToString());
        //            tab = "\t";
        //            Response.Write("\n");
        //        }
        //        Response.End();
        //    }
        //}


        ///////////////////////////AJOUT DJOSSOU: Récuperation du type pole//////////////////////
                               
        private DataTable ChangeDataTableVolumeDirection(DataTable dtFinal)
        {

            DataTable dtFinal2 = new DataTable();

            try
            {

                dtFinal2.Columns.Add("Mois", typeof(string));
                dtFinal2.Columns.Add("Nombre de dossier", typeof(string));
                dtFinal2.Columns.Add("Part Mensuelle", typeof(string));

                //for (int i = 5; i < 10; i++)
                for (int i = 0; i < dtFinal.Columns.Count; i++)
                {
                    DataRow newRow2 = dtFinal2.NewRow();
                    string mois = dtFinal.Columns[i].ColumnName;
                    newRow2[0] = mois.Replace("é", "e").Replace("û", "u");
                    newRow2[1] = dtFinal.Rows[0][mois].ToString();
                    newRow2[2] = dtFinal.Rows[1][mois].ToString();

                    dtFinal2.Rows.Add(newRow2);
                }

            }
            catch (Exception ex)
            { }

            return dtFinal2;

        }

        private Dictionary<string, int> GetListVolumeGlobaleBeneficiaire(string str, string dateDebut, string dateFin)
        {
            Dictionary<string, int> listVolumeGlobaleBeneficiaire = new Dictionary<string, int>();

            try
            {
                str = string.Format("" + str +
                                           " and  do.DATEDOSSIERTPS >= '{0}'  and do.DATEDOSSIERTPS <= '{1}' " +
                                           " group by ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) "
                                           , dateDebut, dateFin);

                //String str = string.Format(" SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) NOMOURAISONSOCIALEBENEFICIAIRE,COUNT(distinct do.NUMERODOSSIERTPS) Total " +
                //                           " FROM FACTURE fa " +
                //                           " inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS " +
                //                           " inner join CONTENIR co on co.NUMERODOSSIERTPS =do.NUMERODOSSIERTPS " +
                //                           " WHERE  do.DATEDOSSIERTPS >= '{0}'  and do.DATEDOSSIERTPS <= '{1}' " +
                //                           " group by ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) "
                //                           , dateDebut, dateFin);

                SqlConnection con = new SqlConnection(strConnString);
                con.Open();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string beneficiaire = row["NOMOURAISONSOCIALEBENEFICIAIRE"].ToString();
                        int volumeGlogale = int.Parse(row["Total"].ToString());
                        listVolumeGlobaleBeneficiaire.Add(beneficiaire, volumeGlogale);
                    }
                }


            }
            catch (Exception ex)
            {

            }

            return listVolumeGlobaleBeneficiaire;

        }


        //////////////////////////////////////////////////////////////////////////////////////////


    }
}