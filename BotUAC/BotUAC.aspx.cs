
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
using System.Diagnostics;           // Process (Stfrt / Kill)
using System.Threading;             // Events? Mutex

namespace BotUAC   
{
    public partial class WebFormBotUAC : System.Web.UI.Page
    {
        private const int nColUserAllow = 2; // с 0 !
        private const int nColUserDeny = 3; // с 0 !

        private bool bVariablesToSessionSaved = false;          // просто как индикатор сохранности переменных в сессии

        private TAppSettings appSet = null;
        private TPermissionsSettings permSetOriginal = null;    // Первоначальная коллекция для Undo (вычитанная из файла или сохраненная в файле)
        private TPermissionsSettings permSetModify = null;      // Корректируемая коллекция

        private TUser userModify = null;    // копия корректируемая
        private TUser userModifyBeforeAdd = null;    // /

        private string sActionModify = null;    // копия имени корректируемой акции (текущей в выпадающем списке)

        private bool bUpdateXmlFile_to_RestartProc = false;       // признак измененности файла xml (данные сохранялись в файле) -  для перезапуска заданного процесса при завершении работы с формой

        private List<TPermissionGridRow> GridData = new List<TPermissionGridRow>();

        bool bUpdateScrMain = false;
        bool bUpdateScrAdd = false;
        string sMainMessage = null;


