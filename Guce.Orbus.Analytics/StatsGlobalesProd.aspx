<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="StatsGlobalesProd.aspx.cs" Inherits="Guce.Orbus.Analytics.StatsGlobalesProd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">

        $(document).ready(function () {

            $("#btnProdCocherTout").on('click', function () {

                var btn_text = $('#btnProdCocherTout').text();

                var strProduit = $("#<%=txtProduit.ClientID %>").val();

                var arrData = [];

                $("#<%=ProduitSearch.ClientID %>  tr").each(function () {

                    var courant = $(this).find("td:eq(1)").text().trim();

                    if (btn_text == 'Cocher Tout') {

                        $htmlImg = '<a href="#"><img src="images/check.png" title="Supprimer" /></a>';

                        $(this).find("td:eq(0)").html($htmlImg);

                        if (strProduit != '') {
                            if (!strProduit.includes(courant)) {
                                strProduit = strProduit + ";" + courant;
                            }
                        }
                        else {
                            strProduit = courant;
                        }

                        $("#btnProdCocherTout").html("Décocher Tout");

                    }
                    else {
                        $htmlImg = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';

                        $(this).find("td:eq(0)").html($htmlImg);

                        if (strProduit != '') {
                            if (strProduit.includes(";" + courant)) {
                                strProduit = strProduit.replace(";" + courant, '');
                            }
                            if (strProduit.includes(courant + ";")) {
                                strProduit = strProduit.replace(courant + ";", '');
                            }
                            else if (strProduit.includes(courant)) {
                                strProduit = strProduit.replace(courant, '');
                            }
                        }

                        $("#btnProdCocherTout").html("Cocher Tout");
                    }

                    $("#<%=txtProduit.ClientID %>").val(strProduit);

                });

            });

            $("#btnBenefCocherTout").on('click', function () {

                var btn_text = $('#btnBenefCocherTout').text();

                var strBeneficiaire = $("#<%=txtBeneficiaire.ClientID %>").val();

                var arrData = [];

                $("#<%=BeneficiaireGrid.ClientID %>  tr").each(function () {

                    var courant = $(this).find("td:eq(1)").text().trim();

                    if (btn_text == 'Cocher Tout') {

                        $htmlImg = '<a href="#"><img src="images/check.png" title="Supprimer" /></a>';

                        $(this).find("td:eq(0)").html($htmlImg);

                        if (strBeneficiaire != '') {

                            if (!strBeneficiaire.includes(courant)) {
                                strBeneficiaire = strBeneficiaire + ";" + courant;
                            }
                        }
                        else {
                            strBeneficiaire = courant;
                        }

                        $("#btnBenefCocherTout").html("Décocher Tout");

                    }
                    else {

                        $htmlImg = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';

                        $(this).find("td:eq(0)").html($htmlImg);

                        if (strBeneficiaire != '') {
                            if (strBeneficiaire.includes(";" + courant)) {
                                strBeneficiaire = strBeneficiaire.replace(";" + courant, '');
                            }
                            if (strBeneficiaire.includes(courant + ";")) {
                                strBeneficiaire = strBeneficiaire.replace(courant + ";", '');
                            }
                            else if (strBeneficiaire.includes(courant)) {
                                strBeneficiaire = strBeneficiaire.replace(courant, '');
                            }
                        }

                        $("#btnBenefCocherTout").html("Cocher Tout");
                    }

                    $("#<%=txtBeneficiaire.ClientID %>").val(strBeneficiaire);

                });

            });

            // Produit

            $("#btnProdEffacer").click(function () {
                $("#<%=txtProduit.ClientID %>").val('');
                $("#<%=txtCodeProduit.ClientID %>").val('');
                $("#<%=txtDesignation.ClientID %>").val('');
                $("#<%=ProduitSearch.ClientID %>").find("tr:gt(0)").remove();
                $("#btnProdCocherTout").html("Cocher Tout");
            });

            $("#btnBenefEffacer").click(function () {
                $("#<%=txtBeneficiaire.ClientID %>").val('');
                $("#<%=txtCodePPM.ClientID %>").val('');
                $("#<%=txtNomBeneficiare.ClientID %>").val('');
                $("#<%=BeneficiaireGrid.ClientID %>").find("tr:gt(0)").remove();
                $("#btnBenefCocherTout").html("Cocher Tout");
            });

            $("#<%=txtProduit.ClientID %>").click(function () {
                $('#ProduitModal').modal('show');
                $("#<%=txtCodeProduit.ClientID %>").val('');
                $("#<%=txtDesignation.ClientID %>").val('');
                $("#<%=ProduitSearch.ClientID %>").find("tr:gt(0)").remove();
                $("#btnProdCocherTout").html("Cocher Tout");
            });

            $("#<%=txtCodeProduit.ClientID %>").keyup(function () {
                var obj = {};
                obj.codeProduit = $("#<%=txtCodeProduit.ClientID %>").val();
                obj.designation = $("#<%=txtDesignation.ClientID %>").val();
                $.ajax({
                    type: "POST",
                    url: "StatsGlobalesProd.aspx/ChargerListeProduit",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        $("#<%=ProduitSearch.ClientID %>").find("tr:gt(0)").remove();

                        for (var i = 0; i < response.d.length; i++) {
                            $html = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                            $("#<%=ProduitSearch.ClientID %>").append("<tr><td> " + $html + " </td><td>" + response.d[i].codeProduit + "</td><td>" + response.d[i].designation + "</td></tr>");
                        }
                    },
                    error: function (response) {
                        //alert("Erreur");
                    }
                });
            });

            $("#<%=txtDesignation.ClientID %>").keyup(function () {
                var obj = {};
                obj.codeProduit = $("#<%=txtCodeProduit.ClientID %>").val();
                obj.designation = $("#<%=txtDesignation.ClientID %>").val();
                $.ajax({
                    type: "POST",
                    url: "StatsGlobalesProd.aspx/ChargerListeProduit",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        $("#<%=ProduitSearch.ClientID %>").find("tr:gt(0)").remove();

                        for (var i = 0; i < response.d.length; i++) {
                            $html = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                            $("#<%=ProduitSearch.ClientID %>").append("<tr><td> " + $html + " </td><td>" + response.d[i].codeProduit + "</td><td>" + response.d[i].designation + "</td></tr>");
                        }
                    },
                    error: function (response) {
                        //alert("Erreur");
                    }
                });
            });

            $('#<%=ProduitSearch.ClientID %> tbody').on('click', 'tr', function () {

                var courant = $(this).find("td:eq(1)").text().trim();

                var strProduit = $("#<%=txtProduit.ClientID %>").val();

                var titleImg = $(this).find("td:eq(0)").find("img").attr("title");

                $(this).find("td:eq(0)").html('');

                if (titleImg == 'Supprimer') {

                    $htmlImg = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                    $(this).find("td:eq(0)").html($htmlImg);
                    if (strProduit != '') {
                        if (strProduit.includes(";" + courant)) {
                            strProduit = strProduit.replace(";" + courant, '');
                        }
                        if (strProduit.includes(courant + ";")) {
                            strProduit = strProduit.replace(courant + ";", '');
                        }
                        else if (strProduit.includes(courant)) {
                            strProduit = strProduit.replace(courant, '');
                        }
                    }
                    $("#<%=txtProduit.ClientID %>").val(strProduit);
                }
                else if (titleImg == 'Choisir') {

                    $htmlImg = '<a href="#"><img src="images/check.png" title="Supprimer" /></a>';
                    $(this).find("td:eq(0)").html($htmlImg);
                    if (strProduit != '') {
                        if (!strProduit.includes(courant)) {
                            strProduit = strProduit + ";" + courant;
                        }
                    }
                    else {
                        strProduit = courant;
                    }
                    $("#<%=txtProduit.ClientID %>").val(strProduit);
                }

                if (strProduit == '') {
                    $("#btnProdCocherTout").html("Cocher Tout");
                }
            });

            // Fin Produit

            // Beneficiaire
            $("#<%=txtBeneficiaire.ClientID %>").click(function () {
                $('#BeneficiaireModal').modal('show');
                $("#<%=txtCodePPM.ClientID %>").val('');
                $("#<%=txtNomBeneficiare.ClientID %>").val('');
                $("#<%=BeneficiaireGrid.ClientID %>").find("tr:gt(0)").remove();
                $("#btnBenefCocherTout").html("Cocher Tout");
            });

            $("#<%=txtNomBeneficiare.ClientID %>").keyup(function () {
                var obj = {};
                obj.codePPM = $("#<%=txtCodePPM.ClientID %>").val();
                obj.beneficiaire = $("#<%=txtNomBeneficiare.ClientID %>").val();
                $.ajax({
                    type: "POST",
                    url: "StatsGlobalesProd.aspx/ChargerListeBeneficiaire",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        $("#<%=BeneficiaireGrid.ClientID %>").find("tr:gt(0)").remove();

                        for (var i = 0; i < response.d.length; i++) {
                            $html = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                            $("#<%=BeneficiaireGrid.ClientID %>").append("<tr><td> " + $html + " </td><td>" + response.d[i].codePPM + "</td><td>" + response.d[i].beneficiaire + "</td></tr>");
                        }
                    },
                    error: function (response) {
                        //alert("Erreur");
                    }
                });
            });

            $("#<%=txtCodePPM.ClientID %>").keyup(function () {
                var obj = {};
                obj.codePPM = $("#<%=txtCodePPM.ClientID %>").val();
                obj.beneficiaire = $("#<%=txtNomBeneficiare.ClientID %>").val();
                $.ajax({
                    type: "POST",
                    url: "StatsGlobalesProd.aspx/ChargerListeBeneficiaire",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        $("#<%=BeneficiaireGrid.ClientID %>").find("tr:gt(0)").remove();

                        for (var i = 0; i < response.d.length; i++) {
                            $html = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                            $("#<%=BeneficiaireGrid.ClientID %>").append("<tr><td> " + $html + " </td><td>" + response.d[i].codePPM + "</td><td>" + response.d[i].beneficiaire + "</td></tr>");
                        }
                    },
                    error: function (response) {
                        //alert("Erreur");
                    }
                });
            });

            $('#<%=BeneficiaireGrid.ClientID %> tbody').on('click', 'tr', function () {

                var strBeneficaire = $("#<%=txtBeneficiaire.ClientID %>").val();
                var courant = $(this).find("td:eq(1)").text().trim();

                var titleImg = $(this).find("td:eq(0)").find("img").attr("title");
                $(this).find("td:eq(0)").html('');

                if (titleImg == 'Supprimer') {

                    $htmlImg = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                    $(this).find("td:eq(0)").html($htmlImg);

                    if (strBeneficaire != '') {
                        if (strBeneficaire.includes(";" + courant)) {
                            strBeneficaire = strBeneficaire.replace(";" + courant, '');
                        }
                        if (strBeneficaire.includes(courant + ";")) {
                            strBeneficaire = strBeneficaire.replace(courant + ";", '');
                        }
                        else if (strBeneficaire.includes(courant)) {
                            strBeneficaire = strBeneficaire.replace(courant, '');
                        }
                    }
                    $("#<%=txtBeneficiaire.ClientID %>").val(strBeneficaire);
                }
                else if (titleImg == 'Choisir') {

                    $htmlImg = '<a href="#"><img src="images/check.png" title="Supprimer" /></a>';
                    $(this).find("td:eq(0)").html($htmlImg);

                    if (strBeneficaire != '') {
                        if (!strBeneficaire.includes(courant)) {
                            strBeneficaire = strBeneficaire + ";" + courant;
                        }
                    }
                    else {
                        strBeneficaire = courant;
                    }
                    $("#<%=txtBeneficiaire.ClientID %>").val(strBeneficaire);
                }

                if (strBeneficaire == '') {
                    $("#btnBenefCocherTout").html("Cocher Tout");
                }

            });

            // Fin Beneficiaire


        });
    </script>

    <div class="modal fade" id="mymodal-dialog" role="dialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Attention</h4>
                </div>
                <div class="modal-body" style="max-height: 400px; overflow-y: auto;">
                    <p>
                        <asp:Label ID="msgPopup" runat="server" />
                    </p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Fermer</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="produit-dialog" tabindex="-1" role="dialog" aria-labelledby="produit-dialogLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div style="flex-direction: column; text-align: center; overflow: auto;">
                    <h3><b>Choisir Produit</b></h3>
                    <div class="col-sm-12">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="text" class="form-control" placeholder="Code Tarifaire" id="txtCodeProduit" runat="server" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="text" class="form-control" placeholder="Description" id="txtDesignation" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="table-responsive">
                        <asp:GridView ID="ProduitSearch" runat="server" CssClass="table" ShowHeader="True" UseAccessibleHeader="true"
                            EmptyDataText="Aucune information trouvée ." AutoGenerateColumns="false" Style="border-collapse: collapse;" CellSpacing="0" BorderStyle="None" GridLines="None" OnSelectedIndexChanged="ProduitSearch_SelectedIndexChanged" OnPageIndexChanging="ProduitSearch_PageIndexChanging">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <a href="#">
                                            <img src="images/check.png" /></a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="codeProduit" HeaderText="Code" />
                                <asp:BoundField DataField="designation" HeaderText="Désignation" />
                            </Columns>
                        </asp:GridView>

                    </div>

                </div>
                <div class="modal-footer ">
                    <button type="button" id="btnProdCocherTout" class="btn btn-warning" data-dismiss="modal ">Cocher Tout</button>&nbsp;
                    <button type="button" id="btnProdEffacer" class="btn btn-danger " data-dismiss="modal ">Annuler</button>&nbsp;
                    <button type="button" class="btn btn-success" data-dismiss="modal">Valider</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="beneficiaire-dialog" tabindex="-1" role="dialog" aria-labelledby="beneficiaire-dialogLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div style="flex-direction: column; text-align: center; overflow: auto;">
                    <h3><b>Choisir Client</b></h3>
                    <div class="col-sm-12">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="text" class="form-control" placeholder="Code PPM" id="txtCodePPM" runat="server" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="text" class="form-control" placeholder="Raison Sociale" id="txtNomBeneficiare" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="table-responsive">
                        <asp:GridView ID="BeneficiaireGrid" runat="server" CssClass="table" ShowHeader="True" UseAccessibleHeader="true"
                            EmptyDataText="Aucune information trouvée ." AutoGenerateColumns="false" Style="border-collapse: collapse;" CellSpacing="0" BorderStyle="None" GridLines="None" OnSelectedIndexChanged="BeneficiaireGrid_SelectedIndexChanged" OnPageIndexChanging="BeneficiaireGrid_PageIndexChanging">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <a href="#">
                                            <img src="images/check.png" /></a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="codePPM" HeaderText="PPM" />
                                <asp:BoundField DataField="beneficiaire" HeaderText="Bénéficiaire" />
                            </Columns>
                        </asp:GridView>

                    </div>

                </div>
                <div class="modal-footer ">
                    <button type="button" id="btnBenefCocherTout" class="btn btn-warning" data-dismiss="modal ">Cocher Tout</button>&nbsp;
                    <button type="button" id="btnBenefEffacer" class="btn btn-danger " data-dismiss="modal ">Annuler</button>&nbsp;
                    <button type="button" class="btn btn-success" data-dismiss="modal">Valider</button>
                </div>
            </div>
        </div>
    </div>

    <div class="content">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-12">

                    <div class="card">
                        <div class="card-header  card-header-icon">
                            <div class="card-icon">
                                <i class="material-icons">search</i>
                            </div>
                            <h3 class="card-title f1" style="text-align: center"><b>Statistiques Globales par Produit</b></h3>
                            <h4 class="card-title f1"><b>Formulaire de Recherche</b></h4>
                        </div>
                        <br />

                        <!-- Debut ligne 2  -->
                        <div class="col-sm-12">
                            <div class="row">
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <select id="Drop_Operation_Rech" class="selectpicker taille " data-style="select-with-transition" title="Nature Opération" data-size="10" runat="server">
                                            <option value="-1">Nature Opération</option>
                                            <option value="1">Importation</option>
                                            <option value="2">Exportation</option>
                                            <option value="3">Transit</option>
                                            <option value="4">Réexportation</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="col-md-10">
                                    <div class="form-group">
                                        <asp:TextBox class="form-control" ID="txtProduit" runat="server" data-toggle="modal" data-target="#produit-dialog" placeholder="Produit" />
                                        
                                    </div>
                                </div>

                            </div>
                        </div>


                        <div class="col-sm-12">
                            <div class="row">

                                <div class="col-md-2">
                                    <div class="form-group">
                                        <label class="form-label">Début</label>
                                        <asp:TextBox class="form-control" ID="Text_DebutPeriode_Rech" runat="server" type="date" placeholder="Début" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="La date de début est obligatoire." ControlToValidate="Text_DebutPeriode_Rech" ForeColor="Red"></asp:RequiredFieldValidator>
                                    </div>
                                </div>

                                <div class="col-md-10">
                                    <div class="form-group">
                                        <asp:TextBox class="form-control" ID="txtBeneficiaire" data-toggle="modal" data-target="#beneficiaire-dialog" runat="server" placeholder="Bénéficiaire"   />
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="col-sm-12">
                            <div class="row">

                                <div class="col-md-2">
                                    <div class="form-group">
                                        <label class="form-label">Fin</label>
                                        <asp:TextBox class="form-control" ID="Text_FinPeriode_Rech" runat="server" type="date" placeholder="Fin" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="La date de fin est obligatoire." ControlToValidate="Text_FinPeriode_Rech" ForeColor="Red"></asp:RequiredFieldValidator>
                                    </div>
                                </div>

                                <div class="col-md-10">
                                    <div class="form-group">
                                        <asp:Label class="form-label" runat="server" ID="txtError" />
                                    </div>
                                </div>

                            </div>
                        </div>

                        <br />

                        <div>
                            <!-- button -->
                            <div class="movebtn">
                                <asp:Button class="btn btn-fill btn-rose" ID="btnRechercher" OnClick="Rechercher_Click" runat="server" Text="Rechercher" />
                                <input id="Button1" type="button" class="btn btn-fill btn-warning" onserverclick="VisualiserExcel_Click" runat="server" value="Exporter sous excel" />
                                <input id="Button2" type="button" class="btn btn-fill form-btn" onserverclick="Annuler_Click" value="Annuler" runat="server" />
                            </div>
                            <br />
                            <br />
                        </div>
                    </div>
                </div>
            </div>

            <div class="row" id="resultatStatsProd" runat="server">
                <div class="col-md-12">
                    <div class="card">
                        <br />
                        <div class="card-body">
                            <div class="form-row">
                                <div class="col">
                                    <div class="form-row">
                                        <label class="col-form-label">Total ORBUS: &nbsp;</label>
                                        <asp:Label ID="lblTotal" class="col-form-label" runat="server" />
                                    </div>
                                </div>

                                <div class="col">
                                    <div class="form-row">
                                        <label class="col-form-label">Part de marché : &nbsp;</label>
                                        <%--<asp:Label ID="lblTransactions" class="col-form-label" runat="server" />
                                        <label class="col-form-label">-->> &nbsp;</label>--%>
                                        <asp:Label ID="lblPourcentagePartMarche" class="col-form-label" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="table-responsive">

                                <asp:GridView ID="ListeGrid" runat="server" CssClass="table" ShowHeader="True" Style="border-collapse: collapse;" CellSpacing="0" BorderStyle="None" GridLines="None"
                                    EmptyDataText="Aucune information trouvée ." Width="100%" UseAccessibleHeader="true"
                                    PagerSettings-Position="Bottom"
                                    OnSelectedIndexChanged="ListeGrid_SelectedIndexChanged"
                                    OnPageIndexChanging="ListeGrid_PageIndexChanging"
                                    AllowPaging="true" PageSize="10" CaptionAlign="Left">
                                    <HeaderStyle CssClass="colone-titre" />
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <asp:GridView ID="TempGridProduits" runat="server" Visible="false"></asp:GridView>
        <asp:GridView ID="TempGridBeneficiaire" runat="server" Visible="false"></asp:GridView>

    </div>

</asp:Content>
