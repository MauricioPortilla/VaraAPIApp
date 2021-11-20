using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VaraAPIApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private ObservableCollection<StrandingRow> _observerStrandings;
        public ObservableCollection<StrandingRow> ObserverStrandings {
            get {
                if (_observerStrandings == null)
                    _observerStrandings = new ObservableCollection<StrandingRow>();
                return _observerStrandings;
            }
        }
        private int total = 0;
        private int totalShown = 0;
        private int limit = 0;

        public MainWindow() {
            DataContext = this;
            InitializeComponent();
            login();
            loadStrandings();
            animalConditionCB.Items.Add("VIVO");
            animalConditionCB.Items.Add("MUERTO");
        }

        private void login() {
            var loginStatus = API.login("mauricio.portilla@hotmail.com", "_Test123");
            if (!loginStatus) {
                MessageBox.Show("Error login");
                return;
            }
        }

        private void loadStrandings(bool loadNextPage = false) {
            if (!loadNextPage) {
                ObserverStrandings.Clear();
                total = 0;
                totalShown = 0;
                limit = 0;
            } else {
                if (totalShown >= total) {
                    return;
                }
            }
            var strandings = API.loadStrandings(true, totalShown + limit, new {
                filtro_condicionAnimal__eq = animalConditionCB.SelectedItem,
                filtro_numeroAnimales__eq = animalsNumberTB.Text
            });
            total = strandings["paginacion"]["total"];
            totalShown = strandings["paginacion"]["salto"];
            limit = strandings["paginacion"]["limite"];
            var results = strandings["resultados"];
            foreach (var strandingData in results) {
                ObserverStrandings.Add(new StrandingRow {
                    Uuid = strandingData["uuid"],
                    Name = strandingData["nombreInformante"],
                    Country = strandingData["pais"],
                    State = strandingData["estado"],
                    IsFinished = strandingData["finalizado"],
                    SightingDate = strandingData["fechaAvistamiento"],
                    AnimalCondition = strandingData["condicionAnimal"],
                    AnimalsNumber = strandingData["numeroAnimales"]
                });
            }
        }

        private void Scroll_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            if (e.VerticalChange > 0) {
                if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight) {
                    loadStrandings(true);
                }
            }
        }

        private void createStranding_Click(object sender, RoutedEventArgs e) {
            new CreateStranding().Show();
        }

        private void reloadInfo_Click(object sender, RoutedEventArgs e) {
            loadStrandings();
        }

        public struct StrandingRow {
            public string Uuid;
            public string Name { get; set; }
            public string Country { get; set; }
            public string State { get; set; }
            public bool IsFinished { get; set; }
            public DateTime SightingDate { get; set; }
            public string AnimalCondition { get; set; }
            public int AnimalsNumber { get; set; }
        }

        private void markAsFinishedButton_Click(object sender, RoutedEventArgs e) {
            if (strandingsTable.SelectedItem != null) {
                var finished = API.MarkStrandingAsFinished((StrandingRow)strandingsTable.SelectedItem);
                if (finished) {
                    MessageBox.Show("Varamiento finalizado.");
                } else {
                    MessageBox.Show("Error al realizar esta acción.");
                }
            }
        }

        private void deleteStrandingButton_Click(object sender, RoutedEventArgs e) {
            if (strandingsTable.SelectedItem != null) {
                var deleted = API.DeleteStranding((StrandingRow)strandingsTable.SelectedItem);
                if (deleted) {
                    MessageBox.Show("Varamiento eliminado.");
                    ObserverStrandings.Remove((StrandingRow)strandingsTable.SelectedItem);
                } else {
                    MessageBox.Show("Error al realizar esta acción.");
                }
            }
        }

        private void filterButton_Click(object sender, RoutedEventArgs e) {
            loadStrandings();
        }

        private void clearButton_Click(object sender, RoutedEventArgs e) {
            animalConditionCB.SelectedItem = null;
            animalsNumberTB.Clear();
        }
    }
}
