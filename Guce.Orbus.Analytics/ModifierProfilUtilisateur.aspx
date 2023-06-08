<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ModifierProfilUtilisateur.aspx.cs" Inherits="Guce.Orbus.Analytics.ModifierProfilUtilisateur" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">

        $(document).ready(function () {


            
        });

    </script>
    
    <div class="modal fade" id="mymodal-dialog" role="dialog">
        <div class="modal-dialog  modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Attention</h4>
                </div>
                <div class="modal-body" style="max-height: 400px; overflow-y: auto;">
                    <p>
                        <asp:Label ID="msgPopup" Text="dddd" runat="server" /></p>
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
                        <br />
                        <asp:HiddenField ID="txtEntreprise" runat="server"></asp:HiddenField>
                        <br />
                        <div class="card-body">
                            <h4 class="card-title">Liste des utilisateurs :  <asp:Label ID="lblNomEntreprise" class="col-form-label" runat="server" /></h4>
                            <br />
                            <div class="table-responsive">
                                <asp:GridView ID="ListeUsers" runat="server" DataKeyNames="Id,IdEntreprise,Profil" CssClass="table" ShowHeader="True" UseAccessibleHeader="true" 
                                                    EmptyDataText="Aucune information trouvée ." AutoGenerateColumns="false" style="border-collapse:collapse;" cellspacing="0" BorderStyle="None" GridLines="None" 
                                    AllowSorting="true" PagerSettings-Position="Bottom"
                                    OnSelectedIndexChanged="ListeUsers_SelectedIndexChanged"
                                    OnPageIndexChanging="ListeUsers_PageIndexChanging"
                                    OnSorting="ListeUsers_Sorting"
                                    OnRowDataBound="ListeUsers_RowDataBound"
                                    AllowPaging="true" PageSize="10" CaptionAlign="Left">
                                    <HeaderStyle CssClass="colone-titre" />
                                    <Columns>
                                        <asp:BoundField DataField="Id">
                                            <ItemStyle CssClass="hidden" />
                                            <HeaderStyle CssClass="hidden" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="IdEntreprise">
                                            <ItemStyle CssClass="hidden" />
                                            <HeaderStyle CssClass="hidden" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Prenom" HeaderText="Prénom" />
                                        <asp:BoundField DataField="Nom" HeaderText="Nom" />
                                        <asp:BoundField DataField="userLogin" HeaderText="Login" />
                                        <asp:BoundField DataField="Profil" HeaderText="Profil" />
                                        <asp:BoundField DataField="Etat" HeaderText="Etat" />

                                        <asp:CommandField ShowSelectButton="true" ButtonType="Image" ControlStyle-Width="20" ControlStyle-Height="20" HeaderStyle-Width="10" />

                                        
                                    </Columns>
                                </asp:GridView>
                            </div>

                            <div>
                            
                            <br />
                        </div>

                        </div>
                    </div>
                </div>

            </div>

        </div>


    </div>
</asp:Content>
