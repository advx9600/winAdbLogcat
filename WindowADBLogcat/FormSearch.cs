using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowADBLogcat
{
    public partial class FormSearch : Form
    {
        static String SearchText;
        RichTextBox mRichBox;
        int mSearchIndex;
        public FormSearch(RichTextBox txBox)
        {
            InitializeComponent();
            mRichBox = txBox;
            if (!String.IsNullOrEmpty(txBox.SelectedText))
            {
                mSearchIndex = txBox.SelectionStart + txBox.SelectionLength;
                this.textBoxSearch.Text = txBox.SelectedText;
                this.textBoxSearch.SelectAll();
            }
            else if (!String.IsNullOrEmpty(SearchText))
            {
                this.textBoxSearch.Text = SearchText;
                this.textBoxSearch.SelectAll();
            }
        }

        private void buttonPre_Click(object sender, EventArgs e)
        {
            findStr(-1);
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            findStr(+1);
        }

        private void findStr(int dir)
        {
            SearchText = textBoxSearch.Text;
            if (!String.IsNullOrEmpty(SearchText))
            {
                bool isFirstSearch = mSearchIndex > 0 ? false : true;
                mSearchIndex = mRichBox.Text.IndexOf(SearchText, mSearchIndex);
                if (mSearchIndex > -1)
                {
                    mRichBox.SelectionStart = mSearchIndex;
                    mRichBox.SelectionLength = SearchText.Length;
                    mSearchIndex += SearchText.Length;
                    mRichBox.Focus();
                }
                else
                {
                    if (isFirstSearch)
                        MessageBox.Show("未找到");
                    else
                        MessageBox.Show("已到结尾");
                    mSearchIndex = 0;
                }
            }
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                findStr(+1);
                Close();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
