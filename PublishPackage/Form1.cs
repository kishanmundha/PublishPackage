using PublishPackage.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PublishPackage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ISqlReader reader = new SqlReaderByDatabase();
            var db1 = reader.Get("data source=.;initial catalog=Quotation;integrated security=False;User Id=sa;Password=12345");
            var db2 = reader.Get("data source=.;initial catalog=QuotationTemp;integrated security=False;User Id=sa;Password=12345");

            //var str = database.GetCreateScript();

            //var str = db1.GetJsonString();

            var dbCompare = SqlDatabaseCompareResult.Compare(db1, db2);

            var str = dbCompare.GetScript();

            var frm = new frmContentView("DB Script", str);
            frm.ShowDialog();
        }
    }
}
