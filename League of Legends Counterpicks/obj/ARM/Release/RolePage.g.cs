﻿

#pragma checksum "C:\Users\Anthony C\Documents\Visual Studio 2015\Projects\League of Legends Counterpicks\League of Legends Counterpicks\RolePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "63CD3CC6C8D6708252CED7BC00349D36"
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
                #line 36 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.ItemView_ItemClick;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 45 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.ChampImage_Tapped;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 66 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListPickerFlyout)(target)).ItemsPicked += this.Role_Picked;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 104 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).GotFocus += this.FilterBox_GotFocus;
                 #line default
                 #line hidden
                #line 105 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).LostFocus += this.FilterBox_LostFocus;
                 #line default
                 #line hidden
                #line 107 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.Controls.TextBox)(target)).TextChanged += this.FilterBox_TextChanged;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 123 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.FilterGridView_Loaded;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 132 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.JumpList_Loaded;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 136 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.ItemView_ItemClick;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 145 "..\..\..\RolePage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.ChampImage_Tapped;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


