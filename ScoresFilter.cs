using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scores_Filter_Ai_Assisted
{
    public partial class ScoresFilter : Form
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "scores", "all scores.txt");
        List<double> allScores = new List<double>();
        int editIndex = -1;
        public ScoresFilter()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (editIndex == -1)
            {
                if (string.IsNullOrWhiteSpace(txtScore.Text))
                {
                    MessageBox.Show("هیچ ورودی دریافت نشد!");
                    return;
                }
                if (double.TryParse(txtScore.Text, out double score))
                {
                    if (score < 0 || score > 20)
                    {
                        MessageBox.Show("لطفا نمره بین 0 تا 20 وارد کنید.");
                        return;
                    }
                    allScores.Add(score);
                }
                else
                {
                    MessageBox.Show("لطفا یک نمره وارد کنید!");
                    return;
                }
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(score);
                }
                lstScoresView.Items.Add(score);
                txtScore.Clear();
                txtScore.Focus();
            }
            else
            {
                if (!double.TryParse(txtScore.Text, out double newScore))
                {
                    MessageBox.Show("لطفا یه عدد معتبر وارد کنید.");
                    return;
                }
                if ( newScore < 0 || newScore > 20)
                {
                    MessageBox.Show("لطفا یه عدد معتبر وارد کنید.");
                    return;
                }
                double oldScore = (double)lstScoresView.SelectedItem;
                int indexInAll = allScores.IndexOf(oldScore);
                allScores[indexInAll] = newScore;
                lstScoresView.Items[editIndex] = newScore;
                File.WriteAllLines(path, allScores.Select(s => s.ToString()));

                editIndex = -1;
                txtScore.Clear();
                txtScore.Focus();

                MessageBox.Show("نمره با موفقیت ویرایش شد.");
            }
        }

        private void ScoresFilter_Load(object sender, EventArgs e)
        {
            if(File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        if (double.TryParse(line, out double score))
                        {
                            allScores.Add(score);
                            lstScoresView.Items.Add(score);
                        }
                }
            }
            lblListTitle.Text = "همه نمرات";
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            editIndex = -1;
            if (string.IsNullOrWhiteSpace(txtThreshold.Text))
            {
                MessageBox.Show("لطفا نمره حد نصاب را وارد کنید");
                return;
            }
            if (!double.TryParse(txtThreshold.Text, out double threshold))
            {
                MessageBox.Show("لطفا یک عدد معتبر وارد کنید!");
                return;
            }
            lstScoresView.Items.Clear();
            foreach (var score in allScores)
                if (score > threshold)
                    lstScoresView.Items.Add(score);
            lblListTitle.Text = $"نمرات بالاتر از {threshold}:";
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            editIndex = -1;
            lstScoresView.Items.Clear();
            foreach (var score in allScores)
                lstScoresView.Items.Add(score);
            lblListTitle.Text = "همه‌ی نمرات:";
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lstScoresView.SelectedIndex == -1)
            {
                MessageBox.Show("لطفا یک نمره را انتخاب کنید!");
                return;
            }
            editIndex = lstScoresView.SelectedIndex;
            txtScore.Text = lstScoresView.SelectedItem.ToString();
            txtScore.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstScoresView.SelectedIndex == -1)
            {
                MessageBox.Show("لطفا یک نمره را انتخاب کنید.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "آیا مطمئن هستید؟",
                "تایید حذف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
                );
            if (result != DialogResult.Yes)
                return;
            double selectedScore = (double)lstScoresView.SelectedItem;
            allScores.Remove(selectedScore);
            lstScoresView.Items.RemoveAt(lstScoresView.SelectedIndex);
            File.WriteAllLines(path, allScores.Select(s => s.ToString()));
            editIndex = -1;
        }
    }
}
