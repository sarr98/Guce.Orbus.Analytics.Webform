<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Accueil.aspx.cs" Inherits="Guce.Orbus.Analytics.Accueil" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <div class="row">
                    <div class="col-lg-3 col-md-6 col-sm-6">
                        <div class="card card-stats">
                            <div class="card-header card-header-warning card-header-icon">
                                <div class="card-icon">
                                    <i class="material-icons">group_add</i>
                                </div>
                                <p class="card-category">Utilisateurs</p>
                                <h3 class="card-title"><asp:Label ID="totalUsers"  runat="server" /></h3>
                            </div>
                            <div class="card-footer">
                                <div class="stats">
                                    <i class="material-icons text-danger">group_add</i>
                                    <a href="#">Voir détails</a>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-3 col-md-6 col-sm-6">
                        <div class="card card-stats">
                            <div class="card-header card-header-rose card-header-icon">
                                <div class="card-icon">
                                    <i class="material-icons">bar_chart</i>
                                </div>
                                <p class="card-category">Dossiers Jour</p>
                                <h3 class="card-title"><asp:Label ID="totalDossiers"  runat="server" /></h3>
                            </div>
                            <div class="card-footer">
                                <div class="stats">
                                    <i class="material-icons">bar_chart</i> <a href="#">Dossiers</a>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-3 col-md-6 col-sm-6">
                        <div class="card card-stats">
                            <div class="card-header card-header-success card-header-icon">
                                <div class="card-icon">
                                    <i class="material-icons">bar_chart</i>
                                </div>
                                <p class="card-category">Total Transactions</p>
                                <h3 class="card-title"><asp:Label ID="totalTransactions"  runat="server" /></h3>
                            </div>
                            <div class="card-footer">
                                <div class="stats">
                                    <i class="material-icons">bar_chart</i><a href="#">Statistiques internes</a>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-3 col-md-6 col-sm-6">
                        <div class="card card-stats">
                            <div class="card-header card-header-info card-header-icon">
                                <div class="card-icon">
                                    <i class="material-icons">search</i>
                                </div>
                                <p class="card-category"> Guides</p>
                                <h3 class="card-title">1</h3>
                            </div>
                            <div class="card-footer">
                                <div class="stats">
                                    <i class="material-icons"></i><a href="#">Voir détails</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
</asp:Content>
