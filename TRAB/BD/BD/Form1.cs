using Npgsql;
using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace BD
{
    public partial class Form1 : Form
    {

        public string status;
        NpgsqlConnection conexao = new NpgsqlConnection("Host = localhost; Username=postgres;Password=root;Database=AT");
        NpgsqlCommand comando = new NpgsqlCommand();
        DataTable ds = new DataTable();
        DataTable combo = new DataTable();
        public string ns, dat,OScheck,Opreco, check, pcheck, preco, gen, cep, tel, cel, em, cpf, tabela;
        public decimal itpreco, desconto, precofinal,OSpreco,OSdesconto,OSprecofinal;
        public int op = 0;
        public string proto,OSprotocol;
        public string Pprotocolo, Pcodigo, Pqtd, OSproto,OSitem,OSquantidade;
        public string usuariocombo;


        #region Cliente

        private void button1_Click_1(object sender, EventArgs e)
        {

            try
            {
                ns = textBox2.Text;
                em = textBox3.Text;
                dat = maskedTextBox1.Text;
                tel = textBox4.Text;
                cel = textBox6.Text;
                gen = comboBox1.Text;
                cpf = textBox5.Text;

                AbrirBd();



                comando.Parameters.Clear();
                comando.CommandText = "INSERT INTO tab.cliente (cli_nome,cli_data_nascimento,cli_telefone,cli_email,cli_celular,cli_genero,cli_cpf) values (@Nome, @data,@Fone, @Email,@Celular,@Genero,@Cpf)";
                comando.Parameters.AddWithValue("@Nome", ns);
                comando.Parameters.AddWithValue("@data", DateTime.Parse(dat));
                comando.Parameters.AddWithValue("@Fone", tel);
                comando.Parameters.AddWithValue("@Email", em);
                comando.Parameters.AddWithValue("@Celular", cel);
                comando.Parameters.AddWithValue("@Genero", gen);
                comando.Parameters.AddWithValue("@Cpf", cpf);

                comando.ExecuteNonQuery();
                status = "Inserido com sucesso";
                MessageBox.Show(status);
                FecharBd();
                textBox2.Text = textBox3.Text = textBox4.Text = textBox6.Text = comboBox1.Text = textBox5.Text = maskedTextBox1.Text = "";
            }

            catch (Exception erro)
            {

                status = "Erro ao Incluir!==> " + erro.Message;
                MessageBox.Show(status);
                FecharBd();
            }

        }






        #endregion Cliente

        #region VerBD

        private void button2_Click(object sender, EventArgs e)
        {
            ds.Clear();
            ds.Columns.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            AbrirBd();
            tabela = comboBox2.Text;


            string sql = "SELECT * FROM tab." + tabela + "";

            NpgsqlDataAdapter regBD = new NpgsqlDataAdapter(sql, conexao);

            regBD.Fill(ds);
            dataGridView1.DataSource = ds;
            regBD = null;
            FecharBd();
        }

        #endregion VerBD

        #region Venda

        public void tabledb()
        {
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (!(row.Cells["Protocolo"].Value == null))
                {
                    AbrirBd();


                    Pprotocolo = row.Cells["Protocolo"].Value.ToString();
                    Pcodigo = row.Cells["Codigo"].Value.ToString();
                    Pqtd = row.Cells["quantidade"].Value.ToString();



                    comando.Parameters.Clear();
                    comando.CommandText = "INSERT INTO tab.prods_venda (pvnd_protocolo,pvnd_produto,pvnd_qtd) values (@Pprotocolo,@Pcodigo, @Pqtd)";
                    comando.Parameters.AddWithValue("@Pprotocolo", int.Parse(Pprotocolo));
                    comando.Parameters.AddWithValue("@Pcodigo", int.Parse(Pcodigo));
                    comando.Parameters.AddWithValue("@Pqtd", int.Parse(Pqtd));

                    comando.ExecuteNonQuery();
                    status = "Inserido com sucesso..";
                    MessageBox.Show(status);

                    FecharBd();
                }

            }
        }

        public void Somatorio()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                total += Convert.ToDecimal(row.Cells["valor"].Value) * Convert.ToDecimal(row.Cells["quantidade"].Value);
            }

            textBox10.Text = Convert.ToDouble(total).ToString();
        }

        public void protocol()
        {

            AbrirBd();
            comando.CommandText = "SELECT vnd_protocolo FROM tab.venda ORDER BY vnd_protocolo DESC LIMIT 1";
            NpgsqlDataReader reg = null;
            reg = comando.ExecuteReader();
            if (!(reg.Read()))
            {

                proto = "1";
            }
            else
            {
                int op2;
                proto = reg["vnd_protocolo"].ToString();

                op2 = Convert.ToInt32(proto);
                op2++;
                proto = op2.ToString();

            }
            FecharBd();
        }

        public void add(int a)
        {

            AbrirBd();
            comando.Parameters.Clear();
            comando.CommandText = "UPDATE tab.itens set it_quantidade = it_quantidade + "+a+" where it_codigo_barra = '" + textBox7.Text + "'";
            comando.ExecuteNonQuery();
            FecharBd();
        }

        public void desc(int a)
        {
            AbrirBd();
            comando.Parameters.Clear();
            comando.CommandText = "UPDATE tab.itens set it_quantidade = it_quantidade - "+a+" where it_codigo_barra = '" + textBox7.Text + "'";
            comando.ExecuteNonQuery();
            FecharBd();

        }

        public void usu()
        {

            AbrirBd();
            string sql = "SELECT usr_nome,usr_email FROM tab.usuario";

            NpgsqlDataAdapter regBD = new NpgsqlDataAdapter(sql, conexao);

            regBD.Fill(ds);
            comboBox5.DataSource = ds;
            comboBox5.DisplayMember = "usr_nome";
            comboBox5.ValueMember = "usr_email";
            comboBox6.DataSource = ds;
            comboBox6.DisplayMember = "usr_nome";
            comboBox6.ValueMember = "usr_email";
            regBD = null;
            FecharBd();

        }


        private void button4_Click(object sender, EventArgs e)
        {
            AbrirBd();

            comando.CommandText = "SELECT cli_nome FROM tab.cliente WHERE cli_cpf = '" + textBox1.Text + "'";
            NpgsqlDataReader reg = null;
            reg = comando.ExecuteReader();
            if (reg.Read())
            {
                check = reg["cli_nome"].ToString();
                MessageBox.Show("Encontrado com sucesso");
                textBox8.Text = check;
                FecharBd();
            }
            else
            {
                if (MessageBox.Show("Registro não existente. \n Deseja fazer o cadastro do cliente ? ", "Atencao", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    tabControl1.SelectedTab = tabPage1;
                    textBox5.Text = textBox1.Text;
                    textBox1.Text = "";
                    FecharBd();
                }
                FecharBd();
            }



        }

        private void button5_Click(object sender, EventArgs e)
        {

            protocol();

            AbrirBd();

            comando.CommandText = "SELECT it_nome,it_preco_venda FROM tab.itens WHERE it_codigo_barra = '" + textBox7.Text + "'";
            NpgsqlDataReader reg = null;
            reg = comando.ExecuteReader();
            if (reg.Read())
            {
                pcheck = reg["it_nome"].ToString();
                preco = reg["it_preco_venda"].ToString();
                MessageBox.Show("Encontrado com sucesso");


                dataGridView2.Rows.Add(proto, textBox7.Text, pcheck, preco, textBox9.Text);



                FecharBd();

                Somatorio();
                desc(int.Parse(textBox9.Text));
                op++;


            }
            else
            {
                MessageBox.Show("Produto não existente");

                FecharBd();
            }


        }

        private void button6_Click(object sender, EventArgs e)
        {
            maskedTextBox3.Text = DateTime.Now.ToString();
        }

      

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

   
        private void button7_Click(object sender, EventArgs e)
        {
            itpreco = Convert.ToDecimal(textBox10.Text);
            desconto = Convert.ToDecimal(textBox11.Text) / 100;

            precofinal = itpreco - (itpreco * desconto);
            textBox12.Text = precofinal.ToString();

        }

        private void button8_Click(object sender, EventArgs e)
        {


            try
            {


                AbrirBd();


                comando.Parameters.Clear();
                comando.CommandText = "INSERT INTO tab.venda (vnd_total,vnd_desconto,vnd_data_criacao,vnd_data_pagamento,vnd_subtotal,vnd_forma_pagamento,vnd_usuario,vnd_cliente) values (@Total,@Desconto, @DataCriacao,@DataPagamento,@SubTotal,@FormaPagamento,@Usuario,@Cliente)";
                comando.Parameters.AddWithValue("@Total", double.Parse(textBox12.Text));
                comando.Parameters.AddWithValue("@Desconto", double.Parse(textBox11.Text));
                comando.Parameters.AddWithValue("@DataCriacao", DateTime.Now);
                comando.Parameters.AddWithValue("@DataPagamento", DateTime.Parse(maskedTextBox3.Text));
                comando.Parameters.AddWithValue("@Subtotal", double.Parse(textBox10.Text));
                comando.Parameters.AddWithValue("@FormaPagamento", comboBox3.Text);
                comando.Parameters.AddWithValue("@Usuario", comboBox5.SelectedValue);
                comando.Parameters.AddWithValue("@Cliente", textBox1.Text);

                comando.ExecuteNonQuery();


                FecharBd();
                tabledb();

                textBox12.Text = textBox11.Text = textBox10.Text = comboBox3.Text = textBox1.Text = textBox7.Text = maskedTextBox3.Text = textBox8.Text = "";
                textBox7.Text = "1";
                dataGridView2.Rows.Clear();
                dataGridView2.Refresh();

                status = "Inserido com sucesso";
                MessageBox.Show(status);
            }

            catch (Exception erro)
            {

                status = "Erro ao Incluir!==> " + erro.Message;
                MessageBox.Show(status);
                FecharBd();
            }

        }


        private void button14_Click(object sender, EventArgs e)
        {

            int i = int.Parse(dataGridView2.CurrentRow.Index.ToString());
            int indx = int.Parse(dataGridView2.Rows[i].Cells[4].Value.ToString());
            dataGridView2.Rows.RemoveAt(dataGridView2.CurrentRow.Index);
            Somatorio();


            add(indx);
        }

        #endregion Venda

        #region Serviço

        public void tabledOS()
        {
            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                if (!(row.Cells["osproto"].Value == null))
                {
                    AbrirBd();


                    OSproto = row.Cells["osproto"].Value.ToString();
                    OSitem = row.Cells["ositem"].Value.ToString();
                    OSquantidade = row.Cells["osquantidade"].Value.ToString();



                    comando.Parameters.Clear();
                    comando.CommandText = "INSERT INTO tab.itens_os (pos_protocolo,pos_item,pos_qtd) values (@OSproto,@OSitem, @OSquantidade)";
                    comando.Parameters.AddWithValue("@OSproto", int.Parse(OSproto));
                    comando.Parameters.AddWithValue("@OSitem", int.Parse(OSitem));
                    comando.Parameters.AddWithValue("@Osquantidade", int.Parse(OSquantidade));

                    comando.ExecuteNonQuery();
                    status = "Inserido com sucesso..";
                    MessageBox.Show(status);

                    FecharBd();
                }
               

            }
        }

        public void SomatorioOS()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                total += Convert.ToDecimal(row.Cells["ospreco"].Value) * Convert.ToDecimal(row.Cells["osquantidade"].Value);
            }

            textBox15.Text = Convert.ToDouble(total).ToString();
        }

        public void protocolOS()
        {

            AbrirBd();
            comando.CommandText = "SELECT os_protocolo FROM tab.ordem_de_servico ORDER BY os_protocolo DESC LIMIT 1";
            NpgsqlDataReader reg = null;
            reg = comando.ExecuteReader();
            if (!(reg.Read()))
            {

                OSprotocol = "1";
            }
            else
            {
                int op2;
                OSprotocol = reg["os_protocolo"].ToString();

                op2 = Convert.ToInt32(OSprotocol);
                op2++;
                OSprotocol = op2.ToString();

            }
            FecharBd();
        }

        public void addOS(int a)
        {

            AbrirBd();
            comando.Parameters.Clear();
            comando.CommandText = "UPDATE tab.itens set it_quantidade = it_quantidade + " + a + " where it_codigo_barra = '" + textBox19.Text + "'";
            comando.ExecuteNonQuery();
            FecharBd();
        }

        public void descOS(int a)
        {
            AbrirBd();
            comando.Parameters.Clear();
            comando.CommandText = "UPDATE tab.itens set it_quantidade = it_quantidade - " + a + " where it_codigo_barra = '" + textBox19.Text + "'";
            comando.ExecuteNonQuery();
            FecharBd();

        }



        private void button9_Click(object sender, EventArgs e)
        {
            try
            {


                AbrirBd();


                comando.Parameters.Clear();
                comando.CommandText = "INSERT INTO tab.ordem_de_servico (os_total,os_desconto,os_data_criacao,os_data_pagamento,os_subtotal,os_aparelho,os_forma_pagamento,os_usuario,os_cliente) values (@OStotal, @OSdesconto,@OSdatacriacao,@OSdatapagamento,@OSsubtotal,@OSaparelho,@OSforma_pagamento,@OSusuario,@OScliente)";
                comando.Parameters.AddWithValue("@OStotal", double.Parse(textBox13.Text));
                comando.Parameters.AddWithValue("@OSdesconto", double.Parse(textBox14.Text));
                comando.Parameters.AddWithValue("@OSdatacriacao", DateTime.Now);
                comando.Parameters.AddWithValue("@OSdatapagamento", DateTime.Parse(maskedTextBox2.Text));
                comando.Parameters.AddWithValue("@OSaparelho", textBox20.Text);
                comando.Parameters.AddWithValue("@OSsubtotal", double.Parse(textBox15.Text));
                comando.Parameters.AddWithValue("@OSforma_pagamento", comboBox4.Text);
                comando.Parameters.AddWithValue("@OSusuario", comboBox6.SelectedValue);
                comando.Parameters.AddWithValue("@OScliente", textBox18.Text);

                comando.ExecuteNonQuery();


                FecharBd();
                tabledOS();

                textBox13.Text = textBox17.Text  = textBox14.Text = textBox20.Text = comboBox4.Text = textBox15.Text = textBox18.Text = maskedTextBox2.Text = textBox16.Text = textBox19.Text = "";
                dataGridView3.Rows.Clear();
                dataGridView3.Refresh();

                status = "Inserido com sucesso";
                MessageBox.Show(status);
            }

            catch (Exception erro)
            {

                status = "Erro ao Incluir!==> " + erro.Message;
                MessageBox.Show(status);
                FecharBd();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            OSpreco = Convert.ToDecimal(textBox15.Text);
            OSdesconto = Convert.ToDecimal(textBox14.Text) / 100;

            OSprecofinal = OSpreco - (OSpreco * OSdesconto);
            textBox13.Text = OSprecofinal.ToString();
        }


        private void button11_Click(object sender, EventArgs e)
        {
            maskedTextBox2.Text = DateTime.Now.ToString();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            protocolOS();

            AbrirBd();
            

            comando.CommandText = "SELECT it_nome,it_preco_venda FROM tab.itens WHERE it_codigo_barra = '" + textBox19.Text + "'";
            NpgsqlDataReader reg = null;
            reg = comando.ExecuteReader();
            if (reg.Read())
            {
                OScheck = reg["it_nome"].ToString();
                Opreco = reg["it_preco_venda"].ToString();
                MessageBox.Show("Encontrado com sucesso");

                dataGridView3.Rows.Add(OSprotocol, textBox19.Text, OScheck, Opreco, textBox17.Text);



                SomatorioOS();
                FecharBd();
                descOS(int.Parse(textBox17.Text));

            }
            else
            {
                MessageBox.Show("Produto não existente");




                FecharBd();

            }

        }


        private void button13_Click(object sender, EventArgs e)
        {
            AbrirBd();

            comando.CommandText = "SELECT cli_nome FROM tab.cliente WHERE cli_cpf = '" + textBox18.Text + "'";
            NpgsqlDataReader reg = null;
            reg = comando.ExecuteReader();
            if (reg.Read())
            {
                check = reg["cli_nome"].ToString();
                MessageBox.Show("Encontrado com sucesso");
                textBox16.Text = check;
                FecharBd();
            }
            else
            {
                if (MessageBox.Show("Registro não existente. \n Deseja fazer o cadastro do cliente ? ", "Atencao", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    tabControl1.SelectedTab = tabPage1;
                    FecharBd();
                }
                FecharBd();
            }

        }

        #endregion Serviço









        private void button15_Click(object sender, EventArgs e)
        {
            int i = int.Parse(dataGridView3.CurrentRow.Index.ToString());
            int indx = int.Parse(dataGridView3.Rows[i].Cells[4].Value.ToString());
            dataGridView3.Rows.RemoveAt(dataGridView3.CurrentRow.Index);
            Somatorio();


            addOS(indx);

        }









        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

     

      
      
       

        private void button3_Click(object sender, EventArgs e)
        {
            
            
           
            
        }

        

       

        private void tabPage1_Click(object sender, EventArgs e)
        {
         
        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public Form1()
        {
            InitializeComponent();

        }


        public NpgsqlCommand AbrirBd()
        {
            comando.Connection = conexao;
            conexao.Open();
            return comando;
        }

        public void FecharBd()
        {
            conexao.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            usu();
           
        }
   
    }
}
