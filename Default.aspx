<%@ Page Title="Download Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>SafeShare Demo</h1>
                <table>
  <tr>
    <th>  <asp:Button id="b1" Text="Download - Sample Video (422MB) " runat="server" OnClick="DownloadFileOPeration" /></th>
      
      <th>  <asp:Button id="Button1" Text="Download -(4.84 GB) " runat="server" OnClick="DownloadFileOPeration4GB" /></th>
  </tr>
  <tr>
      <tr>
          <asp:pro
          <th>
          <td> <asp:Label runat="server" ID="download">File Download in Progress</asp:Label></td>
      </th>
      </tr>
      
    
      <td>   <asp:Label runat="server" ID="filesize">File Size</asp:Label></td>
      <td>  <asp:Label runat="server" ID="Label2"></asp:Label>
</td>
      <td>  <asp:Label runat="server" ID="Label3">Downloaded Bytes</asp:Label></td>
      <td> <asp:Label runat="server" ID="Label4"></asp:Label></td>
  </tr>
</table>
     
       
          
     

       
       
         
       
    </div>
</asp:Content>
