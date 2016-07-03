using System.Linq;

namespace Braz.Models
{
    public class User
    {
        public int Id { get; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Order CurrOrder{ get; }
        public User(int Id)
        {
            this.Id = Id;
            string query = "SELECT * FROM orders WHERE User = "+Id.ToString()+" AND InProgress";
            using (DbConnect db = new DbConnect())
            {
                CurrOrder = db.GetCurrentOrder(query) ?? new Order(this);
            }
            CurrOrder.User = this;
        }
        public User(bool Admin)
        {
            this.Email = "Braz@admin";
            this.Password = "pass";
        }
        public static User Insert(string Name,string Password,string Phone,string Email)
        {
            string query = "INSERT INTO usertable(Name, Password, Email,Phone) VALUES ('" + Name + "','" + GetMd5Hash(System.Security.Cryptography.MD5.Create(), Password) + "','" + Email + "','" + Phone + "')";
            using (DbConnect db = new DbConnect())
            {
                return new User(db.Insert(query)) { Email = Email, Name = Name, Phone = Phone };
            }
        }

        public static User GetUser(string login,string password)
        {
            string MD5Pass = GetMd5Hash(System.Security.Cryptography.MD5.Create(),password);
            string query = "SELECT * FROM usertable WHERE Email = '" + login + "' AND Password = '" + MD5Pass+"'";
            using (DbConnect db = new DbConnect())
            {
                return db.GetUser(query);
            }
        }
        public static User GetUser(int id)
        {
            string query = "SELECT Id,Name,Email,Phone FROM usertable WHERE Id=" + id.ToString();
            using (DbConnect db = new DbConnect())
            {
                return db.GetUser(query);
            }
        }

        public void Update(string Name,string Email,string Phone,string Password)
        {
            string query = "UPDATE usertable SET ";
            if (Name != this.Name && Name != "")
            {
                query += "Name='" + Name + "', ";
                this.Name = Name;
            }
            if (this.Email != Email && Email != "")
            {
                query += "Email='" + Email + "', ";
                this.Email = Email;
            }
            if (this.Phone != Phone && Phone != "")
            {
                query += "Phone='" + Phone + "', ";
                this.Phone = Phone;
            }
            if (Password!="")
                query += "Password='" + GetMd5Hash(System.Security.Cryptography.MD5.Create(), Password) + "', ";
            query = query.Remove(query.Length - 2);
            query += " WHERE Id=" + Id.ToString();
            using (DbConnect db = new DbConnect())
            {
                db.Update(query);
            }
        }

        public bool SubmitOrder(string comment)
        {
            string message = "Новый заказ от пользователя №" + CurrOrder.Id + " с именем " + Name + " и почтой " + Email + " с телефоном " + Phone + System.Environment.NewLine;
            foreach(OrderedItem item in CurrOrder.Cart)
            {
                message += "Товар - " + item.Item.Category.Name + ", Параметры - ";
                foreach(DataSet set in item.Item.Data)
                {
                    message += set.Type.Value + " - " + set.Quantity + ", ";
                }
                message = message.Remove(message.Length - 2);
                message += System.Environment.NewLine;
                message += "Параметры заказа: \nМарка сплава - " + item.AlloyMark + ", ";
                if(item.AlloyMark != "АД0 Без ТО")
                {
                    message += "Термообработка - " + item.ThermalTreatment + ", ";
                }
                message += "Длина - " + item.Length + ", Допускается термообработка - " + (item.Deflection ? "да" : "нет") + System.Environment.NewLine;
                message += "Декоративно-защитное покрытие - " + item.Cover + ", ";
                if (item.Cover == "Анодирование")
                {
                    message += "Толщина - " + item.Width + ", цвет - " + item.Color + ", Допускаются контактные пятна  - " + (item.ContactSpot ? "да" : "нет") + System.Environment.NewLine;
                }
                else if (item.Cover == "Покраска")
                {
                    message += "Цвет по каталогу RAL - " + item.Color + ", Тип краски - " + item.PaintType + ", Степень блеска - " + item.Shine + System.Environment.NewLine;
                }
                else
                    message += System.Environment.NewLine;
                message += "Дополнительная механическая обработка - " + (item.AdditionalProcessing ? "да" : "нет") + System.Environment.NewLine;
                message += "Коментарий к товару - " + item.Comment + System.Environment.NewLine;
            }
            message += System.Environment.NewLine + "Коментарий к заказу - " + comment + System.Environment.NewLine;

            //Delivery
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("mail.ukraine.com.ua", 25);
            string reciever = ((System.Collections.Generic.List<GeneralSetting>)System.Web.HttpContext.Current.Application["GeneralSettings"]).Find(x => x.Identifier == "OrderReciever").Value;
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage("site@braz.com.ua", reciever);
            msg.Subject = "Новый заказ";
            msg.Body = message;
            client.Port = 2525;
            client.EnableSsl = false;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("site@braz.com.ua", "3xxu9LY7S4Um");
            try
            {
                client.Send(msg);
                Order.FinishOrder(CurrOrder.Id);
                this.CurrOrder.SetCart(new System.Collections.Generic.List<OrderedItem>());
                return true;
            }
            catch(System.Exception e)
            {
                string path = (string)System.Web.HttpContext.Current.Application["LogFile"];
                System.IO.StreamWriter writer = System.IO.File.AppendText(path);
                System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("ru-RU");
                writer.WriteLine("[ERROR] " + e.Message);
                writer.Close();
                return false;
            }
        }


        //PASSWORD CRYPTOGRAPHY
        private static string GetMd5Hash(System.Security.Cryptography.MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));
            return sBuilder.ToString();
        }
    }
}