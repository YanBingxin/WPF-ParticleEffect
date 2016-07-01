/*----------------------------------------------------------------
// Copyright© 2016 北京立思辰电子系统技术有限公司 
// 版权所有。 
//
// 文件名：WinBeforeRun
// 文件功能描述：
// 
// 创建标识：DengFei  2016/06/23 9:52:50
//
// 修改标识：
// 修改描述：
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hlwdsj.SY.View
{
    /// <summary>
    /// WinBeforeRun.xaml 的交互逻辑
    /// </summary>
    public partial class WinBeforeRun : Window
    {
        private MessageBoxResult _msgResult = MessageBoxResult.No;

        public MessageBoxResult MsgResult
        {
            get { return _msgResult; }
            set { _msgResult = value; }
        }
        public WinBeforeRun()
        {
            InitializeComponent();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            _msgResult = MessageBoxResult.No;
            //this.Close();
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            _msgResult = MessageBoxResult.Yes;
            this.Close();
        }
    }
}
