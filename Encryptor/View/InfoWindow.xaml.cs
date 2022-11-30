/*
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    |              ENCRYPTOR              |
 *    |      Information Window Class       |
 *    |                                     |
 *    |    Copyright (c) MOlex-dev, 2022    |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 */

using System.Windows;
using Encryptor.ViewModel;

namespace Encryptor.View;

/// <summary>
/// Class <c>InfoWindow</c> describes a window with information about the ENCRYPTOR software
/// </summary>
public partial class InfoWindow : Window
{
    public InfoWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(this);
    }
}
