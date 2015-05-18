<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BotUAC.aspx.cs" Inherits="BotUAC.WebFormBotUAC" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>BotUAC</title>   <!-- отображается на закладке в экплорере -->
    <style type="text/css">
        .FixedHeader {
            position: absolute;
            font-weight: normal;
            vertical-align:text-bottom;
        }      
    </style>
</head>
<body>
    <form id="form1" method="post" runat="server">
    
        <asp:Label ID="lblUserName" runat="server" Text="User"  
           style="position: absolute; top: 41px; left: 17px; width: 49px; "></asp:Label>

        <asp:TextBox ID="txtUserName" runat="server"  AutoPostBack="true" 
           style="position:absolute;top: 39px; left: 104px; width: 146px;" 
            Visible="false" OnTextChanged="txtUserName_TextChanged" >
           </asp:TextBox>

        <asp:Label ID="lblUserName_Error" runat="server"  
           style="position: absolute; top: 41px; left: 274px;" ForeColor="Red"
           ></asp:Label>

        <asp:DropDownList ID="cbxUserName" runat="server" AutoPostBack="True" 
                style="position:absolute; top: 39px; left: 104px; width: 150px;" 
            onselectedindexchanged="cbxUserName_SelectedIndexChanged">
        </asp:DropDownList>

        <asp:ImageButton ID="btnDelUser" style="position:absolute; top: 41px; left: 280px;"
        runat="server" ImageUrl="~/Resources/Button_Del_16x16.png" 
            onClientClick="return confirm('Are you sure you want to delete the user?')"
            onclick="btnDelUser_Click" 
            ToolTip="Delete current User" />
        
        <asp:ImageButton ID="btnAddUser" style="position:absolute; top: 40px; left: 259px;"
        runat="server" ImageUrl="~/Resources/Button_Add_16x16.png" 
            onclick="btnAddUser_Click" ToolTip="Add new User" />
        
        <asp:Label ID="lblAction" runat="server" Text="Action"  
            style="position: absolute; top: 74px; left: 18px; width: 49px; ">
            </asp:Label>
        
        <asp:DropDownList ID="cbxAction" runat="server" AutoPostBack="True" 
            style="position:absolute; top: 74px; left: 104px; width: 150px;" 
            onselectedindexchanged="cbxAction_SelectedIndexChanged">
            </asp:DropDownList>
            
        <asp:Panel ID="Panel1" style="position:absolute; border: 1px solid #000; 
            top: 112px; left: 18px; height: 372px; width: 347px;" runat="server" >
            
            <asp:Label ID="lblExtension"  Text="Extension" 
                style="position: absolute; top: 5px; left: 10px; width: 49px;"  runat="server" ></asp:Label>
                    
            <div style="position:absolute; overflow:scroll; height: 342px; top: 30px; left: -1px; width: 347px; border: 1px solid #000;">
                
                <asp:GridView ID="GridView1"  
                    CellPadding="5" CellSpacing="2"  AutoGenerateColumns="False"
                    Font-Names="Arial"  Font-Size="10pt"
                    HeaderStyle-CssClass="FixedHeader" 
                    ShowHeader="False"
                    AlternatingRowStyle-BackColor="WhiteSmoke" 
                    Height="100px"
                    Width="330px"
                    runat="server" BorderStyle="None">
                    
                    <PagerStyle   
                        BackColor="Crimson"   
                        ForeColor="Snow"   
                        Height="20"  
                        Font-Size="Large"   
                        HorizontalAlign= "Center"   
                        />  
                    <HeaderStyle Height="20" />              
                    <RowStyle Height="8" />              
                    <AlternatingRowStyle BackColor="WhiteSmoke"></AlternatingRowStyle>
                    
                    <Columns>
                        <asp:BoundField HeaderText="Id" DataField="Id" Visible="False" > 
                            <HeaderStyle Width="200px"></HeaderStyle>
                            <ItemStyle Width="200px"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Extension"  DataField="Extension" >
                            <HeaderStyle Width="600px"></HeaderStyle>
                            <ItemStyle Width="600px"></ItemStyle>
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Allow" HeaderStyle-Width="80px" >
                            <HeaderStyle Width="80px"></HeaderStyle>
                            <ItemStyle Width="80px"></ItemStyle>
                            <ItemTemplate> 
                                <asp:CheckBox ID="ckbAllow" 
                                    Checked='<%# Eval("Allow") %>'
                                    runat="server" AutoPostBack="true"
                                    oncheckedchanged="CheckBox1_CheckedChanged"                                    
                                    /> 
                            </ItemTemplate>                 
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Deny" >
                            <HeaderStyle Width="80px"></HeaderStyle>
                            <ItemStyle Width="80px"></ItemStyle>
                            <ItemTemplate> 
                                <asp:CheckBox ID="ckbDeny"  
                                    Checked='<%# Eval("Deny") %>'
                                    runat="server" AutoPostBack="true"
                                    oncheckedchanged="CheckBox1_CheckedChanged"                                    
                                    /> 
                            </ItemTemplate>                 
                        </asp:TemplateField>
                    </Columns>

                </asp:GridView>
             </div>


        </asp:Panel>

        <asp:Button ID="btnSave" runat="server" Text="Save" 
            style="position:absolute; top: 496px; left: 177px; width: 92px; right: 543px;" 
            onclick="btnSave_Click" ToolTip="Save User" />
                
        <asp:Button ID="btnClose" runat="server" Text="Close" 
                style="position:absolute; top: 496px; left: 273px; width: 92px;" />
    
        <asp:Button ID="btnCancel" runat="server" Text="Restore" 
                style="position:absolute; top: 496px; left: 81px; width: 92px;" 
                onclick="btnCancel_Click" ToolTip="Cancel change User" />
                
        <asp:Button ID="btnSaveNew" runat="server" Text="Save New" 
            style="position:absolute; top: 496px; left: 177px; width: 92px; right: 543px;" 
            onclick="btnSaveNew_Click" Visible="False" ToolTip="Save new User" />
                
        <asp:Button ID="btnCancelNew" runat="server" Text="Cancel New" 
                style="position:absolute; top: 496px; left: 81px; width: 92px;" 
                onclick="btnCancelNew_Click" Visible="False" 
            ToolTip="Cancel add new User" />
    
    
    
    
        <asp:Button ID="btnAllow" runat="server" Text="Allow" 
            style="position:absolute; top: 120px; left: 260px;" BackColor="White" 
            Font-Names="Arial"  Font-Size="10pt"
            BorderStyle="None" onclick="btnAllow_Click" />
            
        <asp:Button ID="btnDeny" runat="server" Text="Deny" 
            style="position:absolute; top: 120px; left: 306px;" BackColor="White" 
            Font-Names="Arial"  Font-Size="10pt"
            BorderStyle="None" onclick="btnDeny_Click" />
    
    
    </form>
</body>
</html>
