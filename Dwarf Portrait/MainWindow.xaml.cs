using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using UnitFlags;

namespace Dwarf_Portrait
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Creature> unitList = new ObservableCollection<Creature>();
        DFConnection dfConnection = new DFConnection();

        ObservableCollection<FlagDisplay> flag1List = new ObservableCollection<FlagDisplay>();
        ObservableCollection<FlagDisplay> flag2List = new ObservableCollection<FlagDisplay>();
        ObservableCollection<FlagDisplay> flag3List = new ObservableCollection<FlagDisplay>();

        public MainWindow()
        {
            InitializeComponent();

            creatureList.ItemsSource = unitList;
            flags1ListView.ItemsSource = flag1List;
            flags2ListView.ItemsSource = flag2List;
            flags3ListView.ItemsSource = flag3List;

            TextInfo TI = new CultureInfo("en-US", false).TextInfo;


            for (int i = 0; i < 32; i++)
            {
                uint item = 1u << i;
                UnitFlags1 flag = (UnitFlags1)item;
                string name = flag.ToString();
                name = name.Replace('_', ' ');
                name = TI.ToTitleCase(name);
                flag1List.Add(new FlagDisplay() { Name = name, Enabled = false });
            }
            for (int i = 0; i < 32; i++)
            {
                uint item = 1u << i;
                UnitFlags2 flag = (UnitFlags2)item;
                string name = flag.ToString();
                name = name.Replace('_', ' ');
                name = TI.ToTitleCase(name);
                flag2List.Add(new FlagDisplay() { Name = name, Enabled = false });
            }
            for (int i = 0; i < 29; i++)
            {
                uint item = 1u << i;
                UnitFlags3 flag = (UnitFlags3)item;
                string name = flag.ToString();
                name = name.Replace('_', ' ');
                name = TI.ToTitleCase(name);
                flag3List.Add(new FlagDisplay() { Name = name, Enabled = false });
            }
        }

        private void fetchButton_Click(object sender, RoutedEventArgs e)
        {
            dfConnection.ConnectAndFetch();
            if(dfConnection.unitList != null)
            {
                unitList.Clear();
                foreach (RemoteFortressReader.UnitDefinition unit in dfConnection.unitList.creature_list)
                {
                    Creature listedCreature = new Creature();
                    listedCreature.unitDefinition = unit;
                    if ((listedCreature.flags1 & UnitFlags.UnitFlags1.dead) == UnitFlags.UnitFlags1.dead)
                        continue;
                    if ((listedCreature.flags1 & UnitFlags.UnitFlags1.forest) == UnitFlags.UnitFlags1.forest)
                        continue;
                    unitList.Add(listedCreature);
                }

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(creatureList.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("Race", ListSortDirection.Ascending));
            }
        }

        private void creatureList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                Creature selectedUnit = e.AddedItems[0] as Creature;
                UpdateView(selectedUnit);
            }
        }

        private void UpdateView(Creature selectedUnit)
        {
            UpdateInfo(selectedUnit);
            UpdateFlags(selectedUnit);
        }

        private void UpdateFlags(Creature selectedUnit)
        {
            for (int i = 0; i < 32; i++)
            {
                uint item = 1u << i;
                UnitFlags1 flag = (UnitFlags1)item;
                string name = flag.ToString();
                bool enabled = (selectedUnit.flags1 & flag) == flag;
                flag1List[i].Enabled = enabled;
            }
            for (int i = 0; i < 32; i++)
            {
                uint item = 1u << i;
                UnitFlags2 flag = (UnitFlags2)item;
                string name = flag.ToString();
                bool enabled = (selectedUnit.flags2 & flag) == flag;
                flag2List[i].Enabled = enabled;
            }
            for (int i = 0; i < 29; i++)
            {
                uint item = 1u << i;
                UnitFlags3 flag = (UnitFlags3)item;
                string name = flag.ToString();
                bool enabled = (selectedUnit.flags3 & flag) == flag;
                flag3List[i].Enabled = enabled;
            }
        }

        private void UpdateInfo(Creature selectedUnit)
        {
            unitNameTextBox.Text = selectedUnit.Name;
            unitRaceTextBox.Text = selectedUnit.Race;
        }
    }
}
