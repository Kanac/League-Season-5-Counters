﻿

#pragma checksum "C:\Users\Anthony C\Documents\Visual Studio 2015\Projects\League of Legends Counterpicks\League of Legends Counterpicks\ChampionPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B689FD72CB5CD83DDADB4BD2BCAA1DE0"
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
    partial class ChampionPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 179 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.CounterUpvote_Tapped;
                 #line default
                 #line hidden
                #line 180 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).DataContextChanged += this.CounterUpvote_DataLoaded;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 191 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.CounterDownvote_Tapped;
                 #line default
                 #line hidden
                #line 192 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).DataContextChanged += this.CounterDownvote_DataLoaded;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 127 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Comment_Flyout;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 130 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.FlyoutBase)(target)).Closed += this.Comment_FlyoutClosed;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 167 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Submit_CounterComment;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 85 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).DataContextChanged += this.CounterCommentUpvote_DataLoaded;
                 #line default
                 #line hidden
                #line 86 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.CounterCommentUpvote_Tapped;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 97 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).DataContextChanged += this.CounterCommentDownvote_DataLoaded;
                 #line default
                 #line hidden
                #line 98 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.CounterCommentDownvote_Tapped;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 38 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).DataContextChanged += this.Upvote_DataLoaded;
                 #line default
                 #line hidden
                #line 39 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Upvote_Tapped;
                 #line default
                 #line hidden
                break;
            case 9:
                #line 50 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).DataContextChanged += this.Downvote_DataLoaded;
                 #line default
                 #line hidden
                #line 51 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Downvote_Tapped;
                 #line default
                 #line hidden
                break;
            case 10:
                #line 335 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.StatPage_Navigate;
                 #line default
                 #line hidden
                break;
            case 11:
                #line 304 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).GotFocus += this.Filter_GotFocus;
                 #line default
                 #line hidden
                #line 306 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.Controls.TextBox)(target)).TextChanged += this.Filter_TextChanged;
                 #line default
                 #line hidden
                break;
            case 12:
                #line 315 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.Filter_Click;
                 #line default
                 #line hidden
                break;
            case 13:
                #line 328 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Submit_Counter;
                 #line default
                 #line hidden
                break;
            case 14:
                #line 243 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).GotFocus += this.Name_Focus;
                 #line default
                 #line hidden
                break;
            case 15:
                #line 257 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Send_Feedback;
                 #line default
                 #line hidden
                break;
            case 16:
                #line 342 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.GridAd_Loaded;
                 #line default
                 #line hidden
                break;
            case 17:
                #line 591 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.GridAd_Loaded;
                 #line default
                 #line hidden
                break;
            case 18:
                #line 569 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.GridAd_Loaded;
                 #line default
                 #line hidden
                break;
            case 19:
                #line 516 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.GridAd_Loaded;
                 #line default
                 #line hidden
                break;
            case 20:
                #line 547 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.SynergyChamp_Tapped;
                 #line default
                 #line hidden
                #line 548 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).DataContextChanged += this.Synergy_Loaded;
                 #line default
                 #line hidden
                break;
            case 21:
                #line 469 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.GridAd_Loaded;
                 #line default
                 #line hidden
                break;
            case 22:
                #line 492 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.EasyMatchup_Tapped;
                 #line default
                 #line hidden
                break;
            case 23:
                #line 423 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.GridAd_Loaded;
                 #line default
                 #line hidden
                break;
            case 24:
                #line 446 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Champ_Tapped;
                 #line default
                 #line hidden
                break;
            case 25:
                #line 410 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 410 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 26:
                #line 384 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 384 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 27:
                #line 385 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 385 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 28:
                #line 386 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 386 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 29:
                #line 387 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 387 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 30:
                #line 388 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 388 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 31:
                #line 389 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 389 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 32:
                #line 390 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 390 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 33:
                #line 391 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 391 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 34:
                #line 392 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 392 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 35:
                #line 393 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 393 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 36:
                #line 394 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 394 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 37:
                #line 395 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 395 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 38:
                #line 396 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 396 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 39:
                #line 397 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 397 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 40:
                #line 398 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 398 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 41:
                #line 399 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 399 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 42:
                #line 400 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 400 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 43:
                #line 401 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 401 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 44:
                #line 402 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 402 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 45:
                #line 403 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 403 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 46:
                #line 404 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 404 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 47:
                #line 405 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 405 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 48:
                #line 406 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 406 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            case 49:
                #line 407 "..\..\..\ChampionPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.Ad_Loaded;
                 #line default
                 #line hidden
                #line 407 "..\..\..\ChampionPage.xaml"
                ((global::Microsoft.Advertising.WinRT.UI.AdControl)(target)).ErrorOccurred += this.Ad_Error;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