        protected void Page_Load(object sender, EventArgs e)
        {

            // текущая аутентификация:
            //ResponseWriteInfo("Authentication: Authenticated = " + User.Identity.IsAuthenticated.ToString() + ", User = " + User.Identity.Name + ", Type = " + User.Identity.AuthenticationType);  

            // путь к файлу параметров приложения (Irbis.xml")
            //string sIrbisXmlFileNameFull = HttpContext.Current.Server.MapPath("~/App_Data");
            //string s2 = HostingEnvironment.ApplicationPhysicalPath;
            //string sIrbisXmlFileNameFull = HttpContext.Current.Server.MapPath("~");         // косые в конце уже есть !!!


            // путь к файлу параметров приложения (Irbis.xml") из переменной файла web.config
            // !!! при запуске формы из среды берется файл:
            //     C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\Config\web.config"
            //
            string sIrbisXmlFileNameFull = null;
            string sErr = null;

            /*
            //-------------------------------------------------------------
            // из глобального файла сервера (web.config) 
            System.Configuration.Configuration rootWebConfig1 = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);
            System.Configuration.Configuration rootWebConfig1 = System.Configuration.ConfigurationManager.OpenExeConfiguration(null);
            if (rootWebConfig1.AppSettings.Settings.Count > 0)
            {
                //<appSettings>
                //   <add key="irbisFileNameAndPath" value="с:\irbis\irbis.xml" />
                //</appSettings>
                System.Configuration.KeyValueConfigurationElement irbisFileNameAndPath = rootWebConfig1.AppSettings.Settings["irbisFileNameAndPath"];
                if (irbisFileNameAndPath != null)
                {
                    sIrbisXmlFileNameFull = irbisFileNameAndPath.Value;
                    if (sIrbisXmlFileNameFull.Trim().Length == 0)
                    {
                        sErr = TMess.Mess0019; // "In the web configuration is specified empty AppSettings.irbisFileNameAndPath";
                    }
                }
                else
                {
                    sErr = TMess.Mess0018; // "In the web configuration is not specified AppSettings.irbisFileNameAndPath";
                }
            }
            else
            {   // нет ключей раздела appSettings (и когда нет вообще раздела appSettings)
                sErr = TMess.Mess0017; // "In the web configuration is not specified AppSettings parameters";
            }
            if (sErr != null)
            {
                SayError(sErr);
                appSet = null; // !!! как признак незагруженной - ПОСЛЕ вывода сообщения !!!
                return; //======================>
            }
            //-------------------------------------------------------------
            */

            lblSave_Error.Visible = true;
            lblSave_Error.Text = "";

            //-------------------------------------------------------------
            // из локального файла приложения (web.config) 
            sIrbisXmlFileNameFull = System.Configuration.ConfigurationManager.AppSettings["irbisFileNameAndPath"];
            if (sIrbisXmlFileNameFull != null)
            {
                if (sIrbisXmlFileNameFull.Trim().Length == 0)
                {
                    sErr = TMess.Mess0019; // "In the web configuration is specified empty AppSettings.irbisFileNameAndPath";
                }
            }
            else
            {
                sErr = TMess.Mess0018; // "In the web configuration is not specified AppSettings.irbisFileNameAndPath";
            }
            if (sErr != null)
            {
                SayError(sErr);
                appSet = null; // !!! как признак незагруженной - ПОСЛЕ вывода сообщения !!!
                return; //======================>
            }
            // путь (без имеи) к файлу Irbis.xml - в этой же папке файл Permissions.xml
            //string sIrbisXmlFilePath = Path.GetFullPath(sIrbisXmlFileNameFull);  // с косой в конце !!!
            //string sPermissionsXmlFileNameFull = sIrbisXmlFilePath + "Permissions.xml";


            //---------------------------------------------------------------
            // первоначальная загрузка параметров Приложения и вычитка Объекта (по ИЛИ):
            //      - первоначальная загрузка стараницы
            //      - в прошлый раз не смогли вычитать - дали сообщение об ошибке
            bool bNeedXmlAppLoad = false;
            bool bNeedXmlPerLoad = false;
            if (!IsPostBack)
            {   // первоначальная загрузка стараницы 
                bNeedXmlAppLoad = true;
                bNeedXmlPerLoad = true;
            }
            else
            {
                // не первоначальная загрузка стариницы - вычитываем, если был сбой вычитки (null в сессии!)
                if (Session["appSet"] == null || Session["permSetOriginal"] == null)   // сессия закрылась по таймауту - просто берем текущие ?
                {
                    bNeedXmlAppLoad = true;
                    bNeedXmlPerLoad = true;
                }
            }
            // вычитка из файла/сессии
            if (bNeedXmlAppLoad)
            {
                appSet = new TAppSettings(sIrbisXmlFileNameFull);
                if (!appSet.Load())
                {
                    // "Error application config-file load: " + appSet.FileNameFull);
                    //SayError(""Failed connecting to the application database.");
                    SayError(TMess.Mess0001 + "<br>File: " + appSet.FileNameFull);
                    appSet = null; // !!! как признак незагруженной - ПОСЛЕ вывода сообщения !!!
                    return; //======================>
                }
                // сразу сохраняем в сессии
                Session["appSet"] = appSet;
                //ViewState.Add("appSet", appSet);
            }
            else 
            {
                // берем из сессии
                appSet = (TAppSettings)Session["appSet"];
            }
            if (bNeedXmlPerLoad)  // после загрузки для приложения !!!
            {
                permSetOriginal = new TPermissionsSettings(appSet.PermissionsFileNameAndPath);  // после ЗАГРУЗКИ appSet !!!
                if (!permSetOriginal.Load())
                {
                    // "Error permissions config-file load: " + permSetOriginal.FileNameFull); // "<br>" + 
                    //SayError("Failed connecting to the permissions database."); 
                    SayError(TMess.Mess0002 + "<br>File: " + permSetOriginal.FileNameFull);
                    permSetOriginal = null; // !!! как признак незагруженной - ПОСЛЕ вывода сообщения !!!
                    return; //======================>
                }
                // копируем для коррекции
                permSetModify = permSetOriginal.Clone();
                // сразу сохраняем в сессии (Оригинал и Изменяемую !)
                Session["permSetOriginal"] = permSetOriginal;
                Session["permSetModify"] = permSetModify;
                //ViewState.Add("permSetModify", permSetModify);
            }
            else
            {
                // берем из сессии
                permSetOriginal = (TPermissionsSettings)Session["permSetOriginal"];
                permSetModify = (TPermissionsSettings)Session["permSetModify"];
            }
            // здесь В СЕССИИ и в Переменных всегда уже вычитанные xml 
            //---------------------------------------------------------------

            // убираем сообщение об ошибке (если было) 
            divError.Visible = false;
            divMain.Visible = true;

            // обычная обработка загрузки страницы
            if (!IsPostBack)  // первоначальная загрузка стараницы
            {
                // доопределяем контролы
                btnDelUser.ToolTip   = TMess.Mess0003; // "Delete current User";
                btnAddUser.ToolTip   = TMess.Mess0004; // "Add new User";
                btnCancel.ToolTip    = TMess.Mess0006; // "Cancel change User";
                btnSave.ToolTip      = TMess.Mess0005; // "Save User";
                btnCancelNew.ToolTip = TMess.Mess0008; // "Cancel add new User";
                btnSaveNew.ToolTip   = TMess.Mess0007; // "Save new User";
                btnSaveNew.ToolTip = TMess.Mess0007; // "Save new User";

                btnAllow.ToolTip = TMess.Mess0015;  // "Change the values of all cells in the grid column";
                btnDeny.ToolTip = TMess.Mess0015;   // "Change the values of all cells in the grid column";

                // заголовки выпадающих списков
                lblUserName.Font.Bold = true;
                lblAction.Font.Bold = true;

                // "заголовок" сетки жирным
                lblExtension.Font.Bold = true;
                btnAllow.Font.Bold = true;
                btnDeny.Font.Bold = true;

                // добавляем запрос у оператора при удалении пользователя
                //<asp:ImageButton ID="btnDelUser" 
                //    onClientClick="return confirm('Are you sure you want to delete the user?')"
                //    onclick="btnDelUser_Click" />
                //btnDelUser.OnClientClick = "return confirm('" + "Are you sure you want to delete the user?" + "')";
                btnDelUser.OnClientClick = "return confirm('" + TMess.Mess0014 + "')";

                //==========================
                // начальная установка (первая загрузка формы) пользователя для его коррекции
                UserSetBegin("", "");   // "" - имя пользователя для позиционирования (при пустом - первый по алфавитному списку)

                //----------------------------------------
                // первоначальная загрузка - сохраняем переменные формы для след.загрузки !!!
                // !!! сбой при вычитке после потери сеиии по таймауту !!!
                FormVariablesSaveToSession();

                // восстанавливаем состояние кнопок (после восстановлеиня из формы!)
                SetUpdateScrMain(bUpdateScrMain);
                SetUpdateScrAdd(bUpdateScrAdd);

            }
            else  // не первая загрузка страницы
            {
                //----------------------------------------
                // повторная загрузка - восстанавливаем переменные формы от предыд.загрузки !!!
                // !!! сбой при вычитке после потери сессии по таймауту !!!
                // !!! проверять "bVariablesToSessionSaved" - он всегда не null !!!
                if (Session["bVariablesToSessionSaved"] == null)   // сессия закрылась по таймауту - просто берем текущие ?
                {
                    FormVariablesSaveToSession();
                }

                // восстанавливаем из сессии глобальные переменные
                FormVariablesRestoreFromSession();

                // восстанавливаем состояние кнопок (после восстановлеиня из формы!)
                SetUpdateScrMain(bUpdateScrMain);
                SetUpdateScrAdd(bUpdateScrAdd);

            }  // первая загрузка . нет 

        }


