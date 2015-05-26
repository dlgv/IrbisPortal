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
            
            <asp:Panel ID="panGrid" style="position:absolute; 
                top: 112px; left: 18px; height: 250px; width: 347px;" runat="server" 
                BorderStyle="None" >
                <!-- style="position:absolute; border: 1px solid #000;  -->
                <!-- панель для сетки с "заголовком" -->                        
            
                <asp:Panel ID="panGridHead" style="position:absolute; border: 1px solid #000; 
                    top: 1px; left: 1px; height: 26px; width: 347px;" runat="server">
                    <!-- панель для рамочки под "шапкой" сетки -->                        

                    <asp:Label ID="lblExtension"  Text="Extension" 
                        style="position: absolute; top: 5px; left: 10px; width: 49px;"  runat="server" ></asp:Label>
                            
                    <asp:Button ID="btnAllow" runat="server" Text="Allow" 
                        style="position:absolute; top: 5px; left: 247px;" BackColor="White" 
                        Font-Names="Arial"  Font-Size="10pt"
                        BorderStyle="None" onclick="btnAllow_Click" />
                        
                    <asp:Button ID="btnDeny" runat="server" Text="Deny" 
                        style="position:absolute; top: 5px; left: 290px;" BackColor="White" 
                        Font-Names="Arial"  Font-Size="10pt"
                        BorderStyle="None" onclick="btnDeny_Click" />

                </asp:Panel>
                
                <asp:Panel ID="panGridRows" style="position:absolute;  
                    top: 30px; left: 1px; height: 200px; width: 347px;" runat="server" 
                     BorderStyle="None">
                    
                    <asp:GridView ID="GridView1"  
                        CellPadding="5" CellSpacing="2"  AutoGenerateColumns="False"
                        Font-Names="Arial"  Font-Size="10pt"
                        HeaderStyle-CssClass="FixedHeader" 
                        ShowHeader="False"
                        AlternatingRowStyle-BackColor="WhiteSmoke" 
                        Height="100px"
                        Width="349px"
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
                </asp:Panel>


            </asp:Panel>
            
            <asp:Button ID="btnCancelNew" runat="server" Text="Cancel New" 
                style="position:absolute; top: 112px; left: 378px; width: 92px;" 
                onclick="btnCancelNew_Click" Visible="False" />
                  
            <asp:Button ID="btnSaveNew" runat="server" Text="Save New" 
                style="position:absolute; top: 144px; left: 377px; width: 92px; right: 499px;" 
                OnClientClick="setOnBeforeUnload(false)"
                onclick="btnSaveNew_Click" Visible="False" />
                  

            <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                style="position:absolute; top: 112px; left: 378px; width: 92px;" 
                OnClientClick="setOnBeforeUnload(false)"
                onclick="btnCancel_Click" />
                    
            <asp:Button ID="btnSave" runat="server" Text="Save" 
                style="position:absolute; top: 144px; left: 377px; width: 92px; right: 499px;" 
                onclick="btnSave_Click"  />

            <asp:Label ID="lblSave_Error" runat="server"  
               style="position: absolute; top: 144px; left: 475px;" ForeColor="Red"
               ></asp:Label>

            <!--
            <asp:CheckBox ID="ckbUpdated"  runat="server" AutoPostBack="true" Checked="false"  visible="true"  /> 
            -->

    </div>
    
                    
                    

    </form>

    

    <script type="text/javascript" language="JavaScript">
        // !!! скрипт расположить в конце страницы - чтобы выполнялся после формирования контролов !!!
        
        //=====================
        //=== подтверждающий запрос при закрытии измененного окна
        
        // включение.выключение подтверждающего запроса при закрытии измененного окна
        //window.onbeforeunload = confirmExit;
        function setOnBeforeUnload(on) {
            window.onbeforeunload = (on) ? confirmExit : null;
        }
        
        // сам подтверждающий запрос при закрытии измененного окна
        function confirmExit() {
            var elem = document.getElementById("btnSave"); // недоступная кнопка Save - признак неизмененности данных!
            if (elem) {
                if (elem.type == 'submit' && elem.disabled == true)  
                    return  // закрытие без подтверждения ================>
                else                
                    return 'You have unsaved changes.' // закрытие с подтвержденим ======> 
            }
        }
        
        // сразу при загрузке включаем подтверждающий запрос при закрытии измененного окна
        setOnBeforeUnload(true)
        
        // т.к. серверные контролы при срабатывании выгружают страницу, то в каждом из них надо
        // поставить от ключение подтверждающего запроса (иначе запрос будет при срабатывании каждого контрола) !
        // (в атрибут OnClick добавляем на первое место вызов ф-ии отключения запоса)
        function Set_OnClickAttrValue()
        {
            //alert ("SetOnClickAttrValue()");

            /*
            // работает:
            str = document.getElementById( "GridView1_ctl02_ckbDeny" ).getAttribute ("onCliCk", 0)
            var ind = str.indexOf(":");
            str2 = str.substring(0, ind+1) + "setOnBeforeUnload(false);" + str.substring(ind+1);
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
                    // изменяем/добавляем атрибуты контролов
                    //   id="GridView1_ctl03_ckbDeny"   - IIS на Windows XP
                    //   id="GridView1_ckbDeny_1"       - IIS на Windows Server 2012 
                    //   !!! разные методики формирования ID - искать вхождение подстроки !!!
                    //bRavno1 = (idElem.substr(idElem.length - 8) == "ckbAllow");
                    //bRavno2 = (idElem.substr(idElem.length - 7) == "ckbDeny");
                    bRavno1 = (idElem.indexOf("ckbAllow") + 1);  // indexOf() возвращает если не найдено -1, если найдено - позицию вхождения. В js 0==false, любое др. число даёт true; 
                    bRavno2 = (idElem.indexOf("ckbDeny") + 1);   // indexOf() возвращает если не найдено -1, если найдено - позицию вхождения. В js 0==false, любое др. число даёт true;
                    if ((idName == "cbxUserName") || (idName == "cbxAction") ||
                       (idName == "btnAllow") || (idName == "btnDeny") ||
                       (idName == "btnAddUser") || (idName == "btnDelUser") ||
                       (idName == "btnSave") || (idName == "btnCancel") ||
                       (idName == "btnSaveNew") || (idName == "btnCancelNew") ||
                        bRavno1 || bRavno2 || idName == "txtUserName")
                    {
                        //alert("2: idName = " + idName);
	                    str = elems[i].getAttribute("onClick", 0);
	                    if (str != null)
	                    {
                            // контролы (списки, чекбоксы) с атрибутом - корректируем атрибут (вставляем после начального "javascript:")
                            //alert("idName = " + idName);
                            ind = str.indexOf(":");
                            str2 = str.substring(0, ind+1) + "setOnBeforeUnload(false);" + str.substring(ind+1);
                            //alert(str2);
	                        elems[i].setAttribute("onClick", str2);
	                    } else {
                            // контролы (кнопки) без атрибута - добавляем атрибут
	                        elems[i].setAttribute("onClick", "javascript:setOnBeforeUnload(false);");
	                    }
                    }
                }
            } // for по элементам документа
        } // Set_OnClickAttrValue()
    
        // доопределяем OnClisk своих серверных контролов
        Set_OnClickAttrValue();
    
    </script>
    


    
</body>

</html>
