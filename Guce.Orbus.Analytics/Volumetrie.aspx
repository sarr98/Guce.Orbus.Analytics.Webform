<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Volumetrie.aspx.cs" Inherits="Guce.Orbus.Analytics.Volumetrie" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Windows.Forms" %>  

<%@ Import Namespace="System.Globalization" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">

        //var values = new Array();
        //$("#saveButton").click(function () {

        //    $.each($("input[name='case[]']:checked"), function () {
        //        var data = $(this).parents('tr:eq(0)');
        //        values.push({ 'Item name': $(data).find('td:eq(1)').text(), 'Item code': $(data).find('td:eq(2)').text(), 'Supplier': $(data).find('td:eq(3)').text() });
        //    });

        //    console.log(JSON.stringify(values));
        //});

        $(document).ready(function () {

            $("#<%=Drop_Operation_Rech.ClientID %>").change(function () {

                var selectionNature = $("#<%=Drop_Operation_Rech.ClientID %> option:selected").val();

                if (selectionNature == "2" || selectionNature == "4") {
                    $("#<%=txtPaysProvenance.ClientID %>").attr('placeholder', 'Pays Destination');
                }
                else {
                    $("#<%=txtPaysProvenance.ClientID %>").attr('placeholder', 'Pays Provenance');
                }

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

            $("#btnPaysCocherTout").on('click', function () {

                var btn_text = $('#btnPaysCocherTout').text();

                var strPays = '';

                var arrData = [];

                var typePays = $("#<%=txtTypePays.ClientID %>").val();

                if (typePays == 'Provenance') {
                    strPays = $("#<%=txtPaysProvenance.ClientID %>").val();
                }
                else if (typePays == 'Origine') {
                    strPays = $("#<%=txtPaysOrigine.ClientID %>").val();
                }
                else if (typePays == 'Destination') {
                    strPays = $("#<%=txtPaysDestination.ClientID %>").val();
                }

                $("#<%=PaysGrid.ClientID %>  tr").each(function () {

                    var courant = $(this).find("td:eq(2)").text().trim();

                    if (btn_text == 'Cocher Tout') {

                        $htmlImg = '<a href="#"><img src="images/check.png" title="Supprimer" /></a>';

                        $(this).find("td:eq(0)").html($htmlImg);

                        if (strPays != '') {
                            if (!strPays.includes(courant)) {
                                strPays = strPays + ";" + courant;
                            }
                        }
                        else {
                            strPays = courant;
                        }

                        $("#btnPaysCocherTout").html("Décocher Tout");

                    }
                    else {
                        $htmlImg = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';

                        $(this).find("td:eq(0)").html($htmlImg);

                        if (strPays != '') {
                            if (strPays.includes(";" + courant)) {
                                strPays = strPays.replace(";" + courant, '');
                            }
                            if (strPays.includes(courant + ";")) {
                                strPays = strPays.replace(courant + ";", '');
                            }
                            else if (strPays.includes(courant)) {
                                strPays = strPays.replace(courant, '');
                            }
                        }

                        $("#btnPaysCocherTout").html("Cocher Tout");
                    }

                    if (typePays == 'Provenance') {
                        $("#<%=txtPaysProvenance.ClientID %>").val(strPays);
                    }
                    else if (typePays == 'Origine') {
                        $("#<%=txtPaysOrigine.ClientID %>").val(strPays);
                    }
                    else if (typePays == 'Destination') {
                        $("#<%=txtPaysDestination.ClientID %>").val(strPays);
                    }


                });

            });


            $("#btnProdCocherTout").on('click', function () {

                var btn_text = $('#btnProdCocherTout').text();
         
                var strProduit = $("#<%=txtProduit.ClientID %>").val();

                var arrData = [];

                $("#<%=ProduitGrid.ClientID %>  tr").each(function () {

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
                    else
                    {
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


            // Beneficiaire

            $("#btnBenefEffacer").click(function () {
                $("#<%=txtBeneficiaire.ClientID %>").val('');
                $("#<%=txtCodePPM.ClientID %>").val('');
                $("#<%=txtNomBeneficiare.ClientID %>").val('');
                $("#<%=BeneficiaireGrid.ClientID %>").find("tr:gt(0)").remove();
                $("#btnBenefCocherTout").html("Cocher Tout");
            });

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
                    url: "Volumetrie.aspx/ChargerListeBeneficiaire",
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
                    url: "Volumetrie.aspx/ChargerListeBeneficiaire",
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

            // Produit

            $("#btnProdEffacer").click(function () {
                $("#<%=txtProduit.ClientID %>").val('');
                $("#<%=txtCodeProduit.ClientID %>").val('');
                $("#<%=txtDesignation.ClientID %>").val('');
                $("#<%=ProduitGrid.ClientID %>").find("tr:gt(0)").remove();
            });

            $("#<%=txtProduit.ClientID %>").click(function () {
                $('#ProduitModal').modal('show');
                $("#<%=txtCodeProduit.ClientID %>").val('');
                $("#<%=txtDesignation.ClientID %>").val('');
                $("#<%=ProduitGrid.ClientID %>").find("tr:gt(0)").remove();
                $("#btnProdCocherTout").html("Cocher Tout");
            });

            $("#<%=txtCodeProduit.ClientID %>").keyup(function () {
                var obj = {};
                obj.codeProduit = $("#<%=txtCodeProduit.ClientID %>").val();
                obj.designation = $("#<%=txtDesignation.ClientID %>").val();
                $.ajax({
                    type: "POST",
                    url: "Volumetrie.aspx/ChargerListeProduit",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        $("#<%=ProduitGrid.ClientID %>").find("tr:gt(0)").remove();

                        for (var i = 0; i < response.d.length; i++) {
                            $html = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                            $("#<%=ProduitGrid.ClientID %>").append("<tr><td> " + $html + " </td><td>" + response.d[i].codeProduit + "</td><td>" + response.d[i].designation + "</td></tr>");
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
                    url: "Volumetrie.aspx/ChargerListeProduit",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        $("#<%=ProduitGrid.ClientID %>").find("tr:gt(0)").remove();

                        for (var i = 0; i < response.d.length; i++) {
                            $html = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                            $("#<%=ProduitGrid.ClientID %>").append("<tr><td> " + $html + " </td><td>" + response.d[i].codeProduit + "</td><td>" + response.d[i].designation + "</td></tr>");
                         }
                    },
                    error: function (response) {
                        //alert("Erreur");
                    }
                });
            });

            $('#<%=ProduitGrid.ClientID %> tbody').on('click', 'tr', function () {
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

            // Pays

            $("#btnPaysEffacer").click(function () {

                var typePays = $("#<%=txtTypePays.ClientID %>").val();

                $("#<%=txtCodePays.ClientID %>").val('');
                $("#<%=txtNomPays.ClientID %>").val('');
                $("#<%=PaysGrid.ClientID %>").find("tr:gt(0)").remove();

                if (typePays == 'Provenance') {
                    $("#<%=txtPaysProvenance.ClientID %>").val('');
                }
                else if (typePays == 'Origine') {
                    $("#<%=txtPaysOrigine.ClientID %>").val('');
                }
                else if (typePays == 'Destination') {
                    $("#<%=txtPaysDestination.ClientID %>").val('');
                }

            });

            $("#<%=txtPaysProvenance.ClientID %>").click(function () {
                $('#PaysModal').modal('show');
                $("#<%=txtTypePays.ClientID %>").val('Provenance');
                $("#<%=txtCodePays.ClientID %>").val('');
                $("#<%=txtNomPays.ClientID %>").val('');
                $("#<%=PaysGrid.ClientID %>").find("tr:gt(0)").remove();
                $("#btnPaysCocherTout").html("Cocher Tout");
            });

            $("#<%=txtPaysOrigine.ClientID %>").click(function () {
                $('#PaysModal').modal('show');
                $("#<%=txtTypePays.ClientID %>").val('Origine');
                $("#<%=txtCodePays.ClientID %>").val('');
                $("#<%=txtNomPays.ClientID %>").val('');
                $("#<%=PaysGrid.ClientID %>").find("tr:gt(0)").remove();
                $("#btnPaysCocherTout").html("Cocher Tout");
            });

            $("#<%=txtPaysDestination.ClientID %>").click(function () {
                $('#PaysModal').modal('show');
                $("#<%=txtTypePays.ClientID %>").val('Destination');
                $("#<%=txtCodePays.ClientID %>").val('');
                $("#<%=txtNomPays.ClientID %>").val('');
                $("#<%=PaysGrid.ClientID %>").find("tr:gt(0)").remove();
                $("#btnPaysCocherTout").html("Cocher Tout");
            });


            $("#<%=txtCodePays.ClientID %>").keyup(function () {
                var obj = {};
                obj.codePays = $("#<%=txtCodePays.ClientID %>").val();
                obj.nomPays = $("#<%=txtNomPays.ClientID %>").val();
                $.ajax({
                    type: "POST",
                    url: "Volumetrie.aspx/ChargerListePays",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        $("#<%=PaysGrid.ClientID %>").find("tr:gt(0)").remove();

                        for (var i = 0; i < response.d.length; i++) {
                            $html = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                            $("#<%=PaysGrid.ClientID %>").append("<tr><td> " + $html + " </td><td>" + response.d[i].codePays + "</td><td>" + response.d[i].nomPays + "</td></tr>");
                         }
                    },
                    error: function (response) {
                        //alert("Erreur");
                    }
                });
            });

            $("#<%=txtNomPays.ClientID %>").keyup(function () {
                var obj = {};
                obj.codePays = $("#<%=txtCodePays.ClientID %>").val();
                obj.nomPays = $("#<%=txtNomPays.ClientID %>").val();
                $.ajax({
                    type: "POST",
                    url: "Volumetrie.aspx/ChargerListePays",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        $("#<%=PaysGrid.ClientID %>").find("tr:gt(0)").remove();

                        for (var i = 0; i < response.d.length; i++) {
                            $html = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                            $("#<%=PaysGrid.ClientID %>").append("<tr><td> " + $html + " </td><td>" + response.d[i].codePays + "</td><td>" + response.d[i].nomPays + "</td></tr>");
                         }
                    },
                    error: function (response) {
                        //alert("Erreur");
                    }
                });
            });

            $('#<%=PaysGrid.ClientID %> tbody').on('click', 'tr', function () {

                var typePays = $("#<%=txtTypePays.ClientID %>").val();

                if (typePays == 'Provenance') {

                    var strPays = $("#<%=txtPaysProvenance.ClientID %>").val();

                    var courant = $(this).find("td:eq(2)").text().trim();

                    var titleImg = $(this).find("td:eq(0)").find("img").attr("title");

                    $(this).find("td:eq(0)").html('');

                    if (titleImg == 'Supprimer') {

                        $htmlImg = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                        $(this).find("td:eq(0)").html($htmlImg);
                        if (strPays != '') {
                            if (strPays.includes(";" + courant)) {
                                strPays = strPays.replace(";" + courant, '');
                            }
                            if (strPays.includes(courant + ";")) {
                                strPays = strPays.replace(courant + ";", '');
                            }
                            else if (strPays.includes(courant)) {
                                strPays = strPays.replace(courant, '');
                            }
                        }
                        $("#<%=txtPaysProvenance.ClientID %>").val(strPays);
                     }
                     else if (titleImg == 'Choisir') {

                         $htmlImg = '<a href="#"><img src="images/check.png" title="Supprimer" /></a>';
                         $(this).find("td:eq(0)").html($htmlImg);
                         if (strPays != '') {
                             if (!strPays.includes(courant)) {
                                 strPays = strPays + ";" + courant;
                             }
                         }
                         else {
                             strPays = courant;
                         }
                         $("#<%=txtPaysProvenance.ClientID %>").val(strPays);
                    }

            }
            else if (typePays == 'Origine') {

                var strPays = $("#<%=txtPaysOrigine.ClientID %>").val();

                var courant = $(this).find("td:eq(2)").text().trim();

                var titleImg = $(this).find("td:eq(0)").find("img").attr("title");

                $(this).find("td:eq(0)").html('');

                if (titleImg == 'Supprimer') {

                    $htmlImg = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                    $(this).find("td:eq(0)").html($htmlImg);
                    if (strPays != '') {
                        if (strPays.includes(";" + courant)) {
                            strPays = strPays.replace(";" + courant, '');
                        }
                        if (strPays.includes(courant + ";")) {
                            strPays = strPays.replace(courant + ";", '');
                        }
                        else if (strPays.includes(courant)) {
                            strPays = strPays.replace(courant, '');
                        }
                    }
                    $("#<%=txtPaysOrigine.ClientID %>").val(strPays);
                }
                else if (titleImg == 'Choisir') {

                    $htmlImg = '<a href="#"><img src="images/check.png" title="Supprimer" /></a>';
                    $(this).find("td:eq(0)").html($htmlImg);
                    if (strPays != '') {
                        if (!strPays.includes(courant)) {
                            strPays = strPays + ";" + courant;
                        }
                    }
                    else {
                        strPays = courant;
                    }
                    $("#<%=txtPaysOrigine.ClientID %>").val(strPays);
                }
            }
            else if (typePays == 'Destination') {

                var strPays = $("#<%=txtPaysDestination.ClientID %>").val();

                    var courant = $(this).find("td:eq(2)").text().trim();

                    var titleImg = $(this).find("td:eq(0)").find("img").attr("title");

                    $(this).find("td:eq(0)").html('');

                    if (titleImg == 'Supprimer') {

                        $htmlImg = '<a href="#"><img src="images/uncheck.png" title="Choisir" /></a>';
                        $(this).find("td:eq(0)").html($htmlImg);
                        if (strPays != '') {
                            if (strPays.includes(";" + courant)) {
                                strPays = strPays.replace(";" + courant, '');
                            }
                            if (strPays.includes(courant + ";")) {
                                strPays = strPays.replace(courant + ";", '');
                            }
                            else if (strPays.includes(courant)) {
                                strPays = strPays.replace(courant, '');
                            }
                        }
                        $("#<%=txtPaysDestination.ClientID %>").val(strPays);
                }
                else if (titleImg == 'Choisir') {

                    $htmlImg = '<a href="#"><img src="images/check.png" title="Supprimer" /></a>';
                    $(this).find("td:eq(0)").html($htmlImg);
                    if (strPays != '') {
                        if (!strPays.includes(courant)) {
                            strPays = strPays + ";" + courant;
                        }
                    }
                    else {
                        strPays = courant;
                    }
                    $("#<%=txtPaysDestination.ClientID %>").val(strPays);
                }

        }
                if (strPays == '') {
                    $("#btnPaysCocherTout").html("Cocher Tout");
                }

            });

            // Fin Pays

        });

    </script>

    <%--<div id="myModal" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Attention</h4>
                </div>
                <div class="modal-body">
                    <span class="widget-caption">
                        <asp:Label ID="msgPopup" runat="server" /></span>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Fermer</button>
                </div>
            </div>
        </div>
    </div>--%>

    <div class="modal fade" id="mymodal-dialog" role="dialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Attention</h4>
                </div>
                <div class="modal-body" style="max-height: 400px; overflow-y: auto;">
                    <p>
                        <asp:Label ID="msgPopup" runat="server" /></p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Fermer</button>
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

    <div class="modal fade" id="pays-dialog" tabindex="-1" role="dialog" aria-labelledby="pays-dialogLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <input type="hidden" id="txtTypePays" runat="server" />
                <div style="flex-direction: column; text-align: center; overflow: auto;">
                    <h3><b>Choisir Pays</b></h3>
                    <div class="col-sm-12">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="text" class="form-control" placeholder="Code" id="txtCodePays" runat="server" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="text" class="form-control" placeholder="Nom" id="txtNomPays" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="table-responsive">
                        <asp:GridView ID="PaysGrid" runat="server" CssClass="table" ShowHeader="True" UseAccessibleHeader="true"
                            EmptyDataText="Aucune information trouvée ." AutoGenerateColumns="false" Style="border-collapse: collapse; max-width: 490px" CellSpacing="0" BorderStyle="None" GridLines="None" OnSelectedIndexChanged="PaysGrid_SelectedIndexChanged" OnPageIndexChanging="PaysGrid_PageIndexChanging">

                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <a href="#">
                                            <img src="images/check.png" />
                                        </a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="codePays" HeaderText="Code" />
                                <asp:BoundField DataField="nomPays" HeaderText="Nom" />
                            </Columns>

                        </asp:GridView>

                    </div>

                </div>
                <div class="modal-footer ">
                    <button type="button" id="btnPaysCocherTout" class="btn btn-warning" data-dismiss="modal ">Cocher Tout</button>&nbsp;
                    <button type="button" id="btnPaysEffacer" class="btn btn-danger " data-dismiss="modal ">Annuler</button>&nbsp;
                    <button type="button" class="btn btn-success" data-dismiss="modal">Valider</button>
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
                        <asp:GridView ID="ProduitGrid" runat="server" CssClass="table" ShowHeader="True" UseAccessibleHeader="true"
                            EmptyDataText="Aucune information trouvée ." AutoGenerateColumns="false" Style="border-collapse: collapse;" CellSpacing="0" BorderStyle="None" GridLines="None" OnSelectedIndexChanged="ProduitGrid_SelectedIndexChanged" OnPageIndexChanging="ProduitGrid_PageIndexChanging">
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
                <div class="modal-footer">
                    <button type="button" id="btnProdCocherTout" class="btn btn-warning" data-dismiss="modal">Cocher Tout</button>&nbsp;
                    <button type="button" id="btnProdEffacer" class="btn btn-danger " data-dismiss="modal ">Annuler</button>&nbsp;
                    <button type="button" class="btn btn-success" data-dismiss="modal">Valider</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Formulaire -->
    <div class="content">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-12">
                    <%--<form id="RegisterValidation">--%>
                    <div class="card">
                        <div class="card-header  card-header-icon">
                            <div class="card-icon">
                                <i class="material-icons">search</i>
                            </div>
                            <h3 class="card-title f1" style="text-align:center"><b>Statistiques par Volumétrie</b></h3>
                            <h4 class="card-title f1"><b>Formulaire de Recherche</b></h4>
                        </div>
                        <br />

                        <div class="card-body">
                            <!-- Debut ligne 2  -->
                            <!-- -->
                            <div class="row">
                                <div class="col-lg-12 col-md-12">                            
                                    <div class="row">
                                        <div class="col-lg-3 col-md-12">
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
                                        <div class="col-lg-4 col-md-12">
                                            <div class="form-group">
                                                <asp:TextBox class="form-control" ID="txtProduit" runat="server" data-toggle="modal" data-target="#produit-dialog" placeholder="Produit" />
                                            </div>
                                        </div>


                                        <%
                                           
                                           if(Session["TypeEntreprise"].ToString() == "BNQ")
                                            {
                                             %>
                                           
                                        <div class="col-lg-5 col-md-12">
                                            <div class="form-group">
                                                <select id="TypeDossierList" class="selectpicker taille " data-style="select-with-transition" data-size="10" runat="server">
                                                    <option value="0">Tous les dossiers</option>
                                                    <option value="1">Les dossiers domiciliés</option>
                                                    <option value="2">Les dossiers non domiciliés</option>
                                                </select>
                                                
                                            </div>
                                        </div>

                                           <%
                                            }
                                               %>  


                                        

                                    </div>
                                </div>


                                <div class="col-lg-12 col-md-12">
                                    <div class="row">
                                        <div class="col-lg-3 col-md-12">
                                            <div class="form-group">
                                                <select id="DateParList" class="selectpicker taille " data-style="select-with-transition" title="Date Par" data-size="10" runat="server">
                                                    <option value="1">Jour</option>
                                                    <option value="2">Mois</option>
                                                    <option value="3">Année</option>
                                                </select>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="La périodicité est obligatoire." ControlToValidate="DateParList" ForeColor="Red"></asp:RequiredFieldValidator>

                                            </div>
                                        </div>

                                        <div class="col-lg-4 col-md-12">
                                            <div class="form-group">
                                                <label class="form-label">Début</label>
                                                <asp:TextBox class="form-control" ID="Text_DebutPeriode_Rech" runat="server" type="date" placeholder="Début" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="La date de début est obligatoire." ControlToValidate="Text_DebutPeriode_Rech" ForeColor="Red"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>

                                        <div class="col-lg-5 col-md-12">
                                            <div class="form-group">
                                                <asp:TextBox class="form-control" ID="txtPaysProvenance" data-toggle="modal" data-target="#pays-dialog" placeholder="Pays Provenance" runat="server" />
                                            </div>
                                        </div>

                                       
                                    </div>
                                </div>

                                <div class="col-lg-12 col-md-12">
                                    <div class="row">
                                        <div class="col-lg-3 col-md-12">
                                            <div class="form-group">
                                                <select id="DeviseList" class="selectpicker taille " data-style="select-with-transition" title="Devise" data-size="10" runat="server" datatextfield="NOMDEVISE" datavaluefield="CODEDEVISE">
                                                    <%--<option value="-1"></option>--%>
                                                </select>
                                            </div>
                                        </div>

                                        <div class="col-lg-4 col-md-12">
                                            <div class="form-group">
                                                <label class="form-label">Fin</label>
                                                <asp:TextBox class="form-control" ID="Text_FinPeriode_Rech" runat="server" type="date" placeholder="Fin" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="La date de fin est obligatoire." ControlToValidate="Text_FinPeriode_Rech" ForeColor="Red"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>

                                         <div class="col-lg-5 col-md-12">
                                            <div class="form-group">
                                                <asp:TextBox class="form-control" ID="txtPaysOrigine" runat="server" data-toggle="modal" data-target="#pays-dialog" placeholder="Pays Origine" />
                                            </div>
                                        </div>

                                        

                                    </div>
                                </div>

                                <div class="col-lg-12 col-md-12">
                                    <div class="row">
                                        <div class="col-lg-3 col-md-12">
                                            <div class="form-group">
                                                <select id="GroupageList" class="selectpicker taille " data-style="select-with-transition" title="Groupé par" data-size="10" runat="server">
                                                    <option value="1" >Date</option>
                                                    <option value="2">Pays</option>
                                                    <option value="3">Bénéficiaire</option>
                                                </select>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Le groupage est obligatoire." ControlToValidate="GroupageList" ForeColor="Red"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>

                                        <div class="col-lg-4 col-md-12">
                                            <div class="form-group">
                                                <asp:TextBox class="form-control" ID="txtBeneficiaire" runat="server" data-toggle="modal" data-target="#beneficiaire-dialog" placeholder="Bénéficiaire" />
                                            </div>
                                        </div>
                                        <!--
                                        <div class="col-lg-5 col-md-12">
                                            <div class="form-group">
                                                <asp:TextBox class="form-control" ID="txtPaysDestination" data-toggle="modal" data-target="#pays-dialog" runat="server" placeholder="Pays Destination" />
                                            </div>
                                        </div>
                                        -->

                                    </div>
                                </div>

                            </div>

                        <br />

                            <div>
                                <!-- button -->
                                <div class="movebtn">
                                    <asp:Button class="btn btn-fill btn-rose" ID="btnRechercher" OnClick="Rechercher_Click" runat="server" Text="Rechercher" />
                                    <input type="button" class="btn btn-fill btn-warning" onserverclick="VisualiserExcel_Click" runat="server" value="Exporter sous excel" />
                                    <input type="button" class="btn btn-fill form-btn" onserverclick="Annuler_Click" value="Annuler" runat="server" />
                                </div>
                                <br />
                                <br />
                            </div>

                        </div>

                    </div>
                </div>
            </div>

            <div class="row" id="resultatVolumetrie" runat="server">
                <div class="col-md-12">
                    <div class="card">
                        <br />
                        <div class="card-body" >

                            <div class="form-row">
                                <div class="col">
                                    <div class="form-row">
                                        <label class="col-form-label">Total Dossiers : &nbsp;</label>
                                        <asp:Label ID="lblTotal" class="col-form-label" runat="server" />
                                    </div>
                                </div>

                            </div>
                            <br />
                            <div class="table-responsive">
                                <asp:GridView ID="VolumetrieListeGridView" runat="server" CssClass="table" ShowHeader="True" Style="border-collapse: collapse;" CellSpacing="0" BorderStyle="None" GridLines="None"
                                    EmptyDataText="Aucune information trouvée ." Width="100%" UseAccessibleHeader="true"
                                    PagerSettings-Position="Bottom"
                                    OnSelectedIndexChanged="VolumetrieListeGridView_SelectedIndexChanged"
                                    OnPageIndexChanging="VolumetrieListeGridView_PageIndexChanging"
                                    OnRowDataBound="VolumetrieListeGridView_RowDataBound"
                                    AllowPaging="true" PageSize="15" CaptionAlign="Left">
                                    <HeaderStyle CssClass="colone-titre" />
                                </asp:GridView>

                            </div>

                            <div class="table-responsive">
                                <asp:Chart ID="ChartLine" CanResize="true" runat="server" CssClass="table  table-bordered table-condensed table-responsive" Height="600px" Width="1500px">
                                    <Titles>
                                        <asp:Title ShadowOffset="3" Name="Items" />
                                    </Titles>
                                    <Legends>
                                        <asp:Legend Alignment="Center" Docking="Right" LegendStyle="Table" Name="Default" />
                                    </Legends>
                                    <Series>
                                        <asp:Series Name="Default" />
                                    </Series>
                                    <ChartAreas>
                                        <asp:ChartArea Name="ChartArea1" BorderWidth="2" />
                                    </ChartAreas>
                                </asp:Chart>

                                <asp:Chart ID="ChartSeries" CanResize="true" CssClass="table  table-bordered table-condensed table-responsive" runat="server" Height="600px" Width="1500px">
                                    <Titles>
                                        <asp:Title ShadowOffset="3" Name="Items" />
                                    </Titles>
                                    <Legends>
                                        <asp:Legend Alignment="Center" Docking="Right" LegendStyle="Table" Name="Default" />
                                    </Legends>
                                    <Series>
                                        <asp:Series Name="Default" />
                                    </Series>
                                    <ChartAreas>
                                        <asp:ChartArea Name="ChartArea1" BorderWidth="2" />
                                    </ChartAreas>
                                </asp:Chart>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

        </div>

        <asp:GridView ID="TempGridPays" runat="server" Visible="false"></asp:GridView>
        <asp:GridView ID="TempGridProduits" runat="server" Visible="false"></asp:GridView>
        <asp:GridView ID="TempGridBeneficiaire" runat="server" Visible="false"></asp:GridView>

    </div>


</asp:Content>
