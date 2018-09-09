using NetTelegramBotApi;
using NetTelegramBotApi.Requests;
using NetTelegramBotApi.Types;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace MaterialTelegramBot
{
    class BotCode
    {
        private const string BotToken = "520789610:AAGjxSUWUy8NXvZyybHN4uX6SOvi928w6QY";
        private static ReplyKeyboardMarkup mainMenu;
        private static ReplyKeyboardMarkup BackToMain;
        private static ReplyKeyboardMarkup AdminMenu;
        private static InlineKeyboardMarkup siteOpen;
        private static InlineKeyboardMarkup UserButton;

        private static void Main()
        {
            siteOpen = new InlineKeyboardMarkup
            {
                InlineKeyboard = new[] {
                                           new[] { new InlineKeyboardButton { Text = "آی ترفند", Url = "https://itarfand.com" } },
                                           new[] { new InlineKeyboardButton { Text = "کلیک", CallbackData="te" } }
                },
            };

            mainMenu = new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                                    new[] { new KeyboardButton("\U000023E9 آخرین ارسالی های سایت \U000023E9") , new KeyboardButton("\U000027B0 آدرس سایت \U000027B0") },
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

            var t = Task.Run(() => RunBot(BotToken));
        }

        private static async Task RunBot(string accessToken)
        {
            var bot = new TelegramBot(accessToken);

            var me = await bot.MakeRequestAsync(new GetMe());

            Console.WriteLine("UserName is: " + me.Username);

            long offset = 0;
            var whileConut = 0;
            var whatDo = 0;
            const long adminId = 87310097;

            while (true)
            {
                Console.WriteLine("in while: " + whileConut++);

                var updates = await bot.MakeRequestAsync(new GetUpdates() { Offset = offset });

                Console.WriteLine("in updates: " + updates.Length);
                Console.WriteLine("------------");

                foreach (var item in updates)
                {
                    try
                    {
                        var qGetBotUser = (from a in db.AllUsers
                                           where a.UserID.Equals(item.Message.Chat.Id)
                                           select a).SingleOrDefault();

                        if (qGetBotUser == null)
                        {
                            var TBotUser = new AllUser
                            {
                                UserID = item.Message.Chat.Id,
                                FirstName = item.Message.Chat.FirstName,
                                LastName = item.Message.Chat.LastName,
                                UserName = item.Message.Chat.Username
                            };

                            db.AllUsers.Add(TBotUser);
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

                        //var qGet = (from a in db.LockBots
                        //            where a.Id.Equals(1)
                        //            select a).SingleOrDefault();

                        var checkUser = new GetChatMember(qGet.ChannelName, id);
                        var isUser = await bot.MakeRequestAsync(checkUser);

                        var isBotLock = false;
                        var isUserHasAccess = false;

                        if (qGet.IsLock == true)
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
                                                                  new[] { new KeyboardButton("\U000023E9 آخرین ارسالی های سایت \U000023E9") , new KeyboardButton("\U000027B0 آدرس سایت \U000027B0") },
                                                                  new[] { new KeyboardButton("\U0001F4C2 ابزارها \U0001F4C2") },
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
                                                                  new[] { new KeyboardButton("\U000023E9 آخرین ارسالی های سایت \U000023E9") , new KeyboardButton("\U000027B0 آدرس سایت \U000027B0") },
                                                                  new[] { new KeyboardButton("\U0001F4C2 ابزارها \U0001F4C2") },
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
                                        //await bot.MakeRequestAsync(reqen);

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
                                    case string a when a.Contains("آدرس سایت"):

                                        req = new SendMessage(item.Message.Chat.Id, "https://itarfand.com") { ReplyMarkup = siteOpen, ParseMode = SendMessage.ParseModeEnum.Markdown };

                                        await bot.MakeRequestAsync(req);

                                        //if item.CallbackQuery:
                                        //    if item.CallbackQuery.Data=='te':

                                        //if (queryBack.Data == "te" )
                                        //{
                                        //    var reeq = new SendMessage(item.Message.Chat.Id, "click");

                                        //    await bot.MakeRequestAsync(reeq);
                                        //}

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
                                    case string a when a.Contains("آخرین ارسالی های سایت"):

                                        req = new SendMessage(item.Message.Chat.Id, "چند پست آخر سایت را می خواهید مشاهده کنید؟");

                                        await bot.MakeRequestAsync(req);

                                        whatDo = 13;

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
                                if (whatDo == 6)
                                {
                                    var requserText = new SendMessage(item.Message.Chat.Id, userText) { ReplyMarkup = Tools, ParseMode = SendMessage.ParseModeEnum.Markdown };

                                    await bot.MakeRequestAsync(requserText);

                                    whatDo = 0;
                                }

                                else if (whatDo == 7)
                                {
                                    var calculation = Calculator.Calculate(userText);

                                    if (calculation.IsValid)
                                    {
                                        var req = new SendMessage(item.Message.Chat.Id, "\U0001F448 حاصل \n" + calculation.Result + "\n \U0000261D") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(req);
                                    }
                                    else
                                    {
                                        var req = new SendMessage(item.Message.Chat.Id, "اعداد وارد شده اشتباه است") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(req);
                                    }

                                    whatDo = 0;
                                }

                                else if (whatDo == 9)
                                {
                                    if (item.Message.Photo != null)
                                    {
                                        var reqImage = new SendMessage(item.Message.Chat.Id, "حله!!! الان رو عکس راست کلیک کن و Reply رو بزن");

                                        await bot.MakeRequestAsync(reqImage);

                                        continue;
                                    }

                                    if (item.Message.ReplyToMessage?.Photo != null)
                                    {
                                        var fileId = item.Message.ReplyToMessage.Photo.LastOrDefault().FileId;
                                        var reqShow = new SendMessage(item.Message.Chat.Id, "\U0001F447 بفرما \U0001F447") { ReplyMarkup = Tools };
                                        var reqPhoto = new SendPhoto(item.Message.Chat.Id, new FileToSend(fileId)) { Caption = userText };

                                        await bot.MakeRequestAsync(reqShow);
                                        await bot.MakeRequestAsync(reqPhoto);
                                    }

                                    whatDo = 0;
                                }

                                else if (whatDo == 8)
                                {
                                    var reqShow = new SendMessage(item.Message.Chat.Id, "\U0001F447 بفرما \U0001F447") { ReplyMarkup = Tools };

                                    if (item.Message.Text != null)
                                    {
                                        if (item.Message.ForwardFromChat != null)
                                        {
                                            userText = userText.Replace("@" + item.Message.ForwardFromChat.Username, "").Replace("https://t.me/" + item.Message.ForwardFromChat.Username, "");
                                        }
                                        else if (item.Message.ForwardFrom != null)
                                        {
                                            userText = userText.Replace("@" + item.Message.ForwardFrom.Username, "").Replace("https://t.me/" + item.Message.ForwardFrom.Username, "");
                                        }

                                        var reqImage = new SendMessage(item.Message.Chat.Id, userText);

                                        await bot.MakeRequestAsync(reqShow);
                                        await bot.MakeRequestAsync(reqImage);
                                    }
                                    else if (item.Message.Photo != null)
                                    {
                                        //GetChat chatGet = new GetChat(item.Message.ForwardFromChat.Username);
                                        //var channelInfo = await bot.MakeRequestAsync(chatGet);

                                        if (item.Message.ForwardFromChat != null)
                                        {
                                            item.Message.Caption = item.Message.Caption.Replace("@" + item.Message.ForwardFromChat.Username, "").Replace("https://t.me/" + item.Message.ForwardFromChat.Username, "");
                                        }
                                        else if (item.Message.ForwardFrom != null)
                                        {
                                            userText = userText.Replace("@" + item.Message.ForwardFrom.Username, "").Replace("https://t.me/" + item.Message.ForwardFrom.Username, "");
                                        }

                                        var fileId = item.Message.Photo.LastOrDefault().FileId;
                                        var reqPhoto = new SendPhoto(item.Message.Chat.Id, new FileToSend(fileId)) { Caption = item.Message.Caption };

                                        await bot.MakeRequestAsync(reqShow);
                                        await bot.MakeRequestAsync(reqPhoto);
                                    }
                                    else if (item.Message.Video != null)
                                    {
                                        if (item.Message.ForwardFromChat != null)
                                        {
                                            item.Message.Caption = item.Message.Caption.Replace("@" + item.Message.ForwardFromChat.Username, "").Replace("https://t.me/" + item.Message.ForwardFromChat.Username, "");
                                        }
                                        else if (item.Message.ForwardFrom != null)
                                        {
                                            userText = userText.Replace("@" + item.Message.ForwardFrom.Username, "").Replace("https://t.me/" + item.Message.ForwardFrom.Username, "");
                                        }

                                        var fileId = item.Message.ReplyToMessage.Video.FileId;
                                        var reqPhoto = new SendVideo(item.Message.Chat.Id, new FileToSend(fileId)) { Caption = item.Message.Caption };

                                        await bot.MakeRequestAsync(reqShow);
                                        await bot.MakeRequestAsync(reqPhoto);
                                    }
                                    else if (item.Message.Voice != null)
                                    {
                                        if (item.Message.ForwardFromChat != null)
                                        {
                                            item.Message.Caption = item.Message.Caption.Replace("@" + item.Message.ForwardFromChat.Username, "").Replace("https://t.me/" + item.Message.ForwardFromChat.Username, "");
                                        }
                                        else if (item.Message.ForwardFrom != null)
                                        {
                                            userText = userText.Replace("@" + item.Message.ForwardFrom.Username, "").Replace("https://t.me/" + item.Message.ForwardFrom.Username, "");
                                        }

                                        var fileId = item.Message.ReplyToMessage.Voice.FileId;
                                        var reqPhoto = new SendVoice(item.Message.Chat.Id, new FileToSend(fileId)) { Caption = item.Message.Caption };

                                        await bot.MakeRequestAsync(reqShow);
                                        await bot.MakeRequestAsync(reqPhoto);
                                    }
                                    else if (item.Message.Sticker != null)
                                    {
                                        var fileId = item.Message.ReplyToMessage.Sticker.FileId;
                                        var reqPhoto = new SendSticker(item.Message.Chat.Id, new FileToSend(fileId));

                                        await bot.MakeRequestAsync(reqShow);
                                        await bot.MakeRequestAsync(reqPhoto);
                                    }

                                    whatDo = 0;
                                }

                                else if (whatDo == 10)
                                {
                                    var urltxt = URLShorter(userText);

                                    var requserText = new SendMessage(item.Message.Chat.Id, urltxt) { ReplyMarkup = Tools, DisableWebPagePreview = true };

                                    await bot.MakeRequestAsync(requserText);

                                    whatDo = 0;
                                }

                                else if (whatDo == 11)
                                {
                                    var requserText = new SendMessage(item.Message.Chat.Id, userText) { ReplyMarkup = AdminMenu };

                                    await bot.MakeRequestAsync(requserText);

                                    try
                                    {
                                        var qGetLock = (from a in db.LockBots
                                                        where a.Id.Equals(1)
                                                        select a).SingleOrDefault();

                                        if (userText.Equals("نه"))
                                        {
                                            qGetLock.IsLock = false;
                                            qGetLock.ChannelName = "@itarfand";
                                        }
                                        else
                                        {
                                            qGetLock.IsLock = true;
                                            qGetLock.ChannelName = userText;
                                        }

                                        db.LockBots.Attach(qGetLock);
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

                                else if (whatDo == 13)
                                {
                                    var countpost = 0;

                                    countpost = int.Parse(userText);

#pragma warning disable 618
                                    var feed = FeedReader.Read("http://itarfand.com/feed/");
#pragma warning restore 618

                                    foreach (var itemFeed in feed.Items)
                                    {
                                        var i = 0;
                                        if (i == countpost)
                                        {
                                            break;
                                        }

                                        var requserText = new SendMessage(adminId, "[" + itemFeed.Title + "](" + itemFeed.Link + ")") { ReplyMarkup = mainMenu, ParseMode = SendMessage.ParseModeEnum.Markdown };

                                        await bot.MakeRequestAsync(requserText);

                                        i++;
                                    }

                                    whatDo = 0;
                                }

                                else if (whatDo == 14)
                                {
                                    if (item.Message.Photo != null)
                                    {
                                        var fileId = item.Message.Photo.LastOrDefault().FileId;
                                        var file = await bot.MakeRequestAsync(new GetFile(fileId));

                                        SaveFile(file.FileDownloadUrl, "imageToSticker.jpg");

                                        using (var streem = System.IO.File.Open("imageToSticker.jpg", FileMode.Open))
                                        {
                                            var req2 = new SendSticker(item.Message.Chat.Id, new FileToSend(streem, "imageToSticker.jpg"));

                                            await bot.MakeRequestAsync(req2);

                                            streem.Close();
                                            System.IO.File.Delete("imageToSticker.jpg");
                                        }

                                        var req3 = new SendMessage(item.Message.Chat.Id, "بفرما");

                                        await bot.MakeRequestAsync(req3);
                                    }
                                    else
                                    {
                                        var reqError = new SendMessage(item.Message.Chat.Id, "فایل اشتباه است");

                                        await bot.MakeRequestAsync(reqError);
                                    }

                                    whatDo = 0;
                                }

                                else if (whatDo == 15)
                                {
                                    if (item.Message.Sticker != null)
                                    {
                                        var fileId = item.Message.Sticker.FileId;
                                        var file = await bot.MakeRequestAsync(new GetFile(fileId));

                                        SaveFile(file.FileDownloadUrl, "StickerToImage.jpg");

                                        using (var streem = System.IO.File.Open("StickerToImage.jpg", FileMode.Open))
                                        {
                                            var req2 = new SendPhoto(item.Message.Chat.Id, new FileToSend(streem, "StickerToImage.jpg"));

                                            await bot.MakeRequestAsync(req2);

                                            streem.Close();
                                            System.IO.File.Delete("StickerToImage.jpg");
                                        }

                                        var req3 = new SendMessage(item.Message.Chat.Id, "بفرما");

                                        await bot.MakeRequestAsync(req3);
                                    }
                                    else
                                    {
                                        var reqError = new SendMessage(item.Message.Chat.Id, "فایل اشتباه است");

                                        await bot.MakeRequestAsync(reqError);
                                    }

                                    whatDo = 0;
                                }

                                else if (whatDo == 16)
                                {
                                    if (item.Message.Video != null)
                                    {
                                        var fileId = item.Message.Video.FileId;
                                        var file = await bot.MakeRequestAsync(new GetFile(fileId));

                                        SaveFile(file.FileDownloadUrl, "VideoToMusic.mp3");

                                        using (var streem = System.IO.File.Open("VideoToMusic.mp3", FileMode.Open))
                                        {
                                            var req2 = new SendDocument(item.Message.Chat.Id, new FileToSend(streem, "VideoToMusic.mp3"));

                                            await bot.MakeRequestAsync(req2);

                                            streem.Close();
                                            System.IO.File.Delete("VideoToMusic.mp3");
                                        }

                                        var req3 = new SendMessage(item.Message.Chat.Id, "بفرما");

                                        await bot.MakeRequestAsync(req3);
                                    }
                                    else
                                    {
                                        var reqError = new SendMessage(item.Message.Chat.Id, "فایل اشتباه است");

                                        await bot.MakeRequestAsync(reqError);
                                    }

                                    whatDo = 0;
                                }

                                else if (whatDo == 17)
                                {
                                    if (item.Message.Voice != null)
                                    {
                                        var fileId = item.Message.Voice.FileId;
                                        var file = await bot.MakeRequestAsync(new GetFile(fileId));

                                        SaveFile(file.FileDownloadUrl, "VoiceToMusic.mp3");

                                        using (var streem = System.IO.File.Open("VoiceToMusic.mp3", FileMode.Open))
                                        {
                                            var req2 = new SendDocument(item.Message.Chat.Id, new FileToSend(streem, "VoiceToMusic.mp3"));

                                            await bot.MakeRequestAsync(req2);

                                            streem.Close();
                                            System.IO.File.Delete("VoiceToMusic.mp3");
                                        }

                                        var req3 = new SendMessage(item.Message.Chat.Id, "بفرما");

                                        await bot.MakeRequestAsync(req3);
                                    }
                                    else
                                    {
                                        var reqError = new SendMessage(item.Message.Chat.Id, "فایل اشتباه است");

                                        await bot.MakeRequestAsync(reqError);
                                    }

                                    whatDo = 0;
                                }

                                else if (whatDo == 18)
                                {
                                    var requserText = new SendMessage(item.Message.Chat.Id, "[لینک مستقیم](https://github.com/" + userText + "/archive/master.zip)") { ReplyMarkup = Tools, ParseMode = SendMessage.ParseModeEnum.Markdown };
                                    var requserText1 = new SendMessage(item.Message.Chat.Id, "[نسخه های مختلف](https://github.com/" + userText + "/releases)") { ReplyMarkup = Tools, ParseMode = SendMessage.ParseModeEnum.Markdown };
                                    var requserText2 = new SendMessage(item.Message.Chat.Id, "[توضیحات](https://github.com/" + userText + ")") { ReplyMarkup = Tools, ParseMode = SendMessage.ParseModeEnum.Markdown };

                                    await bot.MakeRequestAsync(requserText);
                                    await bot.MakeRequestAsync(requserText1);
                                    await bot.MakeRequestAsync(requserText2);

                                    whatDo = 0;
                                }

                                else if (whatDo == 19)
                                {
                                    if (item.Message.Photo != null)
                                    {
                                        var fileId = item.Message.Photo.LastOrDefault().FileId;
                                        var fileGet = new GetFile(fileId);
                                        var fileDL = await bot.MakeRequestAsync(fileGet);

                                        var requserText = new SendMessage(item.Message.Chat.Id, fileDL.FileDownloadUrl + "\n\nحجم فایل: " + (float)fileDL.FileSize / 1000000 + "MB") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(requserText);

                                        var req3 = new SendMessage(item.Message.Chat.Id, "لینک تا 1 ساعت معتبر است") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(req3);
                                    }
                                    else if (item.Message.Video != null)
                                    {
                                        var fileId = item.Message.Video.FileId;
                                        var fileGet = new GetFile(fileId);
                                        var fileDL = await bot.MakeRequestAsync(fileGet);

                                        var requserText = new SendMessage(item.Message.Chat.Id, fileDL.FileDownloadUrl + "\n\nحجم فایل: " + (float)fileDL.FileSize / 1000000 + "MB") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(requserText);

                                        var req3 = new SendMessage(item.Message.Chat.Id, "لینک تا 1 ساعت معتبر است") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(req3);
                                    }
                                    else if (item.Message.Voice != null)
                                    {
                                        var fileId = item.Message.Voice.FileId;
                                        var fileGet = new GetFile(fileId);
                                        var fileDL = await bot.MakeRequestAsync(fileGet);

                                        var requserText = new SendMessage(item.Message.Chat.Id, fileDL.FileDownloadUrl + "\n\nحجم فایل: " + (float)fileDL.FileSize / 1000000 + "MB") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(requserText);

                                        var req3 = new SendMessage(item.Message.Chat.Id, "لینک تا 1 ساعت معتبر است") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(req3);
                                    }
                                    else if (item.Message.Sticker != null)
                                    {
                                        var fileId = item.Message.Sticker.FileId;
                                        var fileGet = new GetFile(fileId);
                                        var fileDL = await bot.MakeRequestAsync(fileGet);

                                        var requserText = new SendMessage(item.Message.Chat.Id, fileDL.FileDownloadUrl + "\n\nحجم فایل: " + (float)fileDL.FileSize / 1000000 + "MB") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(requserText);

                                        var req3 = new SendMessage(item.Message.Chat.Id, "لینک تا 1 ساعت معتبر است") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(req3);
                                    }
                                    else if (item.Message.Audio != null)
                                    {
                                        var fileId = item.Message.Audio.FileId;
                                        var fileGet = new GetFile(fileId);
                                        var fileDL = await bot.MakeRequestAsync(fileGet);

                                        var requserText = new SendMessage(item.Message.Chat.Id, fileDL.FileDownloadUrl + "\n\nحجم فایل: " + (float)fileDL.FileSize / 1000000 + "MB") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(requserText);

                                        var req3 = new SendMessage(item.Message.Chat.Id, "لینک تا 1 ساعت معتبر است") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(req3);
                                    }
                                    else if (item.Message.Document != null)
                                    {
                                        var fileId = item.Message.Document.FileId;
                                        var fileGet = new GetFile(fileId);
                                        var fileDL = await bot.MakeRequestAsync(fileGet);

                                        var requserText = new SendMessage(item.Message.Chat.Id, fileDL.FileDownloadUrl + "\n\nحجم فایل: " + (float)fileDL.FileSize / 1000000 + "MB") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(requserText);

                                        var req3 = new SendMessage(item.Message.Chat.Id, "لینک تا 1 ساعت معتبر است") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(req3);
                                    }
                                    else
                                    {
                                        var req3 = new SendMessage(item.Message.Chat.Id, "فایل اشتباه است") { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(req3);
                                    }

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
                                else if (whatDo == 21)
                                {
                                    // whatDo = 0;
                                    var client = new OpenWeatherMapClient("331c060a115abfe9eab4bba1c2c9a3ae");
                                    var currentWeather = await client.CurrentWeather.GetByName(userText);

                                    var req = new SendMessage(item.Message.Chat.Id,
                                        "\n \U0001F516 کشور: " + currentWeather.City.Country +
                                        "\n\n \U00002B50 شهر: " + currentWeather.City.Name +
                                        "\n\n \U00002601 مقدار ابر: " + currentWeather.Clouds.Value + " %" +
                                        "\n\n \U000026C5 وضعیت ابر: " + currentWeather.Clouds.Name +
                                        "\n\n \U0001F300 نام باد: " + currentWeather.Wind.Direction.Name +
                                        "\n\n \U0001F343 سرعت باد: " + currentWeather.Wind.Speed.Name +
                                        "\n\n \U0001F4AE وضعیت فشار هوا: " + currentWeather.Pressure.Value + " " + currentWeather.Pressure.Unit +
                                        "\n\n \U00002601 حداکثر دما: " + (currentWeather.Temperature.Max - 273) + " C" +
                                        "\n\n \U0001F4A6 حداقل دما: " + (currentWeather.Temperature.Min - 273) + " C" +
                                        "\n\n \U0001F536 رطوبت: " + currentWeather.Humidity.Value + " " + currentWeather.Humidity.Unit +
                                        "\n\n \U00002614 وضعیت بارش: " + currentWeather.Precipitation.Mode +
                                        "\n\n \U0001F4A7 مقدار بارش: " + currentWeather.Precipitation.Value + " " + currentWeather.Precipitation.Unit +
                                        "\n\n دمای امروز"
                                        )
                                    { ReplyMarkup = Tools };

                                    await bot.MakeRequestAsync(req);

                                    whatDo = 0;
                                }
                                else if (whatDo == 22)
                                {
                                    //using (SpeechSynthesizer reader = new SpeechSynthesizer())
                                    //{
                                    //    //set some settings
                                    //    reader.Volume = 100;
                                    //    reader.Rate = 0; //medium

                                    //    //save to memory stream
                                    //    MemoryStream ms = new MemoryStream();
                                    //    reader.SetOutputToWaveStream(ms);

                                    //    //do speaking
                                    //    reader.Speak(userText);

                                    //    //now convert to mp3 using LameEncoder or shell out to audiograbber
                                    //    ConvertWavStreamToMp3File(ref ms, "mySpeech.mp3");
                                    //}

                                    var req = new SendMessage(item.Message.Chat.Id, "این قسمت در حال کد نویسی می باشد") { ReplyMarkup = Tools };

                                    await bot.MakeRequestAsync(req);

                                    whatDo = 0;
                                }

                                else if (whatDo == 23)
                                {
                                    UserButton = new InlineKeyboardMarkup
                                    {
                                        InlineKeyboard = new[] {
                                           new[] { new InlineKeyboardButton { Text = userText, Url = "https://itarfand.com" } }
                                        },
                                    };

                                    var req = new SendMessage(item.Message.Chat.Id, "بفرمایید") { ReplyMarkup = UserButton };

                                    await bot.MakeRequestAsync(req);

                                    whatDo = 0;
                                }
                                else if (whatDo == 24)
                                {
                                    if (item.Message.ForwardFrom != null)
                                    {
                                        var req1 = new SendMessage(item.Message.Chat.Id,
                                        "\n \U0001F636 نام: " + item.Message.ForwardFrom.FirstName +
                                        "\n \U0001F31E نام خانوادگی: " + item.Message.ForwardFrom.LastName +
                                        "\n \U0001F539 یوزرنیم: " + item.Message.ForwardFrom.Username +
                                        "\n \U0001F538 آی دی: " + item.Message.ForwardFrom.Id +
                                        "\n\n اطلاعات کاربر")
                                        { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(req1);
                                    }
                                    else
                                    {
                                        var req1 = new SendMessage(item.Message.Chat.Id,
                                        "\n \U0001F636 نام: " + item.Message.From.FirstName +
                                        "\n \U0001F31E نام خانوادگی: " + item.Message.From.LastName +
                                        "\n \U0001F539 یوزرنیم: " + item.Message.From.Username +
                                        "\n \U0001F538 آی دی: " + item.Message.From.Id +
                                        "\n\n اطلاعات کاربر")
                                        { ReplyMarkup = Tools };

                                        await bot.MakeRequestAsync(req1);
                                    }

                                    whatDo = 0;
                                }
                                else if (whatDo == 25)
                                {
                                    var req3 = new SendMessage(item.Message.Chat.Id, "\U00002B06 بفرمایید \U00002B06") { ReplyMarkup = Tools };

                                    await bot.MakeRequestAsync(req3);

                                    whatDo = 0;
                                }
                                else if (whatDo == 26)
                                {
                                    var req = (HttpWebRequest)WebRequest.Create(userText);
                                    req.Method = "HEAD";
                                    using (var resp = (HttpWebResponse)(req.GetResponse()))
                                    {
                                        var len = resp.ContentLength;
                                        const long maxSize = 200000000;

                                        if (len < maxSize)
                                        {
                                            var uri = new Uri(userText);
                                            var fileName = Path.GetFileName(uri.AbsolutePath);

                                            SaveFile(userText, fileName);

                                            using (var streem = System.IO.File.Open(fileName, FileMode.Open))
                                            {
                                                var req2 = new SendDocument(item.Message.Chat.Id, new FileToSend(streem, fileName));

                                                await bot.MakeRequestAsync(req2);

                                                whatDo = 0;

                                                streem.Close();

                                                System.IO.File.Delete(fileName);
                                            }
                                        }
                                        else
                                        {
                                            var req4 = new SendMessage(item.Message.Chat.Id, "حجم فایل بیشتر از 200 مگابایت می باشد") { ReplyMarkup = Tools };

                                            await bot.MakeRequestAsync(req4);

                                            whatDo = 0;

                                            break;
                                        }
                                    }
                                    var req3 = new SendMessage(item.Message.Chat.Id, "\U00002B06 بفرمایید \U00002B06") { ReplyMarkup = Tools };

                                    await bot.MakeRequestAsync(req3);

                                    whatDo = 0;
                                }
                                else if (whatDo == 27)
                                {
                                    var youTube = YouTube.Default;
                                    var video = youTube.GetVideo(userText);
                                    System.IO.File.WriteAllBytes(video.FullName, video.GetBytes());

                                    using (var streem = System.IO.File.Open(video.FullName, FileMode.Open))
                                    {
                                        var req2 = new SendDocument(item.Message.Chat.Id, new FileToSend(streem, video.FullName));

                                        await bot.MakeRequestAsync(req2);

                                        whatDo = 0;

                                        streem.Close();

                                        System.IO.File.Delete(video.FullName);
                                    }

                                    var req3 = new SendMessage(item.Message.Chat.Id, "\U00002B06 بفرمایید \U00002B06") { ReplyMarkup = Tools };

                                    await bot.MakeRequestAsync(req3);

                                    whatDo = 0;
                                }
                                else if (whatDo == 28)
                                {
                                    //Console.OutputEncoding = System.Text.Encoding.Unicode;
                                    //TranslationClient client = TranslationClient.Create();
                                    //var response = client.TranslateText(userText, "fa");

                                    //var service = new TranslateService(new BaseClientService.Initializer { ApiKey = apiKeyTranslate });
                                    //var client = new TranslationClientImpl(service, TranslationModel.ServiceDefault);
                                    //var result = client.TranslateText("Hello World", "fa");

                                    //var req3 = new SendMessage(item.Message.Chat.Id, result.TranslatedText) { ReplyMarkup = Tools };

                                    //await bot.MakeRequestAsync(req3);

                                    whatDo = 0;
                                }
                                else if (whatDo == 29)
                                {
                                    if (item.Message.Photo != null)
                                    {
                                        var fileId = item.Message.Photo.LastOrDefault().FileId;
                                        var file = await bot.MakeRequestAsync(new GetFile(fileId));

                                        SaveFile(file.FileDownloadUrl, "imageWatermark.jpg");

                                        var reqImage = new SendMessage(item.Message.Chat.Id, " الان رو عکس راست کلیک کن و Reply رو بزن");

                                        await bot.MakeRequestAsync(reqImage);

                                        continue;
                                    }

                                    if (item.Message.ReplyToMessage?.Photo != null)
                                    {


                                        var reqShow = new SendMessage(item.Message.Chat.Id, "\U0001F447 بفرما \U0001F447") { ReplyMarkup = Tools };
                                        //var reqPhoto = new SendPhoto(item.Message.Chat.Id, new FileToSend(fileId)) { Caption = userText };

                                        await bot.MakeRequestAsync(reqShow);
                                        //await bot.MakeRequestAsync(reqPhoto);
                                    }

                                    whatDo = 0;
                                }
                                else if (whatDo == 30)
                                {
                                    var fileId = item.Message.Document.FileId;
                                    var file = await bot.MakeRequestAsync(new GetFile(fileId));
                                    SaveFile(file.FileDownloadUrl, "txtToPdf.txt");

                                    var p = new SautinSoft.PdfMetamorphosis();

                                    p.PageSettings.Size.A4();
                                    p.PageSettings.Orientation = SautinSoft.PdfMetamorphosis.PageSetting.Orientations.Landscape;
                                    p.PageSettings.MarginRight.Inch(1.5f);
                                    p.PageSettings.MarginTop.Inch(1.0f);
                                    p.PageSettings.MarginBottom.Inch(1.0f);
                                    p.WaterMarks.Add("CopyRight.png");
                                    p.PageSettings.Numbering.StartingNumber = 1;

                                    const string textPath = "txtToPdf.txt";

                                    const string pdfPath = "pdfFile.pdf";

                                    p.TextToPdfConvertFile(textPath, pdfPath);

                                    using (var streem = System.IO.File.Open("pdfFile.pdf", FileMode.Open))
                                    {
                                        var req2 = new SendDocument(item.Message.Chat.Id, new FileToSend(streem, "pdfFile.pdf"));

                                        await bot.MakeRequestAsync(req2);

                                        whatDo = 0;

                                        streem.Close();

                                        System.IO.File.Delete("pdfFile.pdf");
                                    }

                                    var req3 = new SendMessage(item.Message.Chat.Id, "\U0001F446 بفرمایید \U0001F446") { ReplyMarkup = Tools };

                                    await bot.MakeRequestAsync(req3);

                                    whatDo = 0;
                                }
                                else if (whatDo == 31)
                                {
                                    var fileId = item.Message.Document.FileId;
                                    var file = await bot.MakeRequestAsync(new GetFile(fileId));
                                    SaveFile(file.FileDownloadUrl, "txtToPdf.docx");

                                    var p = new SautinSoft.PdfMetamorphosis();

                                    p.PageSettings.Size.A4();
                                    p.PageSettings.Orientation = SautinSoft.PdfMetamorphosis.PageSetting.Orientations.Landscape;
                                    p.PageSettings.MarginRight.Inch(1.5f);
                                    p.PageSettings.MarginTop.Inch(1.0f);
                                    p.PageSettings.MarginBottom.Inch(1.0f);
                                    p.WaterMarks.Add("CopyRight.png");
                                    p.PageSettings.Numbering.StartingNumber = 1;

                                    const string textPath = "txtToPdf.docx";

                                    const string pdfPath = "pdfFile.pdf";

                                    p.DocxToPdfConvertFile(textPath, pdfPath);

                                    using (var streem = System.IO.File.Open("pdfFile.pdf", FileMode.Open))
                                    {
                                        var req2 = new SendDocument(item.Message.Chat.Id, new FileToSend(streem, "pdfFile.pdf"));

                                        await bot.MakeRequestAsync(req2);

                                        whatDo = 0;

                                        streem.Close();

                                        System.IO.File.Delete("pdfFile.pdf");
                                    }

                                    var req3 = new SendMessage(item.Message.Chat.Id, "\U0001F446 بفرمایید \U0001F446") { ReplyMarkup = Tools };

                                    await bot.MakeRequestAsync(req3);

                                    whatDo = 0;
                                }
                                else if (whatDo == 32)
                                {
                                    var passForUser = new PasswordGenerator(int.Parse(userText)).IncludeLowercase().IncludeUppercase().IncludeSpecial();
                                    var passwordUser = passForUser.Next();

                                    var req3 = new SendMessage(item.Message.Chat.Id, passwordUser) { ReplyMarkup = Tools };

                                    await bot.MakeRequestAsync(req3);

                                    whatDo = 0;
                                }

                                else
                                {
                                    var req3 = new SendMessage(item.Message.Chat.Id, "لطفا ابزار مورد نظر خود را انتخاب کنید") { ReplyMarkup = Tools };

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
            // ReSharper disable once FunctionNeverReturns
        }       
    }
}