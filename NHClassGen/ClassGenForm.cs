///////////////////////////////////////////////////////////////////////////////////
///    NHibernate Class Generator                                               ///
///    Copyright(C) 2016 Lucas Teske                                            ///
///                                                                             ///
///    This program is free software: you can redistribute it and/or modify     ///
///    it under the terms of the GNU General Public License as published by     ///
///    the Free Software Foundation, either version 3 of the License, or        ///
///    any later version.                                                       ///
///                                                                             ///
///    This program is distributed in the hope that it will be useful,          ///
///    but WITHOUT ANY WARRANTY; without even the implied warranty of           ///
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the              ///
///    GNU General Public License for more details.                             ///
///                                                                             ///
///    You should have received a copy of the GNU General Public license        ///
///    along with this program.If not, see<http://www.gnu.org/licenses/>.       ///
///////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace NHClassGen {
  public partial class ClassGenForm : Form {
    List<string> tableList;
    public ClassGenForm() {
      InitializeComponent();
      tableList = new List<string>();
      tableListView.Scrollable = true;
      tableListView.View = View.Details;
      ColumnHeader header = new ColumnHeader();
      header.Text = "Table Name";
      header.Name = "Table Name";
      header.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      header.Width = 100;
      tableListView.Columns.Add(header);
    }

    private void fetchTables() {
      using (SqlConnection connection = new SqlConnection(connectionStringBox.Text)) {
        connection.Open();
        DataTable dt = connection.GetSchema("Tables");
        tableList.Clear();
        foreach (DataRow row in dt.Rows) {
          string tablename = (string)row[2];
          tableList.Add(tablename);
        }
        connection.Close();
      }
    }

    private void generateClasses(object sender, EventArgs e) {
      List<string> listacolumnas = new List<string>();

      using (SqlConnection connection = new SqlConnection(connectionStringBox.Text)) {
        textBox1.AppendText("Opening connection.\r\n");
        connection.Open();
        foreach (int p in tableListView.SelectedIndices) {
          string tableName = tableListView.Items[p].Text;
          using (SqlCommand command = connection.CreateCommand()) {
            textBox1.AppendText("Fetching table members for " + tableName + "\r\n");
            command.CommandText = string.Format("select c.name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name = '{0}' and t.type = 'U'", tableName);
            string[] restrictions = new string[4] { null, null, tableName, null };
            var columnList = connection.GetSchema("Columns", restrictions)
              .AsEnumerable()
              .Select(t => new Column(t))
              .OrderBy(t => t.Index)
              .ToList();
            textBox1.AppendText("Generating Class for " + tableName + "\r\n");
            string classS = GenTools.GenerateClass(tableName, columnList, true, nameSpaceBox.Text);
            textBox1.AppendText("Saving " + tableName + ".cs file\r\n");
            System.IO.StreamWriter file = new System.IO.StreamWriter(string.Format("{0}.cs", tableName));
            file.WriteLine(classS);
            file.Close();
          }
        }
        textBox1.AppendText("Done!\r\n");
        connection.Close();
      }
    }

    private void fetchTableList(object sender, EventArgs e) {
      fetchTables();
      tableListView.Items.Clear();
      foreach (string s in tableList) {
        tableListView.Items.Add(s);
      }
      tableListView.Invalidate();
    }
  }
}
