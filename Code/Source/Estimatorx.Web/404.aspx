﻿<%@ Page Language="C#" AutoEventWireup="true" %>
<% 
Response.StatusCode = 404;
string rootUrl = VirtualPathUtility.ToAbsolute("~");
if (rootUrl == "/") rootUrl = "";
%>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <title>404 - Not Found</title>
    <link href="<%=rootUrl %>/favicon.ico" rel="shortcut icon" type="image/x-icon" />

    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">

    <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/bootstrap/3.3.2/css/bootstrap.min.css">
    <style type="text/css">
        body {
            padding-top: 70px;
            padding-bottom: 20px;
        }

        footer, #footer {
            color: #999;
            text-align: center;
            line-height: normal;
            margin: 0 0 1em 0;
            font-size: .9em;
        }

        #section-main {
            min-height: 300px;
        }
    </style>
</head>
<body>
    <div class="navbar navbar-default navbar-fixed-top" role="navigation">
        <div class="container">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <a class="navbar-brand" href="<%=rootUrl %>/">Estimatorx</a>
            </div>
            <div class="navbar-collapse collapse">
                <ul class="nav navbar-nav">
                    <li><a href="<%=rootUrl %>/">Home</a></li>
                    <li><a href="<%=rootUrl %>/Project">Projects</a></li>
                    <li><a href="<%=rootUrl %>/Template">Templates</a></li>
                </ul>
            </div>
        </div>
    </div>

    <section id="section-main">
        <div class="container">
            <div class="jumbotron">
                <h2 class="text-danger">404 - Page Not Found</h2>
                <p>We are sorry, the page you requested cannot be found.</p>
            </div>
        </div>
    </section>

    <div class="container">
        <hr />
        <footer id="footer" class="row">
            <div class="col-md-4"></div>
            <div class="col-md-4">
                <span>© Copyright LoreSoft 2015. All Rights Reserved.</span>
            </div>
            <div class="col-md-4"></div>
        </footer>
    </div>

    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js"></script>
    <script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.2/js/bootstrap.min.js"></script>
</body>
</html>