using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Sedati
{
    public partial class frmSedati : Form
    {
        DataTable mDataTable = new DataTable();
        List<Commodity> mCommodity = new List<Commodity>();

        public frmSedati()
        {
            InitializeComponent();
        }

        private void frmSedati_Load(object sender, EventArgs e)
        {
            mDataTable.Columns.Add("Goods");
            mDataTable.Columns.Add("Sell");
            mDataTable.Columns.Add("Buy");
            mDataTable.Columns.Add("Demand");
            mDataTable.Columns.Add("Supply");

            if (File.Exists("path.txt"))
            {
                string[] raw = System.IO.File.ReadAllLines("path.txt");
                txtPath.Text = raw[0];

                btnLoad_Click(null, null);
            }
        }

        class Commodity
        {
            public string name;
            public string sellPrice;
            public string buyPrice;
            public string demand;
            public string supply;
            public string demandLevel;
            public string supplyLevel;

            public Commodity(string _name, string _sellPrice, string _buyPrice, string _demand, string _supply, string _demandLevel, string _supplyLevel)
            {
                name = _name;
                sellPrice = _sellPrice;
                buyPrice = _buyPrice;
                demand = _demand;
                supply = _supply;
                demandLevel = _demandLevel;
                supplyLevel = _supplyLevel;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                lstFiles.Items.Clear();
                var sortedFiles = new DirectoryInfo(txtPath.Text).GetFiles("*.csv")
                                                      .OrderByDescending(f => f.LastWriteTime)
                                                      .ToList();

                using (StreamWriter writer = new StreamWriter("path.txt"))
                {
                    writer.WriteLine(txtPath.Text);
                }

                foreach (FileInfo file in sortedFiles)
                {
                    lstFiles.Items.Add(file.Name);
                }
            }catch(Exception aaa)
            {
                MessageBox.Show("Cannot find any csv files. Make sure you entered the correct path.");
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            mDataTable.Clear(); mCommodity.Clear();
            string[] raw = System.IO.File.ReadAllLines(txtPath.Text + "\\" + lstFiles.Text);

            string systemname = raw[1].Split(';')[0];
            string stationname = raw[1].Split(';')[1];
            string date = "(" + (int)(DateTime.Now - File.GetLastWriteTime(txtPath.Text + "\\" + lstFiles.Text)).TotalDays + " days old) " + File.GetLastWriteTime(txtPath.Text + "\\" + lstFiles.Text).ToShortDateString() + " @ "  + File.GetLastWriteTime(txtPath.Text + "\\" + lstFiles.Text).ToShortTimeString();

            int x = 0;
            foreach (string row in raw)
            {
                if (x > 0)
                {
                    string[] split = raw[x].Split(';');

                    string _system = split[0];
                    string _station = split[1];
                    string _commodityName = split[2];
                    string _sellPrice = split[3];
                    string _buyPrice = split[4];
                    string _demand = split[5];
                    string _demandLevel = split[6];
                    string _supply = split[7];
                    string _supplyLevel = split[8];

                    Commodity item = new Commodity(_commodityName, _sellPrice, _buyPrice, _demand, _supply, _demandLevel, _supplyLevel);
                    mCommodity.Add(item);
                }
                x++;
            }

            foreach (Commodity item in mCommodity)
            {

                mDataTable.Rows.Add(item.name, item.sellPrice, item.buyPrice, item.demand + " " + item.demandLevel, item.supply + " " + item.supplyLevel);
            }

 
            dgvTable.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dgvTable.DataSource = mDataTable;
            dgvTable.AutoResizeColumn(0);
            dgvTable.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvTable.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvTable.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvTable.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            lblSystem.Text = systemname;
            lblStation.Text = stationname;
            lblDate.Text = date;
        }

        private void btnPurgeData_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("On stations that have more than 1 data, only the newest data will be kept and others will be deleted. Are you sure you want to do this?", "Purge Older Data", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                List<string> temp = new List<string>();
                var sortedFiles = new DirectoryInfo(txtPath.Text).GetFiles("*.csv")
                                                  .OrderByDescending(f => f.LastWriteTime)
                                                  .ToList();
                foreach (FileInfo file in sortedFiles)
                {
                    string[] raw = System.IO.File.ReadAllLines(file.FullName);

                    string systemname = raw[1].Split(';')[0];
                    string stationname = raw[1].Split(';')[1];
                    string systemnstationname = systemname + "&" + stationname;

                    if(!temp.Contains(systemnstationname))
                    {
                        temp.Add(systemnstationname);
                    }
                    else
                    {
                        File.Delete(file.FullName);
                    }

                }
                btnLoad_Click(null, null);
                MessageBox.Show("Done!");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                lstFiles.Items.Clear();
                var sortedFiles = new DirectoryInfo(txtPath.Text).GetFiles("*.csv")
                                                      .OrderByDescending(f => f.LastWriteTime)
                                                      .ToList();
                foreach (FileInfo file in sortedFiles)
                {
                    if (file.Name.ToLower().Contains(txtSearch.Text.ToLower()))
                    {
                        lstFiles.Items.Add(file.Name);
                    }
                }
            }
            catch (Exception aaa)
            {

            }
        }

        private void btnQuickNote_Click(object sender, EventArgs e)
        {
            frmQuickNote _frmQuickNote = new frmQuickNote();
            _frmQuickNote.StartPosition = this.StartPosition;
            _frmQuickNote.ShowDialog();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            frmAbout _frmAbout = new frmAbout();
            _frmAbout.StartPosition = this.StartPosition;
            _frmAbout.ShowDialog();
        }
    }
}