        protected void Page_Unload(object sender, EventArgs e)
        {

            //----------------------------------------
            // сохраняем переменные формы для след.загрузки !!!
            if (appSet != null)   // могло быть при ошибке начальной загрузки !
            {
                FormVariablesSaveToSession();
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            // сохранение в файле ВСЕГО СПИСКА ПОЛЬЗОВАТЕЛЕЙ USERS
            if (!UsersSave())
            {
                lblSave_Error.Visible = true;
                lblSave_Error.Text = TMess.Mess0020;  // "Failed to save changes."
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            UsersCancel(cbxUserName.Text);
            //RestoreUser(cbxUserName.Text);
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
            userModify = userModifyBeforeAdd.Clone();
            userModifyBeforeAdd = null;     // уничтожаем
            // выводим на экран все данные МОДИФИЦИРУЕМОГО пользователя
            RefreshScreenForUser();
            // активируем список имени
            cbxUserName.Focus();

            // восстанвливаем доступность кнопок в основном режимке
            SetUpdateScrMain(bUpdateScrMain);

        }


        protected void btnAddUser_Click(object sender, ImageClickEventArgs e)
        {

            // перед Добавлением переносим текущее состояние пользователя c экрана в объект-модификации (если не сменили на экране пользователя !)
            UserApply();  // берем Роль и Разрешения сетки с экрана в модифицируемого 

            // сохраняем нового тек.пользователя на момент добавления нового - для Cancel !
            userModifyBeforeAdd = userModify.Clone();

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
            // при добавлении создаем нового пользователя для коррекции
            userModify = new TUser("", "", new TPermissions());  
            //---------------

            // очищаем поля формы - разрешения
            txtUserName.Text = "";      // иначе останется от прошлого ввода, но потом выведем для изменяемого - можно не делать
            lblUserName_Error.Text = "";

            // выводим на экран все данные МОДИФИЦИРУЕМОГО пользователя
            RefreshScreenForUser();
            // доступность кнопок нового пользователя  - от заполненности имени 
            btnSaveNew.Enabled = (txtUserName.Text.Trim().Length > 0);
            // активируем ввод имени !!!
            txtUserName.Focus();

            // снимаеи признак измененности нового
            SetUpdateScrAdd(false);

        }


        //---------------------------------------------------
        // занесение нового (модифицируемого) пользователя с экрана в коллекцию
        public bool UserAddToCollection()
        {
            bool bRet = false;
            string sErr = "";
            try
            {
                if (permSetModify == null)  // не должно быть !!!
                {
                    ResponseWriteError("Error UserAddToCollection(): PermissionSettingtngs not defined!");
                    return bRet; //=================>
                }
                if (userModify == null)  // не должно быть !!!
                {
                    ResponseWriteError("Error UserAddToCollection(): Modify User not defined!");
                    return bRet; //=================>
                }

                // доопределяем нового (модифицированного) по вводу с экрана
                userModify.UserName = txtUserName.Text;

                // после Добавлеия пользователя переносим текущее состояние пользователя c экрана в объект-модификации (если не сменили на экране пользователя !)
                UserApply();  // берем Роль и Разрешения сетки с экрана в модифицируемого 

                // добавляем нового (модифицированного) в коллекцию
                permSetModify.Users.Add(userModify.Clone());

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

                // добавляем новое имя в список имен пользователей 
                cbxUserName.Items.Add(userModify.UserName);
                // сортируем элементы списка Имен
                SortCbxItems(cbxUserName);
                // qqq позиционируем в списке имен пользователей на добавленного
                int index = cbxUserName.Items.IndexOf(cbxUserName.Items.FindByText(userModify.UserName));
                if (index >= 0)  // должно быть!
                {
                    cbxUserName.SelectedIndex = index; // c 0 !?
                }
                else
                {
                    cbxUserName.SelectedIndex = 0; // c 0 !?
                }

                // выводим на экран все данные МОДИФИЦИРУЕМОГО пользователя
                RefreshScreenForUser();

                // восстанвливаем доступность кнопок в основном режимке - true (добавили нового в коллекцию) !!!
                SetUpdateScrMain(true);

            }
            catch (Exception e)
            {
                sErr = sErr + (sErr == ""? "" : " // ") + e.Message;
                //ResponseWriteError("Error save User:" + sErr);
            }
            if (bRet)
            {
                sMainMessage = "";
            }
            else
            {
                sMainMessage = sErr;
            }
            return bRet; //=======================>

        } // UserAddToCollection()


        //---------------------------------------------------
        // удалене модифицируемого пользователя из коллекцию
        public bool UserDel()
        {
            bool bRet = false;
            string sErr = "";
            try
            {
                if (permSetModify == null)  // не должно быть !!!
                {
                    ResponseWriteError("Error UserDel(): PermissionSettingtngs not defined!");
                    return bRet; //=================>
                }
                if (userModify == null)  // не должно быть !!!
                {
                    ResponseWriteError("Error UserDel(): Modify User not defined!");
                    return bRet; //=================>
                }

                // !!! имя пользователя для УДАЛЕНИЯ (могди уже в форме перейти на другого !)
                string sUserName = userModify.UserName;
                if (sUserName == "") // не должно быть !!!
                {
                    ResponseWriteError("Error UserDel(): Modify User Name is Empty!");
                    return bRet; //=================>
                }

                // удаляем пользователя из объекта
                permSetModify.Users.Drop(sUserName);
                // удаляем пользователя из списка
                if (cbxUserName.Items.Count > 1)
                {
                    cbxUserName.Items.Remove(sUserName);
                }
                else  // последний элемент не удаляется обычным образом - просто чистим список !
                {
                    cbxUserName.Items.Clear();
                }
                // позиционируем список
                if (cbxUserName.Items.Count > 0)
                {
                    cbxUserName.SelectedIndex = 0; // c 0 !
                }

                //-----------------
                // спозиционировали список после удаления
                // назначаем нового тек. пользователя по имени в списке ПОСЛЕ УДАЛЕНИЯ пользователя (с обновляем данных на экране для нового тек. пользователя)
                UserModifySet(cbxUserName.Text, false);  // false - не сохранять текущего модиф.пользователя в коллекции перед назначением нового!!!
                // выводим на экран все данные МОДИФИЦИРУЕМОГО пользователя
                RefreshScreenForUser();

                // восстанвливаем доступность кнопок в основном режимке - true (удалили из коллекцию) !!!
                SetUpdateScrMain(true);

            }
            catch (Exception e)
            {
                sErr = sErr + (sErr == "" ? "" : " // ") + e.Message;
                //MessageBox.Show("Error save User:" + Environment.NewLine +
                //    Environment.NewLine + sErr, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (bRet)
            {
                sMainMessage = "";
            }
            else
            {
                sMainMessage = sErr;
            }
            return bRet; //=======================>

        } // UserDel()


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

                // доступность кнопки Удаления пользователя
                btnDelUser.Enabled = (cbxUserName.Text.Trim() != "");

                //-------------------------------------
                // рзрешаем / запрещаем дедактирование (нельзя, если нет пользовтелей - пустой выпадающий список!)
                bool bEnabled = ((cbxUserName.Visible && cbxUserName.Text.Trim() != "") || (txtUserName.Visible));
                cbxUserName.Enabled = bEnabled;
                cbxAction.Enabled = bEnabled;
                btnAllow.Enabled = bEnabled;
                btnDeny.Enabled = bEnabled;
                // сетка
                foreach (GridViewRow row in GridView1.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox ckb;
                        ckb = (row.Cells[nColUserAllow].FindControl("ckbAllow") as CheckBox);
                        ckb.Enabled = bEnabled;
                        ckb = (row.Cells[nColUserAllow].FindControl("ckbDeny") as CheckBox);
                        ckb.Enabled = bEnabled;
                    }
                } // по строкам сетки
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
                                        //string extensId = row.Cells[0].Text; - всегда пустой в скрытой колонке !!!
                                        string extensId = null;
                                        TExtension extens = permSetModify.Extensions.FindExtensionName(extensName);
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

            }
            else  // userModify == null  - не должно быть !!!
            {
                ResponseWriteError("Error RefreshScreenForPermissions(): Modify User not defined!");
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

        //------------------
        // установка признака измененности данных КОЛЛЕКЦИИ(с вкл/выкл кнопок)
        private void SetUpdateScrMain(bool upd)
        {
            bUpdateScrMain = upd;
            if (bUpdateScrMain)
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
        }  // SetUpdateScrMain()

        //------------------
        // установка признака измененности данных НОВОГО ПОЛЬЗОВАТЕЛЯ (с вкл/выкл кнопок)
        private void SetUpdateScrAdd(bool upd)
        {
            bUpdateScrAdd = upd;
            btnCancelNew.Enabled = true;  // всегда !
            if (bUpdateScrAdd)
            {
                btnSaveNew.Enabled = true;
            }
            else
            {
                btnSaveNew.Enabled = false;
            }
        }  // SetUpdateScrAdd()


        protected void cbxUserName_SelectedIndexChanged(object sender, EventArgs e)
        {

            // переносим текущее состояние ПРЕДЫДУЩЕГО пользователя c экрана в объект-модификации (если не сменили на экране пользователя !)
            // !!! делаем здесь, т.к. нет события cbxUserName_DropDown(object sender, EventArgs e) !!!
            UserApply();  // берем Роль и Разрешения сетки с экрана в модифицируемого 

            // назначаем нового текущего пользователя по имени в списке ПОСЛЕ СМЕНЫ ПОЛЬЗОВАЕТЯ в выпадающем списке
            UserModifySet(cbxUserName.Text, true);  // true - сохранять текущего модиф.пользователя в коллекции перед назначением нового!!!

            // доступность кнопки Удаления рользователя
            btnDelUser.Enabled = (cbxUserName.Text.Trim() != "");

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
                if (txtUserName.Visible)
                {
                    SetUpdateScrAdd(true);
                }
                else
                {
                    SetUpdateScrMain(true);
                }

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
                if (txtUserName.Visible)
                {
                    SetUpdateScrAdd(true);
                }
                else 
                {
                    SetUpdateScrMain(true);
                }
            }

        }

        protected void cbxAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            // переносим текущее состояние пользователя c экрана в объект-модификации (если не сменили на экране пользователя !)
            // !!! делаем здесь, т.к. нет события cbxAction_DropDown(object sender, EventArgs e) !!!
            // ! до назначения новой на место текущей !!!
            UserApply();  // берем Роль и Разрешения сетки с экрана в модифицируемого 

            // назначаем новую акцию текущей
            sActionModify = cbxAction.Text;

            // выводим на экран Разрешения (сетку) МОДИФИЦИРУЕМОГО пользователя
            RefreshScreenForPermissions();
        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            // обработка события для чекбокса в колонке сетки (ckbAllow и ckbDeny) 
            if (txtUserName.Visible)
            {
                SetUpdateScrAdd(true);
            }
            else
            {
                SetUpdateScrMain(true);
            }
        }

