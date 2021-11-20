using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace VaraAPIApp {
    /// <summary>
    /// Lógica de interacción para CreateStranding.xaml
    /// </summary>
    public partial class CreateStranding : Window {
        public CreateStranding() {
            InitializeComponent();
            animalOrder.Items.Add("SIRENO");
            animalCondition.Items.Add("VIVO");
            animalCondition.Items.Add("MUERTO");
            sustrato.Items.Add("ARENA");
            firstSeen.Items.Add("AGUA");
        }

        private void registerButton_Click(object sender, RoutedEventArgs e) {
            var registered = API.RegisterStranding(new {
                nombreInformante = name.Text,
                telefonoInformante = phone.Text,
                ordenAnimal = animalOrder.SelectedItem,
                condicionAnimal = animalCondition.SelectedItem,
                numeroAnimales = Convert.ToInt32(animalsNumber.Text),
                facilAcceso = easyAccess.IsChecked,
                acantilado = acantilado.IsChecked,
                sustrato = sustrato.SelectedItem,
                primeraVezVisto = firstSeen.SelectedItem,
                fechaAvistamiento = sightingDate.Text,
                pais = country.Text,
                estado = state.Text,
                ciudad = city.Text,
                localidad = locality.Text,
                latitud = latitude.Text,
                longitud = longitude.Text
            });
            if (registered) {
                MessageBox.Show("Varamiento registrado.");
                Close();
            } else {
                MessageBox.Show("Error al registrar el varamiento.");
            }
        }
    }
}
