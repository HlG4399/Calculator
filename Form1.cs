using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Calculator
{
    /// <summary>
    /// 记录操作数和运算符
    /// </summary>
    public struct union
    {
        public string Operator;
        public double Number;
    }

    /// <summary>
    /// 查找运算符时用
    /// </summary>
    public struct OP
    {
        public string operation;
        public int index;//括号层数,当这个index被标记为-1时，就不会再次被查找到
        public int location;//位置
    }

    public partial class calculator : Form
    {
        public int index = 0;//记录最大的括号层数

        public calculator()
        {
            InitializeComponent();
        }

        private void btn0_Click(object sender, EventArgs e)
        {
            formula.Text += (sender as Button).Text;
        }

        private void btn_powerExponent_Click(object sender, EventArgs e)
        {
            formula.Text += "^";
        }

        private void btn_sqrt_Click(object sender, EventArgs e)
        {
            formula.Text += "sqrt";
        }

        private void btn_tan_Click(object sender, EventArgs e)
        {
            formula.Text += "tan";
        }

        private void calculate_Click(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode();
            OP[] Aop = new OP[100];
            string str=formula.Text.Replace(" ","");
            int tail = Sortop1(str, ref Aop);
            try
            {
                createTreenode(node, str, 0, str.Length - 1, tail,ref Aop);
                result.Text = caculate(node).ToString();
            }
            catch
            {
                MessageBox.Show("表达式输入有误，请重新输入。");
                result.Text = "";
            }
        }

        /// <summary>
        /// 查找op,并填充Aop数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="Aop"></param>
        /// <returns></returns>
        public int Sortop1(string str,ref OP[] Aop)
        {
            int j = 0,ind = 0;//ind记录Aop的top
            for (int i = 0;i<str.Length;i++)
            {
                if (str[i] == '(')
                    ind++;
                else if (str[i] == ')')
                    ind--;
                else if (Sortop2(str,i,j,ref Aop))
                {
                    Aop[j].index = ind;
                    Aop[j].location = i;
                    j++;
                }
                index = (index > ind) ? index : ind;
            }
            return j;
        }

        /// <summary>
        /// 判断字符串中运算符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool Sortop2(string str,int i,int j,ref OP[] Aop)
        {
            switch (str[i])
            {
                case '+':
                    Aop[j].operation = str.Substring(i,1);
                    return true;
                case '-':
                    Aop[j].operation = str.Substring(i, 1);
                    return true;
                case '*':
                    Aop[j].operation = str.Substring(i, 1);
                    return true;
                case '/':
                    Aop[j].operation = str.Substring(i, 1);
                    return true;
                case '^':
                    Aop[j].operation = str.Substring(i, 1);
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 将字符串转化为浮点数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        double str2float(string str, int p, int q)
        {
            if (q < 0)
            {
                throw new ArgumentException();
            }
            return Convert.ToDouble(str.Substring(p, q - p + 1));
        }

        /// <summary>
        /// 判断字符串是否只含有数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        bool isnum(string str,int p,int q)
        {
            if (0 == q)
            {
                if ('0' <= str[0] && str[0] <= '9')
                {
                    return true;
                }
                return false;
            }
            if (str.Substring(p,q-p+1).Contains('+') || str.Substring(p,q-p+1).Contains('-') || str.Substring(p, q - p + 1).Contains('*') || str.Substring(p, q - p + 1).Contains('/') || str.Substring(p, q - p + 1).Contains('^') || str.Substring(p, q - p + 1).Contains("sin") || str.Substring(p, q - p + 1).Contains("cos") || str.Substring(p, q - p + 1).Contains("tan") || str.Substring(p, q - p + 1).Contains("ln") || str.Substring(p, q - p + 1).Contains("sqrt"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断字符串是否只含有运算符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        bool isoperator(string str,int p,int q)
        {
            if (str.Substring(p, q - p + 1).Contains('0') || str.Substring(p, q - p + 1).Contains('1') || str.Substring(p, q - p + 1).Contains('2') || str.Substring(p, q - p + 1).Contains('3') || str.Substring(p, q - p + 1).Contains('4') || str.Substring(p, q - p + 1).Contains('5') || str.Substring(p, q - p + 1).Contains('6') || str.Substring(p, q - p + 1).Contains('7') || str.Substring(p, q - p + 1).Contains('8') || str.Substring(p, q - p + 1).Contains('9') || str.Substring(p, q - p + 1).Contains('0'))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断字符串是否为一个三角函数表达式、一个对数表达式或一个根号表达式
        /// </summary>
        /// <param name="str"></param>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        bool istrigonometric(string str,int p,int q,ref string tr,int tail,ref OP[] Aop)
        {
            for (int i = tail; i >= 0; i--)
            {
                if (Aop[i].operation!=null && Aop[i].location >= p && Aop[i].location <= q && Aop[i].index <= 0)
                {
                    return false;
                }
            }
            if (p+3< q)
            {
                if(str.Substring(p,4)=="sin(" && str[q] == ')')
                {
                    tr = "sin";
                    return true;
                }
                if (str.Substring(p, 4) == "cos(" && str[q] == ')')
                {
                    tr = "cos";
                    return true;
                }
                if (str.Substring(p, 4) == "tan(" && str[q] == ')')
                {
                    tr = "tan";
                    return true;
                }
            }
            if (p + 2 < q)
            {
                if (str.Substring(p, 3) == "ln(" && str[q] == ')')
                {
                    tr = "ln";
                    return true;
                }
            }
            if (p + 4 < q)
            {
                if (str.Substring(p, 5) == "sqrt(" && str[q] == ')')
                {
                    tr = "sqrt";
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 定义二叉树结点类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class TreeNode
        {
            public union data;               //数据域
            public TreeNode lChild;   //左孩子   树中一个结点的子树的根结点称为这个结点的孩子
            public TreeNode rChild;   //右孩子

            public TreeNode(union val, TreeNode lp, TreeNode rp)
            {
                data = val;
                lChild = lp;
                rChild = rp;
            }

            public TreeNode()
            {
                data.Number = 0f;
                data.Operator = "";
                lChild = null;
                rChild = null;
            }

            public TreeNode LChild
            {
                get { return lChild; }
                set { lChild = value; }
            }

            public TreeNode RChild
            {
                get { return rChild; }
                set { rChild = value; }
            }
        }

        /// <summary>
        /// 根据算术表达式创建二叉树
        /// </summary>
        /// <param name="str"></param>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="tail"></param>
        void createTreenode(TreeNode node,string str,int p,int q,int tail,ref OP[] Aop)
        {
            //p,q分别标志Aop的首尾
            int i = 0,find=0;
            if (str[p] == '(' && str[q] == ')')
            {
                for (int j = tail; j >= 0; j--)
                {
                    if (Aop[j].operation!=null && Aop[j].location>=p && Aop[j].location <= q)
                    {
                        Aop[j].location--;
                        Aop[j].index--;
                    }
                }
                str = str.Substring(p + 1, q - p - 1);
                q = q - 2;
            }
            string tr = "";
            string substr = "";
            if (istrigonometric(str, p, q,ref tr,tail,ref Aop))
            {
                TreeNode subnode = new TreeNode();
                if(tr=="sin" || tr=="cos" || tr == "tan")
                {
                    substr = str.Substring(p + 4, q - p - 4);
                }
                if (tr == "ln")
                {
                    substr = str.Substring(p + 3, q - p - 3);
                }
                if (tr == "sqrt")
                {
                    substr = str.Substring(p + 5, q - p - 5);
                }
                for (int j = tail; i >= 0; i--)
                {
                    if (Aop[i].operation != null && Aop[i].location >= p && Aop[i].location <= q)
                    {
                        Aop[i].index = -1;
                    }
                }
                OP[] Bop = new OP[100];
                int tail1 = Sortop1(substr,ref Bop);
                createTreenode(subnode, substr, 0, substr.Length - 1, tail1,ref Bop);
                switch (tr)
                {
                    case "sin":
                        node.data.Number = Math.Sin(caculate(subnode));
                        break;
                    case "cos":
                        node.data.Number = Math.Cos(caculate(subnode));
                        break;
                    case "tan":
                        node.data.Number = Math.Tan(caculate(subnode));
                        break;
                    case "ln":
                        node.data.Number = Math.Log(caculate(subnode));
                        break;
                    case "sqrt":
                        node.data.Number = Math.Sqrt(caculate(subnode));
                        break;
                }                
                node.lChild = null;
                node.rChild = null;
                return;
            }
            if (isnum(str,p,q))//如果数据仅含运算数
            {
                //创建头节点，并将数据位置为str2float
                node.data.Number = str2float(str,p,q);
                node.lChild = null;
                node.rChild = null;
                return;
            }
            else if (isoperator(str,p,q))//如果数据仅含运算符
            {
                return;
            }
            else
                for (int j = 0; j <= index; j++)//按照括号一层一层地找
                {
                    for (i = tail; i >= 0; i--)//从后往前找，才符合运算的法则，前面先算后面后算
                    {
                        if (Aop[i].index == j && ((Aop[i].operation == "+") || (Aop[i].operation == "-")) && Aop[i].location >= p && Aop[i].location <= q)
                        {
                            find++;
                            Aop[i].index = -1;
                            node.lChild = new TreeNode();
                            node.rChild = new TreeNode();
                            node.data.Operator = Aop[i].operation;
                            createTreenode(node.lChild,str,p,Aop[i].location-1,tail,ref Aop);
                            createTreenode(node.rChild, str, Aop[i].location + 1,q, tail,ref Aop);
                        }
                    }
                    if (find == 0)
                        for (i = tail; i >= 0; i--)
                        {
                            if (Aop[i].index == j && ((Aop[i].operation == "*") || (Aop[i].operation == "/")) && Aop[i].location >= p && Aop[i].location <= q)
                            {
                                find++;
                                Aop[i].index = -1;
                                node.lChild = new TreeNode();
                                node.rChild = new TreeNode();
                                node.data.Operator = Aop[i].operation;
                                createTreenode(node.lChild, str, p, Aop[i].location - 1, tail,ref Aop);
                                createTreenode(node.rChild, str, Aop[i].location + 1, q, tail,ref Aop);
                            }
                        }
                    if (find == 0)
                        for (i = tail; i >= 0; i--)
                        {
                            if (Aop[i].index == j && (Aop[i].operation == "^") && Aop[i].location >= p && Aop[i].location <= q)
                            {
                                Aop[i].index = -1;
                                node.lChild = new TreeNode();
                                node.rChild = new TreeNode();
                                node.data.Operator = Aop[i].operation;
                                createTreenode(node.lChild, str, p, Aop[i].location - 1, tail,ref Aop);
                                createTreenode(node.rChild, str, Aop[i].location + 1, q, tail,ref Aop);
                            }
                        }
                }
        }

        /// <summary>
        /// 利用二叉树计算算式表达式的值
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public double caculate(TreeNode node)
        {
            double num1, num2;
            if(node.lChild==null && node.rChild == null)
            {
                return node.data.Number;
            }
            num1 = caculate(node.lChild);
            num2 = caculate(node.rChild);
            switch (node.data.Operator)
            {
                case "+":
                    return num1 + num2;
                case "-":
                    return num1 - num2;
                case "*":
                    return num1 * num2;
                case "/":
                    if (num2 != 0)
                    {
                        return num1 / num2;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                case "^":
                    return Math.Pow(num1, num2);
            }
            throw new ArgumentException();
        }

    }
 
}