        protected void txtUserName_TextChanged(object sender, EventArgs e)
        {
            // !!! отрабатывает только ПО Enter или ПРИ ПЕРЕХОДЕ на другой контрол !!!

            // проверяем поле Имени пользователя
            bool isValid;
            isValid = ValidateUserName();
            btnSaveNew.Enabled = isValid;
        }


        protected void btnDelUser_Click(object sender, ImageClickEventArgs e)
        {
            UserRemove();
        }

        //--------------------------------------------------------------------
        // удаление пользователя (из списка и из файла)
        private void UserRemove()
        {
            // проверки
            // ...

            // само удаление старого пользователя
            UserDel();   
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

            // нормируем текст поля ввода Нового пльзователя - ЗАГЛАВНЫМИ на всякий влучай (переводились стилем при вводе) !!!
            txtUserName.Text = txtUserName.Text.Trim().ToUpper();

            // === заполненность Имени
            if (txtUserName.Text.Length == 0)   // пустое имя поля не сохраняем !
            {
                isValid = false;
                //lblUserName_Error.Text = (lblUserName_Error.Text == "" ? "" : " ") + "User name is empty!";
                lblUserName_Error.Text = (lblUserName_Error.Text == "" ? "" : " ") + TMess.Mess0011;
            }
            // === уникальность Имени
            int ind = cbxUserName.Items.IndexOf(cbxUserName.Items.FindByText(txtUserName.Text));
            if (ind >= 0)  // нет уникальности
            {
                isValid = false;
                //lblUserName_Error.Text = (lblUserName_Error.Text == "" ? "" : " ") + "User name is already registered:" + " \"" + txtUserName.Text + "\ !";
                lblUserName_Error.Text = (lblUserName_Error.Text == "" ? "" : " ") +  TMess.Mess0012 + " \"" + txtUserName.Text + "\" !";
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
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {

            /*
            //Response.Redirect("~/Logout.aspx");

            // http://stackoverflow.com/questions/31221/response-redirect-using-path
            //                 http://localhost:49843/BotUAC.aspx
            // !!! не находит: http://localhost/Logout.aspx
            //Response.Redirect(String.Format("http://{0}/Logout.aspx", Request.ServerVariables["SERVER_NAME"]));

            // явное задание  (Work) - работает !
            Response.Redirect("http://localhost:49843/Logout.aspx");
            */

            //Process[] localAll = Process.GetProcesses();
            //foreach (Process prc in localAll)
            //{
            //    ResponseWriteError("" + prc.ProcessName + "<br>");  //  Приложение было принудительно закрыто:
            //}




            //===================================
            // перезапускаем заданный процесс (если был изменен xml-файл)
            //bool bHaveOldProc = false;
            bool bStopOldProc = false;
            if (bUpdateXmlFile_to_RestartProc)       // признак измененности файла xml (данные сохранялись в файле) -  для перезапуска заданного процесса при завершении работы с формой
            {
                string sProcName = "cmd";   // без расширения !!!
                string sProcFullFileName = "cmd.exe";

                //-------------------------------
                // остановка процесса
                try
                {
                    Process[] Processes = Process.GetProcessesByName(sProcName);
                    foreach (Process Proc in Processes)
                    {
                        //bHaveOldProc = true;
                        // Попытка закрытия окна. Если имеются несохраненные документы,
                        // появится окно, предлагающее сохранить изменения.
                        Proc.CloseMainWindow();
                        // Ожидание до 30 секунд.
                        Proc.WaitForExit(300);  // 10000 - 30 секунд (1 - 3 msec)
                        // Если процесс еще работает, он "убивается".
                        if (!Proc.HasExited)
                        {
                            Proc.Kill();
                            ResponseWriteError("The application has been forcibly closed:" + " " + sProcName);  //  Приложение было принудительно закрыто:
                            //ResponseWriteError(TMess.Mess0001 + " " + sProcName);  
                        }
                        else
                        {
                            ResponseWriteError("The application has been closed properly: " + sProcName);  // Приложение было закрыто правильно.
                            //ResponseWriteError(TMess.Mess0001 + " " + sProcName);  
                        }
                        bStopOldProc = true;
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    ResponseWriteError("Error process stop:" + " " + ex.Message); 
                }

                //-------------------------------
                // !!! фиксируем, что данные "не изменены" - по потношению к новому процессу
                bUpdateXmlFile_to_RestartProc = false;

                //-------------------------------
                // старт процесса
                if (bStopOldProc) // !!! только если был запущен старый процесс
                {
                    try
                    {
                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                        p.StartInfo.FileName = sProcFullFileName;
                        //p.StartInfo.Arguments = "/C ping 127.0.0.1"; //чтобы программно нажать enter
                        //p.StartInfo.Arguments = "/K ping 127.0.0.1"; /**чтобы консоль сразу не закрывалась, чтобы её закрыть - закоментируй эту строку*/
                        p.Start();
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                        ResponseWriteError("Error process start:" + " " + ex.Message);
                    }
                }
            }

        }


        protected void SayError(string ErrorMessage)
        {
            if (divMain.Visible)
            {
                // оформляем текст ошибки
                string InnerHtml = ErrorMessage;
                InnerHtml = "<font color=\"red\">" + InnerHtml + "</font>";  // текст красным цветом
                //InnerHtml = "<em>" + InnerHtml + "</em>";           // теги курсива 
                InnerHtml = "<strong>" + InnerHtml + "</strong>";   // теги полужирного начертания
                // показываем ошибку
                divMain.Visible = false;
                divError.Visible = true;
                //divError.InnerText = InnerText;
                divErrorMessage.InnerHtml = InnerHtml;
            }
            else
            {
                // прячем ошибку
                divMain.Visible = true;
                divError.Visible = false;
                divError.InnerText = "";
            }

        }


        //---------------------------------------------------
        // сохранение в файле ВСЕГО СПИСКА ПОЛЬЗОВАТЕЛЕЙ USERS
        public bool UsersSave()
        {
            bool bRet = false;
            string sErr = "";

            try
            {
                if (permSetModify == null)  // не должно быть !!!
                {
                    ResponseWriteError("Error UsersSave(): PermissionSettingtngs not defined!");
                    return bRet; //=================>
                }
                if (userModify == null)  // не должно быть !!!
                {
                    ResponseWriteError("Error UsersSave(): Modify User not defined!");
                    return bRet; //=================>
                }

                //-------------------------
                // переносим текущее состояние пользователя c экрана в объект-модификации (если не сменили на экране пользователя !)
                UserApply();  // берем Роль и Разрешения сетки с экрана в модифицируемого 
                // сохраняем в Объекте модифицированного пользователя (заменяем - уже должен быть в коллекции, и для нового!)
                if (userModify.UserName.Trim() != "")  // !!! с пустым именем будет после удаления последнего в списке !!!
                {
                    permSetModify.Users.Set(userModify.Clone());
                }
                // берем список пользователей из Коллекции (объекта)
                TUsers newUsers = permSetModify.Users;

                //======================
                // вычитываем документ из файла - откорректируем его по введенному и сохраним
                XDocument docNew = XDocument.Load(permSetModify.FileNameFull);
                //======================

                //  ищем Список пользователей в Документе перебором
                XElement xeUsers = docNew.Root.Element("users");
                // удаляем Список пользователей  в Докуенте
                if (xeUsers != null)   // может не быть пользователя (новый) !!!
                {
                    // удаляем пользователей из Докуента (сохраним документ)
                    xeUsers.Remove();
                }
                // добавляем НОВЫЙ ИЗМЕНЕННЫЙ Список пользователей в документ
                docNew.Root.Add(newUsers.ToXElement());

                //======================
                // сохраняем откоректирванный документ в файле (с перезапуском процесса)
                string Message = null;
                if (WriteChangesToXMLFileAndRestart(docNew, permSetModify.FileNameFull, out Message))
                {
                    // признак нормального сохранения в файле
                    bRet = true;
                    // назначаем текущую коллекцию оригинальной (откат будет уже к ней)
                    permSetOriginal = permSetModify.Clone();
                    // снимаем признак измененности коллекции 
                    SetUpdateScrMain(false);
                }
                else
                {
                    if (Message != "")
                    {
                        sErr = sErr + (sErr == "" ? "" : " // ") + Message;
                    }
                }
                //======================
            }
            catch (Exception e)
            {
                sErr = sErr + (sErr == "" ? "" : " // ") + e.Message;
                //MessageBox.Show("Error save User:" + Environment.NewLine +
                //    Environment.NewLine + sErr, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (bRet)
            {
                sMainMessage = "";
            }
            else
            {
                sMainMessage = sErr;
            }

            return bRet; //=======================>

        } // UsersSave()


        //----------------------------------------
        // восстанавливаем первоначальные данные СПИСКА пользователей (были до начала коррекции)
        private void UsersCancel(string UserName)
        {
            if (txtUserName.Visible)   // !!! только для старого !
            {
                return; //=============>
            }

            // запоминаем имя текущего редуктируемого старого пользователя
            string sUserName = cbxUserName.Text;
            string sActionName = cbxAction.Text;

            // восстанвливаем модифицируемую коллекцию из сохраненной первоначальной
            permSetModify = permSetOriginal.Clone();

            // установка пользователя после отмены коррекции коллекции 
            // (заполнение списков, полей, назначение модиф.пользователя, обновлеине экрана для модиф.пользователя)
            UserSetBegin(sUserName, sActionName);  //  имя пользователя для позиционирования (при пустом - первый по алфавитному списку)

            // снимаем признак измененности коллекции 
            SetUpdateScrMain(false);

        } // UsersCancel()

        //--------------------------------------------
        // начальная установка пользователя для его коррекции
        //    Параметр - имя пользователя для позиционирования (при пустом - первый по алфавитному списку)
        public void UserSetBegin(string UserName, string ActionName)
        {
            int nSetIndex;

            //----------------------
            // свойства контролов по умолчанию
            txtUserName.Visible = false;
            lblUserName_Error.Visible = false;

            btnCancelNew.Visible = false;
            btnSaveNew.Visible = false;

            btnCancel.Text = "Undo";
            btnSave.Text = "Save";

            btnCancelNew.Text = "Cancel";
            btnSaveNew.Text = "Add";

            //------------------------
            // UserName - заполняем выпадаюший список
            cbxUserName.Items.Clear();
            foreach (TUser user in permSetModify.Users)
            {
                cbxUserName.Items.Add(user.UserName);
            }
            // сортируем элементы списка Имен
            SortCbxItems(cbxUserName);
            // позиционируем выпадающий список 
            if (cbxUserName.Items.Count > 0)  // на первый элемент списка
            {
                nSetIndex = 0;
                if (UserName != "")
                {
                    int index = cbxUserName.Items.IndexOf(cbxUserName.Items.FindByText(UserName));
                    if (index >= 0)  // может не быть быть!
                    {
                        nSetIndex = index; 
                    }
                }
                cbxUserName.SelectedIndex = nSetIndex; // c 0 !?
            }
            //if (cbxUserName.Items.Count == 0) - считается допустимым !
            //{
            //    //ResponseWriteError("Error permissions config-file - empty Users list: " + permSetModify.FileNameFull); // "<br>" + 
            //    ResponseWriteError(TMess.Mess0009 + " " + permSetModify.FileNameFull); // "<br>" + 
            //    return; //======================>
            //}

            //-----------------------------
            // Операции - заполняем выпадаюший список
            cbxAction.Items.Clear();
            foreach (TAction act in permSetModify.Actions)
            {
                cbxAction.Items.Add(act.ActionName);
            }
            if (cbxAction.Items.Count > 0)  
            {
                nSetIndex = 0;
                if (UserName != "")
                {
                    int index = cbxAction.Items.IndexOf(cbxAction.Items.FindByText(ActionName));
                    if (index >= 0)  // может не быть быть!
                    {
                        nSetIndex = index;
                    }
                }
                cbxAction.SelectedIndex = nSetIndex;
                sActionModify = cbxAction.Text;   // !!! надо здесь - не срабатывает событие смены индекса !!!
            }
            if (cbxAction.Items.Count == 0)  // не должно быть
            {
                //SayError(("Error permissions config-file - empty Actionы list: " + permSetModify.FileNameFull); 
                SayError(TMess.Mess0010 + "<br>File: " + permSetModify.FileNameFull);
                return; //======================>
            }

            //----------------------------------
            // Разрешения - устанавливаем строки сетки
            TPermissionGridRow TPermissionGridRow;
            foreach (TExtension ext in permSetModify.Extensions) // добавляем строки в сетку
            {
                TPermissionGridRow = new TPermissionGridRow(ext.ExtensionId, ext.ExtensionName, false, false);
                this.GridData.Add(TPermissionGridRow);
            }
            GridView1.DataSource = this.GridData;
            GridView1.DataBind();

            // изменяем высоту зоны просмотра сетки по числу строк в сетке (resize)
            //panGrid.Height = 40 + 30 * GridView1.Rows.Count;  - отказались т скроллинга !

            //---------------------
            // назначаем модиф.пользователя  по имени в списке ПОСЛЕ НАЧАЛЬНОГО ЗАПОЛНЕНИЯ страницы (с обновляем данных на экране для нового тек. пользователя)
            UserModifySet(cbxUserName.Text, false);  // false - не сохранять текущего модиф.пользователя в коллекции перед назначением нового!!!

        } // UserSetBegin()

        protected void btnSaveNew_Click(object sender, EventArgs e)
        {
            // переносим текущее состояние пользователя c экрана в объект-модификации (если не сменили на экране пользователя !)
            UserApply();  // берем Роль и Разрешения сетки с экрана в модифицируемого 

            // занесение нового (модифицируемого) пользователя с экрана в коллекцию
            UserAddToCollection();
        }


        //------------------------------------------------------------------
        // сохранение документа в файле, перезапуск процесса через событие
        //   BotUAC  - клиент (испльзуем существующее событие, если оно есть)
        //   Process - сервер (создает событие)
        // 
        public bool WriteChangesToXMLFileAndRestart(XDocument docNew, string FileNameFull, out string Message)
        {
            bool bFileSaved = false;
            bool bEventExist = false;
            Message = "";
            try
            {
                // готовим событие
                string sEventName = appSet.EventName;
                string sMutexName = appSet.MutexName;
                EventWaitHandle eventUpdFile = null;
                // открываем именованое событие (поцесс его должен был создать!)
                eventUpdFile = EventWaitHandle.OpenExisting(sEventName);
                bEventExist = true;
                // пытаемся создать Mutex (или открыть существующий)
                bool createdNew = false;
                Mutex mutexUpdFile = new Mutex(false, sMutexName, out createdNew);  // -> возвращает false, если mutex уже существует.
                // ждем Mutex
                mutexUpdFile.WaitOne();
                {
                    //------------------------------------
                    // сохраняем документ в файле с генерацией события
                    docNew.Save(permSetModify.FileNameFull);
                    bFileSaved = true;
                    //------------------------------------
                }
                // отпускаем мутекс
                mutexUpdFile.ReleaseMutex();
                // дергаем событие
                eventUpdFile.Set();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            // процесса (события) может  не быть - тогда просто сохраняем в файле
            if (!bEventExist)  
            {
                //------------------------------------
                // сохраняем документ в файле с генерацией события
                docNew.Save(permSetModify.FileNameFull);
                bFileSaved = true;
                //------------------------------------
            }
            // результат
            return bFileSaved; //===============>
        }

        //-----------------------------------------
        // назначает МОДИФИЦИРУЕМОГО пользователя ПО ИМЕНИ В СПИСКЕ пользователей на экране
        //   UserName - может быть пустым после удаления последнего пользователя !
        private void UserModifySet(string UserName, bool SaveCurrentModifyUserBeforeSet)
        {
            // сохраняем текущего модифицируемого пользователя в модифичируемую коллекцию перед назначением нового модифицируемого
            if (SaveCurrentModifyUserBeforeSet) // удаленный (после удаления пользователя) не сохраняем !!!
            {
                if (userModify != null)  // не начальный
                {
                    permSetModify.Users.Set(userModify.Clone());
                }
            }

            // назначаем нового модифицируемого пользователя
            if (UserName.Trim().Length > 0)
            {
                TUser userTmp = permSetModify.Users.FindUser(UserName);
                if (userTmp != null)  // должен быть !!!
                {
                    userModify = userTmp.Clone(); 
                }
                else  // не должно быть !!!
                {
                    ResponseWriteError("Erro UserModifySet(): Lost user " + UserName + "!");
                    userModify = new TUser("", "", new TPermissions());
                }
            }
            else
            {
                userModify = new TUser("", "", new TPermissions());
            }

            // выводим на экран все данные МОДИФИЦИРУЕМОГО пользователя
            RefreshScreenForUser();
        }  // UserModifySet()


        //----------------------------------------------------------
        // применене состояние экрана (СТАРОГО пользователя на экрана) в объекте-модификации 
        //  !!! берем Роль и Разрешения сетки с экрана в модифицируемого 
        private void UserApply()
        {
            // назначаем Имя
            if (txtUserName.Visible)  // только в режиме Добавления пользователя !
            {
                userModify.UserName = txtUserName.Text;   // на всякий случай
            }

            // назначаем Роль
            //userModify.UserRole = sUserRole;

            // назначаем Разрешения для action (allow + typedelay)
            string sActName = sActionModify; ;
            TPermission permAllow = new TPermission(sActName, TPermissionsSettings.NameAllow, new TParameters());
            TPermission permDeny = new TPermission(sActName, TPermissionsSettings.NameDeny, new TParameters());
            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    string extensName = row.Cells[1].Text;
                    //string extensId = row.Cells[0].Text;  -- у скрытой ячейки значение всегда ПУСТО !
                    string extensId = "";
                    TExtension extens = permSetModify.Extensions.FindExtensionName(extensName);
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
        }  // UserApply()


        //----------------------------------
        // сохранение в Сессии глобальных переменныхформы
        public void FormVariablesSaveToSession()
        {
            bVariablesToSessionSaved = true;          // просто как индикатор сохранности переменных в сессии
            Session["bVariablesToSessionSaved"] = bVariablesToSessionSaved;

            Session["appSet"] = appSet;
            Session["permSetOriginal"] = permSetOriginal;
            Session["permSetModify"] = permSetModify;
            Session["userModify"] = userModify;
            Session["userModifyBeforeAdd"] = userModifyBeforeAdd;
            Session["sActionModify"] = sActionModify;
            Session["bUpdateXmlFile_to_RestartProc"] = bUpdateXmlFile_to_RestartProc;
            Session["bUpdateScrMain"] = bUpdateScrMain;
            Session["bUpdateScrAdd"] = bUpdateScrAdd;
            Session["sMainMessage"] = sMainMessage;

            // !!! ViewState - требует сериализации объектов (доделать типы!)
            //ViewState["appSet"] = appSet;
            //ViewState["permSetModify"] = permSetModify;
            //ViewState["userModify"] = userModify;
            //ViewState["sActionModify"] = sActionModify;
            //ViewState["bUpdateXmlFile_to_RestartProc"] = bUpdateXmlFile_to_RestartProc;
            //ViewState["bUpdateScrMain"] = bUpdateScrMain;
            //ViewState["sMainMessage"] = sMainMessage;
        } // FormVariablesSaveToSession()

        //----------------------------------
        // восстановление из Сессии глобальных переменныхформы
        public void FormVariablesRestoreFromSession()
        {
            bVariablesToSessionSaved = (bool)Session["bVariablesToSessionSaved"];

            appSet = (TAppSettings)Session["appSet"];
            permSetOriginal = (TPermissionsSettings)Session["permSetOriginal"];
            permSetModify = (TPermissionsSettings)Session["permSetModify"];
            userModify = (TUser)Session["userModify"];
            userModifyBeforeAdd = (TUser)Session["userModifyBeforeAdd"];
            sActionModify = (string)Session["sActionModify"];
            bUpdateXmlFile_to_RestartProc = (bool)Session["bUpdateXmlFile_to_RestartProc"];
            bUpdateScrMain = (bool)Session["bUpdateScrMain"];
            bUpdateScrAdd = (bool)Session["bUpdateScrAdd"];
            sMainMessage = (string)Session["sMainMessage"];
            // !!! ViewState - требует сериализации объектов (доделать типы!)
            //appSet = (TAppSettings)ViewState["appSet"]; 
            //permSetModify = (TPermissionsSettings)ViewState["permSetModify"];
            //userModify = (TUser)ViewState["userModify"];
            //sActionModify = (string)ViewState["sActionModify"];
            //bUpdateXmlFile_to_RestartProc = (bool)ViewState["bUpdateXmlFile_to_RestartProc"];
            //bUpdateScrMain = (bool)ViewState["bUpdateScrMain"];
            //sMainMessage = (string)ViewState["sMainMessage"];
        } // FormVariablesRestoreFromSession()

        //----------------------------------
        // вывод в первю строку формы сообщения об ошибке
        public void ResponseWriteError(string sMessage)
        {
            // <font size="5" color="red" face="Arial"> текст </font>
            //ResponseWriteError( "<font size=\"5\" color=\"red\" face=\"Arial\">" +  sMessage + "</font>");
            Response.Write("<font color=\"red\">" + sMessage + "</font>");
        }

        //----------------------------------
        // вывод в первю строку формы инфомационное сообщение
        public void ResponseWriteInfo(string sMessage)
        {
            Response.Write(sMessage);
        } 



        //#######################################################################
        // !!! ПОСЛЕДНЯЯ - НЕ УДАЛЯТЬ, чтобы не стирался комментарий к закрывабщей процедуру скобке !!!
        public void Empty()
        {
        } 

    }  // WebFormBotUAC

}
