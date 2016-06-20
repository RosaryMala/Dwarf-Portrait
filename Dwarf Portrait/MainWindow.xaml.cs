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
        DFConnection dfConnection = new DFConnection();

        ObservableCollection<Creature> UnitList { get; set; }
        ObservableCollection<Creature> RaceList { get; set; }
        ObservableCollection<FlagDisplay> flag1List { get; set; }
        ObservableCollection<FlagDisplay> flag2List { get; set; }
        ObservableCollection<FlagDisplay> flag3List { get; set; }

        TextInfo TI = new CultureInfo("en-US", false).TextInfo;

        public MainWindow()
        {
            InitializeComponent();

            UnitList = new ObservableCollection<Creature>();
            RaceList = new ObservableCollection<Creature>();
            flag1List = new ObservableCollection<FlagDisplay>();
            flag2List = new ObservableCollection<FlagDisplay>();
            flag3List = new ObservableCollection<FlagDisplay>();

            unitListView.ItemsSource = UnitList;
            raceListView.ItemsSource = RaceList;

            flags1ListView.ItemsSource = flag1List;
            flags2ListView.ItemsSource = flag2List;
            flags3ListView.ItemsSource = flag3List;



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
            if (dfConnection.unitList != null)
            {
                UnitList.Clear();
                for (int i = 0; i < dfConnection.unitList.creature_list.Count; i++)
                {
                    RemoteFortressReader.UnitDefinition unit = dfConnection.unitList.creature_list[i];
                    Creature listedCreature = new Creature();
                    listedCreature.Index = i;
                    listedCreature.unitDefinition = unit;
                    if ((listedCreature.flags1 & UnitFlags.UnitFlags1.dead) == UnitFlags.UnitFlags1.dead)
                        continue;
                    if ((listedCreature.flags1 & UnitFlags.UnitFlags1.forest) == UnitFlags.UnitFlags1.forest)
                        continue;
                    UnitList.Add(listedCreature);
                }

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(unitListView.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("Race", ListSortDirection.Ascending));
            }
            if(DFConnection.creatureRawList != null)
            {
                RaceList.Clear();
                for(int raceIndex = 0; raceIndex < DFConnection.creatureRawList.creature_raws.Count; raceIndex++)
                {
                    var raceRaw = DFConnection.creatureRawList.creature_raws[raceIndex];
                    for(int casteIndex = 0; casteIndex < raceRaw.caste.Count; casteIndex++)
                    {
                        var casteRaw = raceRaw.caste[casteIndex];
                        RemoteFortressReader.UnitDefinition fakeUnit = new RemoteFortressReader.UnitDefinition();
                        fakeUnit.race = new RemoteFortressReader.MatPair() { mat_type = raceIndex, mat_index = casteIndex };
                        string name = casteRaw.caste_name[0];
                        switch (casteRaw.gender)
                        {
                            case 0:
                                name += " ♀";
                                break;
                            case 1:
                                name += " ♂";
                                break;
                            //case -1:
                            //    name += " ⚪";
                            //    break;
                            default:
                                break;
                        }
                        name = TI.ToTitleCase(name);
                        fakeUnit.name = name;
                        Creature fakeCreature = new Creature() { unitDefinition = fakeUnit };
                        RaceList.Add(fakeCreature);
                    }
                }

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(raceListView.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("Race", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
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
            unitViewTabControl.DataContext = selectedUnit;
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

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            BodyPart part = e.NewValue as BodyPart;
            bodyPartPropertyGrid.DataContext = part;
        }
    }
}
