<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ListeEntreprises.aspx.cs" Inherits="Guce.Orbus.Analytics.ListeEntreprises" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">

        $(document).ready(function () {

            $("[id*=btnListeUsers]").click(function () {

                var courant = $(this).closest("tr").find('span').html();
                $("#<%=txtNumeroEntreprise.ClientID %>").val(courant);
                var ent = $("#<%=txtNumeroEntreprise.ClientID %>").val();
                event.preventDefault();
                window.location.href = "../ListeUtilisateurs.aspx?Ent=" + ent;

            });

            $("[id$=BtnAjouterUtilisateur]").bind("click", function (event) {

                var courant = $(this).closest("tr").find('span').html();
                $("#<%=txtNumeroEntreprise.ClientID %>").val(courant);
                var ent = $("#<%=txtNumeroEntreprise.ClientID %>").val();
                event.preventDefault();
                window.location.href = "../AjouterUtilisateur.aspx?idEnt=" + ent;
            });

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
                            <h4 class="card-title f1"><b>Recherche</b></h4>
                        </div>
                        <br />

                        <!-- Debut ligne 2  -->
                        <div class="col-sm-12">
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        
                                        <asp:DropDownList ID="typeActeurList" runat="server" class="form-control" OnSelectedIndexChanged="listActeur_SelectedIndexChanged" AutoPostBack="true" placeholder="Type Acteur">
                                            <asp:ListItem Value="-1" Text="Type Acteur"></asp:ListItem>
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
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <%--<input type="text" class="form-control" placeholder="Raison Sociale">--%>
                                        <asp:TextBox class="form-control" ID="txtNomEntreprise" runat="server" type="text" placeholder="Raison Sociale" />
                                        <asp:DropDownList ID="EntrepriseList" runat="server" class="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <%--<input type="text" class="form-control" placeholder="Numéro Orbus">--%>
                                        <asp:TextBox class="form-control" ID="txtNumeroOrbus" runat="server" type="text" placeholder="Numéro Orbus" />
                                    </div>
                                </div>

                            </div>
                        </div>
                        <br />
                        <!-- end ligne 2  -->
                        <!-- Debut ligne 3  -->
                        <div class="col-sm-12">
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <%--<input type="text" class="form-control" placeholder="Etat">--%>
                                        <asp:DropDownList ID="EtatList" runat="server" class="form-control">
                                            <asp:ListItem Value="-1" Text="Etat"></asp:ListItem>
                                            <asp:ListItem Value="1">Activé</asp:ListItem>
                                            <asp:ListItem Value="0">Désactivé</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label class="form-label">Date Inscription</label>
                                        <asp:TextBox class="form-control" ID="txtDateInscription" runat="server" type="date" />
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label class="form-label">Date Expiration</label>
                                        <asp:TextBox class="form-control" ID="txtDateExpiration" runat="server" type="date" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />

                        <div>
                            <!-- button -->
                            <div class="movebtn">
                                <asp:Button ID="btnRechercher" class="btn btn-fill btn-rose" OnClick="Rechercher_Click" runat="server" Text="Valider" />
                                <button type="button" class="btn btn-fill form-btn" onclick="Annuler_Click">Annuler</button>
                            </div>
                            <br />
                        </div>
                    </div>

                </div>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <input type="hidden" name="txtNumeroEntreprise" id="txtNumeroEntreprise" runat="server" />
                    <div class="card">
                        <br />
                        <asp:HiddenField ID="txtEntreprise" runat="server"></asp:HiddenField>
                        <div class="card-body">
                            <div class="table-responsive">
                                <asp:GridView ID="GridEntreprises" runat="server" DataKeyNames="Id,Etat" CssClass="table" ShowHeader="True" UseAccessibleHeader="true"
                                    EmptyDataText="Aucune information trouvée ." AutoGenerateColumns="false" Style="border-collapse: collapse;" CellSpacing="0" BorderStyle="None" GridLines="None"
                                    AllowSorting="true" PagerSettings-Position="Bottom"
                                    OnSelectedIndexChanged="GridEntreprises_SelectedIndexChanged"
                                    OnPageIndexChanging="GridEntreprises_PageIndexChanging"
                                    OnSorting="GridEntreprises_Sorting"
                                    OnRowDataBound="GridEntreprises_RowDataBound"
                                    AllowPaging="true" PageSize="10" CaptionAlign="Left">

                                    <HeaderStyle CssClass="colone-titre" />

                                    <Columns>
                                        <asp:BoundField DataField="Id" HeaderText="Id" />
                                        <asp:BoundField DataField="IdOrbus" HeaderText="N° Inscription" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center" />
                                        <asp:BoundField DataField="NomouRaisonSociale" HeaderText="Nom ou Raison Sociale" />
                                        <asp:BoundField DataField="LibelleTypeEntreprise" HeaderText="Type" />
                                        <asp:BoundField DataField="DateInscription" HeaderText="Date Inscription" />
                                        <asp:BoundField DataField="DateExpiration" HeaderText="Date Expiration" />
                                        <asp:BoundField DataField="Etat" HeaderText="Etat" />

                                        <asp:CommandField ShowSelectButton="true" ButtonType="Image" ControlStyle-Width="20" ControlStyle-Height="20" HeaderStyle-Width="10" />
                                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <span style="display: none"><%# Eval("Id") %></span>
                                                <asp:ImageButton ID="btnListeUsers" ImageUrl="~/Images/users.jpg" runat="server" Height="25px" Width="21px" ToolTip="Utilisateurs" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>


    </div>


</asp:Content>
