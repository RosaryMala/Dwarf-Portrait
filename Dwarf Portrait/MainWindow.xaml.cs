﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
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
        private double zoomLevel = 1;

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
                view.Filter = UnitlistFilter;
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
                view.Filter = UnitlistFilter;
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

        private bool UnitlistFilter (object item)
        {
            if (string.IsNullOrEmpty(unitFilterTextbox.Text))
                return true;
            else
                return ((item as Creature).Name.IndexOf(unitFilterTextbox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    || ((item as Creature).Race.IndexOf(unitFilterTextbox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void unitFilterTextbox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if(unitListView.ItemsSource != null)
                CollectionViewSource.GetDefaultView(unitListView.ItemsSource).Refresh();
            if(raceListView.ItemsSource != null)
                CollectionViewSource.GetDefaultView(raceListView.ItemsSource).Refresh();
        }

        private void portraitCanvas_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Canvas canvas = sender as Canvas;

            Creature creature = e.NewValue as Creature;

            UpdateCanvas(canvas, creature);
        }

        private void UpdateCanvas(Canvas canvas, Creature creature)
        {
            canvas.Children.Clear();
            if (creature == null)
                return;

            double shift = 0;
            double scale = ((double)creature.Size / creature.CasteRaw.total_relsize);


            foreach (BodyPart part in creature.BodypartTree)
            {
                if (part.IsInternal)
                    continue;

                AddPart(canvas, new Vector(0, 0), part, new Vector(shift, 0), scale, zoomLevel);

                shift += 5;
            }
        }

        static readonly double cubeRootPi = Math.Pow(Math.PI, 1.0 / 3.0);
        static readonly double cubeRootSquare2 = Math.Pow(2.0, 2.0 / 3.0);

        static double GetRatioCylinderWidth(double volume, double ratio)
        {
            return (cubeRootSquare2 * Math.Pow(volume, 1.0 / 3.0)) / (cubeRootPi * Math.Pow(ratio, 1.0 / 3.0));
        }

        private static void AddPart(Canvas canvas, Vector parentPos, BodyPart part, Vector pos, double creatureScale, double visualScale, bool centered = false)
        {
            Shape partShape;

            double length;
            double width;

            switch (part.OriginalPart.category)
            {
                case "NECK":
                    partShape = new Rectangle();

                    double headDia = VolumeToDiameterConverter.Convert(part.OriginalPart.relsize * creatureScale);
                    foreach (var child in part.Children)
                    {
                        if(child.OriginalPart.category == "HEAD")
                        {
                            headDia = VolumeToDiameterConverter.Convert(child.OriginalPart.relsize * creatureScale);
                            break;
                        }
                    }
                    headDia *= 0.75;
                    double neckArea = Math.PI * (headDia / 2) * (headDia / 2);
                    length = ((part.OriginalPart.relsize * creatureScale) / neckArea) * visualScale;
                    partShape.Width = headDia * visualScale;
                    partShape.Height = length;

                    break;
                case "LEG_UPPER":
                case "LEG_LOWER":
                case "LEG_FRONT":
                case "LEG_REAR":
                case "ARM_UPPER":
                case "ARM_LOWER":
                    partShape = new Rectangle();

                    width = GetRatioCylinderWidth(part.OriginalPart.relsize * creatureScale, 2.5) * visualScale;
                    length = width * 2.5;

                    partShape.Width = width;
                    partShape.Height = length;

                    break;
                case "TAIL":
                    partShape = new Rectangle();

                    width = GetRatioCylinderWidth(part.OriginalPart.relsize * creatureScale, 3) * visualScale;
                    length = width * 3;

                    partShape.Width = width;
                    partShape.Height = length;

                    break;
                case "HORN":
                case "TUSK":
                    partShape = new Rectangle();

                    width = GetRatioCylinderWidth(part.OriginalPart.relsize * creatureScale, 3) * visualScale;
                    length = width * 3;

                    partShape.Width = width;
                    partShape.Height = length;

                    break;
                case "BODY_UPPER":
                case "BODY_LOWER":
                case "MANDIBLE":
                    partShape = new Rectangle();

                    width = GetRatioCylinderWidth(part.OriginalPart.relsize * creatureScale, 1) * visualScale;
                    length = width * 1;

                    partShape.Width = width;
                    partShape.Height = length;
                    break;
                case "FINGER":
                    partShape = new Rectangle();

                    width = GetRatioCylinderWidth(part.OriginalPart.relsize * creatureScale, 3) * visualScale;
                    length = width * 3;

                    partShape.Width = width;
                    partShape.Height = length;
                    break;
                case "TENTACLE":
                case "ANTENNA":
                case "ARM":
                case "LEG":
                case "STINGER":
                    partShape = new Rectangle();

                    width = GetRatioCylinderWidth(part.OriginalPart.relsize * creatureScale, 5) * visualScale;
                    length = width * 5;

                    partShape.Width = width;
                    partShape.Height = length;
                    break;
                case "WING":
                    partShape = new Ellipse();

                    length = VolumeToDiameterConverter.Convert(part.OriginalPart.relsize * creatureScale) * visualScale * 3;

                    partShape.Width = length * 2.0 / 3.0;
                    partShape.Height = length;
                    break;
                default:
                    partShape = new Ellipse();

                    length = VolumeToDiameterConverter.Convert(part.OriginalPart.relsize * creatureScale) * visualScale;

                    partShape.Width = length;
                    partShape.Height = length;
                    break;
            }

            Vector localPos = pos - parentPos;
            RotateTransform rotation = new RotateTransform(-Math.Atan2(localPos.X, localPos.Y) * 180 / Math.PI, partShape.Width / 2, partShape.Height / 2);

            partShape.RenderTransform = rotation;


            if (!centered)
            {
                var relPos = pos - parentPos;
                if (relPos.LengthSquared > 0.0001)
                {
                    relPos.Normalize();
                    relPos *= (length / 2);
                    pos += relPos;
                }
            }

            Canvas.SetLeft(partShape, (-partShape.Width / 2) + pos.X);
            Canvas.SetTop(partShape, (-partShape.Height / 2) + pos.Y);


            BodyPartLayer usedLayer = null;
            foreach (var layer in part.Layers)
            {
                if (usedLayer == null)
                    usedLayer = layer;
                else
                    if(usedLayer.Original.layer_depth >= layer.Original.layer_depth)
                        if (usedLayer.Original.tissue_id > layer.Original.layer_depth)
                            usedLayer = layer;
            }
            Color fillColor = Color.FromRgb(255, 255, 255);
            if (usedLayer != null && usedLayer.ColorMod != null)
            {
                var color = usedLayer.ColorMod.CurrentPatterns[0].Original.colors[0];
                fillColor = Color.FromRgb((byte)color.red, (byte)color.green, (byte)color.blue);
            }

            if ((fillColor.R + fillColor.G + fillColor.B) < 15)
                partShape.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            else
                partShape.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));


            partShape.Fill = new SolidColorBrush(fillColor);

            partShape.ToolTip = part.OriginalPart.token + " - " + part.OriginalPart.category + " - " + part.OriginalPart.relsize;

            canvas.Children.Add(partShape);

            if (usedLayer.ColorMod != null)
            {
                if (usedLayer.ColorMod.CurrentPatterns[0].Pattern == RemoteFortressReader.PatternType.IRIS_EYE)
                {
                    Ellipse iris = new Ellipse();
                    iris.Width = length * 2 / 3;
                    iris.Height = length * 2 / 3;
                    Canvas.SetLeft(iris, (-iris.Width / 2) + pos.X);
                    Canvas.SetTop(iris, (-iris.Height / 2) + pos.Y);
                    var color = usedLayer.ColorMod.CurrentPatterns[0].Original.colors[2];
                    iris.Fill = new SolidColorBrush(Color.FromRgb((byte)color.red, (byte)color.green, (byte)color.blue));
                    iris.ToolTip = partShape.ToolTip;

                    canvas.Children.Add(iris);
                    Ellipse pupil = new Ellipse();
                    pupil.Width = length / 3;
                    pupil.Height = length / 3;
                    Canvas.SetLeft(pupil, (-pupil.Width / 2) + pos.X);
                    Canvas.SetTop(pupil, (-pupil.Height / 2) + pos.Y);
                    var pupilColor = usedLayer.ColorMod.CurrentPatterns[0].Original.colors[1];
                    pupil.Fill = new SolidColorBrush(Color.FromRgb((byte)pupilColor.red, (byte)pupilColor.green, (byte)pupilColor.blue));
                    pupil.ToolTip = partShape.ToolTip;
                    canvas.Children.Add(pupil);
                }
                if (usedLayer.ColorMod.CurrentPatterns[0].Pattern == RemoteFortressReader.PatternType.PUPIL_EYE)
                {
                    Ellipse pupil = new Ellipse();
                    pupil.Width = length / 3;
                    pupil.Height = length / 3;
                    Canvas.SetLeft(pupil, (-pupil.Width / 2) + pos.X);
                    Canvas.SetTop(pupil, (-pupil.Height / 2) + pos.Y);
                    var pupilColor = usedLayer.ColorMod.CurrentPatterns[0].Original.colors[1];
                    pupil.Fill = new SolidColorBrush(Color.FromRgb((byte)pupilColor.red, (byte)pupilColor.green, (byte)pupilColor.blue));
                    pupil.ToolTip = partShape.ToolTip;
                    canvas.Children.Add(pupil);
                }
            }

            Vector direction = (pos - parentPos);
            if (direction.LengthSquared < 0.0001)
                direction = new Vector(0, -1);
            direction.Normalize();

            List<BodyPart> ExternalParts = new List<BodyPart>();
            List<BodyPart> LowerBodyParts = new List<BodyPart>();
            List<BodyPart> LeftParts = new List<BodyPart>();
            List<BodyPart> RightParts = new List<BodyPart>();
            List<BodyPart> EmbeddedParts = new List<BodyPart>();
            List<BodyPart> EyeParts = new List<BodyPart>();
            List<BodyPart> EyelidParts = new List<BodyPart>();
            List<BodyPart> MouthParts = new List<BodyPart>();
            List<BodyPart> LipParts = new List<BodyPart>();
            List<BodyPart> NoseParts = new List<BodyPart>();
            List<BodyPart> CheekParts = new List<BodyPart>();
            List<BodyPart> TuskParts = new List<BodyPart>();
            List<BodyPart> LeftEarParts = new List<BodyPart>();
            List<BodyPart> RightEarParts = new List<BodyPart>();

            foreach (var child in part.Children)
            {
                if (!child.IsInternal)
                {
                    switch (child.OriginalPart.category)
                    {
                        case "EYE":
                            EyeParts.Add(child);
                            break;
                        case "MOUTH":
                        case "BEAK":
                            MouthParts.Add(child);
                            break;
                        case "EYELID":
                            EyelidParts.Add(child);
                            break;
                        case "LIP":
                            LipParts.Add(child);
                            break;
                        case "NOSE":
                            NoseParts.Add(child);
                            break;
                        case "CHEEK":
                            CheekParts.Add(child);
                            break;
                        case "TUSK":
                        case "MANDIBLE":
                            TuskParts.Add(child);
                            break;
                        case "EAR":
                            if (child.IsLeft)
                                LeftEarParts.Add(child);
                            else
                                RightEarParts.Add(child);
                            break;
                        case "TOOTH":
                        case "TONGUE":
                        case "THROAT":
                            break;
                        default:
                            if (child.IsLowerBody)
                                LowerBodyParts.Add(child);
                            else if (child.IsEmbedded)
                                EmbeddedParts.Add(child);
                            else if (child.IsLeft && !part.IsLeft)
                                LeftParts.Add(child);
                            else if (child.IsRight && !part.IsRight)
                                RightParts.Add(child);
                            else
                                ExternalParts.Add(child);
                            break;
                    }
                }
            }

            bool root = pos.Length < 0.0001;

            for (int i = 0; i < ExternalParts.Count; i++)
            {
                BodyPart child = ExternalParts[i];
                double rotateAngle = 0;
                if (part.IsLeft)
                    rotateAngle = Range(i, ExternalParts.Count, -45, 45);
                else
                    rotateAngle = Range(i, ExternalParts.Count, 45, -45);
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale);
            }
            for (int i = 0; i < LowerBodyParts.Count; i++)
            {
                BodyPart child = LowerBodyParts[i];
                double rotateAngle = Range(i, LowerBodyParts.Count, 135, 225);
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale);
            }
            for (int i = 0; i < LeftParts.Count; i++)
            {
                BodyPart child = LeftParts[i];
                double rotateAngle = 0;
                if (part.IsLowerBody)
                    if (root)
                        rotateAngle = Range(i, LeftParts.Count, 45, 135);
                    else
                        rotateAngle = Range(i, LeftParts.Count, -20, -40);
                else
                    rotateAngle = Range(i, LeftParts.Count, 45, 90);
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale);
            }
            for (int i = 0; i < RightParts.Count; i++)
            {
                BodyPart child = RightParts[i];
                double rotateAngle = 0;
                if (part.IsLowerBody)
                    if (root)
                        rotateAngle = Range(i, RightParts.Count, -45, -135);
                    else
                        rotateAngle = Range(i, RightParts.Count, 20, 40);
                else
                    rotateAngle = Range(i, RightParts.Count, -45, -90);
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale);
            }
            for (int i = 0; i < LeftEarParts.Count; i++)
            {
                BodyPart child = LeftEarParts[i];
                double rotateAngle = 0;
                if (part.IsLowerBody)
                    if (root)
                        rotateAngle = Range(i, LeftEarParts.Count, 45, 135);
                    else
                        rotateAngle = Range(i, LeftEarParts.Count, -20, -90);
                else
                    rotateAngle = Range(i, LeftEarParts.Count, 45, 90);
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale);
            }
            for (int i = 0; i < RightEarParts.Count; i++)
            {
                BodyPart child = RightEarParts[i];
                double rotateAngle = 0;
                if (part.IsLowerBody)
                    if (root)
                        rotateAngle = Range(i, RightEarParts.Count, -45, -135);
                    else
                        rotateAngle = Range(i, RightEarParts.Count, 20, 90);
                else
                    rotateAngle = Range(i, RightEarParts.Count, -45, -90);
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale);
            }
            for (int i = 0; i < EmbeddedParts.Count; i++)
            {
                BodyPart child = EmbeddedParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(90);
                childPos *= Range(i, EmbeddedParts.Count, -length / 4, length / 4);
                AddPart(canvas, pos, child, pos + childPos, creatureScale, visualScale, true);
            }
            for (int i = 0; i < EyelidParts.Count; i++)
            {
                BodyPart child = EyelidParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(90);
                childPos *= Range(i, EyelidParts.Count, -length * 0.2, length * 0.2);
                AddPart(canvas, pos, child, pos + childPos + direction * length * 0.1, creatureScale, visualScale, true);
            }
            for (int i = 0; i < CheekParts.Count; i++)
            {
                BodyPart child = CheekParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(90);
                childPos *= Range(i, CheekParts.Count, -length * 0.2, length * 0.2);
                AddPart(canvas, pos, child, pos + childPos + direction * length * -0.2, creatureScale, visualScale, true);
            }
            for (int i = 0; i < EyeParts.Count; i++)
            {
                BodyPart child = EyeParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(90);
                childPos *= Range(i, EyeParts.Count, -length * 0.2, length * 0.2);
                AddPart(canvas, pos, child, pos + childPos, creatureScale, visualScale, true);
            }
            for (int i = 0; i < MouthParts.Count; i++)
            {
                BodyPart child = MouthParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(180);
                childPos.Normalize();
                childPos *= length / 2;
                childPos *= Range(i, MouthParts.Count, 0.75, 0.5);
                AddPart(canvas, pos, child, pos + childPos, creatureScale, visualScale, true);
            }
            for (int i = 0; i < LipParts.Count; i++)
            {
                BodyPart child = LipParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(180);
                childPos.Normalize();
                childPos *= length / 2;
                childPos *= Range(i, LipParts.Count, 0.75, 0.5);
                AddPart(canvas, pos, child, pos + childPos, creatureScale, visualScale, true);
            }
            for (int i = 0; i < TuskParts.Count; i++)
            {
                BodyPart child = TuskParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(90);
                childPos *= Range(i, TuskParts.Count, -length * 0.2, length * 0.2);
                AddPart(canvas, pos, child, pos + childPos + direction * length * 0.5 * -0.75, creatureScale, visualScale, true);
            }
            for (int i = 0; i < NoseParts.Count; i++)
            {
                BodyPart child = NoseParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(180);
                childPos.Normalize();
                childPos *= length / 2;
                childPos *= Range(i, NoseParts.Count, 0.1, 0.5);
                AddPart(canvas, pos, child, pos + childPos, creatureScale, visualScale, true);
            }
        }

        private static double Range(int i, int count, double min, double max)
        {
            if (count == 1)
                return (min + max) / 2;
            else
                return min + (i * (max - min) / (count - 1));
        }

        private void portraitZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            zoomLevel = Math.Pow(2, e.NewValue);
            portraitZoomTextBlock.Text = e.NewValue.ToString();
            UpdateCanvas(portraitCanvas, portraitCanvas.DataContext as Creature);
        }
    }
}
