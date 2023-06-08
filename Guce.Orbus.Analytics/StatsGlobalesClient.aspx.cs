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
    public partial class StatsGlobalesClient : System.Web.UI.Page
    {
        private String strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private String strNumInscription = "";
        private String strIdPole = "";
        private String str = "";
        public static string StrConn { get; set; }
        public static DataTable tempDtProduits = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            StrConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            //ChargerListeDevise();

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");

            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            resultatStatsProd.Visible = false;

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
                    ChargerTempProduits();
                    tempDtProduits = (DataTable)TempGridProduits.DataSource;

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

            String debut = Text_DebutPeriode_Rech.Text;
            debut = debut.Trim();

            String fin = Text_FinPeriode_Rech.Text;
            fin = fin.Trim();

            String produit = txtProduit.Text;

            String strParam = "";

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

            str = " SELECT ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) [Client], co.NUMEROTARIFDOUANE [Produit], ta.LIBELLETARIFDOUANE [DescProduit],  SUM(co.QUANTITEMESURE) [Quantite], co.UNITEMESURE [Mesure], SUM(fa.VALEURTOTALECFA) [Total] , COUNT(distinct do.NUMERODOSSIERTPS) [Dossier] " +
                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE ";
            if (numIncsriptionClient > 0)
            {
                str = str + " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR " +
                    " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient;
            }
            else if (idPole > 0)
            {
                str = str + " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS =do.NUMERODOSSIERTPS) ";
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

                    strParam = strParam + " and co.NUMEROTARIFDOUANE in (" + strProduit + ")";

                }
                else
                {
                    strParam = strParam + " and co.NUMEROTARIFDOUANE = '" + produit + "'";
                }

            }

            if (natureOperation != "-1")
            {
                if (natureOperation == "1")
                {
                    strParam = strParam + " and do.IMPORTATIONOUEXPORTATION = 'I' ";
                }
                else if (natureOperation == "2")
                {
                    strParam = strParam + " and do.IMPORTATIONOUEXPORTATION = 'E' ";
                }
                else if (natureOperation == "3")
                {
                    strParam = strParam + " and do.IMPORTATIONOUEXPORTATION = 'S' ";
                }
                else if (natureOperation == "4")
                {
                    strParam = strParam + " and do.IMPORTATIONOUEXPORTATION = 'R' ";
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

                strParam = strParam + " and do.DATEDOSSIERTPS  >= '" + debut + "' ";
                //convert(datetime,'" + debut + "',103)";
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
                strParam = strParam + " and do.DATEDOSSIERTPS  <= '" + dateFin.ToString() + "' ";
                //str = str + " and do.DATEDOSSIERTPS  <= '" + fin + "' ";
                //convert(datetime,'" + fin + "',103)";
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

            if (verifChamps)
            {
                str = str + strParam + " GROUP BY do.NOMOURAISONSOCIALEBENEFICIAIRE , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , co.UNITEMESURE " +
                                " ORDER BY do.NOMOURAISONSOCIALEBENEFICIAIRE , co.NUMEROTARIFDOUANE , ta.LIBELLETARIFDOUANE , co.UNITEMESURE ";
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
            SqlCommand cmd2 = new SqlCommand();
            try
            {
                if (produit != "")
                {
                    using (cmd1 = new SqlCommand(str, con))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd1);
                        da.Fill(dtDepart);

                        if (dtDepart.Rows.Count > 0)
                        {
                            resultatStatsProd.Visible = true;

                            var query = (from row in dtDepart.AsEnumerable()
                                         group row by new
                                         {
                                             CodeTarif = row.Field<string>("Produit")
                                         } into grp
                                         select new
                                         {
                                             Produit = grp.Key.CodeTarif.ToString(),
                                             Count = grp.Count(),
                                             Beneficiaire = grp.Select(i => new { client = i.Field<string>("Client"), dossier = i.Field<int>("Dossier"), uniteMesure = i.Field<string>("Mesure") }).Distinct().OrderBy(i => i.client).ToList()
                                             //Beneficiaire = grp.Select(i => new { client = i.Field<string>("Client") , dossier = i.Field<int>("Dossier"), uniteMesure = i.Field<string>("Mesure"), qte = i.Field<decimal>("Quantite"), valeurTotale = i.Field<decimal>("Total") }).Distinct().OrderBy(i => i.uniteMesure).ToList()
                                         }).ToList();

                            maxLignes = query.Count();

                            var uniqueProduit = (from data in dtDepart.AsEnumerable()
                                                 select new
                                                 {
                                                     client = data.Field<string>("Client"),
                                                     dossier = data.Field<int>("Dossier"),
                                                     uniteMesure = data.Field<string>("Mesure")
                                                 }).Distinct().OrderBy(i => i.client).ToList();

                            string[] indicateurProduit = new string[uniqueProduit.Count()];

                            dtFinal.Columns.Add("Produit", typeof(string));

                            dtFinal.Columns.Add("DPI", typeof(string));

                            dtFinal.Columns.Add("Client", typeof(string));

                            dtFinal.Columns.Add("Mesure", typeof(string));

                            dtFinal.Columns.Add("Quantité", typeof(string));

                            dtFinal.Columns.Add("Valeur", typeof(string));

                            int numeroProduit = 0;

                            foreach (var resultat in query)
                            {
                                int totalClientParProduit = (from product in dtDepart.AsEnumerable()
                                                             where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                             select product.Field<string>("Client")).Distinct().Count();

                                var listeClientParProduit = (from product in dtDepart.AsEnumerable()
                                                             where product.Field<string>("Produit") == resultat.Produit.ToString()
                                                             select product.Field<string>("Client")).Distinct().ToList();

                                string[] indicateurClient = new string[totalClientParProduit];

                                int q = 0;

                                foreach (var unique in listeClientParProduit)
                                {
                                    indicateurClient[q] = unique.ToString();
                                    ++q;
                                }

                                for (int z = 0; z < totalClientParProduit; z++)
                                {
                                    dtFinal.Rows.Add();
                                }

                                int PositionProduit = Array.FindIndex(indicateurProduit, m => m == resultat.Produit.ToString());

                                foreach (var courant in resultat.Beneficiaire)
                                {

                                    int PositionClient = Array.FindIndex(indicateurClient, m => m == courant.client.ToString());

                                    int LigneReelle = PositionClient + numeroProduit;

                                    var dataRow = (from product in dtDepart.AsEnumerable()
                                                   where product.Field<string>("Produit") == resultat.Produit.ToString() && product.Field<string>("Client") == courant.client && product.Field<int>("Dossier") == courant.dossier && product.Field<string>("Mesure") == courant.uniteMesure
                                                   select product).ToList();

                                    foreach (var pointage in dataRow)
                                    {
                                        double qte = 0;

                                        double valeur = 0;

                                        if (pointage.Field<double?>("Quantite").ToString() != null && pointage.Field<double?>("Quantite").ToString() != "" && pointage.Field<double?>("Quantite").ToString() != "0")
                                        {
                                            qte = double.Parse(pointage.Field<double?>("Quantite").ToString());

                                            if (pointage.Field<double?>("Total").ToString() != null && pointage.Field<double?>("Total").ToString() != "" && pointage.Field<double?>("Total").ToString() != "0")
                                            {
                                                valeur = double.Parse(pointage.Field<double?>("Total").ToString());
                                            }

                                            dtFinal.Rows[LigneReelle][0] = pointage.Field<string>("Produit").ToString();
                                            dtFinal.Rows[LigneReelle][1] = pointage.Field<int>("Dossier").ToString();
                                            dtFinal.Rows[LigneReelle][2] = pointage.Field<string>("Client").ToString();
                                            dtFinal.Rows[LigneReelle][3] = pointage.Field<string>("Mesure").ToString();
                                            dtFinal.Rows[LigneReelle][4] = qte;
                                            dtFinal.Rows[LigneReelle][5] = valeur;
                                            totalValeurCFA = totalValeurCFA + Convert.ToDecimal(valeur);
                                        }


                                    }

                                }
                                numeroProduit = numeroProduit + totalClientParProduit;
                            }

                            for (int i = 0; i < dtFinal.Rows.Count; i++)
                            {
                                if (dtFinal.Rows[i][dtFinal.Columns.Count - 1].ToString() != "")
                                {
                                    decimal val = Convert.ToDecimal(dtFinal.Rows[i][dtFinal.Columns.Count - 1].ToString());
                                    dtFinal.Rows[i][dtFinal.Columns.Count - 1] = val.ToString("n", f).Substring(0, val.ToString("n", f).LastIndexOf("."));
                                }
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

                lblTotal.Font.Bold = true;
                lblTotal.ForeColor = Color.Green;

                String strGlobalOrbus = "";

                if (numIncsriptionClient > 0)
                {
                    strGlobalOrbus = " SELECT SUM(co.VALEURCFA) [Total] " +
                              " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                              " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                              " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                              " inner join OPERATEUR op on do.IDTPSOPERATEUR = op.IDTPSOPERATEUR " +
                              " WHERE op.NUMEROINSCRIPTIONABONNE = " + numIncsriptionClient + " " + strParam;

                }
                else if (idPole > 0)
                {
                    strGlobalOrbus = " SELECT SUM(co.VALEURCFA) [Total] " +
                                  " FROM FACTURE  fa inner join DOSSIERTPS do on do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS" +
                                  " inner join CONTENIR co on co.NUMERODOSSIERTPS=do.NUMERODOSSIERTPS " +
                                  " inner join TARIFDOUANE ta on co.NUMEROTARIFDOUANE=ta.NUMEROTARIFDOUANE " +
                                  " and exists(select * from JOINDRE_" + idPole + " jo where jo.NUMERODOSSIERTPS =do.NUMERODOSSIERTPS) " + strParam;

                }


                using (cmd2 = new SqlCommand(strGlobalOrbus, con))
                {
                    SqlDataReader sqlReader = cmd2.ExecuteReader();

                    if (sqlReader.Read())
                    {
                        decimal courant = 0;

                        if (!string.IsNullOrEmpty(sqlReader.GetValue(0).ToString()))
                        {
                            decimal.Parse(sqlReader.GetValue(0).ToString());

                            courant = courant + decimal.Parse(sqlReader.GetValue(0).ToString());

                            decimal pourcentage = (Math.Abs((decimal)courant / totalValeurCFA)) * 100;

                            lblTransactions.Text = courant.ToString("n", f).Substring(0, courant.ToString("n", f).LastIndexOf("."));

                            lblPourcentagePartMarche.Text = "  " + Math.Round(pourcentage, 2) + " % ";
                        }

                    }
                    else
                    {
                        lblTransactions.Text = " 0 % ";
                    }

                    lblTransactions.Font.Bold = true;
                    lblTransactions.ForeColor = Color.Green;

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
                cmd2.Dispose();
                con.Dispose();
            }

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

        protected void ProduitSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = ProduitSearch.SelectedRow;
        }

        protected void ProduitSearch_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            ProduitSearch.PageIndex = e.NewPageIndex;
            BindColumnProduits();
        }

        private void BindColumnProduits()
        {

            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[2] { new DataColumn("codeProduit"), new DataColumn("designation") });
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
        protected void ListeGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gr = ListeGrid.SelectedRow;
        }

        protected void ListeGrid_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            ListeGrid.PageIndex = e.NewPageIndex;
            ChargerDonnees();
        }


        protected void Annuler_Click(object sender, EventArgs e)
        {
            Drop_Operation_Rech.Value = "-1";

            Text_DebutPeriode_Rech.Text = "";

            Text_FinPeriode_Rech.Text = "";

            Request.Form["txtProduit"] = "";

            ListeGrid.DataSource = null;
            ListeGrid.DataBind();

            //lblTotal.Text = "";
            //lblTransactions.Text = "";
        }

        public override void VerifyRenderingInServerForm(System.Web.UI.Control control) { }

    }
}