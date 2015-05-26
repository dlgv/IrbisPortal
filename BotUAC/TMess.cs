using System;
using System.Text;

namespace BotUAC
{
    // сообщения для вывода на экран
    public class TMess : Exception
    {
        // ошибка при загрузке (обработке) файла
        public const string Mess0001 = "Failed connecting to the application database.";   
        public const string Mess0002 = "Failed connecting to the permissions database.";   

        // тултипы кнопок
        public const string Mess0003 = "Delete current User";
        public const string Mess0004 = "Add new User";
        public const string Mess0005 = "Save User";
        public const string Mess0006 = "Cancel change User";
        public const string Mess0007 = "Save new User";
        public const string Mess0008 = "Cancel add new User";

        // пустой список пользователей в файле permsiions.xml
        public const string Mess0009 = "Invalid permissions config file: Users list is empty.";  // имя файла
        // пустой список действий в файле permsions.xml
        public const string Mess0010 = "Invalid permissions config file: Actions list is empty.";  // : имя файла

        // проверка имени нового пользователя
        public const string Mess0011 = "User name is empty!";
        public const string Mess0012 = "User name is already registered:";  // : новое имя пользователя

        // ошибка пи отмене изменений для пользователя - не должно такое произойти!
        public const string Mess0013 = "Restore Error - user not found:";  // : имя пользователя

        // запрос подтверждения удалнния пользователя
        public const string Mess0014 = "Are you sure you want to delete the user?";

        // тултипы кнопок заголовка сетки: Allow, Deny
        public const string Mess0015 = "Check All"; // Allow
        public const string Mess0016 = "Check All"; // Deny

        // загрузка пераметров приложения из web.config
        public const string Mess0017 = "<AppSettings> is not found in the web configuration";
        public const string Mess0018 = "The key <AppSettings.irbisFileNameAndPath> is not specified in the web configuration";
        public const string Mess0019 = "The value of the key <AppSettings.irbisFileNameAndPath> is invalid";

        // ошибка сохранения изменений в файле
        public const string Mess0020 = "Failed saving changes.";

    } // class TMessages
}
