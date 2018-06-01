using KargoTakip.Helper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KargoTakip.View
{
    public partial class employeeForm : Form
    {
        //Variables
        private SqlConnection con = new SqlConnection("Data Source=FARUK;Initial Catalog=KargoTakip;User Id=root;Password=03102593");
        private SqlDataAdapter adapter;
        private SqlCommand cmd;
        private DataTable dt;
        private customerModel customer = new customerModel();
        private orderModel order = new orderModel();
        //Variables
        public employeeForm()
        {
            InitializeComponent();
        }
        private async Task<DataTable> getOrder()
        {
            adapter = new SqlDataAdapter("SELECT orderId,employeeName,orderState,deliveryTC FROM orders WHERE businessName=(SELECT companyName FROM company WHERE id='" + entryForm.login_id + "') and isDelete='No'", con);
            dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        private async void getData()
        {
            try
            {

                dataGridView1.DataSource = await Task.Run(getOrder);
                dataGridView1.Columns[0].HeaderText = "Sipariş No";
                dataGridView1.Columns[1].HeaderText = "Teslimatçı Adı";
                dataGridView1.Columns[2].HeaderText = "Sipariş Durumu";
                dataGridView1.Columns[3].HeaderText = "Alıcı TC No";
                for(int i=0;i<dataGridView1.Rows.Count;i++) //Siparişleri Renklendirme!
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    if(dataGridView1.Rows[i].Cells[2].Value.ToString() =="Siparisiniz Teslim Edildi !")
                    {
                        style.BackColor = Color.Green;
                        dataGridView1.Rows[i].DefaultCellStyle = style;
                    }
                    else
                    {
                        style.BackColor = Color.Red;
                        dataGridView1.Rows[i].DefaultCellStyle = style;
                    }
                }
                GC.SuppressFinalize(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        } // Veri Çekme
        private void groupBxControlsClear()
        {
            txtCustomerTC.Text = txtOrderId.Text = "";
            cbxOrderState.SelectedIndex = -1;
        }
        //Async Task
        private async Task getAddress()
        {
            try
            {
                adapter = new SqlDataAdapter("SELECT nameSurname,address FROM customers WHERE tc='"+customer.tcNo+"'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                MessageBox.Show(dt.Rows[0][0].ToString()+" Adlı Müşterinin Adresi:"+dt.Rows[0][1].ToString(),dt.Rows[0][0].ToString(),MessageBoxButtons.OK);
                customer.tcNo = null;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        } // Adresi Bilgisi Alma
        private async Task<Boolean> addOrder()
        {
            try
            {
                //Sipariş No Kontrol
                adapter = new SqlDataAdapter("SELECT * FROM orders WHERE orderId='"+order.orderId+"'",con);
                dt = new DataTable();
                adapter.Fill(dt);
                if(dt.Rows.Count>0)
                {
                    return false;
                }
                else
                {
                    //Müşteri Kontrol
                    adapter = new SqlDataAdapter("SELECT * FROM customers WHERE tc='"+order.deliveryTC+"'",con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    if(dt.Rows.Count>0)
                    {
                        //Şirket Adı Alma
                        adapter = new SqlDataAdapter("SELECT companyName FROM company WHERE id='"+entryForm.login_id+"'",con);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        string bsName = dt.Rows[0][0].ToString();
                        await con.OpenAsync();
                        cmd = new SqlCommand("INSERT INTO orders(orderId, businessName, employeeName, orderState, deliveryTC, isDelete) VALUES('"+order.orderId+"','"+bsName+"','"+order.orderEmployeeName+"','"+order.orderState+"','"+order.deliveryTC+"','No')",con);
                        await cmd.ExecuteNonQueryAsync();
                        order.orderId = order.orderEmployeeName = order.orderState = order.deliveryTC = null;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }catch(Exception)
            {
                return false;
            }
            finally
            {
                if(con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        private async Task<Boolean> deleteOrder()
        {
            try
            {
                await con.OpenAsync();
                cmd = new SqlCommand("UPDATE orders SET isDelete='Yes' WHERE orderId='"+order.orderId+"'",con);
                await cmd.ExecuteNonQueryAsync();
                cmd.Dispose();
                order.orderId = null;
                return true;
            }catch(Exception)
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        private async Task<Boolean> updateOrderState()
        {
            try
            {
                await con.OpenAsync();
                cmd = new SqlCommand("Update orders SET orderState='"+order.orderState+"' WHERE orderId='"+order.orderId+"'",con);
                await cmd.ExecuteNonQueryAsync();
                cmd.Dispose();
                order.orderState = order.orderId = null;
                return true;
            }catch(Exception)
            {
                return false;
            }
            finally
            {
                if(con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        //Async Task
        private void employeeForm_Load(object sender, EventArgs e)
        {
            getData();
        }
        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) //Adres Bilgisi İçin Model 
        {
            customer.tcNo = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            getAddress();
        }
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) //Listeyi Yenile Butonu
        {
            barButtonItem4.Enabled = false;
            getData();
            barButtonItem4.Enabled = true;
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            groupBxControlsClear();
        } // Temizle Butonu
        private async void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barButtonItem1.Enabled = false;
           if(txtCustomerTC.Text !=""&&txtOrderId.Text!=""&&cbxOrderState.Text!="")
            {
                order.deliveryTC = txtCustomerTC.Text;
                order.orderId = txtOrderId.Text;
                order.orderState = cbxOrderState.Text;
                order.orderEmployeeName = entryForm.employeeName;
                bool isOk = false;
                isOk = await Task.Run(addOrder);
                if(isOk == true)
                {
                    MessageBox.Show("Sipariş Eklendi!");
                }
                else
                {
                    MessageBox.Show("Sipariş Eklenemedi!");
                }
                groupBxControlsClear();
            }
            else
            {
                MessageBox.Show("Lütfen İlgili Yerleri Doldurunuz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            barButtonItem1.Enabled = true;

        } // Sipariş Ekleme
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            txtOrderId.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            for(int i=0;i<cbxOrderState.Items.Count;i++)
            {
                if(cbxOrderState.Items[i].ToString() == dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString())
                {
                    cbxOrderState.Text = cbxOrderState.Items[i].ToString();
                    break;
                }
            }
            txtCustomerTC.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            order.orderId = txtOrderId.Text;
            order.deliveryTC = txtCustomerTC.Text;
        }//Sipariş Bilgisi Alma
        private async void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)//Sipariş Silme
        {
            barButtonItem2.Enabled = false;
            if (order.orderId!=null)
            {
                DialogResult result = MessageBox.Show("Siparişi silmek istediğinize emin misiniz?","Emin Misiniz?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    bool isOk = false;
                    isOk = await Task.Run(deleteOrder);
                    if(isOk == true)
                    {
                        MessageBox.Show("Sipariş Başarıyla Silindi!");
                    }
                    else
                    {
                        MessageBox.Show("Sipariş Silinemedi!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen listeden bir sipariş seçiniz!", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            barButtonItem2.Enabled = true;

        }
        private async void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) // Sipariş Güncelleme
        {
            if(order.orderId!=null && order.orderState!=null)
            {
                bool isOk = false;
                isOk = await Task.Run(updateOrderState);
                if(isOk == true)
                {
                    MessageBox.Show("Sipariş Durumu Başarıyla Güncellendi!");
                }
                else
                {
                    MessageBox.Show("Sipariş Durumu Güncellenemedi!");
                }
            }
            else
            {
                MessageBox.Show("Lütfen listeden bir sipariş seçiniz!","HATA",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void cbxOrderState_SelectedIndexChanged(object sender, EventArgs e)
        {
            order.orderState = cbxOrderState.Text;
        } //Sipariş Durum Seçme İşlemi
        private void printOrder_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) // Çıktı Alma
        {
            Font font = new Font("Verdana",12,FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);
            SolidBrush titleBrush = new SolidBrush(Color.Red);
            e.Graphics.DrawString("Tarih:"+DateTime.Now.ToShortDateString()+"\nSipariş Sayısı:"+dt.Rows.Count,font,titleBrush,650,0);
            e.Graphics.DrawString("\t\t\t\t\tSiparişler\n-------------------------------------------------------------------------------------------------",font,titleBrush,0,20);
            int y = 50;
            for(int i=0;i<dataGridView1.Rows.Count;i++)
            {
                e.Graphics.DrawString("Sipariş No:"+dataGridView1.Rows[i].Cells[0].Value.ToString()+"\nTeslimatçı Adı:"+dataGridView1.Rows[i].Cells[1].Value.ToString()+"\nSipariş Durumu:"+dataGridView1.Rows[i].Cells[2].Value.ToString()+"\nAlıcı T.C:"+dataGridView1.Rows[i].Cells[3].Value.ToString(),font,brush,0,y);
                y += 90;
            }
        }
        private void Y_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
            {
                printOrder.Print();
            }
        } // Çıktı Alma Buton

        private void employeeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Form x = new entryForm();
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //x.Show();
            Application.Exit();

        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult result = MessageBox.Show("Oturumu Kapatmak İstediğinize Emin Misiniz?", "EMİN MİSİNİZ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Form x = new entryForm();
                this.Hide();
                x.Show();
            }
        }
    }
}
