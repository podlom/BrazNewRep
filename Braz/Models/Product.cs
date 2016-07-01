/*using System;

namespace Braz.Models
{
    public class Product
    {
        public int Id;
        public int CatId;
        public string Article;
        public Decimal P;
        public Decimal R;
        public Decimal Q;
        public Decimal S;
        public Decimal D;
        public Decimal A;
        public Decimal B;
        public string Amount;
        public virtual void AddToDB()
        {
            using(DbConnect db = new DbConnect())
            {
                string insert = "INSERT INTO products(catId,Article,S,Q,D,P,A,B,R) VALUES(" + CatId.ToString() + ",'" + Article + "'," + S.ToString().Replace(',', '.') + "," + Q.ToString().Replace(',', '.') + "," + D.ToString().Replace(',', '.') + "," + P.ToString().Replace(',', '.') + "," + A.ToString().Replace(',', '.') + "," + B.ToString().Replace(',', '.') + "," + R.ToString().Replace(',', '.') + ")";
                Id=db.Insert(insert);
            }
        }
        public virtual void SubmitChanges(string OldArticle)
        {
            using (DbConnect db = new DbConnect())
            {
                string update = "UPDATE products SET Article=" + Article + ", S=" + S.ToString() + ", Q=" + Q.ToString().Replace(',', '.') + ", D=" + D.ToString().Replace(',', '.') + ", P=" + P.ToString().Replace(',', '.') + ", A=" + A.ToString().Replace(',', '.') + ", B=" + B.ToString().Replace(',', '.') + ",R=" + R.ToString().Replace(',', '.') + " WHERE Article=" + OldArticle;
                db.Update(update);
            }
        }
    }

    public class NSTProduct:Product
    {
        public Decimal S1;
        public Decimal S2;
        public Decimal S3;
        public Decimal S4;
        public Decimal D1;
        public Decimal D2;
        public Decimal C;
        public Decimal T;
        public Decimal R1;
        public Decimal R2;
        public override void AddToDB()
        {
            using (DbConnect db = new DbConnect())
            {
                string insert = "INSERT INTO nst_products(catId,Article,S1,S2,S3,S4,Q,D1,D2,P,A,B,C,T,R1,R2) VALUES(" + CatId.ToString() + ",'" + Article + "'," + S1.ToString().Replace(',', '.') + ","+S2.ToString().Replace(',', '.') + "," + S3.ToString().Replace(',', '.')+","+S4.ToString().Replace(',','.') + "," + Q.ToString().Replace(',', '.') + "," + D1.ToString().Replace(',', '.') + "," + D2.ToString().Replace(',', '.') + "," + P.ToString().Replace(',', '.') + "," + A.ToString().Replace(',', '.') + "," + B.ToString().Replace(',', '.') + "," + C.ToString().Replace(',', '.') + "," + T.ToString().Replace(',', '.') + "," + R1.ToString().Replace(',', '.')+','+R2.ToString().Replace(',', '.') +  ")";
                Id = db.Insert(insert);
            }
        }
        public override void SubmitChanges(string OldArticle)
        {
            using (DbConnect db = new DbConnect())
            {
                string update = "UPDATE products SET Article=" + Article + ", S1=" + S1.ToString().Replace(',', '.') + ", S2=" + S2.ToString().Replace(',', '.') + ", S3=" + S3.ToString().Replace(',', '.') + ", S4=" + S4.ToString().Replace(',', '.') + ", Q=" + Q.ToString().Replace(',', '.') + ", D1=" + D1.ToString().Replace(',', '.') + ", D2=" + D2.ToString().Replace(',', '.') + ", P=" + P.ToString().Replace(',', '.') + ", A=" + A.ToString().Replace(',', '.') + ", B=" + B.ToString().Replace(',', '.') + ", C=" + C.ToString().Replace(',', '.') + ", T=" + T.ToString().Replace(',', '.') + ",R1=" + R1.ToString().Replace(',', '.') + ", R2=" + R2.ToString().Replace(',', '.') + " WHERE Article=" + OldArticle;
                db.Update(update);
            }
        }
    }
}*/