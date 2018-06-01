using KargoTakip.Helper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KargoTakip.View
{
    public partial class customerForm : Form
    {
        public customerForm()
        {
            InitializeComponent();
        }
        private SqlConnection con = new SqlConnection("Data Source=FARUK;Initial Catalog=KargoTakip;User Id=root;Password=03102593");
        private SqlDataAdapter adapter;
        private SqlCommand cmd;
        private DataTable dt;
        private feedbacksModel feedbacks = new feedbacksModel();
        private async Task<Boolean> addFeedbacks()
        {
            try
            {
                adapter = new SqlDataAdapter("SELECT * FROM feedbacks WHERE orderId='"+feedbacks.orderId+"'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                if(dt.Rows.Count>0)
                {
                    return false;
                }
                else
                {
                    adapter = new SqlDataAdapter("SELECT id FROM company WHERE companyName='"+feedbacks.businessName+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    string businessId = dt.Rows[0][0].ToString();
                    await con.OpenAsync();
                    cmd = new SqlCommand("INSERT INTO feedbacks VALUES('"+entryForm.login_id+"','"+feedbacks.orderId+"','"+businessId+"','"+feedbacks.feedBacksText+"')",con);
                    await cmd.ExecuteNonQueryAsync();
                    feedbacks.orderId = feedbacks.feedBacksText = null;
                    return true;
                }
            }catch(Exception)
            {
                return false;
            }
            finally
            {
                if(con.State  == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        private async Task<DataTable> getOrder()
        {
            adapter = new SqlDataAdapter("SELECT orderId,businessName,employeeName,orderState FROM orders WHERE deliveryTC=(SELECT tc FROM customers WHERE id='" + entryForm.login_id + "') and isDelete='No'", con);
            dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        private async void getData()
        {

            dataGridView1.DataSource = await Task.Run(getOrder);
            dataGridView1.Columns[0].HeaderText = "Sipariş No";
            dataGridView1.Columns[1].HeaderText = "Firma Adı";
            dataGridView1.Columns[2].HeaderText = "Teslimatçı Adı";
            dataGridView1.Columns[3].HeaderText = "Sipariş Durum";
            GC.SuppressFinalize(dt);
            for(int i=0;i<dataGridView1.Rows.Count;i++)
            {
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                if(dataGridView1.Rows[i].Cells[3].Value.ToString() == "Siparisiniz Teslim Edildi !")
                {
                    style.BackColor = Color.Green;
                }
                else
                {
                    style.BackColor = Color.Red;
                }
                dataGridView1.Rows[i].DefaultCellStyle = style;
            }
        }
        private void customerForm_Load(object sender, EventArgs e)
        {
            dataGridView1.MultiSelect = false;
            getData();
        }
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) 
        {
            if(dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString()=="Siparisiniz Teslim Edildi !")
            {
                txtOrderId.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                feedbacks.employeeName = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                feedbacks.businessName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            }
            else
            {
                MessageBox.Show("Sipariş Durumu Teslim Edildi Olmadan Geri Bildirim Bırakamazsınız!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private async void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barButtonItem1.Enabled = false;
            if(txtFeedBacks.Text!=""&&feedbacks.employeeName!=null&&feedbacks.businessName!=null)
            {
                feedbacks.orderId = txtOrderId.Text;
                feedbacks.feedBacksText = txtFeedBacks.Text;
                bool isOk = false;
                isOk = await Task.Run(addFeedbacks);
                if(isOk == true)
                {
                    MessageBox.Show("Geri Bildirim Eklendi!");
                }
                else
                {
                    MessageBox.Show("Geri Bildirim Eklenemedi!");
                }
                txtOrderId.Text = "";
                txtFeedBacks.Text = "";
            }
            else
            {
                MessageBox.Show("Lütfen ilgili yerleri doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            barButtonItem1.Enabled = true;

        }
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barButtonItem3.Enabled = false;
            getData();
            barButtonItem3.Enabled = true;
        }
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(printPreviewDialog1.ShowDialog() == DialogResult.OK)
            {
                orderPrint.Print();
            }
        }
        private void orderPrint_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font font = new Font("Verdana",12,FontStyle.Bold);
            SolidBrush titleBrush = new SolidBrush(Color.Red);
            SolidBrush bodyBrush = new SolidBrush(Color.Black);
            e.Graphics.DrawString("Tarih:" + DateTime.Now.ToShortDateString() + "\nSipariş Sayısı:" + dt.Rows.Count, font, titleBrush, 650, 0);
            e.Graphics.DrawString("\t\t\t\t\tSiparişler\n-------------------------------------------------------------------------------------------------", font, titleBrush, 0, 20);
            int y = 50;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                e.Graphics.DrawString("Sipariş No:"+dataGridView1.Rows[i].Cells[0].Value.ToString()+"\nFirma Adı:"+dataGridView1.Rows[i].Cells[1].Value.ToString()+"\nTeslimatçı Adı:"+dataGridView1.Rows[i].Cells[2].Value.ToString()+"\nSipariş Durum:"+dataGridView1.Rows[i].Cells[3].Value.ToString(), font, bodyBrush, 0, y);
                y += 90;
            }
        }

        private void customerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Form x = new entryForm();
            //x.Show();
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.SuppressFinalize(this);
            Application.Exit();

        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult result = MessageBox.Show("Oturumu Kapatmak İstediğinize Emin Misiniz?","EMİN MİSİNİZ?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if(result == DialogResult.Yes)
            {
                Form x = new entryForm();
                this.Hide();
                x.Show();
            }
        }
    }
}
