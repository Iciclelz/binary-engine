using Binary_Engine.HexBox;
using Binary_Engine.Utilities;
using Binary_Engine.ViewModel;
using Binary_Engine.Windows;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace Binary_Engine.Views
{
    /// <summary>
    /// Interaction logic for BinaryEngineView.xaml
    /// </summary>
    public partial class BinaryEngineView
    {
        private BinaryEngineViewModel ViewModel = new BinaryEngineViewModel();
        private FindOptions ByteViewFindOptions = new FindOptions();
        private int SignatureSearchResult = 0;
        private ContextMenu ByteViewerContextMenu = null;
        public BinaryEngineView()
        {
            InitializeComponent();

            ByteViewer.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            ByteViewer.BackColor = System.Drawing.ColorTranslator.FromHtml("#2D2D2D");

            InitializeByteViewerContextMenu();

            DataContext = ViewModel;
        }

        public void InitializeByteViewerContextMenu()
        {
            Func<byte[]> GetSelectedBytes = () =>
            {
                byte[] buffer = new byte[ByteViewer.SelectionLength];
                for (long i = ByteViewer.SelectionStart, j = 0; i < ByteViewer.SelectionStart + ByteViewer.SelectionLength; i++, j++)
                {
                    buffer[j] = ByteViewer.ByteProvider.ReadByte(i);
                }

                return buffer;
            };

            Func<string, RoutedEventHandler, MenuItem> NewMenuItem = (menuItemHeader, clickEvent) =>
            {
                MenuItem menuItem = new MenuItem { Header = menuItemHeader };
                menuItem.Click += clickEvent;
                return menuItem;
            };

            if (ByteViewerContextMenu == null)
            {
                ByteViewerContextMenu = new ContextMenu();

                /* COPY SUBMENU BEGIN */

                MenuItem copyMenuItem = new MenuItem { Header = "Copy" };

                copyMenuItem.Items.Add(NewMenuItem("Decimal Bytes", (sender, e) =>
                {
                    byte[] buffer = GetSelectedBytes();
                    if (buffer.Length > 0)
                    {
                        Clipboard.SetText(new BytesConverter(buffer).ToDecimalString());
                    }
                }));

                copyMenuItem.Items.Add(new Separator());

                copyMenuItem.Items.Add(NewMenuItem("Hexadecimal Bytes", (sender, e) =>
                {
                    //ok
                    byte[] buffer = GetSelectedBytes();
                    if (buffer.Length > 0)
                    {
                        Clipboard.SetText(new BytesConverter(buffer).ToHexadecimalString(String.Empty));
                    }
                }));

                copyMenuItem.Items.Add(NewMenuItem("Hexadecimal Bytes (\\x00)", (sender, e) =>
                {
                    //ok
                    byte[] buffer = GetSelectedBytes();
                    if (buffer.Length > 0)
                    {
                        Clipboard.SetText(new BytesConverter(buffer).ToHexadecimalString("\\x"));
                    }
                }));

                copyMenuItem.Items.Add(NewMenuItem("Hexadecimal Bytes (0x00)", (sender, e) =>
                {
                    //ok
                    byte[] buffer = GetSelectedBytes();
                    if (buffer.Length > 0)
                    {
                        Clipboard.SetText(new BytesConverter(buffer).ToHexadecimalString("0x"));
                    }
                }));

                copyMenuItem.Items.Add(NewMenuItem("Hexadecimal Bytes (Formatted)", (sender, e) =>
                {
                    ByteViewer.CopyHex();
                }));

                copyMenuItem.Items.Add(new Separator());

                copyMenuItem.Items.Add(NewMenuItem("Hexadecimal Words (2 Bytes)", (sender, e) =>
                {
                    byte[] buffer = GetSelectedBytes();
                    if (buffer.Length > 0 && buffer.Length % 2 == 0)
                    {
                        Clipboard.SetText(new BytesConverter(buffer).ToHexadecimalString(2));
                    }
                }));

                copyMenuItem.Items.Add(NewMenuItem("Hexadecimal Dwords (4 Bytes)", (sender, e) =>
                {
                    //ok
                    byte[] buffer = GetSelectedBytes();
                    if (buffer.Length > 0 && buffer.Length % 4 == 0)
                    {
                        Clipboard.SetText(new BytesConverter(buffer).ToHexadecimalString(4));
                    }
                }));

                copyMenuItem.Items.Add(NewMenuItem("Hexadecimal Qwords (8 Bytes)", (sender, e) =>
                {
                    //ok
                    byte[] buffer = GetSelectedBytes();
                    if (buffer.Length > 0 && buffer.Length % 8 == 0)
                    {
                        Clipboard.SetText(new BytesConverter(buffer).ToHexadecimalString(8));
                    }
                }));

                copyMenuItem.Items.Add(new Separator());

                copyMenuItem.Items.Add(NewMenuItem("String (Ascii)", (sender, e) =>
                {
                    //ok
                    byte[] buffer = GetSelectedBytes();
                    if (buffer.Length > 0)
                    {
                        Clipboard.SetText(Encoding.ASCII.GetString(buffer));
                    }
                }));

                copyMenuItem.Items.Add(NewMenuItem("String (Utf-8)", (sender, e) =>
                {
                    //ok
                    byte[] buffer = GetSelectedBytes();
                    if (buffer.Length > 0)
                    {
                        Clipboard.SetText(Encoding.UTF8.GetString(buffer));
                    }
                }));

                copyMenuItem.Items.Add(new Separator());

                copyMenuItem.Items.Add(NewMenuItem("Address", (sender, e) =>
                {
                    string address = ByteViewer.SelectionStart.ToString("X8");
                    if (ByteViewer.SelectionLength > 1)
                    {
                        address += "-" + (ByteViewer.SelectionStart + ByteViewer.SelectionLength).ToString("X8");
                    }

                    Clipboard.SetText(address);
                }));


                /* COPY SUBMENU END */

                ByteViewerContextMenu.Items.Add(copyMenuItem);

                ByteViewerContextMenu.Items.Add(NewMenuItem("Paste", (sender, e) =>
                {
                    ByteViewer.PasteHex();
                }));

                ByteViewerContextMenu.Items.Add(new Separator());

                ByteViewerContextMenu.Items.Add(NewMenuItem("Insert Hexadecimal Bytes...", (sender, e) =>
                {
                    long index = ByteViewer.SelectionStart;
                    if (index >= 0)
                    {
                        string bytes = InputWindow.InputBox("Hexadecimal Bytes: ", "Binary Engine: Insert Hexadecimal Bytes");
                        if (bytes.Length > 0 && !String.IsNullOrEmpty(bytes))
                        {
                            ByteViewer.ByteProvider.InsertBytes(index, BytesConverter.StringToBytes(bytes));
                            ByteViewer.Refresh();
                        }
                    }
                }));

                ByteViewerContextMenu.Items.Add(NewMenuItem("Insert String...", (sender, e) => 
                {
                    long index = ByteViewer.SelectionStart;
                    if (index >= 0)
                    {
                        string data = InputWindow.InputBox("String: ", "Binary Engine: Insert String");
                        if (data.Length > 0 && !String.IsNullOrEmpty(data))
                        {
                            ByteViewer.ByteProvider.InsertBytes(index, Encoding.ASCII.GetBytes(data));
                            ByteViewer.Refresh();
                        }
                    }
                }));

                ByteViewerContextMenu.Items.Add(new Separator());

                ByteViewerContextMenu.Items.Add(NewMenuItem("Separator Group Size", (sender, e) => 
                {
                    string m = InputWindow.InputBox("Size: ", "Binary Engine: Separator Group Size", ByteViewer.GroupSize.ToString());
                    if (int.TryParse(m, out int n))
                    {
                        if (n % 2 == 0)
                        {
                            ByteViewer.GroupSize = n;
                        }
                    }
                }));
                
                ByteViewerContextMenu.Items.Add(new Separator());

                ByteViewerContextMenu.Items.Add(NewMenuItem("Select All", (sender, e) => { ByteViewer.SelectAll(); }));
            }
            
        }

        public Func<long, string> GetDisplayBytes = (size) =>
        {
            Func<long, long, string> ConvertToOneDigit = (size_, q) =>
            {
                return ((double)size_ / q).ToString("0.#", CultureInfo.CurrentCulture);
            };

            string BytesDisplay = size.ToString("###,###,###,###,###", CultureInfo.CurrentCulture);

            long kb = 1024;
            long mb = kb * 1024;
            long gb = mb * 1024;
            long tb = gb * 1024;
            if (size < kb)
            {
                return $"{size} BYTES";
            }
            else if (size < mb)
            {
                return $"{ConvertToOneDigit(size, kb)} KB ({BytesDisplay} BYTES)";
            }
            else if (size < gb)
            {
                return $"{ConvertToOneDigit(size, mb)} MB ({BytesDisplay} BYTES)";
            }
            else if (size < tb)
            {
                return $"{ConvertToOneDigit(size, gb)} GB ({BytesDisplay} BYTES)";
            }
            else
            {
                return $"{ConvertToOneDigit(size, tb)} TB ({BytesDisplay} BYTES)";
            }
        };


        public bool SetByteView(string filepath)
        {
            if (!File.Exists(filepath))
            {
                return false;
            }

            try
            {
                using (new FileStream(filepath, FileMode.Open))
                {

                }
            }
            catch
            {
                return false;
            }

            ViewModel.FilePath = filepath;

            foreach (KeyValuePair<TearableTabItem, BinaryEngineView> p in MainWindowViewModel.Instance.BinaryEngineViewDictionary)
            {
                if (p.Value == this)
                {
                    p.Key.Header = Path.GetFileName(ViewModel.FilePath);

                    break;
                }
            }

            //ByteViewer

            DynamicFileByteProvider byteProvider = new DynamicFileByteProvider(ViewModel.FilePath);
            try
            {
                byteProvider.LengthChanged += (sender, e) =>
                {
                    ByteSizeStatusBarItem.Content = GetDisplayBytes(ByteViewer.ByteProvider.Length);
                };

            }
            catch
            {

            }

            if (ByteViewer.ByteProvider != null)
            {
                (ByteViewer.ByteProvider as DynamicFileByteProvider).Dispose();
            }

            ByteViewer.ByteProvider = byteProvider;
            ByteViewer.ContextMenuStrip = null;

            ByteViewer.CurrentLineChanged += new EventHandler((sender, e) =>
            {
                OnUpdatePosition();
            });
            ByteViewer.CurrentPositionInLineChanged += new EventHandler((sender, e) =>
            {
                OnUpdatePosition();
            });

            ByteSizeStatusBarItem.Content = GetDisplayBytes(ByteViewer.ByteProvider.Length);


            return true;
        }

        private void OnUpdatePosition()
        {
            string content = $"Ln {ByteViewer.CurrentLine}\tCol {ByteViewer.CurrentPositionInLine}";

            if (ByteViewer.ByteProvider != null && ByteViewer.ByteProvider.Length > ByteViewer.SelectionStart)
            {
                content += $"\tAddress: {ByteViewer.SelectionStart.ToString("X8")}";
                byte current = ByteViewer.ByteProvider.ReadByte(ByteViewer.SelectionStart);
                content += $"\tBits of Byte {ByteViewer.SelectionStart} ({current.ToString()} || {"0x" + current.ToString("X2")}): {BytesConverter.ToBits(current)}";
            }

            content += $"\t{GetDisplayBytes(ByteViewer.ByteProvider.Length)}";

            ByteSizeStatusBarItem.Content = content;
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "All Files (*.*)|*.*" };
            if (openFileDialog.ShowDialog() == true)
            {
                if (!String.IsNullOrEmpty(openFileDialog.FileName) && File.Exists(openFileDialog.FileName))
                {
                    SetByteView(openFileDialog.FileName);
                }
            }
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ByteViewer.ByteProvider == null || !ByteViewer.ByteProvider.HasChanges())
            {
                return;
            }

            if (!String.IsNullOrEmpty(ViewModel.FilePath) && File.Exists(ViewModel.FilePath))
            {
                if (MessageBox.Show($"You are writing to \"{ViewModel.FilePath}\".\n\nDo you want to create a backup of the original file?", "Binary Engine", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    string backup;
                    do
                    {
                        backup = Path.Combine(Path.GetDirectoryName(ViewModel.FilePath), Path.GetFileNameWithoutExtension(ViewModel.FilePath) + "." + new BytesConverter(new SHA1CryptoServiceProvider().ComputeHash(BitConverter.GetBytes(DateTime.Now.ToBinary()))).ToHexadecimalString(String.Empty).ToLower() + Path.GetExtension(ViewModel.FilePath) + ".bak");
                    } while (File.Exists(backup));

                    File.Copy(ViewModel.FilePath, backup, true);
                }


                (ByteViewer.ByteProvider as DynamicFileByteProvider).ApplyChanges();
            }
        }

        private void CutCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ByteViewer.CopyHex();
            ByteViewer.ByteProvider.DeleteBytes(ByteViewer.SelectionStart, ByteViewer.SelectionLength);
        }

        private void CopyCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ByteViewer.CopyHex();
        }

        private void PasteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ByteViewer.PasteHex();
        }

        private void FindCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchGroupBox.Visibility = Visibility.Visible;
            FindAllReferencesGroupBox.Visibility = Visibility.Collapsed;
            ByteViewerHost.Margin = new Thickness(0, 20, 0, 105);
        }

        private void GoToAddressCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ByteViewer.ByteProvider == null)
            {
                return;
            }

            Func<char, bool> isxdigit = (c) =>
            {
                return
                    '0' <= c && c <= '9' ||
                    'a' <= c && c <= 'f' ||
                    'A' <= c && c <= 'F';
            };

            /*
            MainWindow mainwindow = (Window.GetWindow(this) as MainWindow);
            mainwindow.TearableTabControl.Visibility = Visibility.Collapsed;
            await mainwindow.ShowInputAsync("Binary Engine - Go to Address", "Address (Byte Index):");
            mainwindow.TearableTabControl.Visibility = Visibility.Visible;
            */
            
            string address = InputWindow.InputBox("Address (Byte Index):", "Binary Engine: Go to Address");
            if (string.IsNullOrEmpty(address))
            {
                return;
            }

            for (int i = 0; i < address.Length; ++i)
            {
                if (!isxdigit(address[i]))
                {
                    return;
                }
            }

            long selectionStart = Convert.ToInt64(address, 16);

            if (selectionStart >= 0 && selectionStart < ByteViewer.ByteProvider.Length)
            {
                ByteViewer.SelectionStart = selectionStart;
                ByteViewer.SelectionLength = 1;
                ByteViewer.Focus();
            }
        }


        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenCommandBinding_Executed(this, null);
        }

        private void SaveFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveCommandBinding_Executed(this, null);
        }

        private void SaveAsFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Save"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                if (!String.IsNullOrEmpty(saveFileDialog.FileName))
                {
                    List<byte> file = new List<byte>();
                    for (long i = 0; i < ByteViewer.ByteProvider.Length; ++i)
                    {
                        file.Add(ByteViewer.ByteProvider.ReadByte(i));
                    }

                    File.WriteAllBytes(saveFileDialog.FileName, file.ToArray());



                    SetByteView(saveFileDialog.FileName);
                }
            }
        }

        private void CutTextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ByteViewer.CanCut())
            {
                ByteViewer.Cut();
            }
        }

        private void CopyTextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ByteViewer.CanCopy())
            {
                ByteViewer.Copy();
            }
        }

        private void PasteTextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ByteViewer.CanPaste())
            {
                ByteViewer.Paste();
            }
        }

        private void CutBytesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CutCommandBinding_Executed(this, null);
        }

        private void CopyBytesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CopyCommandBinding_Executed(this, null);
        }

        private void PasteBytesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CutCommandBinding_Executed(this, null);
        }

        private void FindMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FindCommandBinding_Executed(this, null);
        }

        private void GoToAddressMenuItem_Click(object sender, RoutedEventArgs e)
        {
            GoToAddressCommandBinding_Executed(this, null);
        }

        private void SelectAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ByteViewer.SelectAll();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }


        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    FindNextButton_Click(this, e);
                }
                catch
                {
                    FindButton_Click(this, e);
                }
            }
        }

        private void SearchTextRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.SearchOption = "Match Case";
        }

        private void SearchBytesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.SearchOption = "Wildcards (?)";
        }

        private void SearchCloseButton_Click(object sender, RoutedEventArgs e)
        {
            SearchGroupBox.Visibility = Visibility.Collapsed;
            FindAllReferencesGroupBox.Visibility = Visibility.Collapsed;
            ByteViewerHost.Margin = new Thickness(0, 20, 0, 20);
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(SearchTextBox.Text) || ByteViewer.ByteProvider == null)
            {
                return;
            }

            if (SearchBytesRadioButton.IsChecked == true && SearchOptionCheckBox.IsChecked == true)
            {
                SignatureSearchResult = 1;
                SignatureScan scan = new SignatureScan(SearchTextBox.Text, ByteViewer.ByteProvider, SignatureSearchResult);
                long address = scan.Address();
                if (address >= 0)
                {
                    ByteViewer.SelectionStart = address;
                    ByteViewer.SelectionLength = scan.PatternSize;
                }
                else
                {
                    MessageBox.Show("The following specified hexadecimal bytes was not found: \n\n" + SearchTextBox.Text, "Binary Engine", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                return;
            }
            else
            {
                try
                {
                    ByteViewer.SelectionStart = 0;
                    ByteViewFindOptions.MatchCase = (bool)SearchOptionCheckBox.IsChecked;
                    ByteViewFindOptions.Type = (bool)SearchBytesRadioButton.IsChecked ? FindType.Hex : FindType.Text;

                    if (ByteViewFindOptions.Type == FindType.Hex)
                    {
                        ByteViewFindOptions.Hex = BytesConverter.StringToBytes(SearchTextBox.Text);
                    }
                    else
                    {
                        ByteViewFindOptions.Text = SearchTextBox.Text;
                    }

                    ByteViewFindOptions.IsValid = true;

                }
                catch
                {

                }
            }

            FindNextButton_Click(this, e);

        }

        private void FindNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(SearchTextBox.Text) || ByteViewer.ByteProvider == null)
            {
                return;
            }

            if (SearchBytesRadioButton.IsChecked == true && SearchOptionCheckBox.IsChecked == true)
            {
                SignatureSearchResult++;

                SignatureScan scan = new SignatureScan(SearchTextBox.Text, ByteViewer.ByteProvider, SignatureSearchResult);
                long address = scan.Address();
                if (address >= 0)
                {
                    ByteViewer.SelectionStart = address;
                    ByteViewer.SelectionLength = scan.PatternSize;
                }
                else
                {
                    MessageBox.Show("The following specified hexadecimal bytes was not found: \n\n" + SearchTextBox.Text, "Binary Engine", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            try
            {
                const long NO_MATCH = -1;
                const long OPERATION_ABORTED = -2;

                switch (ByteViewer.Find(ByteViewFindOptions))
                {
                    case NO_MATCH:
                        MessageBox.Show("The following specified text or hexadecimal bytes was not found: \n\n" + SearchTextBox.Text, "Binary Engine", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;

                    case OPERATION_ABORTED:
                        break;

                    default:
                        //success
                        if (!ByteViewer.Focused)
                        {
                            ByteViewer.Focus();
                        }
                        break;
                }

            }
            catch { }
        }

        private void FindAllButton_Click(object sender, RoutedEventArgs e)
        {
            FindAllReferencesGroupBox.Visibility = Visibility.Visible;
            ByteViewerHost.Margin = new Thickness(0, 20, 250, 105);

            FindAllListBox.Items.Clear();

            if (String.IsNullOrEmpty(SearchTextBox.Text) || ByteViewer.ByteProvider == null)
            {
                return;
            }

            if (SearchBytesRadioButton.IsChecked == true && SearchOptionCheckBox.IsChecked == true)
            {
                SignatureSearchResult = 1;
                SignatureScan scan = new SignatureScan(SearchTextBox.Text, ByteViewer.ByteProvider, SignatureSearchResult);
                for (long address = scan.Address(); address >= 0; scan.Result++, address = scan.Address())
                {
                    byte[] data = new byte[scan.PatternSize];
                    for (int i = 0; i < scan.PatternSize; ++i)
                    {
                        data[i] = ByteViewer.ByteProvider.ReadByte(address + i);
                    }

                    string content = $"{address.ToString("X8")} ({scan.PatternSize}): {new BytesConverter(data).ToHexadecimalString()} [\"{Encoding.ASCII.GetString(data)}\"]";
                    FindAllListBox.Items.Add(new ListBoxItem()
                    {
                        Content = content,
                        Tag = new KeyValuePair<long, long>(address, scan.PatternSize),
                        ToolTip = new ToolTip()
                        {
                            Content = content
                        }

                    });
                }

                return;
            }
            else
            {
                try
                {
                    ByteViewer.SelectionStart = 0;
                    ByteViewFindOptions.MatchCase = (bool)SearchOptionCheckBox.IsChecked;
                    ByteViewFindOptions.Type = (bool)SearchBytesRadioButton.IsChecked ? FindType.Hex : FindType.Text;

                    if (ByteViewFindOptions.Type == FindType.Hex)
                    {
                        ByteViewFindOptions.Hex = BytesConverter.StringToBytes(SearchTextBox.Text);
                    }
                    else
                    {
                        ByteViewFindOptions.Text = SearchTextBox.Text;
                    }

                    ByteViewFindOptions.IsValid = true;


                    const long NO_MATCH = -1;

                    while (ByteViewer.Find(ByteViewFindOptions) != NO_MATCH)
                    {
                        byte[] data = new byte[ByteViewer.SelectionLength];
                        for (int i = 0; i < ByteViewer.SelectionLength; ++i)
                        {
                            data[i] = ByteViewer.ByteProvider.ReadByte(ByteViewer.SelectionStart + i);
                        }

                        string content = $"{ByteViewer.SelectionStart.ToString("X8")} ({ByteViewer.SelectionLength}): {new BytesConverter(data).ToHexadecimalString()} [\"{Encoding.ASCII.GetString(data)}\"]";
                        FindAllListBox.Items.Add(new ListBoxItem()
                        {
                            Content = content,
                            Tag = new KeyValuePair<long, long>(ByteViewer.SelectionStart, ByteViewer.SelectionLength),
                            ToolTip = new ToolTip()
                            {
                                Content = content
                            }

                        });
                    }


                }
                catch
                {

                }
            }
        }

        private void FindAllListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FindAllListBox.SelectedItem != null && FindAllListBox.SelectedItem is ListBoxItem)
            {
                ListBoxItem item = FindAllListBox.SelectedItem as ListBoxItem;
                if (item.Tag != null && item.Tag is KeyValuePair<long, long>)
                {
                    KeyValuePair<long, long> tag = (KeyValuePair<long, long>)item.Tag;

                    ByteViewer.SelectionStart = tag.Key;
                    ByteViewer.SelectionLength = tag.Value;
                }

            }
        }

        private void ByteViewer_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && ByteViewer.ByteProvider != null)
            {
                ByteViewerContextMenu.IsOpen = true;
            }
        }
    }
}