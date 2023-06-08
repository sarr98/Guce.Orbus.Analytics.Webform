<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Inscription.aspx.cs" Inherits="Guce.Orbus.Analytics.Inscription" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">

        //function CmbChange(obj) {
        //    var cmbValue = document.getElementById("typeActeurList").value;
        //    __doPostBack('typeActeurList', cmbValue);
        //}


        $(document).ready(function () {


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


    <!-- Formulaire -->
    <div class="content">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-12">
                    <div class="card">
                        <div class="card-header  card-header-icon">
                            <div class="card-icon">
                                <i class="material-icons">search</i>
                            </div>
                            <h4 class="card-title f1"><b>Nouvelle Inscription</b></h4>
                        </div>
                        <br />
                        <input type="hidden" name="txtIdentificationOrbus" id="txtIdentificationOrbus" runat="server" />
                        <!-- Debut ligne 2  -->
                        <div class="card-header">
                            <h4 class="card-title f1">Informations Entreprise</></h4>
                        </div>

                        <div class="col-sm-12">
                            <div class="form-row">
                                <div class="col-md-4 mb-3">
                                    <label for="typeActeurList">Type Acteur</label>
                                    <asp:DropDownList ID="typeActeurList" runat="server" class="form-control" OnSelectedIndexChanged="listActeur_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Value="-1" Text="Choisir"></asp:ListItem>
                                        <asp:ListItem Value="CA">CAD</asp:ListItem>
                                        <asp:ListItem Value="ASS">Assurance</asp:ListItem>
                                        <asp:ListItem Value="BNQ">Banque</asp:ListItem>
                                        <asp:ListItem Value="IA">Industriel</asp:ListItem>
                                        <asp:ListItem Value="CC">Chambre de Commerce</asp:ListItem>
                                        <asp:ListItem Value="AU">Autre Client Orbus</asp:ListItem>
                                        <asp:ListItem Value="OC">Client Occasionnel</asp:ListItem>
                                        <asp:ListItem Value="ADM">Orbus - Administration</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-4 mb-3">
                                    <label for="EntrepriseList">Raison Sociale</label>
                                    <asp:TextBox class="form-control" ID="txtNomEntreprise" runat="server" type="text" Visible="false" />
                                    <asp:DropDownList ID="EntrepriseList" runat="server" class="form-control" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-4 mb-3">
                                    <label for="txtDateInscription">Date Inscription</label>
                                    <asp:TextBox class="form-control" ID="txtDateInscription" runat="server" type="date" />
                                </div>
                            </div>
                        </div>

                        <div class="col-sm-12">
                            <div class="form-row">
                                <div class="col-md-4 mb-3">
                                </div>
                                <div class="col-md-4 mb-3">
                                </div>
                                <div class="col-md-4 mb-3">
                                </div>
                            </div>
                        </div>

                        <div class="col-sm-12">
                            <div class="form-row">
                                <div class="col-md-4 mb-3">
                                    <label for="txtTelEntreprise">Téléphone</label>
                                    <asp:TextBox class="form-control" ID="txtTelEntreprise" runat="server" />
                                </div>
                                <div class="col-md-4 mb-3">
                                    <label for="txtAdresseEntreprise">Adresse</label>
                                    <asp:TextBox class="form-control" ID="txtAdresseEntreprise" runat="server" />
                                </div>
                                <div class="col-md-4 mb-3">
                                </div>
                            </div>
                        </div>


                        <!-- end ligne 2  -->
                        <!-- Debut ligne 3  -->


                        <br />
                        <div class="card-header">
                            <h4 class="card-title f1">Informations Superviseur</h4>
                        </div>
                        <div class="col-sm-12">
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <%--<input type="date" class="form-control" placeholder="Date Inscription">--%>
                                        <asp:TextBox class="form-control" ID="txtNomAdmin" runat="server" placeholder="Nom Superviseur" />
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <%--<input type="date" class="form-control" placeholder="Date Inscription">--%>
                                        <asp:TextBox class="form-control" ID="txtPrenomSuperviseur" runat="server" placeholder="Prénom Superviseur" />
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <%--<input type="date" class="form-control" placeholder="Date Expiration">--%>
                                        <asp:TextBox class="form-control" ID="txtLoginAdmin" runat="server" placeholder="Login Superviseur" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-sm-12">
                            <div class="form-row">
                                <div class="col-md-4 mb-3">
                                </div>
                                <div class="col-md-4 mb-3">
                                </div>
                                <div class="col-md-4 mb-3">
                                </div>
                            </div>
                        </div>

                        <div class="col-sm-12">
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <%--<input type="date" class="form-control" placeholder="Date Inscription">--%>
                                        <asp:TextBox class="form-control" ID="txtEmailSuperviseur" runat="server" placeholder="Email Superviseur" />
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <%--<input type="date" class="form-control" placeholder="Date Inscription">--%>
                                        <asp:TextBox class="form-control" ID="txtTelAdmin" runat="server" placeholder="Téléphone Superviseur" />
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div>
                            <!-- button -->
                            <div class="movebtn">
                                <asp:Button ID="btnValider" class="btn btn-fill btn-rose" OnClick="Valider_Click" runat="server" Text="Valider" />
                                <button type="button" class="btn btn-fill form-btn" onclick="Annuler_Click">Annuler</button>
                            </div>
                            <br />
                        </div>
                    </div>

                </div>
            </div>


        </div>


    </div>

</asp:Content>
