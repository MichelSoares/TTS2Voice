using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TTS2Voice.Model;

namespace TTS2Voice.ViewModel
{
    public class WindowViewModel : INotifyPropertyChanged
    {
        public WindowViewModel()
        {
            IsProcessingProgressBar = false;
            _cbxSelectedItem = "Vitoria";
        }

        private ICommand _clickCommand;

        public ICommand ClickCommand
        {
            get
            {
                if (_clickCommand == null)
                {
                    _clickCommand = new RelayCommand(param => ExecuteClick(), param => CanExecuteClick());
                }
                return _clickCommand;
            }
        }

        private bool _isProcessing;
        public bool IsProcessing
        {
            get { return _isProcessing; }
            set
            {
                _isProcessing = value;
                OnPropertyChanged(nameof(IsProcessing));
            }
        }

        private bool _isProcessingProgressBar;
        public bool IsProcessingProgressBar
        {
            get { return _isProcessingProgressBar; }
            set
            {
                _isProcessingProgressBar = value;
                OnPropertyChanged(nameof(IsProcessingProgressBar));
            }
        }

        private string _cbxSelectedItem;
        public string CbxSelectedItem
        {
            get { return _cbxSelectedItem; }
            set
            {
                if (_cbxSelectedItem != value)
                {
                    _cbxSelectedItem = value;
                    OnPropertyChanged(nameof(CbxSelectedItem));
                }
            }
        }

        private async Task ExecuteClick()
        {
            IsProcessing = true;
            //IsProcessingProgressBar = true;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Arquivos CSV (*.csv)|*.csv",
                Title = "Selecionar Arquivo CSV"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                Console.WriteLine($"Arquivo CSV Selecionado: {filePath}");

                bool isHeaderValid = VerificaHeaderCSV(filePath);

                if (isHeaderValid)
                {
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ";",
                        HasHeaderRecord = true,
                    }))
                    {
                        string vozSelecionada = _cbxSelectedItem.Split(':').LastOrDefault().Trim();

                        var records = csv.GetRecords<ArquivoCSV>().ToList();

                        foreach (var record in records)
                        {
                            IsProcessingProgressBar = true;
                            Console.WriteLine($"Caminho do Arquivo: {record.Arquivo}, Nome: {record.Nome}, Frase: {record.Frase}");
                            await TTStoVoice.TTStoVoicePollyAWS(record.Nome, record.Frase, record.Arquivo, vozSelecionada);
                        }
                        IsProcessingProgressBar = false;
                        MessageBox.Show("Gravações concluídas com sucesso.");
                    }
                }
                else
                {
                    MessageBox.Show("CSV fora do padrão esperado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                Console.WriteLine("Nenhum arquivo CSV selecionado.");
            }

            IsProcessing = false;
            IsProcessingProgressBar = false;
        }



        private bool CanExecuteClick()
        {
            return true;
        }

        private bool VerificaHeaderCSV(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    var headerLine = reader.ReadLine();
                    if (headerLine != null && headerLine.Trim() == "Arquivo;Nome;Frase")
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao verificar o cabeçalho do arquivo CSV: " + ex.Message);
            }
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }
    }
}
