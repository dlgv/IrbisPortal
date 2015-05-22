<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BotUAC.aspx.cs" Inherits="BotUAC.WebFormBotUAC" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>BotUAC</title>   


    
    
    
    <style type="text/css">
        .FixedHeader {
            position: absolute;
            font-weight: normal;
            vertical-align:text-bottom;
        }      
    </style>
    <style type="text/css">
        .uppercase
        {
            text-transform: uppercase;
        }    
    </style>
</head>
<body>
    <form id="form1" method="post" runat="server">

    <div id="divError" 
        visible="false" 
        runat="server">

        <div id="divErrorMessage" 
            runat="server">
        
        </div>
        
    </div>
    
    <div id="divMain" runat="server">
    
            <asp:Label ID="lblUserName" runat="server" Text="User"  
               style="position: absolute; top: 41px; left: 17px; width: 49px; "></asp:Label>

            <asp:TextBox ID="txtUserName" runat="server"  AutoPostBack="true" 
               style="position:absolute;top: 39px; left: 104px; width: 146px;" 
                Visible="false" OnTextChanged="txtUserName_TextChanged"
                CssClass="uppercase" >
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
                onclick="btnDelUser_Click" />
            
            <asp:ImageButton ID="btnAddUser" style="position:absolute; top: 40px; left: 259px;"
            runat="server" ImageUrl="~/Resources/Button_Add_16x16.png" 
                onclick="btnAddUser_Click" />
            
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
                        
                    <asp:Button ID="btnAllow" runat="server" Text="Allow" 
                        style="position:absolute; top: 5px; left: 235px;" BackColor="White" 
                        Font-Names="Arial"  Font-Size="10pt"
                        BorderStyle="None" onclick="btnAllow_Click" />
                        
                    <asp:Button ID="btnDeny" runat="server" Text="Deny" 
                        style="position:absolute; top: 5px; left: 281px;" BackColor="White" 
                        Font-Names="Arial"  Font-Size="10pt"
                        BorderStyle="None" onclick="btnDeny_Click" />
                    
                        
                        
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
                            <asp:BoundField HeaderText="Id" DataField="Id" Visible="False"> 
                                <HeaderStyle Width="200px"></HeaderStyle>
                                <ItemStyle Width="200px"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField HeaderText="Extension"  DataField="Extension" >
                                <HeaderStyle Width="600px"></HeaderStyle>
                                <ItemStyle Width="600px"></ItemStyle>
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Allow"
                                HeaderStyle-Width="80px" >
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

            <asp:Button ID="btnCancelNew" runat="server" Text="Cancel New" 
                style="position:absolute; top: 496px; left: 176px; width: 92px;" 
                onclick="btnCancelNew_Click" Visible="False" />
                  
            <asp:Button ID="btnSaveNew" runat="server" Text="Save New" 
                style="position:absolute; top: 496px; left: 270px; width: 92px; right: 391px;" 
                onclick="btnSaveNew_Click" Visible="False" />
                    

            <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                style="position:absolute; top: 496px; left: 176px; width: 92px;" 
                onclick="btnCancel_Click" />
                    
            <asp:Button ID="btnSave" runat="server" Text="Save" 
                style="position:absolute; top: 496px; left: 270px; width: 92px; right: 391px;" 
                onclick="btnSave_Click"  />

            <!--
            <asp:CheckBox ID="ckbUpdated"  runat="server" AutoPostBack="true" Checked="false"  visible="true"  /> 
            -->

    </div>
    
                    
    </form>

    
    <body>
        <button onclick="GetOnClickAttrValue (this);" >Get the value of the onclick attribute!</button>
        <button onclick="SetOnClickAttrValue (this);" >Set the value of the onclick attribute!</button>
    </body>


    <script type="text/javascript" language="JavaScript">
    // If changes not saved, prompt user
    window.onbeforeunload = confirmExit;
    function confirmExit() {
        var elem = document.getElementById("btnSave");
        if (elem)
            //if (elem.type == 'submit')  
            if (elem.type == 'submit' && elem.disabled == true)  
                return  // закрытие без подтверждения ================>
            else                
                return 'You have unsaved changes.' // закрытие с подтвержденим ======> 
    }
    </script>
    


    
    
    
    
    
    <script type="text/javascript">
        //http://help.dottoro.com/ljuurooj.php
        function GetOnClickAttrValue (button) {
            //alert (button.getAttribute ("onCliCk", 0));
            //alert (button.getAttribute ("onCliCk", 1));        // Internet Explorer (case-sensitive search, returns null)

            str = document.getElementById( "GridView1_ctl02_ckbDeny" ).getAttribute ("onCliCk", 0)
            alert(str);

            /*        
            var str;
            var str2;
            // onclick="javascript:setTimeout('__doPostBack(\'GridView1$ctl03$ckbDeny\',\'\')', 0)"
            str = document.getElementById( "GridView1_ctl02_ckbDeny" ).getAttribute ("onCliCk", 0)
            var ind = str.indexOf(":");
            str2 = str.substring(0, ind+1) + "setBunload(false);" + str.substring(ind+2);
            alert(str2);
            //alert (document.getElementById( "GridView1_ctl02_ckbDeny" ).getAttribute ("onCliCk", 1));        // Internet Explorer (case-sensitive search, returns null)
            */
        }
        
        function SetOnClickAttrValue (button) {
            //alert (button.getAttribute ("onCliCk", 0));
            //alert (button.getAttribute ("onCliCk", 1));        // Internet Explorer (case-sensitive search, returns null)

            /*
            // работает:
            str = document.getElementById( "GridView1_ctl02_ckbDeny" ).getAttribute ("onCliCk", 0)
            var ind = str.indexOf(":");
            str2 = str.substring(0, ind+1) + "setBunload(false);" + str.substring(ind+1);
            document.getElementById( "GridView1_ctl02_ckbDeny" ).setAttribute ("onCliCk", str2)
            */
            
        	var str;
        	var str2;
            var ind;
            var idElem;
        	var elems = document.getElementsByTagName('*');
        	//alert("Элементов: " + elems.length)
            var bRavno = false;
            var bRavno1 = false;
            var bRavno2 = false;
            for(var i=0; i < elems.length; i++)
            {
                idElem = elems[i].getAttribute('id');
                idType = elems[i].getAttribute('type');
                idName = elems[i].getAttribute('name');
                if (idName != null)
                {

                   bRavno1 = (idElem.substr(idElem.length - 8) == "ckbAllow");
                   bRavno2 = (idElem.substr(idElem.length - 7) == "ckbDeny");
                   if ((idName == "cbxUserName") || (idName == "cbxAction") ||
                       (idName == "btnAddUser") || (idName == "btnDelUser") ||
                       (idName == "btnSave") || (idName == "btnCancel") ||
                       (idName == "btnSaveNew") || (idName == "btnCancelNew") ||
                       bRavno1 || bRavno2 || idName == "txtUserName")
                   {
                        //alert("idName = " + idName);
		                str = elems[i].getAttribute("onClick", 0);
		                if (str != null)
		                {
                            //alert("idName = " + idName);
                            ind = str.indexOf(":");
                            str2 = str.substring(0, ind+1) + "setBunload(false);" + str.substring(ind+1);
                            //alert(str2);
		                    elems[i].setAttribute("onClick", str2);
		                }
                   }
                }

//                if (idType == "checkbox" || idType == "submit"  || idType == "image" )                  
//                {
//                    //alert("idElem = " + idElem);
//                    //alert("idName = " + idName);
//                    if (idElem == "GridView1_ctl02_ckbDeny")
//                    {
//                       // alert("Type = " + idType);
//                        //alert("idElem = " + idElem);
//                        //alert("idElem = '" + idElem.substr(idElem.length - 7) + "'");
//                        //alert("idElem равно = " + (idElem.substr(idElem.length - 7) == "ckbDeny"));
//                        //bRavno = (idElem.substr(idElem.length - 7) == "ckbDeny");
//                        //alert("idElem равно = " + bRavno);
//                    }
//                }
//                
//                    bRavno = (idElem.substr(idElem.length - 7) == "ckbDeny");
//                    alert("idElem равно = " + bRavno);

                
//                var bRavno = false;
//                bRavno = (idElem.substr(idElem.length - 7) == "ckbDeny");
//                             (idElem.substr(idElem.length - 8) == "ckbAllow");
                //if (idElem.substr(idElem.length - 7) == "ckbDeny")
                
//                    alert("Substr idElem = " + idElem);
                
//                if (1 == 1)
//                {
//                    alert("Substr idElem = " + idElem);
//                    //alert("Substr idElem = " + idElem.substr(idElem.length - 7));
//                }
//                
                
                /*
                if (idElem.substr(idElem.length - 7) == "ckbDeny")
                {
                    alert("idElem = " + idElem);
		            str = elems[i].getAttribute("onClick", 0);
                    ind = str.indexOf(":");
                    str2 = str.substring(0, ind+1) + "setBunload(false);" + str.substring(ind+1);
                    alert(str2);
		            elems[i].setAttribute("onClick", str2);
                    //alert("str2 = " + str2);
                }
                */
                
            } // for

        } // SetOnClickAttrValue(button)
        
    </script>    


    <script type="text/javascript" language="JavaScript">
        // !!! скрипт расположить в конце страницы - чтобы выполнялся после формирования контролов !!!
        
        function setOnClientClick() {
            //--- работает:
            //document.getElementById( "GridView1_ctl02_ckbDeny" ).setAttribute( "onClick", "javascript: Boo2('QQQ');" );

            //document.getElementById( "GridView1_ctl02_ckbDeny" ).setAttribute( "onClick", "javascript: Boo2('" + 
            //        document.getElementById( "GridView1_ctl02_ckbDeny" ).getAttribute("onClick", 2)
            //+ "');" );
            
            alert("Выхвали !");
            
        	var str;
        	var str2;
            var ind;
            var idElem;
        	var elems = document.getElementsByTagName('*');
        	//alert("Элементов: " + elems.length)
            var bRavno = false;
            var bRavno1 = false;
            var bRavno2 = false;
            for(var i=0; i < elems.length; i++)
            {
                idElem = elems[i].getAttribute('id');
                idType = elems[i].getAttribute('type');
                idName = elems[i].getAttribute('name');
                if (idName != null)
                {

                   bRavno1 = (idElem.substr(idElem.length - 8) == "ckbAllow");
                   bRavno2 = (idElem.substr(idElem.length - 7) == "ckbDeny");
                   if ((idName == "cbxUserName") || (idName == "cbxAction") ||
                       (idName == "btnAddUser") || (idName == "btnDelUser") ||
                       (idName == "btnSave") || (idName == "btnCancel") ||
                       (idName == "btnSaveNew") || (idName == "btnCancelNew") ||
                       bRavno1 || bRavno2 || idName == "txtUserName")
                   {
                        //alert("idName = " + idName);
		                str = elems[i].getAttribute("onClick", 0);
		                if (str != null)
		                {
                            //alert("idName = " + idName);
                            ind = str.indexOf(":");
                            str2 = str.substring(0, ind+1) + "setBunload(false);" + str.substring(ind+1);
                            //alert(str2);
		                    elems[i].setAttribute("onClick", str2);
		                }
                   }
                }
            }
        }    

        setOnClientClick();
        
    </script>

    
    
</body>

</html>
