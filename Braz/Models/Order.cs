using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Braz.Models
{
    public class Order
    {
        //FIELDS
        private List<OrderedItem> cart;
        public int Id { get; }
        public bool InProgress { get; set; }
        public List<OrderedItem> Cart { get { return cart; } }
        public User User { get; set; }
        public DateTime Date { get; set; }
        //METHODS
        public Order(User user)
        {
            cart = new List<OrderedItem>();
            string query = "INSERT INTO orders(User,InProgress) VALUES("+user.Id+",TRUE)";
            using (DbConnect db = new DbConnect())
            {
                Id = db.Insert(query);
            }
        }
        public Order(int Id)
        {
            this.Id = Id;
            this.cart = new List<OrderedItem>();
        }
        public void AddToCart(OrderedItem item)
        {
            int NewId = 0;
            //Add to local cart
            cart.Add(item);
            //Add to bd new item
            string query = "INSERT INTO ordereditems(`ItemId`,`AlloyMark`, `ThermalTreatment`, `Length`, `Deflection`, `Cover`, `Width`, `Color`, `ContactSpot`, `PaintType`, `Shine`, `AdditionalProcessing`, `Comment`) VALUES(" + item.Item.Id+", '"+item.AlloyMark+"', '"+item.ThermalTreatment+ "', '" + item.Length+ "', " + item.Deflection.ToString().ToUpper() + ", '" + item.Cover+ "', '" + item.Width+ "', '" + item.Color+ "', " + item.ContactSpot.ToString().ToUpper() + ", '" + item.PaintType+ "', '" + item.Shine+"', "+item.AdditionalProcessing.ToString().ToUpper() + ", '" + item.Comment+"')";
            using (DbConnect db = new DbConnect())
            {
                NewId=db.Insert(query);
            }
            //Add to order new item id
            query= "UPDATE orders set ordereditems=CONCAT(ordereditems,(CASE WHEN ordereditems='' THEN '"+NewId.ToString()+"' ELSE '|"+NewId.ToString()+"' END)) WHERE Id="+Id.ToString();
            using (DbConnect db = new DbConnect())
            {
                db.Update(query);
            }
        }
        public void SetCart(List<OrderedItem> list)
        {
            cart = list;
        }
        static public List<Order> GetUserOrders(int id)
        {
            string query = "SELECT * FROM orders WHERE User=" + id.ToString()+" AND NOT InProgress";
            using (DbConnect db = new DbConnect())
            {
                return db.GetUserOrders(query);
            }
        }
        static public void FinishOrder(int id)
        {
            string query = "UPDATE orders SET Date='"+DateTime.Now.ToString("yyyy-MM-dd") +"', InProgress=0 WHERE Id="+id.ToString();
            using (DbConnect db = new DbConnect())
            {
                db.Update(query);
            }
        }
        
    }
    //DATA CARRIER
    public class OrderedItem
    {
        public int Id { get; set; }
        public Item Item { get; set; }
        public string AlloyMark { get; set; }
        public string ThermalTreatment { get; set; }
        public string Length { get; set; }
        public bool Deflection { get; set; }
        public string Cover { get; set; }
        public string Width { get; set; }
        public string Color { get; set; }
        public bool ContactSpot { get; set; }
        public string PaintType { get; set; }
        public string Shine { get; set; }
        public bool AdditionalProcessing { get; set; }
        public string Comment { get; set; }
    }
}