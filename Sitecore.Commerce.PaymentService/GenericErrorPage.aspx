<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GenericErrorPage.aspx.cs" Inherits="Sitecore.Commerce.PaymentService.GenericErrorPage" %>

<!--
// Copyright 2016 Sitecore Corporation A/S 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at 
//       http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License. 
-->

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="background-color:white">
    <form id="ErrorForm" runat="server">
    <div>
        <label id="ErrorLabel"><asp:Literal runat="server" Text="<%$ Resources:WebResources, GenericErrorPage_ErrorLabel %>"/></label>
    </div>
    </form>
</body>
</html>
