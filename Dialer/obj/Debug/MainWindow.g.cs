﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "8AB4C89AEE52E02857A5B41CA8846C0C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Dialer {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Dialer.MainWindow win_mgt;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cb_ignoreheaders;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tb_csvfile;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_import;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dg_import;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dg_arrangement;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_listName;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_listDesc;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_save;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_reset;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_cancel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Dialer;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.win_mgt = ((Dialer.MainWindow)(target));
            return;
            case 2:
            
            #line 12 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.TabItem)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.onTabFocus);
            
            #line default
            #line hidden
            return;
            case 3:
            this.cb_ignoreheaders = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 4:
            this.tb_csvfile = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.btn_import = ((System.Windows.Controls.Button)(target));
            
            #line 34 "..\..\MainWindow.xaml"
            this.btn_import.Click += new System.Windows.RoutedEventHandler(this.importBtn_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.dg_import = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            this.dg_arrangement = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 8:
            this.txt_listName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.txt_listDesc = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.btn_save = ((System.Windows.Controls.Button)(target));
            return;
            case 11:
            this.btn_reset = ((System.Windows.Controls.Button)(target));
            return;
            case 12:
            this.btn_cancel = ((System.Windows.Controls.Button)(target));
            return;
            case 13:
            
            #line 91 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.TabItem)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.listdatatab_focus);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 110 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.TabItem)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.onTabFocus);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 115 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.TabItem)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.onTabFocus);
            
            #line default
            #line hidden
            return;
            case 16:
            
            #line 120 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.TabItem)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.onTabFocus);
            
            #line default
            #line hidden
            return;
            case 17:
            
            #line 125 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.TabItem)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.onTabFocus);
            
            #line default
            #line hidden
            return;
            case 18:
            
            #line 130 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.TabItem)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.onTabFocus);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

