/*
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    |              ENCRYPTOR              |
 *    |          Main Window Class          |
 *    |                                     |
 *    |    Copyright (c) MOlex-dev, 2022    |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 */

using System.Windows;
using Encryptor.ViewModel;

namespace Encryptor.View;

/// <summary>
/// Class <c>MainWindow</c> describes the behavior of the main window of the ENCRYPTOR software
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(this);
    }
}
