﻿

#pragma checksum "C:\Users\Anthony\documents\visual studio 2013\Projects\League of Legends Counterpicks\League of Legends Counterpicks\RolePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1965B032119C76540551489D56F28A4F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace League_of_Legends_Counterpicks
{
    partial class RolePage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 33 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.ItemView_ItemClick;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 405 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).GotFocus += this.TextBox_GotFocus;
                 #line default
                 #line hidden
                #line 407 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.Controls.TextBox)(target)).TextChanged += this.TextBox_TextChanged;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 78 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.JumpList_Loaded;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 82 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.ItemView_ItemClick;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

