<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ListeUtilisateurs.aspx.cs" Inherits="Guce.Orbus.Analytics.ListeUtilisateurs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">

        $(document).ready(function () {

            $("input[type='radio']").change(function () {

                
                if ($(this).val() == "1") {
                    $("#<%=txtProfil.ClientID %>").val('Agent');
                }
                else {
                    $("#<%=txtProfil.ClientID %>").val('Superviseur');
                } 
            });


            //$('body').on('shown.bs.modal', '.modal', function () {
            //    $(this).find('select').each(function () {
            //        var dropdownParent = $(document.body);
            //        if ($(this).parents('.modal.in:first').length !== 0)
            //            dropdownParent = $(this).parents('.modal.in:first');
            //        $(this).select2({
            //            dropdownParent: dropdownParent
            //            // ...
            //        });
            //    });
            //});

            




            $("[id*=btnResetPassword]").click(function () {

                var courant = $(this).closest("tr").find('span').html();

                var obj = {};
                obj.id = courant;
                $.ajax({
                    type: "POST",
                    url: "ListeUtilisateurs.aspx/ResetPassword",
                    data: JSON.stringify(obj),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $("#mdpModal").modal();
                    },
                    error: function (response) {

                    }
                });
                return false;

            });

            
            

        });

    </script>
    <script>
        
    </script>


    <%--<div class="modal fade" id="" role="dialog">
        <div class="modal-dialog">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Réinitialisation</h4>
                </div>
                <div class="modal-body">
                    <p>Mot de passe réinitialisé !!!</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Fermer</button>
                </div>
            </div>

        </div>
    </div>--%>

    <div class="modal fade" id="mdpModal" role="dialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Mot de passe</h4>
                </div>
                <div class="modal-body" style="max-height: 400px; overflow-y: auto;">
                    <p>
                        <asp:Label ID="Label1" Text="Mot de passe réinitialisé avec succès !!!" runat="server" />
                    </p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Fermer</button>
                </div>
            </div>
        </div>
    </div>

    <%--<div id="mymodal-dialog" class="modal fade" role="dialog">
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

    <!-- Modal -->
    <div class="modal fade" id="utilisateur-dialog" style="overflow:hidden;" role="dialog" aria-labelledby="utilisateur-dialogLabel" aria-hidden="true">
        <%--<div class="modal fade" id="Div1" tabindex="-1" role="dialog" aria-labelledby="utilisateur-dialogLabel" aria-hidden="true">--%>
        <div class="modal-dialog" role="document">
            <div class="modal-content">

                <div class="row">
                            <div class="col-md-12">

                <div style="display: flex; flex-direction: column; text-align: center;">
                    <h3><b>Nouvel utilisateur</b></h3>


                    <div class="col-sm-12">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="text" class="form-control" placeholder="Prénom" id="txtPrenom" runat="server" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="text" class="form-control" placeholder="Nom" id="txtNom" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-sm-12">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="text" class="form-control" placeholder="Email" id="txtEmail" runat="server" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="text" class="form-control" placeholder="Login" id="txtLogin" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-sm-12">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="text" class="form-control" placeholder="Téléphone" id="txtTelephone" runat="server" />
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">

                                    <input type="date" class="form-control" placeholder="Date Inscription" id="txtDateInscription" runat="server" />

                                    
                                  
                                   
                                    <%--<asp:DropDownList ID="txtProfil" runat="server" class="form-control" OnSelectedIndexChanged="Profil_SelectedIndexChanged" >
                                        
                                        <asp:ListItem Value="Agent">Agent</asp:ListItem>
                                        <asp:ListItem Value="Superviseur">Superviseur</asp:ListItem>
                                        
                                    </asp:DropDownList>--%>
                                    <%--<input type="text" class="form-control" placeholder="Profil" id="txtProfil" runat="server" value="Agent" readonly="true" />--%>
                                </div>
                            </div>
                        </div>
                    </div>
                    <input type="hidden" id="txtProfil" runat="server" value="Agent" />
                    <div class="col-sm-12">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <%--<label for="Agent">Profil</label>
                                    &nbsp;&nbsp;
                                    <a href="#" id="Agent"><img src="images/check.png"  />Agent</a>&nbsp;&nbsp;&nbsp;
                                    <a href="#" id="Superviseur"><img src="images/uncheck.png" />Superviseur</a>--%>
                                    <label for="Agent">Profil</label>
                                    <input type="radio" id="txtProfil1" name="Profil" value="1" checked="checked" />&nbsp;Agent&nbsp;
                                    <input type="radio" id="txtProfil2" name="Profil" value="2" style="margin-left: 15px;" />&nbsp;Superviseur

                                </div>
                            </div>
                            
                        </div>
                    </div>


                </div>
                <div class="modal-footer ">
                    <asp:Button ID="btnAjouter" class="btn btn-fill btn-rose" OnClick="Valider_Click" runat="server" Text="Valider" />&nbsp;&nbsp;
                    <button type="button " class="btn btn-danger " data-dismiss="modal ">Fermer</button>
                    <br /><asp:Label style="color: red" id="labelErrorMsg" runat="server" />
                </div>

                        </div>
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
                                <asp:GridView ID="ListeUsers" runat="server" DataKeyNames="Id,IdEntreprise,Etat" CssClass="table" ShowHeader="True" UseAccessibleHeader="true" 
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

                                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <span style="display: none"><%# Eval("Id") %></span>
                                                <asp:ImageButton ID="btnResetPassword" ImageUrl="~/Images/password.jpg" runat="server" Height="20px" Width="20px" ToolTip="Réinitialiser Mot de passe" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>

                            <div>
                            <!-- button -->
                            <div class="movebtn">
                                <button type="button" class="btn btn-fill btn-rose " data-toggle="modal" data-target="#utilisateur-dialog">Nouvel Utilisateur</button>
                                <%--<asp:Button ID="BtnAjouterUtilisateur" class="btn btn-fill btn-rose" runat="server" Text="Nouvel Utilisateur" data-dismiss="modal" data-target="#utilisateur-dialog"  />--%>
                            </div>
                            <br />
                        </div>

                        </div>
                    </div>
                </div>

            </div>

        </div>


    </div>
</asp:Content>
