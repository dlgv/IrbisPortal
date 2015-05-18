
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
//---------------
using System.IO;                    // для FileStream 
using System.Xml;
using System.Collections.Generic;   // List<>

namespace BotUAC   
{
    public partial class WebFormBotUAC : System.Web.UI.Page
    {

        private List<PermRow> GridData = new List<PermRow>();

        private const int nColUserAllow = 2; // с 0 !
        private const int nColUserDeny = 3; // с 0 !

        private TAppSettings appSet = null;
        private TPermissionsSettings permSet = null; // нельзя здесь взять имя файла из настроек приложеия (сделано ниже)!

        private TUser userCurrent = null;   // указатель в общем списке (первоначальные данные)
        private TUser userModify = null;    // копия корректируемая
        private TUser userCurrentBeforeAdd = null;   // \  на момент добавления нового пользователя
        private TUser userModifyBeforeAdd = null;    // /

        private string actionModify = null; // копия имени корректируемой акции

        bool bUpdateScr = false;
        string message = null;


        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)  // первоначальная загрузка стараницы
            {

                // текущая аутентификация:
                //Response.Write("Authentication: Authenticated = " + User.Identity.IsAuthenticated.ToString() + ", User = " + User.Identity.Name + ", Type = " + User.Identity.AuthenticationType);  

                //string sMain = HttpContext.Current.Server.MapPath("~/App_Data");
                //string s2 = HostingEnvironment.ApplicationPhysicalPath;
                string sMain = HttpContext.Current.Server.MapPath("~");         // косые в конце уже есть !!!

                //----------------------------------
                // вычитываем документ
                appSet = new TAppSettings(sMain + "Irbis.xml");
                if (!appSet.Load())
                {
                    Response.Write("Error application config-file load: " + appSet.FileNameFull);  // "<br>" + 
                    return; //======================>
                }
                permSet = new TPermissionsSettings(appSet.PermissionsFilePath);  // после ЗАГРУЗКИ appSet !!!
                if (!permSet.Load())
                {
                    Response.Write("Error permissions config-file load: " + permSet.FileNameFull); // "<br>" + 
                    return; //======================>
                }

                // свойства контролов по умолчанию
                txtUserName.Visible = false;
                lblUserName_Error.Visible = false;

                btnCancelNew.Visible = false;
                btnSaveNew.Visible = false;

                btnCancelNew.Text = "Cancel";
                btnSaveNew.Text = "Save";

                // UserName - выпадаюший список
                cbxUserName.Items.Clear();
                foreach (TUser user in permSet.Users)
                {
                    cbxUserName.Items.Add(user.UserName);
                }
                // сортируем элементы списка Имен
                SortCbxItems(cbxUserName);

                /*
                // работает:
                List<string> aTmp = new List<string>();
                foreach (TUser user in permSet.Users)
                {
                    aTmp.Add(user.UserName);
                }
                aTmp.Sort();
                foreach (string str in aTmp)
                {
                    cbxUserName.Items.Add(str);
                }
                */ 
                  
                if (cbxUserName.Items.Count > 0)  // на первый элемент списка
                {
                    cbxUserName.SelectedIndex = 0; // c 0 !?
                }
                if (cbxUserName.Items.Count == 0)  // не должно быть
                {
                    Response.Write("Error permissions config-file - empty Users list: " + permSet.FileNameFull); // "<br>" + 
                    return; //======================>
                }

                // Операции - выпадаюший список
                cbxAction.Items.Clear();
                foreach (TAction act in permSet.Actions)
                {
                    cbxAction.Items.Add(act.ActionName);
                }
                if (cbxAction.Items.Count > 0)  // на первый элемент списка
                {
                    cbxAction.SelectedIndex = 0; // c 0 !?
                    actionModify = cbxAction.Text;   // !!! надо здесь - не срабатывает событие смены индекса !!!
                }
                if (cbxAction.Items.Count == 0)  // не должно быть
                {
                    Response.Write("Error permissions config-file - empty Actionы list: " + permSet.FileNameFull); // "<br>" + 
                    return; //======================>
                }

                // Разрешения - строки сетки
                PermRow permRow;
                foreach (TExtension ext in permSet.Extensions) // добавляем строки в сетку
                {
                    permRow = new PermRow(ext.ExtensionId, ext.ExtensionName, false, false);
                    this.GridData.Add(permRow);
                }
                GridView1.DataSource = this.GridData;
                GridView1.DataBind();

                // обновляем данные на экране для нового тек. пользователя, назначаем modify !
                UserSet(cbxUserName.Text);


                //----------------------------------------
                // сохраняем переменные формы для след.загрузки !!!
                // !!! сбой при вычитке после потери сеиии по таймауту !!!
                Session["appSet"] = appSet;  
                Session["permSet"] = permSet;
                Session["userCurrent"] = userCurrent;
                Session["userModify"] = userModify;
                Session["userCurrentBeforeAdd"] = userCurrentBeforeAdd;
                Session["userModifyBeforeAdd"] = userModifyBeforeAdd;
                Session["actionModify"] = actionModify;
                Session["bUpdateScr"] = bUpdateScr;
                Session["message"] = message;

                //ViewState.Add("appSet", appSet);
                //ViewState.Add("permSet", permSet);
                //ViewState.Add("userCurrent", userCurrent);
                //ViewState.Add("userModify", userModify);
                //ViewState.Add("actionModify", actionModify);
                //ViewState.Add("bUpdateScr", bUpdateScr);
                //ViewState.Add("message", message);

                // восстанавливаем состояние кнопок (после восстановлеиня из формы!)
                SetUpdateScr(bUpdateScr);

            }
            else  // не первая загрузка страницы
            {
                //----------------------------------------
                // восстанавливаем переменные формы от предыд.загрузки !!!
                // !!! сбой при вычитке после потери сеиии по таймауту !!!
                if (Session["appSet"] == null)   // сессия закрылась по таймауту - просто берем текущие ?
                {
                    Session["appSet"] = appSet;
                    Session["permSet"] = permSet;
                    Session["userCurrent"] = userCurrent;
                    Session["userModify"] = userModify;
                    Session["userCurrentBeforeAdd"] = userCurrentBeforeAdd;
                    Session["userModifyBeforeAdd"] = userModifyBeforeAdd;
                    Session["actionModify"] = actionModify;
                    Session["bUpdateScr"] = bUpdateScr;
                    Session["message"] = message;
                }
                appSet = (TAppSettings)Session["appSet"];   
                permSet = (TPermissionsSettings)Session["permSet"];
                userCurrent = (TUser)Session["userCurrent"];
                userModify = (TUser)Session["userModify"];
                userCurrentBeforeAdd = (TUser)Session["userCurrentBeforeAdd"];
                userModifyBeforeAdd = (TUser)Session["userModifyBeforeAdd"];
                actionModify = (string)Session["actionModify"];
                bUpdateScr = (bool)Session["bUpdateScr"];
                message = (string)Session["message"];

                // !!! ViewState - требует сериализации объектов (доделать типы!)
                //appSet = (TAppSettings)ViewState["appSet"]; 
                //permSet = (TPermissionsSettings)ViewState["permSet"];
                //userCurrent = (TUser)ViewState["userCurrent"];
                //userModify = (TUser)ViewState["userModify"];
                //actionModify = (string)ViewState["actionModify"];
                //bUpdateScr = (bool)ViewState["bUpdateScr"];
                //message = (string)ViewState["message"];

                // восстанавливаем состояние кнопок (после восстановлеиня из формы!)
                SetUpdateScr(bUpdateScr);

            }  // первая загрузка . нет 

        }


        protected void Page_Unload(object sender, EventArgs e)
        {

            //----------------------------------------
            // сохраняем переменные формы для след.загрузки !!!
            Session["appSet"] = appSet;
            Session["permSet"] = permSet;
            Session["userCurrent"] = userCurrent;
            Session["userModify"] = userModify;
            Session["userCurrentBeforeAdd"] = userCurrentBeforeAdd;
            Session["userModifyBeforeAdd"] = userModifyBeforeAdd;
            Session["actionModify"] = actionModify;
            Session["bUpdateScr"] = bUpdateScr;
            Session["message"] = message;

            // !!! ViewState - требует сериализации объектов (доделать типы!)
            //ViewState["appSet"] = appSet;
            //ViewState["permSet"] = permSet;
            //ViewState["userCurrent"] = userCurrent;
            //ViewState["userModify"] = userModify;
            //ViewState["actionModify"] = actionModify;
            //ViewState["bUpdateScr"] = bUpdateScr;
            //ViewState["message"] = message;

        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            // переносим текущее состояние пользователя c экрана в объект-модификации (если не сменили на экране пользователя !)
            ApplyUser();  // берем Роль и Разрешения сетки с экрана в модифицируемого 

            // само сохранение
            UserSave(false);   // не удаление пользователя
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RestoreUser(cbxUserName.Text);
        }

        protected void btnCancelNew_Click(object sender, EventArgs e)
        {
            // отказались от ввода нового пользователя - просто возвражаемся к коррекции списка пользователей
            // прячем поле ввода имени
            txtUserName.Visible = false;
            lblUserName_Error.Visible = false;
            // прячем кнопки Нового пользователя
            btnCancelNew.Visible = false;
            btnSaveNew.Visible = false;
            // показываем список
            cbxUserName.Visible = true;
            btnAddUser.Visible = true;
            btnDelUser.Visible = true;
            btnCancel.Visible = true;
            btnSave.Visible = true;

            // восстанавливаем пользователя, бывшего перед добавлением
            userCurrent = userCurrentBeforeAdd;
            userModify = userModifyBeforeAdd;
            userCurrentBeforeAdd = null;    // \ уничтожаем
            userModifyBeforeAdd = null;     // /
            // выводим на экран все данные МОДИФИЦИРУЕМОГО пользователя
            RefreshScreenForUser();
            // активируем список имени
            //cbxUserName.Focus();
        }


        // восстанавливаем первоначальные данные пользователя
        private void RestoreUser(string UserName)
        {
            // ищем тек.пользователя в общем списке Объекта
            TUser user = permSet.Users.FindUser(UserName);
            if (user != null)  // нашли пользователя 
            {
                // копируем исходного пользователя в модифицируемого
                userModify = user.Clone();
                // выводим на экран все данные МОДИФИЦИРУЕМОГО пользователя
                RefreshScreenForUser();
            }
            else
            {
                Response.Write("Restore Error : user " + UserName + " not found!"); 
            }
        }



        protected void btnAddUser_Click(object sender, ImageClickEventArgs e)
        {

            // переносим текущее состояние пользователя c экрана в объект-модификации (если не сменили на экране пользователя !)
            ApplyUser();  // берем Роль и Разрешения сетки с экрана в модифицируемого 

            // перед добавлением нового пользователя проверяем измененность атрибутов старого пользователя 
            if (bUpdateScr)
            {
                //if (MessageBox.Show("Data changed. Save User " + userCurrent.UserName + " ?", "Save User", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    UserSave(false);   // не удаление пользователя
                }
            }

            // сохраняем тек.пользователя на момент добавления нового - для Cancel !
            userCurrentBeforeAdd = userCurrent;
            userModifyBeforeAdd = userModify;

            // прячем список и кнопки
            cbxUserName.Visible = false;
            btnAddUser.Visible = false;
            btnDelUser.Visible = false;
            btnCancel.Visible = false;
            btnSave.Visible = false;
            // показываем кнопки Нового пользователя
            btnCancelNew.Visible = true;
            btnSaveNew.Visible = true;
            // открываем текст ввод нового имени - вместо выпадающего списка
            txtUserName.Visible = true;
            lblUserName_Error.Visible = true;

            //---------------
            // создаем нового пользователя для коррекции
            userCurrent = null;   // новый - еще нет в списке !
            // userModify = new TUser();   - может из-за пустого слетало в ApplyUser() на имени пользоватлея !?
            userModify = new TUser("", "", new TPermissions());  
            //---------------

            // очищаем поля формы - разрешения
            txtUserName.Text = "";      // иначе останется от прошлого ввода !
            lblUserName_Error.Text = "";
            // выводим на экран все данные МОДИФИЦИРУЕМОГО пользователя
            RefreshScreenForUser();
            // доступность кнопок нового пользователя  - от заполненности имени 
            btnSaveNew.Enabled = (txtUserName.Text.Trim().Length > 0);
            // активируем ввод имени
            //txtUserName.Focus();

        }

        //----------------------------------------------------------
        // сохраняем состояние пользователя на экрана в объекте-модификации 
        //  !!! берем Роль и Разрешения сетки с экрана в модифицируемого 
        private void ApplyUser()
        {
            // назначаем Имя
            if (txtUserName.Visible)  // только в режиме Добавления пользователя !
            {
                userModify.UserName = txtUserName.Text;   // на всякий случай
            }

            // назначаем Роль
            //userModify.UserRole = sUserRole;

            // назначаем Разрешения для action (allow + typedelay)
            string sActName = actionModify; ;
            TPermission permAllow = new TPermission(sActName, TPermissionsSettings.NameAllow, new TParameters());
            TPermission permDeny = new TPermission(sActName, TPermissionsSettings.NameDeny, new TParameters());
            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    string extensName = row.Cells[1].Text;
                    //string extensId = row.Cells[0].Text;  -- у скрытой ячейки значение всегда ПУСТО !
                    string extensId = "";
                    TExtension extens = permSet.Extensions.FindExtensionName(extensName);
                    if (extens != null)
                    {
                        extensId = extens.ExtensionId;
                    }
                    CheckBox chkRowAllow = (row.Cells[nColUserAllow].FindControl("ckbAllow") as CheckBox);
                    if (chkRowAllow.Checked)
                        permAllow.Parameters.Add(new TParameter("extensionId", extensId));
                    CheckBox chkRowDeny = (row.Cells[nColUserAllow].FindControl("ckbDeny") as CheckBox);
                    if (chkRowDeny.Checked)
                        permDeny.Parameters.Add(new TParameter("extensionId", extensId));
                }
            }
            userModify.Permissions.Set(permAllow);  // заменыем/добавляем к старым
            userModify.Permissions.Set(permDeny);   // заменыем/добавляем к старым
        }  // ApplyUser()


        //---------------------------------------------------
        // сохранение в файле ТЕКУЩЕГО USER
        public bool UserSave(bool bRemoveUser)
        {
            bool bRet = false;
            string sErr = "";
            try
            {
                if (permSet == null)  // не должно быть !!!
                {
                    //MessageBox.Show("PermissionSettingtngs not defined!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //return bRet; //=================>
                }
                if (userModify == null)  // не должно быть !!!
                {
                    //MessageBox.Show("Modify User not defined!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //return bRet; //=================>
                }

                // !!! сохраняем МОДИФИРОВАННОГО пользователя (могди уже в форме перейти на другого !)
                string sUserName = userModify.UserName;
                if (sUserName == "") // не должно быть !!!
                {
                    //MessageBox.Show("Modify User Name is Empty!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //return bRet; //=================>
                }

                //======================
                // вычитываем документ из файла - откорректируем его по введенному и сохраним
                XDocument docNew = XDocument.Load(permSet.FileNameFull);
                //======================

                //--------------------
                //  ищем модифицированного пользователы в Документе перебором
                XElement xeUsers = docNew.Root.Element("users");
                //XElement xeUser = xeUsers.Element("userName"); - == null !
                IEnumerable<XElement> childUsers = from el in xeUsers.Elements() select el;
                XElement xeUser = null;
                foreach (XElement xe in childUsers)
                {
                    if (xe.Element("userName") != null)
                    {
                        if (xe.Element("userName").Value == sUserName)
                        {
                            xeUser = xe; break; //----------->
                        }
                    }
                }

                //--------------------
                // удаляем модифицированного текущего Пользователя  в Докуенте
                if (xeUser != null)   // может не быть пользователя (новый) !!!
                {
                    // удаляем пользователя из Докуента (сохраним документ)
                    xeUser.Remove();
                }

                //--------------------
                // добавляем модифицированного текущего Пользователя в документ
                if (!bRemoveUser)  // НЕ УДАЛЕНИЕ пользователя
                {
                    XElement xeUserModify = userModify.ToXElement();
                    xeUsers.Add(xeUserModify);
                }

                //--------------------
                // удаляем модифицированного текущего Пользователя в Объекте - после формирования для Докуента !
                if (bRemoveUser)    // УДАЛЕНИЕ пользователя
                {
                    // снимаем признак измененности данных
                    SetUpdateScr(false);
                    // удаляем пользователя из объекта
                    permSet.Users.Drop(sUserName);
                    // удаляем пользователя из списка
                    cbxUserName.Items.Remove(sUserName);
                    cbxUserName.SelectedIndex = 0; // c 0 !
                    //Application.DoEvents();
                    // назначаем нового тек. пользователя
                    sUserName = cbxUserName.Text;
                    UserSet(sUserName);
                    //throw new Exception("XElement 'user' not found !");  //====>
                }
                else // не удаление пользователя
                {
                    // сохраняем в Объекте (добавляем/заменыем)
                    permSet.Users.Set(userModify);

                    // выводим на экран все данные МОДИФИЦИРУЕМОГО пользователя
                    RefreshScreenForUser();
                }

                //======================
                // сохраняем откоректирванный документ в файле
                try
                {
                    docNew.Save(permSet.FileNameFull);
                    //MessageBox.Show("File saved." + Environment.NewLine +
                    //     Environment.NewLine + permSet.FileNameFull + "_new", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    sErr = sErr + (sErr == "" ? "" : " // ") + ex.Message;
                    //MessageBox.Show("File not saved." + Environment.NewLine +
                    //     Environment.NewLine + permSet.FileNameFull + "_new" + Environment.NewLine +
                    //     Environment.NewLine + "Error: " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                //======================

            }
            catch (Exception e)
            {
                sErr = sErr + (sErr == ""? "" : " // ") + e.Message;
                //MessageBox.Show("Error save User:" + Environment.NewLine +
                //    Environment.NewLine + sErr, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (bRet)
            {
                message = "";
            }
            else
            {
                message = sErr;
            }
            return bRet; //=======================>

        } // UserSave()


        //-----------------------------------------
        // назначает МОДИФИЦИРУЕМОГО пользователя
        private void UserSet(string UserName)
        {
            userCurrent = permSet.Users.FindUser(UserName);  // должен быть !!!
            // если не нашли текущего пользователя - считаем, что это добавленный, создаем 
            if (userCurrent == null)  //  (для отката значений !)
            {
                userCurrent = new TUser(cbxUserName.Text, cbxUserName.Text, new TPermissions());
            }
            // пользователь для модификации
            userModify = userCurrent.Clone();
            // выводим на экран все данные МОДИФИЦИРУЕМОГО пользователя
            RefreshScreenForUser();
        }  // UserSet()

        //-----------------------------------------
        // выводит на экран все данные МОДИФИЦИРУЕМОГО пользователя
        private void RefreshScreenForUser()
        {
            if (userModify != null)
            {
                // ОБНОВЛЕНИЕ ЭКРАНА ДЛЯ user
                string sUserName = cbxUserName.Text;

                //// роль пользователя
                //string sUserRole = userModify.UserRole;
                //// позиционируем в списке Ролей
                //if (sUserRole != null)  // пока отключили у ползователя !
                //{
                //    if (cbxUserRole.Items.Count > 0)
                //    {
                //        int index = cbxUserRole.Items.IndexOf(sUserRole);
                //        if (index >= 0)
                //        {
                //            cbxUserRole.SelectedIndex = index; // c 0 !?
                //        }
                //        else
                //        {
                //            cbxUserRole.SelectedIndex = 0; // c 0 !?
                //        }
                //    }
                //}

                // операция
                //  --- оставляем прежнюю

                // выводим на экран Разрешения (сетку) МОДИФИЦИРУЕМОГО пользователя
                RefreshScreenForPermissions();

                // снимаем признак измененности данных на форме
                SetUpdateScr(false);
            }
        }  // RefreshScreenForUser()

        //-----------------------------------------
        // выводим на экран Разрешения (сетку) МОДИФИЦИРУЕМОГО пользователя
        private void RefreshScreenForPermissions()
        {
            if (userModify != null)
            {
                string sUserName = cbxUserName.Text;
                string sActName = cbxAction.Text;

                // сначала все очищаем у юзера
                ClearPermissionsUser();

                // усранавливаем разрешения по модифицируемому ползователю
                bool bAllAllow = false;
                bool bAllDeny = false;
                foreach (TPermission perm in userModify.Permissions)
                {
                    // колонка чекбокса
                    int nColCbx = (perm.Type == TPermissionsSettings.NameAllow ? nColUserAllow : nColUserDeny);
                    bool bAllow = (perm.Type == TPermissionsSettings.NameAllow);
                    // берем только заданную операцию
                    if (perm.Action == sActName)
                    {
                        // перебираем параметры 
                        foreach (TParameter par in perm.Parameters)
                        {
                            // только для параметров с кодом Расширения
                            if (par.Name == "extensionId")
                            {
                                string sExtensionId = par.Value; // код расширения
                                if (sExtensionId == "*")  // "All"
                                {
                                    if (perm.Type == TPermissionsSettings.NameAllow)
                                        bAllAllow = true;
                                    if (perm.Type == TPermissionsSettings.NameDeny)
                                        bAllDeny = true;
                                }
                                // ишем код расширения в сетке 
                                foreach (GridViewRow row in GridView1.Rows)
                                {
                                    if (row.RowType == DataControlRowType.DataRow)
                                    {
                                        string extensName = row.Cells[1].Text;
                                        //string extensId = row.Cells[0].Text; qqq - всегда пустой в скрытой колонке !!!
                                        string extensId = null;
                                        TExtension extens = permSet.Extensions.FindExtensionName(extensName);
                                        if (extens != null)
                                        {
                                            extensId = extens.ExtensionId;
                                        }
                                        if ((extensId == sExtensionId) || (bAllow && bAllAllow) || (!bAllow && bAllDeny))   
                                        {
                                            // налши строку с кодом - устанавливаем чекбокс
                                            if (bAllow)
                                            {
                                                CheckBox ckb = (row.Cells[nColUserAllow].FindControl("ckbAllow") as CheckBox);
                                                ckb.Checked = true;
                                            }
                                            else 
                                            {
                                                CheckBox ckb = (row.Cells[nColUserAllow].FindControl("ckbDeny") as CheckBox);
                                                ckb.Checked = true;
                                            }
                                            // на след параметр
                                            break;  //---------->
                                        }
                                    }
                                } // по строкам сетки

                                //for (int i = 0; i < dataGridView1.Rows.Count; i++)
                                //{
                                //    if (dataGridView1.Rows[i].Cells[0].Value.ToString() == sExtensionId)     // Row, Col - с 0 !        
                                //    {
                                //        // налши строку с колосм - устанавливаем чекбокс
                                //        dataGridView1.Rows[i].Cells[nColCbx].Value = true;     // Row, Col - с 0 !        
                                //        // на след параметр
                                //        break;  //---------->
                                //    }
                                //} // for по строкам сетки

                            }
                        } // for по parameters
                    }  // наша Action c экрана
                } // по разрешениям

                // помечаем всю соотв.колонку (если встретилась "*")
                foreach (GridViewRow row in GridView1.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        if (bAllAllow)  // ставив на всех Allow
                        {
                            CheckBox ckb = (row.Cells[nColUserAllow].FindControl("ckbAllow") as CheckBox);
                            ckb.Checked = true;
                        }
                        if (bAllDeny)  // ставив на всех Deny
                        {
                            CheckBox ckb = (row.Cells[nColUserAllow].FindControl("ckbDeny") as CheckBox);
                            ckb.Checked = true;
                        }
                    }
                } // по строкам сетки

                //for (int i = 0; i < dataGridView1.Rows.Count; i++)
                //{
                //    if (bAllAllow)
                //        dataGridView1.Rows[i].Cells[nColUserAllow].Value = true;     // Row, Col - с 0 !        
                //    if (bAllDeny)
                //        dataGridView1.Rows[i].Cells[nColUserDeny].Value = true;     // Row, Col - с 0 !        
                //} // for по строкам сетки

            } // (userModify != null)

        } //  RefreshScreenForPermissions()


        //-----------------------------------------
        // очищает на экране разрешения
        private void ClearPermissionsUser()
        {
            // все очищаем 
            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox ckb;

                    ckb = (row.Cells[nColUserAllow].FindControl("ckbAllow") as CheckBox);
                    ckb.Checked = false;
                    
                    ckb = (row.Cells[nColUserAllow].FindControl("ckbDeny") as CheckBox);
                    ckb.Checked = false;
                }
            } // по строкам сетки

            //for (int i = 0; i < dataGridView1.Rows.Count; i++)
            //{
            //    dataGridView1.Rows[i].Cells[nColUserAllow].Value = false;     // Row, Col - с 0 !        
            //    dataGridView1.Rows[i].Cells[nColUserDeny].Value = false;     // Row, Col - с 0 !        
            //}

        } //  ClearPermissionsUser()


        private void SetUpdateScr(bool upd)
        {
            bUpdateScr = upd;
            if (bUpdateScr)
            {
                btnCancel.Enabled = true;
                btnSave.Enabled = true;
                //btnClose.Enabled = true;
            }
            else
            {
                btnCancel.Enabled = false;
                btnSave.Enabled = false;
                //btnClose.Enabled = true;
            }
        }  // SetUpdateScr()

        protected void cbxUserName_SelectedIndexChanged(object sender, EventArgs e)
        {

            // переносим текущее состояние пользователя c экрана в объект-модификации (если не сменили на экране пользователя !)
            // !!! делаем здесь, т.к. нет события cbxUserName_DropDown(object sender, EventArgs e) !!!
            ApplyUser();  // берем Роль и Разрешения сетки с экрана в модифицируемого 

            // перед назначением нового пользователя проверяем измененность атрибутов старого пользователя 
            if (bUpdateScr)
            {
                //if (MessageBox.Show("Data changed. Save User " + userCurrent.UserName + " ?", "Save User", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    UserSave(false);   // не удаление пользователя
                }
            }
            // назначаем нового текущего пользователя
            UserSet(cbxUserName.Text);
        }

        protected void btnAllow_Click(object sender, EventArgs e)
        {
            // устанавливаем птички "Allow" у всез строк сетки
            bool bUpd = false;
            bool allSet = true;
            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox ckb = (row.Cells[nColUserAllow].FindControl("ckbAllow") as CheckBox);
                    if (!ckb.Checked)
                    {
                        allSet = false; break; //-------->
                    }
                }
            } // по строкам сетки
            bool newVal = (allSet ? false : true);
            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox ckb = (row.Cells[nColUserAllow].FindControl("ckbAllow") as CheckBox);
                    if (ckb.Checked != newVal)
                    {
                        bUpd = true;
                    }
                    ckb.Checked = newVal;
                }
            } // по строкам сетки
            if (bUpd)
            {
                SetUpdateScr(true);
            }
        }

        protected void btnDeny_Click(object sender, EventArgs e)
        {
            // устанавливаем птички "Deny" у всез строк сетки
            bool bUpd = false;
            bool allSet = true;
            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox ckb = (row.Cells[nColUserDeny].FindControl("ckbDeny") as CheckBox);
                    if (!ckb.Checked)
                    {
                        allSet = false; break; //-------->
                    }
                }
            } // по строкам сетки
            bool newVal = (allSet ? false : true);
            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox ckb = (row.Cells[nColUserDeny].FindControl("ckbDeny") as CheckBox);
                    if (ckb.Checked != newVal)
                    {
                        bUpd = true;
                    }
                    ckb.Checked = newVal;
                }
            } // по строкам сетки
            if (bUpd)
            {
                SetUpdateScr(true);
            }

        }

        protected void cbxAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            // переносим текущее состояние пользователя c экрана в объект-модификации (если не сменили на экране пользователя !)
            // !!! делаем здесь, т.к. нет события cbxAction_DropDown(object sender, EventArgs e) !!!
            // ! до назначения новой на место текущей !!!
            ApplyUser();  // берем Роль и Разрешения сетки с экрана в модифицируемого 

            // назначаем новую акцию текущей
            actionModify = cbxAction.Text;

            // выводим на экран Разрешения (сетку) МОДИФИЦИРУЕМОГО пользователя
            RefreshScreenForPermissions();
        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            SetUpdateScr(true);
        }

        protected void txtUserName_TextChanged(object sender, EventArgs e)
        {
            // !!! отрабатывает только ПО Enter или ПРИ ПЕРЕХОДЕ на другой контрол !!!

            // проверяем поле Имени пользователя
            bool isValid;
            isValid = ValidateUserName();
            btnSaveNew.Enabled = isValid;
        }

        protected void btnSaveNew_Click(object sender, EventArgs e)
        {
            // переносим текущее состояние пользователя c экрана в объект-модификации (если не сменили на экране пользователя !)
            ApplyUser();  // берем Роль и Разрешения сетки с экрана в модифицируемого 

            // само сохранение
            UserSaveNew(); 
        }


        //---------------------------------------------------
        // сохранение в файле НОВОГО USER
        public bool UserSaveNew()
        {
            bool bRet = false;
            string sErr = "";
            try
            {
                if (permSet == null)  // не должно быть !!!
                {
                    //MessageBox.Show("PermissionSettingtngs not defined!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //return bRet; //=================>
                }
                if (userModify == null)  // не должно быть !!!
                {
                    //MessageBox.Show("Modify User not defined!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //return bRet; //=================>
                }

                //------------------------------
                // проверяем поля ввода Нового пльзователя
                txtUserName.Text = txtUserName.Text.Trim();

                // проверяем значение поля Имени пользователя
                if (!ValidateUserName())
                {
                    return bRet; //=================>
                }

                //--------------------------------
                // берем имя нового пользователя в модифицируемого (ghb lj,fdltybb ,skj gecnjt!)
                userModify.UserName = txtUserName.Text;
                // добавляем в объект
                TUser user = new TUser(txtUserName.Text, null, new TPermissions());
                permSet.Users.Add(user);
                userCurrent = user;
                //--------------------------------

                // прячем поле ввода имени
                txtUserName.Visible = false;
                // прячем кнопки Нового пользователя
                btnCancelNew.Visible = false;
                btnSaveNew.Visible = false;
                // показываем список
                cbxUserName.Visible = true;
                btnAddUser.Visible = true;
                btnDelUser.Visible = true;
                btnCancel.Visible = true;
                btnSave.Visible = true;
                // активируем список имени
                //cbxUserName.Focus();

                //=================
                // само сохранение - нового уже добавили объект потзоватеелй, назначили текушим 
                UserSave(false);   // не удаление пользователя
                //=================

                // добавляем в список, позиционируем на него в списке - !!! после сохранения!!!
                cbxUserName.Items.Add(txtUserName.Text);
                // позиционируем - !!! список с авт.сотировкой, надо искать занчение !!!
                //int index = cbxUserName.FindStringExact(txtUserName.Text); -- нет метода в WEB !!!
                //int index = cbxUserName.Items.FindByText(txtUserName.Text);  -- возвращает объект
                int index = cbxUserName.Items.IndexOf(cbxUserName.Items.FindByText(txtUserName.Text));
                if (index >= 0)  // должно быть!
                {
                    cbxUserName.SelectedIndex = index; // c 0 !?
                }
                else
                {
                    cbxUserName.SelectedIndex = 0; // c 0 !?
                }
                // сортируем элементы списка Имен
                SortCbxItems(cbxUserName);

                // выводим на экран все данные МОДИФИЦИРУЕМОГО пользователя
                RefreshScreenForUser();
                // снимаем признак измененности данных на экране
                SetUpdateScr(false);

            }
            catch (Exception e)
            {
                sErr = sErr + (sErr == ""? "" : " // ") + e.Message;
                //MessageBox.Show("Error save User:" + Environment.NewLine +
                //    Environment.NewLine + sErr, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (bRet)
            {
                message = "";
            }
            else
            {
                message = sErr;
            }
            return bRet; //=======================>

        } // UserSaveNew()


        protected void btnDelUser_Click(object sender, ImageClickEventArgs e)
        {
            UserRemove();
        }

        //--------------------------------------------------------------------
        // удаление пользователя (из списка и из файла)
        private void UserRemove()
        {
            if (cbxUserName.Items.Count < 2)
            {
                //MessageBox.Show(cbxUserName + "  is the last user in the list !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return; //===========>
            }
            //if (MessageBox.Show("Remove user " + cbxUserName.Text + " ?" + Environment.NewLine +
            //    Environment.NewLine + "Recovery will be impossible !", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            //    return; //===========>

            // само удаление
            UserSave(true);   // true - удаление пользователя
        }


        protected void Validate_txtUserName_ServerValidate(object source, ServerValidateEventArgs args)
        {   
            // отказался от Валидатора !!!
            try
            {
                //args.IsValid = (int.Parse(args.Value) % 5 == 0);
                args.IsValid = false;
            }
            catch
            {
                args.IsValid = false;
            }
        }

        //-----------------------------------------------
        // проверяем значение поля Имени пользователя
        protected bool ValidateUserName()
        {
            bool isValid = true;

            // стриаем старую ошибку 
            lblUserName_Error.Text = "";

            // === заполненность Имени
            if (txtUserName.Text.Length == 0)   // пустое имя поля не сохраняем !
            {
                isValid = false;
                lblUserName_Error.Text = (lblUserName_Error.Text == "" ? "" : " ") + "User name is empty!";
            }
            // === уникальность Имени
            int ind = cbxUserName.Items.IndexOf(cbxUserName.Items.FindByText(txtUserName.Text));
            if (ind >= 0)  // нет уникальности
            {
                //Response.Write("Error: User name \"" + txtUserName.Text + "\" is already registered!");  // "<br>" + 
                isValid = false;
                lblUserName_Error.Text = (lblUserName_Error.Text == "" ? "" : " ") + "User name \"" + txtUserName.Text + "\" is already registered!";
            }
            return isValid; //=================>

        } // ValidateUserName()

        //-----------------------------------------------
        // проверяем значение поля Имени пользователя
        protected void SortCbxItems(DropDownList cbx)
        {
            if (cbx.Items.Count > 0)
            {
                // текущий елемент списка
                string sName = cbx.Text;
                // массив для сортировки
                List<string> aTmp = new List<string>();
                foreach (System.Web.UI.WebControls.ListItem li in cbx.Items)
                {
                    aTmp.Add(li.Text);
                }
                aTmp.Sort();
                // очищаем старые
                cbx.Items.Clear();
                // добавляем отсортированные
                foreach (string str in aTmp)
                {
                    cbx.Items.Add(str);
                }
                // позиционируем на старый
                int index = cbx.Items.IndexOf(cbx.Items.FindByText(sName));
                if (index >= 0)  // должно быть!
                {
                    cbx.SelectedIndex = index; // c 0 !?
                }
                else
                {
                    cbx.SelectedIndex = 0; // c 0 !?
                }
            }
        }   // SortCbxItems()


    }  // WebFormBotUAC

}
