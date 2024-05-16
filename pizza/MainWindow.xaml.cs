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
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using static pizza.MainWindow;
    using System.IO;

    namespace pizza
    {
        /// <summary>
        /// Interaction logic for MainWindow.xaml
        /// </summary>
        public partial class MainWindow : Window
        {
        public MainWindow()
        {
            InitializeComponent();
            MeretComboBox.ItemsSource = new List<string> { "L", "Xl", "XXl" };
            LoadPizzasFromTextFile("pizzas.txt");
            LoadOntetekFromTextFile("öntetek.txt");
            PizzaListBox.Loaded += PizzaListBox_Loaded;
        }


        private void PizzaListBox_Loaded(object sender, RoutedEventArgs e)
    {

        ImageChange(sender, null);
    }

            public class Pizza
            {
                public string Nev { get; set; }
                public double Ar { get; set; }
                public string KepUrl { get; set; } 

                public Pizza(string nev, double ar, string kepUrl)
                {
                    Nev = nev;
                    Ar = ar;
                    KepUrl = kepUrl; 
                }
            }


            public class Ontet
            {
                public string Nev { get; set; }
                public double Ar { get; set; }

                public Ontet(string nev, double ar)
                {
                    Nev = nev;
                    Ar = ar;
                }
            }
            List<Pizza> pizzas = new List<Pizza>();
            List<Ontet> ontetek = new List<Ontet>();
            private void LoadPizzasFromTextFile(string filename)
            {
                try
                {
                    string[] lines = File.ReadAllLines(filename);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] parts = lines[i].Split(';');

                        string nev = parts[0];
                        double ar = double.Parse(parts[1]);
                        string kepUrl = parts[2]; 

                        Pizza pizza = new Pizza(nev, ar, kepUrl);
                        pizzas.Add(pizza);

                    
                        PizzaListBox.Items.Add(nev);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt a pizzák betöltése során: {ex.Message}");
                }
            }
            private void ImageChange(object sender, SelectionChangedEventArgs e)
            {
                foreach (var item in PizzaListBox.Items)
                {
                    string selectedPizzaName = item.ToString();
                    Pizza selectedPizza = pizzas.Find(p => p.Nev == selectedPizzaName);

                    if (selectedPizza != null)
                    {
                        var listBoxItem = PizzaListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;

                        if (listBoxItem != null)
                        {

                            Image pizzaImageControl = FindVisualChild<Image>(listBoxItem);


                            if (pizzaImageControl != null)
                            {
                                pizzaImageControl.Source = new BitmapImage(new Uri(selectedPizza.KepUrl, UriKind.RelativeOrAbsolute));
                            }
                        }
                    }
                }
            }


            private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }
                    else
                    {
                        T childOfChild = FindVisualChild<T>(child);
                        if (childOfChild != null)
                        {
                            return childOfChild;
                        }
                    }
                }
                return null;
            }






            private void LoadOntetekFromTextFile(string filename)
            {
                try
                {
                    string[] lines = File.ReadAllLines(filename);

                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(';');

                        string nev = parts[0];
                        double ar = double.Parse(parts[1]);

                        Ontet ontet = new Ontet(nev, ar);
                        ontetek.Add(ontet);

                   
                        if(nev == "empty")
                        { break; }
                        OntetComboBox.Items.Add(nev);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt az öntetek betöltése során: {ex.Message}");
                }
            }

            private void AddOrderToFile(string order, string filePath)
            {
                try
                {
               
                    if (!File.Exists(filePath))
                    {
                    
                        File.WriteAllText(filePath, "Pizza;Méret;Darabszám;Öntet;Ár (Ft)\n");
                    }

               
                    File.AppendAllText(filePath, order + "\n");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt a rendelés hozzáadása során: {ex.Message}");
                }
            }



            private void MegrendelemButton_Click(object sender, RoutedEventArgs e)
            {
            
                if (PizzaListBox.SelectedItem == null ||  string.IsNullOrEmpty(DarabszamTextBox.Text))
                {
                    MessageBox.Show("Kérlek válaszd ki a pizzát, és add meg a darabszámot!");
                    return;
                }

           
                string pizzaNev = PizzaListBox.SelectedItem.ToString();
                string ontetNev = "";
                if(OntetComboBox.SelectedItem == null)
                {
                    ontetNev = "empty";
                }
                else
                {
                    ontetNev = OntetComboBox.SelectedItem.ToString();
                }
            

            
                int darabszam;
                if (!int.TryParse(DarabszamTextBox.Text, out darabszam) || darabszam <= 0)
                {
                    MessageBox.Show("Kérlek adj meg egy érvényes darabszámot!");
                    return;
                }

            
                string meret = MeretComboBox.SelectedItem.ToString();

            
                Pizza kivalasztottPizza = pizzas.Find(p => p.Nev == pizzaNev);
                Ontet kivalasztottOntet = ontetek.Find(o => o.Nev == ontetNev);

                string Meretnev = MeretComboBox.SelectedItem.ToString();
                double MeretSzer = 1;
                if(Meretnev == "L")
                {
                    MeretSzer = 1;
                }
                else if(Meretnev == "Xl")
                {
                    MeretSzer = 1.2;
                }
                else if(Meretnev == "XXl")
                {
                    MeretSzer = 1.5;
                }
         
                double rendelesAr = kivalasztottPizza.Ar * MeretSzer * darabszam + kivalasztottOntet.Ar;
                string rendeles = $"{pizzaNev};{ontetNev};{meret};{darabszam};{rendelesAr}";

            
                string filePath = "rendelesek.csv";
                AddOrderToFile(rendeles, filePath);

            
                MessageBox.Show($"A rendelésed sikeresen felvéve!\nÖsszesen fizetendő: {rendelesAr} Ft");
            }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
    }
