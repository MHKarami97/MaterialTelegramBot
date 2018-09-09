using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using MaterialAlarm;
using Microsoft.Win32;
using ToastNotifications.Core;
using NetTelegramBotApi.Requests;
using NetTelegramBotApi.Types;
using NetTelegramBotApi;
using System.Threading.Tasks;

namespace MaterialTelegramBot
{
    public partial class MainWindow : Window
    {
        private const string BotToken = "520789610:AAGjxSUWUy8NXvZyybHN4uX6SOvi928w6QY";
        private static ReplyKeyboardMarkup mainMenu;
        private static ReplyKeyboardMarkup BackToMain;
        private static ReplyKeyboardMarkup AdminMenu;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new ToastViewModel();
        }
        private readonly ToastViewModel _vm;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key?.SetValue("MaterialTelegramBot", "\"" + System.Windows.Forms.Application.ExecutablePath + "\"");
            }

            GetIcon1().Icon = Properties.Resources.taskbarIcon;
            GetIcon1().Visible = true;

            mainMenu = new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[] { new KeyboardButton("\U0001F4C2 ابزارها \U0001F4C2") },
                    new[] { new KeyboardButton("\U000000A9 درباره \U000000A9"), new KeyboardButton("\U00002753 راهنما \U00002753") },
                    new[] { new KeyboardButton("\U0001F4DE تماس با ما \U0001F4DE") },
                },
                ResizeKeyboard = true
            };

            BackToMain = new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[] { new KeyboardButton("بازگشت به خانه \U0001F3E0") }
                },
                ResizeKeyboard = true
            };

            AdminMenu = new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[] { new KeyboardButton("\U0001F50D آمار \U0001F50D"),new KeyboardButton("\U0001F50A پیغام همگانی \U0001F50A") },
                    new[] {new KeyboardButton("\U0001F510 ربات قفل دار \U0001F510") ,new KeyboardButton("\U0001F511 مسدودسازی کاربر  \U0001F511") },
                    new[] { new KeyboardButton("\U00002714 پاسخ به پیغام ها \U00002714") },
                    new[] { new KeyboardButton("\U0001F3E0 بازگشت به خانه \U0001F3E0") }
                },
                ResizeKeyboard = true
            };

            Task.Run(RunBot);
        }

        string _lastMessage;
        void ShowMessage(Action<string, MessageOptions> action, string name)
        {
            MessageOptions opts = new MessageOptions
            {
                FreezeOnMouseEnter = true,
                UnfreezeOnMouseLeave = true,
                ShowCloseButton = true
            };
            _lastMessage = $"{name}";
            action(_lastMessage, opts);
        }

        NotifyIcon _icon = new NotifyIcon();

        public NotifyIcon GetIcon1()
        {
            return _icon;
        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void ShowSite(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://itarfand.com");
        }

        private void MiniBtn(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void SelecAllAlarm(object sender, RoutedEventArgs e)
        {

        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ColorChangeToggleButton(object sender, RoutedEventArgs e)
        {
            var bc = new BrushConverter();

            switch (ChangeColor.IsChecked)
            {
                case true:
                    MainGrid.Background = (Brush)bc.ConvertFrom("#303030");
                    TopMenu.Background = (Brush)bc.ConvertFrom("#607D8B");
                    TopRectangle.Fill = (Brush)bc.ConvertFrom("#607D8B");
                    ShowMessage(_vm.ShowSuccess, "Dark Theme");

                    break;
                case false:
                    MainGrid.Background = (Brush)bc.ConvertFrom("#FAFAFA");
                    TopMenu.Background = (Brush)bc.ConvertFrom("#512DA8");
                    TopRectangle.Fill = (Brush)bc.ConvertFrom("#512DA8");
                    ShowMessage(_vm.ShowInformation, "Light Theme");
                    break;
            }
        }

        private void AboutMe(object sender, RoutedEventArgs e)
        {

        }

        private static async Task RunBot()
        {
            var bot = new TelegramBot(BotToken);
            var me = await bot.MakeRequestAsync(new GetMe());

            var db = new MaterialtelegramBotdbEntities();

            long offset = 0;
            var whatDo = 0;
            const long adminId = 87310097;

            while (true)
            {
                var updates = await bot.MakeRequestAsync(new GetUpdates() { Offset = offset });

                foreach (var item in updates)
                {
                    try
                    {
                        var qGetBotUser = (from a in db.Users
                                           where a.UserID.Equals(item.Message.Chat.Id)
                                           select a).SingleOrDefault();

                        if (qGetBotUser == null)
                        {
                            var TBotUser = new User
                            {
                                UserID = item.Message.Chat.Id,
                                FirstName = item.Message.Chat.FirstName,
                                LastName = item.Message.Chat.LastName,
                                UserName = item.Message.Chat.Username
                            };

                            db.Users.Add(TBotUser);
                            db.SaveChanges();
                        }

                        offset = item.UpdateId + 1;

                        var id = item.Message.Chat.Id;
                        var userText = item.Message.Text;
                        var from = item.Message.From;
                        var photos = item.Message.Photo;
                        var contact = item.Message.Contact;
                        var location = item.Message.Location;
                        var queryBack = item.CallbackQuery;

                        var qGet = (from a in db.Settings
                                    where a.Id.Equals(1)
                                    select a).SingleOrDefault();

                        var checkUser = new GetChatMember(qGet.Channel ?? 0, id);
                        var isUser = await bot.MakeRequestAsync(checkUser);

                        var isBotLock = false;
                        var isUserHasAccess = false;

                        if (qGet.Channel != null)
                        {
                            isBotLock = true;
                            if (isUser.Status == "administrator" || isUser.Status == "member" || isUser.Status == "creator")
                            {
                                isUserHasAccess = true;
                            }
                        }

                        if (item.Message.Chat.Id == adminId)
                        {
                            mainMenu = new ReplyKeyboardMarkup
                            {
                                Keyboard = new[] {
                                                                  new[] { new KeyboardButton("\U000000A9 درباره \U000000A9"), new KeyboardButton("\U00002753 راهنما \U00002753") },
                                                                  new[] { new KeyboardButton("تنظیمات مدیر \U0001F3E0") }
                                                                  },
                                ResizeKeyboard = true
                            };
                        }
                        else
                        {
                            mainMenu = new ReplyKeyboardMarkup
                            {
                                Keyboard = new[] {
                                                                  new[] { new KeyboardButton("\U000000A9 درباره \U000000A9"), new KeyboardButton("\U00002753 راهنما \U00002753") },
                                                                  new[] { new KeyboardButton("\U0001F4DE تماس با ما \U0001F4DE") },
                                                                  },
                                ResizeKeyboard = true
                            };
                        }

                        if ((isBotLock && isUserHasAccess) || (isBotLock == false))
                        {
                            if (whatDo == 0)
                            {
                                switch (userText)
                                {
                                    case "/start":

                                        var req = new SendMessage(item.Message.Chat.Id, "به ربات پیام رسان خوش آمدید") { ReplyMarkup = mainMenu };

                                        await bot.MakeRequestAsync(req);

                                        break;

                                    //************************************
                                    case string a when a.Contains("خانه"):

                                        req = new SendMessage(item.Message.Chat.Id, "خانه ربات") { ReplyMarkup = mainMenu };

                                        await bot.MakeRequestAsync(req);

                                        break;

                                    //************************************
                                    case string a when a.Contains("تنظیمات مدیر"):

                                        req = new SendMessage(item.Message.Chat.Id, "در این بخش می توانید تنظیمات خود را اعمال کنید") { ReplyMarkup = AdminMenu };

                                        await bot.MakeRequestAsync(req);

                                        break;

                                    //************************************
                                    case string a when a.Contains("درباره"):

                                        req = new SendMessage(item.Message.Chat.Id, "ربات سایت آی ترفند") { ReplyMarkup = BackToMain };

                                        await bot.MakeRequestAsync(req);

                                        break;

                                    //************************************
                                    case string a when a.Contains("راهنما"):

                                        req = new SendMessage(item.Message.Chat.Id, "این روبات برای سایت آی ترفند می باشد \n" +
                                                                                    " توسط آن می توانید کارهای محتلفی انجام بدهید")
                                        { ReplyMarkup = BackToMain };

                                        await bot.MakeRequestAsync(req);

                                        break;

                                    //************************************
                                    case string a when a.Contains("ربات قفل دار"):

                                        if (item.Message.Chat.Id == adminId)
                                        {
                                            req = new SendMessage(item.Message.Chat.Id, "لطفا آدرس کانالی که میخواهید کاربر در صورت عضو بود در آن بتواند از ربات استفاده کند را به همراه @ مانند @itarfand وارد کنید" +
                                                                                        "\n\nاگر نمی خواهید کلمه *نه* را وارد کنید")
                                            { ReplyMarkup = AdminMenu, ParseMode = SendMessage.ParseModeEnum.Markdown };

                                            await bot.MakeRequestAsync(req);

                                            whatDo = 11;
                                        }
                                        else
                                        {
                                            req = new SendMessage(item.Message.Chat.Id, "شما مدیر نیستید") { ReplyMarkup = mainMenu };

                                            await bot.MakeRequestAsync(req);
                                        }
                                        break;

                                    //************************************
                                    case string a when a.Contains("تماس با ما"):

                                        req = new SendMessage(item.Message.Chat.Id, "لطفا پیغام خود را بفرستید. در اسرع وقت پاسخ داده خواهد شد" +
                                            "\nنکته: پیغام خود را فقط در این پست بفرستید");

                                        await bot.MakeRequestAsync(req);

                                        whatDo = 12;

                                        break;

                                    //************************************
                                    case string a when a.Contains("پاسخ به پیغام ها"):

                                        if (item.Message.Chat.Id == adminId)
                                        {
                                            req = new SendMessage(item.Message.Chat.Id, "لطفا بر روی پیغام مورد نظر راست کلیک کنید و reply را انتخاب کرده و پیغام خود را بنویسید");

                                            await bot.MakeRequestAsync(req);

                                            whatDo = 20;
                                        }
                                        else
                                        {
                                            req = new SendMessage(item.Message.Chat.Id, "شما مدیر نیستید") { ReplyMarkup = mainMenu };

                                            await bot.MakeRequestAsync(req);
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                if (whatDo == 11)
                                {
                                    var requserText = new SendMessage(item.Message.Chat.Id, userText) { ReplyMarkup = AdminMenu };

                                    await bot.MakeRequestAsync(requserText);

                                    try
                                    {
                                        var qGetLock = (from a in db.Settings
                                                        where a.Id.Equals(1)
                                                        select a).SingleOrDefault();

                                        if (userText.Equals("نه"))
                                        {
                                            qGetLock.Channel = 1235;
                                        }
                                        else
                                        {
                                            qGetLock.Channel = Convert.ToInt64(userText);
                                        }

                                        db.Settings.Attach(qGetLock);
                                        db.Entry(qGetLock).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    catch (Exception)
                                    {
                                        var req = new SendMessage(item.Message.Chat.Id, "خطا") { ReplyMarkup = mainMenu };

                                        await bot.MakeRequestAsync(req);

                                        continue;
                                    }

                                    whatDo = 0;
                                }

                                else if (whatDo == 12)
                                {
                                    //var requserText = new SendMessage(adminId, from + userText) { ReplyMarkup = mainMenu, ParseMode = SendMessage.ParseModeEnum.Markdown };

                                    var fileId = item.Message.MessageId;
                                    var fromUser = item.Message.From.Id;
                                    var req2 = new ForwardMessage(adminId, fromUser, fileId);

                                    //await bot.MakeRequestAsync(requserText);
                                    await bot.MakeRequestAsync(req2);

                                    whatDo = 0;
                                }

                                else if (whatDo == 20)
                                {
                                    if (item.Message.ReplyToMessage?.Text != null)
                                    {
                                        var toUser = item.Message.ReplyToMessage.ForwardFrom.Id;

                                        var req2 = new SendMessage(toUser, item.Message.Text);
                                        await bot.MakeRequestAsync(req2);

                                        whatDo = 0;
                                    }
                                    whatDo = 0;
                                }

                                else
                                {
                                    var req3 = new SendMessage(item.Message.Chat.Id, "لطفا ابزار مورد نظر خود را انتخاب کنید") { ReplyMarkup = mainMenu };

                                    await bot.MakeRequestAsync(req3);
                                }
                            }
                        }
                        else
                        {
                            var req = new SendMessage(item.Message.Chat.Id, "برای استفاده از این ربات ابتدا باید در کانال @itarfand عضو شوید");

                            await bot.MakeRequestAsync(req);
                        }
                    }

                    catch (Exception)
                    {
                        var req = new SendMessage(item.Message.Chat.Id, "خطا") { ReplyMarkup = mainMenu };

                        await bot.MakeRequestAsync(req);

                        whatDo = 0;

                        break;
                    }
                }
            }
        }
    }
}
