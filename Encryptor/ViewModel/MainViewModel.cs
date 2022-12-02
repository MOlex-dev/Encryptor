/*
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 *    |              ENCRYPTOR              |
 *    |           Main View Model           |
 *    |                                     |
 *    |    Copyright (c) MOlex-dev, 2022    |
 *    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
 */

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using Encryptor.Model;
using Microsoft.Win32;

namespace Encryptor.ViewModel;

/// <summary>
/// Class <c>MainViewModel</c> describes the ViewModel of the Main form, representing user's interaction with software
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    private readonly Window _window;
    public event PropertyChangedEventHandler? PropertyChanged;
    
    // Information for MainWindow
    private Preprocessor.PreprocessorSettings _preprocessorSettings;
    private EncryptionAlgorithm _algorithm;
    private readonly ObservableCollection<string> _listOfAlgorithms;
    private int _selectedIndex;

    private string? _defaultText;
    private string? _processedText;
    private bool _processedTextSaved = true;

    private RelayCommand? _infoCommand;
    private RelayCommand? _cleanCommand;
    private RelayCommand? _exportCommand;
    private RelayCommand? _startCommand;
    private RelayCommand? _openCommand;
    
    // Application version for InfoWindow
    private readonly string _appVersion;
    
    
    public MainViewModel(Window window)
    {
        _window = window;
        _preprocessorSettings = new Preprocessor.PreprocessorSettings();
        
        // Set AppVersion
        var version = Assembly.GetExecutingAssembly()
                              .GetName()
                              .Version;
        AppVersion = $"{version!.Major}.{version.Minor}";
        
        // Algorithms
        ListOfAlgorithms = new ObservableCollection<string>()
            { "Caesar Cipher (shift 1)", "Caesar Cipher (progressive)", "Scytale Cipher (6 sides)" };
        SelectedIndex = 0;
        OnPropertyChanged(nameof(SelectedIndex));
    }
    
    
    private void OnPropertyChanged([CallerMemberName]string prop = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

    // MainWindow
    
    /// <summary>
    /// Collection of encryption algorithms names
    /// </summary>
    public ObservableCollection<string> ListOfAlgorithms
    {
        get => _listOfAlgorithms;
        init
        {
            _listOfAlgorithms = value;
            OnPropertyChanged(nameof(ListOfAlgorithms));
        }
    }

    /// <summary>
    /// Selected encryption algorithm
    /// </summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            OnPropertyChanged(nameof(SelectedIndex));
        }
    }
    
    /// <summary>
    /// Command, representing Info button click
    /// </summary>
    public RelayCommand InfoCommand
    {
        get
        {
            return _infoCommand ??= new RelayCommand(obj =>
            {
                Window infoWindow = new View.InfoWindow();
                infoWindow.Owner = _window;
                infoWindow.ShowDialog();
            });
        }
    }
    
    /// <summary>
    /// Command, representing Exit button click
    /// </summary>
    public RelayCommand ExitCommand
    {
        get
        {
            return new RelayCommand(obj =>
            {
                CloseApplication();
            });
        }
    }

    /// <summary>
    /// Command, representing Clean button click
    /// </summary>
    public RelayCommand CleanTextCommand
    {
        get
        {
            return _cleanCommand ??= new RelayCommand(obj =>
            {
                CleanTextBoxes();
            }); 
        }
    }

    /// <summary>
    /// Command, representing Export button click
    /// </summary>
    public RelayCommand ExportCommand
    {
        get
        {
            return _exportCommand ??= new RelayCommand(obj =>
            {
                if (String.IsNullOrEmpty(ProcessedText))
                {
                    MessageBox.Show("Processed text is empty!", "Warning",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                try
                {
                    SaveEncryptedMessage();
                }
                catch (Exception e)
                {
                    ShowThrewException(e);
                }
            }); 
        }
    }
    
    /// <summary>
    /// Command, representing Open button click
    /// </summary>
    public RelayCommand OpenCommand
    {
        get
        {
            return _openCommand ??= new RelayCommand(obj =>
            {
                string tmp = OpenDefaultMessage();
                if (tmp == string.Empty) return;
                
                CleanTextBoxes();
                DefaultText = tmp;
            }); 
        }
    }

    /// <summary>
    /// Command, representing Start button click
    /// </summary>
    public RelayCommand StartCommand
    {
        get
        {
            return _startCommand ??= new RelayCommand(obj =>
            {
                if (String.IsNullOrEmpty(DefaultText))
                {
                    
                    MessageBox.Show("Your message is empty!", "Warning", MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                    if (!String.IsNullOrEmpty(ProcessedText)) ProcessedText = string.Empty;
                    return;
                }
                
                _algorithm = CreateConcreteAlgorithm(ListOfAlgorithms[SelectedIndex]);
                
                ProcessedText = _algorithm.Process(Preprocessor.ProcessMessage(DefaultText, _preprocessorSettings));
            }); 
        }
    }

    /// <summary>
    /// String, representing input text
    /// </summary>
    public string DefaultText
    {
        get => _defaultText;
        set
        {
            _defaultText = value;
            OnPropertyChanged(nameof(DefaultText));
        }
    }

    /// <summary>
    /// String, representing encrypted text
    /// </summary>
    public string ProcessedText
    {
        get => _processedText;
        set
        {
            _processedText = value;
            _processedTextSaved = _processedText == string.Empty;
            OnPropertyChanged(nameof(ProcessedText));
        }
    }

    /// <summary>
    /// Remove Whitespaces checkbox
    /// </summary>
    public bool RemoveWhitespaces
    {
        get => _preprocessorSettings.RemoveWhitespaces;
        set
        {
            _preprocessorSettings.RemoveWhitespaces = value;
            OnPropertyChanged(nameof(RemoveWhitespaces));
        }
    }
    
    /// <summary>
    /// Remove Punctuation checkbox
    /// </summary>
    public bool RemovePunctuation
    {
        get => _preprocessorSettings.RemovePunctuation;
        set
        {
            _preprocessorSettings.RemovePunctuation = value;
            OnPropertyChanged(nameof(RemovePunctuation));
        }
    }
    
    /// <summary>
    /// Remove Symbols checkbox
    /// </summary>
    public bool RemoveSymbols
    {
        get => _preprocessorSettings.RemoveSymbols;
        set
        {
            _preprocessorSettings.RemoveSymbols = value;
            OnPropertyChanged(nameof(RemoveSymbols));
        }
    }
    
    /// <summary>
    /// Case -> Not change radiobutton 
    /// </summary>
    public bool CaseNotChange
    {
        get => _preprocessorSettings.ChangeCase == Preprocessor.CaseChanging.NotChange;
        set { if (value == true) _preprocessorSettings.ChangeCase = Preprocessor.CaseChanging.NotChange; }
    }
    
    /// <summary>
    /// Case -> To Lower radiobutton 
    /// </summary>
    public bool CaseToLower
    {
        get => _preprocessorSettings.ChangeCase == Preprocessor.CaseChanging.ToLower;
        set { if (value == true) _preprocessorSettings.ChangeCase = Preprocessor.CaseChanging.ToLower; }
    }
    
    /// <summary>
    /// Case -> To Upper radiobutton
    /// </summary>
    public bool CaseToUpper
    {
        get => _preprocessorSettings.ChangeCase == Preprocessor.CaseChanging.ToUpper;
        set { if (value == true) _preprocessorSettings.ChangeCase = Preprocessor.CaseChanging.ToUpper; }
    }

    
    private void CloseApplication()
    {
        int exitCode = 0;

        if (!_processedTextSaved)
        {
            if (AskForSave() == true)
            {
                try
                {
                    SaveEncryptedMessage();
                }
                catch (Exception e)
                {
                    ShowThrewException(e);
                    exitCode = 1;
                }
            }
        }
        Environment.Exit(exitCode);
    }

    private bool AskForSave()
    {
        var result = MessageBox.Show("Encrypted message isn't saved. Save it?", "Save result?",
                                     MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result switch
        {
            MessageBoxResult.Yes => true,
            MessageBoxResult.No  => false,
            _ => false
        };
    }

    private async void SaveEncryptedMessage()
    {
        SaveFileDialog saveDialog = new SaveFileDialog
        {
            Filter = "Text files (*.txt)|*.txt"
        };

        if (saveDialog.ShowDialog() != true) return;

        try
        {
            using (var stream = saveDialog.OpenFile())
            {
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(ProcessedText);
                }
            }
            _processedTextSaved = true;
        }
        catch (Exception e)
        {
            ShowThrewException(e);
        }
    }

    private void CleanTextBoxes()
    {
        if (!_processedTextSaved)
        {
            if (AskForSave() == true)
            {
                try
                {
                    SaveEncryptedMessage();
                }
                catch (Exception e)
                {
                    ShowThrewException(e);
                }
            }
        }
        DefaultText = string.Empty;
        ProcessedText = string.Empty;
    }

    private string OpenDefaultMessage()
    {
        OpenFileDialog openDialog = new OpenFileDialog()
        {
            Filter = "Text files (*.txt)|*.txt"
        };
        
        if (openDialog.ShowDialog() != true) return string.Empty;
        
        try
        {
            using (var stream = openDialog.OpenFile())
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        catch (Exception e)
        {
            ShowThrewException(e);
            return String.Empty;
        }
    }

    private EncryptionAlgorithm? CreateConcreteAlgorithm(string selectedAlgorithm) =>
        selectedAlgorithm switch
        {
            "Caesar Cipher (shift 1)"     => new CaesarCipher(1),
            "Caesar Cipher (progressive)" => new CaesarCipher(0),
            "Scytale Cipher (6 sides)"    => new ScytaleCipher(6),
            _ => null
        };
    
    private void ShowThrewException(Exception except) =>
        MessageBox.Show(except.Message, "Exception threw!", MessageBoxButton.OK, MessageBoxImage.Error);
    
    
    
    // InfoWindow

    /// <summary>
    /// String representation of assembly version in <c>Major.Minor</c> format
    /// </summary>
    public string AppVersion
    {
        get => _appVersion;
        init
        {
            _appVersion = value;
            OnPropertyChanged(nameof(AppVersion));
        }
    }
}
