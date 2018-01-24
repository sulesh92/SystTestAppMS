using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace TestAppSysTech
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// Обработчик кнопки бокового меню. 
        /// Изменяет состояние панели на обратное (Открыто, Скрыто)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private void MainPageListBox_Click(object sender, SelectionChangedEventArgs e)
        {
            //if (PostingPageButton.IsSelected)
            //{
            //    CurrentContentFrame.Navigate(typeof(PostingPage));
            //    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed; //кнопка назад скрыта
            //}
            if (UserEditPageButton.IsSelected)
            //{
                CurrentContentFrame.Navigate(typeof(UserEditPage));
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible; //кнопка назад доступна
            //}
            //if (MyChannelsPageButton.IsSelected)
            //{
            //    CurrentContentFrame.Navigate(typeof(MyChannelsPage));
            //    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible; //кнопка назад доступна
            //}
        }
    }
}

