using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace Braz.Models
{
    class DbConnect : IDisposable
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private string charset;
        private bool isalive;
        public bool IsAlive { get { return isalive; } }
        public DbConnect()
        {
            isalive=Initialize();
        }
        public void Dispose()
        {
            connection.Close();
        }
        private bool Initialize()
        {
            server = "MYSQL5015.Smarterasp.net";
            database = "db_9f9f17_braz";
            uid = "9f9f17_braz";
            password = "S0m3Pass";
            charset = "utf8";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                                database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";charset="+charset+";";
            connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                return true;
            }
            catch (Exception) { return false; }
        }
        public int Insert(string input)
        {
            MySqlCommand command = new MySqlCommand(input, connection);
            command.ExecuteNonQuery();
            return (int)command.LastInsertedId;
        }
        public void Update(string input)
        {
            new MySqlCommand(input, connection).ExecuteNonQuery();
        }
        public void Delete(string input)
        {
            new MySqlCommand(input, connection).ExecuteNonQuery();
        }





        
        //main get queries processing
        public List<SubCategory> GetCatalog(string query)
        {
            List<SubCategory> result = new List<SubCategory>();

            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                SubCategory temp = new SubCategory((int)reader["Id"]);
                temp.Name = (string)reader["Name"];
                temp.Parent = new Category((int)reader["catid"]);
                //temp.Url = (string)reader["Url"];
                result.Add(temp);
            }
            reader.Close();

            return result;
        }

        public SubCategory GetSubCategory(string query)
        {
            SubCategory result = null;
            string TParams = "";

            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                result = new SubCategory((int)reader["Id"]);
                result.Name = (string)reader["Name"];
                result.Parent = new Category((int)reader["catid"]);
                //result.Url = (string)reader["Url"];
                TParams = (string)reader["ParamSet"];
            }
            reader.Close();
            if(result!=null)
                result.Params = ParseParams(TParams);
            return result;
        }

        public Item GetItem(string query)
        {
            Item result = null;
            ItemDataSet TSet = new ItemDataSet();
            //Reading table
            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                result = new Item((int)reader["ItemID"]);
                result.Category = new SubCategory((int)reader["CategoryID"]) { Name = (string)reader["Name"], Parent = new Category((int)reader["catid"]) };
                TSet = new ItemDataSet { Id = result.Id, ParamSet = (string)reader["ParamSet"], DataSet = (string)reader["DataSet"] };
            }
            reader.Close();
            if (result == null)
                return result;
            //DataSet processing
            result.Data = ParseDataSet(new List<ItemDataSet> { TSet }).Values.First();

            return result;
        }

        public List<Item> GetItemList(string query)
        { 
            Dictionary<Item, ItemDataSet> result = new Dictionary<Item, ItemDataSet>();
            //Reading Table


            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while (reader.Read())
            {
                Item temp = new Item((int)reader["ItemID"]);
                temp.Category = new SubCategory((int)reader["CategoryID"]) { Name = (string)reader["Name"], Parent = new Category((int)reader["catid"]) };
                result.Add(temp, new ItemDataSet() { Id=temp.Id, ParamSet = (string)reader["ParamSet"], DataSet = (string)reader["DataSet"]});
            }
            reader.Close();
            if (result.Count == 0)
                return new List<Item>();
            //DataSet Processing
            Dictionary<int,List<DataSet>> datasets = ParseDataSet(result.Values.ToList());

            foreach(Item item in result.Keys)
            {
                item.Data = datasets[item.Id];
            }

            return result.Select(X => X.Key).ToList();
        }

        public List<Filter> GetFilterList(string query)
        {
            List<Filter> result = new List<Filter>();
            string filterset = "";
            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                filterset = (string)reader["FilterSet"];
            }
            reader.Close();

            return ParseFilters(filterset);
        }
        
        //Filter processing
        public List<Filter> ParseFilters(string list)
        {
            List<Filter> result = new List<Filter>();

            if (list.Length == 0)
                return result;

            string query = "SELECT filters.Id, filters.Max,filters.Step,filters.Description,parameter_list.Id AS Parameter, parameter_list.Name, parameter_list.Measure, parameter_list.Description FROM filters INNER JOIN parameter_list ON filters.Parameter=parameter_list.Id WHERE filters.Id=";
            foreach(string item in list.Split('|'))
            {
                query += item + " OR filters.Id=";
            }
            query = query.Remove(query.Length - " OR filters.Id=".Length);

            query += " GROUP BY FIELD(filters.Id,";
            foreach (string item in list.Split('|'))
            {
                query += item + ", ";
            }
            query = query.Remove(query.Length - 2);
            query += ")";

            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                Filter temp = new Filter() { Id = (int)reader["Id"], Max = (double)reader["Max"], Step = (double)reader["Step"] };
                temp.Parameter = new Parameter() { Id = (int)reader["Parameter"], Key = (string)reader["Name"], Value = (string)reader["Measure"], Description = (string)reader["Description"] };
                result.Add(temp);
            }

            return result;
        }

        //Parameter processing
        public List<Parameter> ParseParams(string input)
        {
            List<Parameter> result = new List<Parameter>();
            //empty check
            if (input.Length == 0)
                return result;
            //Provide query
            string query = "SELECT * FROM parameter_list WHERE Id=";
            foreach (string item in input.Split('|'))
            {
                query += item + " OR Id=";
            }
            query = query.Remove(query.Length - 7);

            query += " GROUP BY FIELD(Id,";
            foreach(string item in input.Split('|'))
            {
                query += item + ", ";
            }
            query = query.Remove(query.Length - 2);
            query += ")";
            //Read table
            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while (reader.Read())
            {
                Parameter temp = new Parameter() { Id = (int)reader["Id"], Key = (string)reader["Name"], Value = (string)reader["Measure"], Description = (string)reader["Description"] };
                result.Add(temp);
            }

            return result;
        }

        public Dictionary<int,List<DataSet>> ParseDataSet(List<ItemDataSet> list)
        {
            Dictionary<int, List<DataSet>> result = new Dictionary<int, List<DataSet>>();
            List<Parameter> AvailableParams = new List<Parameter>();
            Dictionary<int,List<string>> AllParameters = new Dictionary<int, List<string>>();
            List<List<string>> AllQuantities = new List<List<string>>();

            if (list.Count == 0)
                return result;

            //get all available parameters
            string query = "SELECT * FROM parameter_list";
            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                AvailableParams.Add(new Parameter() { Id = (int)reader["Id"], Key = (string)reader["Name"], Value = (string)reader["Measure"], Description = (string)reader["Description"] });
            }
            reader.Close();

            //init data arrays
            foreach(ItemDataSet set in list)
            {
                AllParameters.Add(set.Id, set.ParamSet.Split('|').ToList());
            }
            AllQuantities = list.Select(x => x.DataSet.Split('|').ToList()).ToList();


            if (AllParameters.Count != AllQuantities.Count)
                return result;

            int i = 0;
            foreach (KeyValuePair<int, List<string>> TPair in AllParameters)
            {
                List<DataSet> TList = new List<DataSet>();
                for (int j = 0; j < TPair.Value.Count; j++)
                {
                    DataSet temp = new DataSet();
                    temp.Type = AvailableParams.Where(x => x.Id.ToString() == TPair.Value[j]).First();
                    temp.Quantity = AllQuantities[i][j];
                    TList.Add(temp);
                }
                result.Add(TPair.Key, TList);
                i++;
            }

            return result;
        }

        public List<SubCategory> GetSubCategories(string query)
        {
            List<SubCategory> result = new List<SubCategory>();
            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while (reader.Read())
            {
                SubCategory temp = new SubCategory((int)reader["Id"]) { Name = (string)reader["Name"] };
                result.Add(temp);
            }
            return result;
        }


        public List<Post> GetNews()
        {
            List<Post> result = new List<Post>();

            string query = "SELECT * FROM news";
            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                Post temp = new Post((int)reader["Id"]) { Header = ((string)reader["Header"]).Replace("[0]","'"), Text = ((string)reader["Text"]).Replace("[0]","'"), Date = (DateTime)reader["Date"] };
                result.Add(temp);
            }
            reader.Close();
            return result;
        }
        public Post GetPost(string id)
        {
            Post result = null;
            string query = "SELECT * FROM news WHERE Id=" + id;
            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while (reader.Read())
            {
                result = new Post((int)reader["Id"]) { Header = ((string)reader["Header"]).Replace("[0]", "'"), Text = ((string)reader["Text"]).Replace("[0]", "'"), Date = (DateTime)reader["Date"] };
                break;
            }
            reader.Close();
            query = "SELECT MIN(Id) FROM news WHERE Id > " + id;
            reader = new MySqlCommand(query, connection).ExecuteReader();
            while (reader.Read())
            {
                try {
                    result.Next = (int)reader["MIN(Id)"];
                }
                catch (InvalidCastException) { }
                break;
            }
            reader.Close();
            query = "SELECT MAX(Id) FROM news WHERE Id < " + id;
            reader = new MySqlCommand(query, connection).ExecuteReader();
            while (reader.Read())
            {
                try {
                    result.Previous = (int)reader["MAX(Id)"];
                }
                catch (InvalidCastException) { }
                break;
            }
            reader.Close();
            return result;
        }

        //RETURN USER IF USER EXISTS
        public User GetUser(string query)
        {
            int TId = -1;
            string[] ResString = new string[3];
            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                TId = (int)reader["Id"];
                ResString[0] = (string)reader["Email"];
                ResString[1] = (string)reader["Phone"];
                ResString[2] = (string)reader["Name"];
            }
            reader.Close();
            if (TId != -1)
                return new User(TId) { Email = ResString[0], Name = ResString[2], Phone = ResString[1] };
            return null;
        }
        
        public Order GetCurrentOrder(string query)
        {
            Order result = null;
            string OrderedItems = "";

            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                result = new Order((int)reader["Id"]);
                result.InProgress = (bool)reader["InProgress"];
                OrderedItems = (string)reader["OrderedItems"];
            }
            reader.Close();
            if(result!= null)
                result.SetCart(ParseCart(OrderedItems));
            return result;
        }

        public List<Order> GetUserOrders(string query)
        {
            List<Order> result = new List<Order>();
            Dictionary<int, string> OrderedItems = new Dictionary<int, string>();

            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                Order TempOrder = new Order((int)reader["Id"]);
                TempOrder.Date = (DateTime)reader["Date"];
                TempOrder.User = new User((int)reader["User"]);
                TempOrder.InProgress = (bool)reader["InProgress"];
                OrderedItems.Add(TempOrder.Id, (string)reader["OrderedItems"]);
                result.Add(TempOrder);
            }
            reader.Close();
            
            foreach(int id in OrderedItems.Select(x=>x.Key))
            {
                result.Where(x => x.Id == id).First().SetCart(ParseCart(OrderedItems[id]));
            }


            return result;
        }

        public List<OrderedItem> ParseCart(string input)
        {
            List<string> items = input.Split('|').ToList();
            if (items[0]=="")
                return new List<OrderedItem>();
            string query = "SELECT * FROM ordereditems WHERE Id=";
            foreach(string item in items)
            {
                query += item + " OR Id=";
            }
            query = query.Remove(query.Length - 7);

            Dictionary<OrderedItem, int> TItems = new Dictionary<OrderedItem, int>();

            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                OrderedItem TempItem = new OrderedItem();
                TempItem.Id = (int)reader["Id"];
                TempItem.AdditionalProcessing = (bool)reader["AdditionalProcessing"];
                TempItem.AlloyMark = (string)reader["AlloyMark"];
                TempItem.Color = (string)reader["Color"];
                TempItem.Comment = (string)reader["Comment"];
                TempItem.Cover = (string)reader["Cover"];
                TempItem.Length = (string)reader["Length"];
                TempItem.PaintType = (string)reader["PaintType"];
                TempItem.Shine = (string)reader["Shine"];
                TempItem.ThermalTreatment = (string)reader["ThermalTreatment"];
                TempItem.Width = (string)reader["Width"];
                TempItem.Deflection = (bool)reader["Deflection"];
                TempItem.ContactSpot = (bool)reader["ContactSpot"];
                TItems.Add(TempItem, (int)reader["ItemId"]);
            }
            reader.Close();

            foreach(OrderedItem id in TItems.Select(x=>x.Key))
            {
                TItems.Where(x => x.Key == id).First().Key.Item = Item.GetItem(TItems[id]);
            }

            return TItems.Select(x => x.Key).ToList();
        }

        public Vacancy GetVacancy(string query)
        {
            Vacancy result = null;
            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                result = new Vacancy((int)reader["Id"]);
                result.Description = (string)reader["Description"];
                result.Header = (string)reader["Header"];
                result.Salary = (string)reader["Salary"];
                result.Url = (string)reader["Url"];
                result.Type = (int)reader["Type"];
                result.Requirements = ReadVacancy((string)reader["Requirements"]);
                result.Duties = ReadVacancy((string)reader["Duties"]);
            }
            reader.Close();
            return result;

        }

        public List<Vacancy> GetVacancies(string query)
        {
            List<Vacancy> result = new List<Vacancy>();

            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                Vacancy temp = new Vacancy((int)reader["Id"]);
                temp.Description = (string)reader["Description"];
                temp.Header = (string)reader["Header"];
                temp.Salary = (string)reader["Salary"];
                temp.Url = (string)reader["Url"];
                temp.Type = (int)reader["Type"];
                temp.Requirements = ReadVacancy((string)reader["Requirements"]);
                temp.Duties = ReadVacancy((string)reader["Duties"]);
                result.Add(temp);
            }
            reader.Close();
            return result;
        }



        private List<string> ReadVacancy(string input)
        {
            List<string> result = new List<string>();
            foreach(string data in input.Split('|'))
            {
                result.Add(data);
            }
            return result;
        }

        public List<GeneralSetting> ReadSettings(string query)
        {
            List<GeneralSetting> result = new List<GeneralSetting>();
            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                result.Add(new GeneralSetting() { Identifier=(string)reader["Identifier"], Value=(string)reader["Value"], Display=(string)reader["Display"] });
            }
            reader.Close();
            return result;
        }

        public Dictionary<string,Dictionary<int,Dictionary<string,string>>> ReadLocalization(string query)
        {
            Dictionary<string, Dictionary<int, Dictionary<string, string>>> result = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
            foreach (string lang in (List<string>)System.Web.HttpContext.Current.Application["Languages"])
                result.Add(lang, new Dictionary<int, Dictionary<string, string>>());
            MySqlDataReader reader = new MySqlCommand(query, connection).ExecuteReader();
            while(reader.Read())
            {
                int page = (int)reader["PageID"];
                foreach (string lang in (List<string>)System.Web.HttpContext.Current.Application["Languages"])
                {
                    if (!result[lang].ContainsKey(page))
                        result[lang].Add(page, new Dictionary<string, string>());
                    result[lang][page].Add((string)reader["Anchor"], (string)reader[lang]);
                }
            }
            reader.Close();
            return result;
        }
    }


    internal struct ItemDataSet
    {
        public int Id;
        public string ParamSet;
        public string DataSet;
    }
    internal struct FilterValue
    {
        public Filter Filter;
        public string Max;
        public string Min;
    }
}