﻿#pragma checksum "..\..\VacationPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "6FE51BE405EEBC2BE7BE6F4131443B1CDB49AC9ED19AE9126BCD11F91DEBA045"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using HR_department;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace HR_department {
    
    
    /// <summary>
    /// VacationPage
    /// </summary>
    public partial class VacationPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 358 "..\..\VacationPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox StatusFilterComboBox;
        
        #line default
        #line hidden
        
        
        #line 359 "..\..\VacationPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ResetFiltersButton;
        
        #line default
        #line hidden
        
        
        #line 365 "..\..\VacationPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid VacationDataGrid;
        
        #line default
        #line hidden
        
        
        #line 405 "..\..\VacationPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddVacationButton;
        
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
            System.Uri resourceLocater = new System.Uri("/HR_department;component/vacationpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\VacationPage.xaml"
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
            this.StatusFilterComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.ResetFiltersButton = ((System.Windows.Controls.Button)(target));
            
            #line 359 "..\..\VacationPage.xaml"
            this.ResetFiltersButton.Click += new System.Windows.RoutedEventHandler(this.ResetFiltersButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.VacationDataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 4:
            this.AddVacationButton = ((System.Windows.Controls.Button)(target));
            
            #line 406 "..\..\VacationPage.xaml"
            this.AddVacationButton.Click += new System.Windows.RoutedEventHandler(this.AddVacationButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

