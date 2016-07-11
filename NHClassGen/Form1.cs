using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NHClassGen {
  public partial class Form1 : Form {
    public Form1() {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e) {
      List<string> listacolumnas = new List<string>();

      using (SqlConnection connection = new SqlConnection("Data Source=NBLUCAS\\SQLEXPRESS;Initial Catalog=PMEC2;Integrated Security=SSPI;")) {
        using (SqlCommand command = connection.CreateCommand()) {
          command.CommandText = "select c.name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name = 'Agenda' and t.type = 'U'";
          connection.Open();
          string[] restrictions = new string[4] { null, null, "Agenda", null };
          var columnList = connection.GetSchema("Columns", restrictions)
            .AsEnumerable()
            .Select(t => new Column(t))
            .OrderBy(t => t.Index)
            .ToList();
          string classS = GenTools.GenerateClass("Agenda", columnList, true);
          textBox1.Text = classS;
        }
      }
    }

    private void button2_Click(object sender, EventArgs e) {
      Clipboard.SetText(textBox1.Text);
    }
  }
}
