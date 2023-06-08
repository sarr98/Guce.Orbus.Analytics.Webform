﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Analytics.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Orbus Analytics - Login</title>
	<meta charset="UTF-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1" />
<!--===============================================================================================-->	
	<link rel="icon" type="image/png" href="LoginDesign/images/icons/favicon.ico"/>
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="LoginDesign/vendor/bootstrap/css/bootstrap.min.css" />
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="LoginDesign/fonts/font-awesome-4.7.0/css/font-awesome.min.css" />
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="LoginDesign/fonts/Linearicons-Free-v1.0.0/icon-font.min.css" />
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="LoginDesign/vendor/animate/animate.css" />
<!--===============================================================================================-->	
	<link rel="stylesheet" type="text/css" href="LoginDesign/vendor/css-hamburgers/hamburgers.min.css" />
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="LoginDesign/vendor/animsition/css/animsition.min.css" />
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="LoginDesign/vendor/select2/select2.min.css" />
<!--===============================================================================================-->	
	<link rel="stylesheet" type="text/css" href="LoginDesign/vendor/daterangepicker/daterangepicker.css" />
<!--===============================================================================================-->
	<link rel="stylesheet" type="text/css" href="LoginDesign/css/util.css" />
	<link rel="stylesheet" type="text/css" href="LoginDesign/css/main.css" />
<!--===============================================================================================-->
</head>
<body>
	
	<div class="limiter">
		<div class="container-login100">
			<div class="wrap-login100">
				<div class="login100-form-title" style="background-image: url(images/accueil2.jfif);">
                    <%--<div class="login100-form-title" style="background-image: url(LoginDesign/images/log-03.jpg);">--%>
					<span class="login100-form-title-1">
						Orbus Analytics
					</span>
				</div>

				<form id="Form1" class="login100-form validate-form" runat="server">
					<div class="wrap-input100 validate-input m-b-26" data-validate="Username is required">
						<span class="label-input100">Utilisateur</span>
						<input class="input100" type="text" id="TextBox1" name="TextBox1" placeholder="Entrez un login " />
						<span class="focus-input100"></span>
					</div>

					<div class="wrap-input100 validate-input m-b-18" data-validate = "Password is required">
						<span class="label-input100">Mot de passe</span>
						<input class="input100" type="password" id="TextBox2" name="TextBox2" placeholder="Entrez un mot de passe " />
						<span class="focus-input100"></span>
					</div>

					<div class="flex-sb-m w-full p-b-30">
						<div class="contact100-form-checkbox">
							<%--<input class="input-checkbox100" id="ckb1" type="checkbox" name="remember-me" />--%>
							<label style="color: red" id="labelMsg" runat="server">
								
							</label>
						</div>

						<%--<div>
							<a href="#" class="txt1">
								Mot de passe oublié?
							</a>
						</div>--%>
					</div>

					<div class="container-login100-form-btn">
                        <input type="button" id="Button1" name="btnValider" onserverclick ="Button1_Click" value="Se connecter" class="login100-form-btn" runat="server" />
						<%--<button class="login100-form-btn">
							Login
						</button>--%>
					</div>
				</form>
			</div>
		</div>
	</div>

	
<!--===============================================================================================-->
	<script src="LoginDesign/vendor/jquery/jquery-3.2.1.min.js"></script>
<!--===============================================================================================-->
	<script src="LoginDesign/vendor/animsition/js/animsition.min.js"></script>
<!--===============================================================================================-->
	<script src="LoginDesign/vendor/bootstrap/js/popper.js"></script>
	<script src="LoginDesign/vendor/bootstrap/js/bootstrap.min.js"></script>
<!--===============================================================================================-->
	<script src="LoginDesign/vendor/select2/select2.min.js"></script>
<!--===============================================================================================-->
	<script src="LoginDesign/vendor/daterangepicker/moment.min.js"></script>
	<script src="LoginDesign/vendor/daterangepicker/daterangepicker.js"></script>
<!--===============================================================================================-->
	<script src="LoginDesign/vendor/countdowntime/countdowntime.js"></script>
<!--===============================================================================================-->
	<script src="LoginDesign/js/main.js"></script>
    
</body>
</html>
