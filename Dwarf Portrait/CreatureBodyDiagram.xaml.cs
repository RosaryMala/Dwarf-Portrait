using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RemoteFortressReader;
using UnitFlags;

namespace Dwarf_Portrait
{
    /// <summary>
    /// Interaction logic for CreatureBodyDiagram.xaml
    /// </summary>
    public partial class CreatureBodyDiagram : UserControl
    {
        public CreatureBodyDiagram()
        {
            InitializeComponent();
        }

        private void DiagramCanvas_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RegenerateCreature();
        }

        private void RegenerateCreature()
        {
            DiagramCanvas.Children.Clear();

            Creature creature = DataContext as Creature;
            if (creature == null)
                return;


            int totalRelsize = creature.CasteRaw.total_relsize;

            int calculatedRelsize = 0;

            foreach (var part in creature.CasteRaw.body_parts)
            {
                if (!(part.flags[(int)body_part_raw_flags.INTERNAL] || part.flags[(int)body_part_raw_flags.EMBEDDED]))
                    calculatedRelsize += part.relsize;
            }

            if (calculatedRelsize == 0)
                calculatedRelsize = 1; //This is probably from having no body.

            if (totalRelsize == 0)
                totalRelsize = calculatedRelsize;

            Debug.Assert(totalRelsize == calculatedRelsize, string.Format("totalRelsize {0} and calculatedRelsize {1} are not the same!", totalRelsize, calculatedRelsize));

            double scale = ((double)creature.Size / totalRelsize);

            Vector sizeMod = new Vector(1, 1);

            foreach (BodyPartMod mod in creature.AppearanceMods)
            {
                switch (mod.Original.type)
                {
                    case "HEIGHT":
                    case "LENGTH":
                        sizeMod.Y *= (mod.CurrentValue / 100.0);
                        break;
                    case "BROADNESS":
                        sizeMod.X *= (mod.CurrentValue / 100.0);
                        break;
                    default:
                        break;
                }
            }

            if (creature.BodypartTree.Count > 0)
                AddPart(DiagramCanvas, new Vector(), creature.BodypartTree[0], new Vector(), scale, Zoom, sizeMod, false, true);

            FitCanvas(DiagramCanvas);
        }



        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Zoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register(
                "Zoom",
                typeof(double),
                typeof(CreatureBodyDiagram),
                new PropertyMetadata(1.0, new PropertyChangedCallback(OnZoomChanged)));

        private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CreatureBodyDiagram)d).RegenerateCreature();
        }

        private static void AddPart(Canvas canvas, Vector parentPos, BodyPart part, Vector pos, double creatureScale, double visualScale, Vector sizeMod, bool centered = false, bool root = false)
        {
            #region Shapes

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
                        if (child.OriginalPart.category == "HEAD")
                        {
                            headDia = VolumeToDiameterConverter.Convert(child.OriginalPart.relsize * creatureScale);
                            break;
                        }
                    }
                    headDia *= 0.75;
                    double neckArea = Math.PI * (headDia / 2) * (headDia / 2);
                    length = ((part.OriginalPart.relsize * creatureScale) / neckArea) * visualScale;
                    width = headDia * visualScale;
                    break;
                case "LEG_UPPER":
                case "LEG_LOWER":
                case "LEG_FRONT":
                case "LEG_REAR":
                case "ARM_UPPER":
                case "ARM_LOWER":
                    partShape = MakeCylinder(part.OriginalPart, 2.5, creatureScale, visualScale, out width, out length);
                    break;
                case "TAIL":
                    partShape = MakeCylinder(part.OriginalPart, 5, creatureScale, visualScale, out width, out length);
                    break;
                case "HORN":
                case "TUSK":
                    partShape = MakeCylinder(part.OriginalPart, 3, creatureScale, visualScale, out width, out length);
                    break;
                case "BODY_UPPER":
                case "BODY_LOWER":
                case "MANDIBLE":
                    partShape = MakeCylinder(part.OriginalPart, 1, creatureScale, visualScale, out width, out length);
                    break;
                case "FINGER":
                    partShape = MakeCylinder(part.OriginalPart, 3, creatureScale, visualScale, out width, out length);
                    break;
                case "RIB_TRUE":
                case "RIB_FALSE":
                case "RIB_FLOATING":
                    partShape = MakeCylinder(part.OriginalPart, 10, creatureScale / 24, visualScale, out width, out length);
                    break;
                case "LIP":
                    partShape = MakeCylinder(part.OriginalPart, 0.2, creatureScale, visualScale, out width, out length);
                    foreach (BodyPartMod mod in part.AppearanceMods)
                    {
                        switch (mod.Original.type)
                        {
                            case "THICKNESS":
                                length *= (mod.CurrentValue / 100.0);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "TENTACLE":
                case "ANTENNA":
                case "ARM":
                case "LEG":
                case "STINGER":
                    partShape = MakeCylinder(part.OriginalPart, 5, creatureScale, visualScale, out width, out length);
                    break;
                case "WING":
                    partShape = new Ellipse();

                    length = VolumeToDiameterConverter.Convert(part.OriginalPart.relsize * creatureScale) * visualScale * 3;
                    width = length * 2.0 / 3.0;
                    break;
                case "FIN":
                case "FLIPPER":
                case "PINCER":
                    partShape = new Ellipse();

                    length = VolumeToDiameterConverter.Convert(part.OriginalPart.relsize * creatureScale) * visualScale * 2;
                    width = length * 2.0 / 3.0;
                    break;
                default:
                    partShape = new Ellipse();

                    length = VolumeToDiameterConverter.Convert(part.OriginalPart.relsize * creatureScale) * visualScale;
                    width = length;
                    break;
            }

            foreach (BodyPartMod mod in part.AppearanceMods)
            {
                switch (mod.Original.type)
                {
                    case "HEIGHT":
                        sizeMod.Y *= (mod.CurrentValue / 100.0);
                        break;
                    case "BROADNESS":
                        sizeMod.X *= (mod.CurrentValue / 100.0);
                        break;
                    case "ROUND_VS_NARROW":
                        sizeMod.X = 0.05 + (mod.CurrentValue * 0.0045);
                        sizeMod.Y = 1;
                        break;
                    default:
                        break;
                }
            }

            width *= sizeMod.X;
            length *= sizeMod.Y;

            #endregion

            partShape.Width = width;
            partShape.Height = length;

            partShape.Measure(new Size(1000, 1000));
            partShape.Arrange(new Rect(0, 0, 1000, 1000));

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

            #region Colors

            BodyPartLayer usedLayer = null;
            foreach (var layer in part.Layers)
            {
                // These layer types should never be used to decide the head color.
                switch (layer.Original.layer_name)
                {
                    case "SIDEBURNS":
                    case "CHIN_WHISKERS":
                    case "MOUSTACHE":
                    case "EYEBROW":
                        continue;
                }
                if (usedLayer == null || layer.Original.layer_depth <= usedLayer.Original.layer_depth) // Lower depth = shallower layer. -1 = surface.
                {
                    if (usedLayer == null || layer.Original.tissue_id > usedLayer.Original.tissue_id)
                    {
                        int duplicates = 0;
                        foreach (var testLayer in part.Layers)
                        {
                            if (layer.Original.layer_depth == testLayer.Original.layer_depth && layer.Original.tissue_id == testLayer.Original.tissue_id)
                            {
                                duplicates++;
                            }
                        }
                        if (duplicates <= 1)
                            usedLayer = layer;
                    }
                }
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

            if (usedLayer != null && usedLayer.ColorMod != null)
            {
                switch (usedLayer.ColorMod.CurrentPatterns[0].Pattern)
                {
                    case PatternType.MONOTONE:
                        break;
                    case PatternType.STRIPES:
                        {
                            var stripeBrush = new LinearGradientBrush();
                            for (int i = 0; i <= 10; i++)
                            {
                                var color = usedLayer.ColorMod.CurrentPatterns[0].Original.colors[i % usedLayer.ColorMod.CurrentPatterns[0].Original.colors.Count];
                                var stripeColor = Color.FromRgb((byte)color.red, (byte)color.green, (byte)color.blue);
                                stripeBrush.GradientStops.Add(new GradientStop(stripeColor, i / 10.0));
                            }
                            partShape.Fill = stripeBrush;
                        }
                        break;
                    case PatternType.IRIS_EYE:
                        {
                            var color = usedLayer.ColorMod.CurrentPatterns[0].Original.colors[2];
                            var irisColor = Color.FromRgb((byte)color.red, (byte)color.green, (byte)color.blue);
                            color = usedLayer.ColorMod.CurrentPatterns[0].Original.colors[1];
                            var pupilColor = Color.FromRgb((byte)color.red, (byte)color.green, (byte)color.blue);
                            var eyeBrush = new RadialGradientBrush();
                            eyeBrush.GradientStops.Add(new GradientStop(pupilColor, 0));
                            eyeBrush.GradientStops.Add(new GradientStop(pupilColor, 0.2));
                            eyeBrush.GradientStops.Add(new GradientStop(irisColor, 0.21));
                            eyeBrush.GradientStops.Add(new GradientStop(irisColor, 0.5));
                            eyeBrush.GradientStops.Add(new GradientStop(fillColor, 0.51));
                            eyeBrush.GradientStops.Add(new GradientStop(fillColor, 1));

                            eyeBrush.RadiusX = length / width * 0.5;
                            eyeBrush.RadiusY = 0.5;

                            partShape.Fill = eyeBrush;
                        }
                        break;
                    case PatternType.SPOTS:
                        break;
                    case PatternType.PUPIL_EYE:
                        {
                            var color = usedLayer.ColorMod.CurrentPatterns[0].Original.colors[1];
                            var pupilColor = Color.FromRgb((byte)color.red, (byte)color.green, (byte)color.blue);
                            var eyeBrush = new RadialGradientBrush();
                            eyeBrush.GradientStops.Add(new GradientStop(pupilColor, 0));
                            eyeBrush.GradientStops.Add(new GradientStop(pupilColor, 0.2));
                            eyeBrush.GradientStops.Add(new GradientStop(fillColor, 0.21));
                            eyeBrush.GradientStops.Add(new GradientStop(fillColor, 1));

                            eyeBrush.RadiusX = length / width * 0.5;
                            eyeBrush.RadiusY = 0.5;

                            partShape.Fill = eyeBrush;
                        }
                        break;
                    case PatternType.MOTTLED:
                        break;
                    default:
                        break;
                }
            }

            #endregion

            Vector direction = (pos - parentPos);
            if (direction.LengthSquared < 0.0001)
                direction = new Vector(0, -1);
            direction.Normalize();

            #region Categorization

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
            List<BodyPart> HeadParts = new List<BodyPart>();
            List<BodyPart> BackParts = new List<BodyPart>();
            List<BodyPart> TailParts = new List<BodyPart>();
            List<BodyPart> LeftRibParts = new List<BodyPart>();
            List<BodyPart> RightRibParts = new List<BodyPart>();

            foreach (var child in part.Children)
            {
                if (!child.IsInternal)
                {
                    if (child.LeadsToHead)
                        HeadParts.Add(child);
                    else
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
                            case "SHELL":
                            case "HUMP":
                                BackParts.Add(child);
                                break;
                            case "FIN":
                                if (child.IsLeft && !part.IsLeft)
                                    LeftParts.Add(child);
                                else if (child.IsRight && !part.IsRight)
                                    RightParts.Add(child);
                                else
                                    BackParts.Add(child);
                                break;
                            case "TAIL":
                                TailParts.Add(child);
                                break;
                            case "RIB_TRUE":
                                for (int i = 0; i < 7; i++)
                                    if (child.OriginalPart.token.StartsWith("L_"))
                                        LeftRibParts.Add(child);
                                    else
                                        RightRibParts.Add(child);
                                break;
                            case "RIB_FALSE":
                                for (int i = 0; i < 3; i++)
                                    if (child.OriginalPart.token.StartsWith("L_"))
                                        LeftRibParts.Add(child);
                                    else
                                        RightRibParts.Add(child);
                                break;
                            case "RIB_FLOATING":
                                for (int i = 0; i < 2; i++)
                                    if (child.OriginalPart.token.StartsWith("L_"))
                                        LeftRibParts.Add(child);
                                    else
                                        RightRibParts.Add(child);
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

            #endregion

            #region ChildrenCreation

            for (int i = 0; i < LeftEarParts.Count; i++)
            {
                BodyPart child = LeftEarParts[i];
                double rotateAngle = 0;
                double childWidth = 0;
                foreach (BodyPartMod mod in child.AppearanceMods)
                {
                    if (mod.Original.type == "SPLAYED_OUT")
                    {
                        childWidth = ((mod.CurrentValue - 100) / 100.0) * VolumeToDiameterConverter.Convert((child.OriginalPart.relsize * creatureScale) * visualScale);
                    }
                }
                if (part.IsLowerBody)
                    if (root)
                        rotateAngle = Range(i, LeftEarParts.Count, 80, 100);
                    else
                        rotateAngle = Range(i, LeftEarParts.Count, -20, -90);
                else
                    rotateAngle = Range(i, LeftEarParts.Count, 80, 100);
                AddPart(canvas, pos, child, pos + (direction * ((width / 2) + childWidth)).Rotate(rotateAngle), creatureScale, visualScale, sizeMod, true);
            }
            for (int i = 0; i < RightEarParts.Count; i++)
            {
                BodyPart child = RightEarParts[i];
                double rotateAngle = 0;
                double childWidth = 0;
                foreach (BodyPartMod mod in child.AppearanceMods)
                {
                    if (mod.Original.type == "SPLAYED_OUT")
                    {
                        childWidth = ((mod.CurrentValue / 200.0) - 0.5) * VolumeToDiameterConverter.Convert((child.OriginalPart.relsize * creatureScale) * visualScale);
                    }
                }
                if (part.IsLowerBody)
                    if (root)
                        rotateAngle = Range(i, RightEarParts.Count, -80, -100);
                    else
                        rotateAngle = Range(i, RightEarParts.Count, 20, 90);
                else
                    rotateAngle = Range(i, RightEarParts.Count, -80, -100);
                AddPart(canvas, pos, child, pos + (direction * ((width / 2) + childWidth)).Rotate(rotateAngle), creatureScale, visualScale, sizeMod, true);
            }
            for (int i = 0; i < BackParts.Count; i++)
            {
                BodyPart child = BackParts[i];
                double rotateAngle = 0;
                if (part.IsLeft)
                    rotateAngle = Range(i, BackParts.Count, -45, 45);
                else
                    rotateAngle = Range(i, BackParts.Count, 45, -45);
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale, sizeMod, true);
            }
            for (int i = 0; i < TailParts.Count; i++)
            {
                BodyPart child = TailParts[i];
                double rotateAngle = Range(i, TailParts.Count, 60, 90);
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale, sizeMod);
            }

            //Anything before this is below the part.
            canvas.Children.Add(partShape);
            //Anything after this is above the part.

            for (int i = 0; i < ExternalParts.Count; i++)
            {
                BodyPart child = ExternalParts[i];
                double rotateAngle = 0;
                if (part.IsLeft)
                    rotateAngle = Range(i, ExternalParts.Count, -45, 45);
                else
                    rotateAngle = Range(i, ExternalParts.Count, 45, -45);
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale, sizeMod);
            }
            for (int i = 0; i < HeadParts.Count; i++)
            {
                BodyPart child = HeadParts[i];
                double rotateAngle = 0;
                if (part.IsLeft)
                    rotateAngle = Range(i, HeadParts.Count, -45, 45);
                else
                    rotateAngle = Range(i, HeadParts.Count, 45, -45);
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale, sizeMod);
            }
            for (int i = 0; i < LowerBodyParts.Count; i++)
            {
                BodyPart child = LowerBodyParts[i];
                double rotateAngle = Range(i, LowerBodyParts.Count, 135, 225);
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale, sizeMod);
            }
            for (int i = 0; i < EmbeddedParts.Count; i++)
            {
                BodyPart child = EmbeddedParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(90);
                childPos *= Range(i, EmbeddedParts.Count, -length / 4, length / 4);
                AddPart(canvas, pos, child, pos + childPos, creatureScale, visualScale, sizeMod, true);
            }
            for (int i = 0; i < RightRibParts.Count; i++)
            {
                BodyPart child = RightRibParts[i];
                Vector childPos = (direction.Rotate(90) * width * 0.25) + (direction * Range(i, RightRibParts.Count, length * 0.45, -length * 0.45));
                Vector fakeParentPos = direction * Range(i, RightRibParts.Count, length * 0.45, -length * 0.45);
                AddPart(canvas, fakeParentPos, child, pos + childPos, creatureScale, visualScale, sizeMod, true);
            }
            for (int i = 0; i < LeftRibParts.Count; i++)
            {
                BodyPart child = LeftRibParts[i];
                Vector childPos = (direction.Rotate(-90) * width * 0.25) + (direction * Range(i, LeftRibParts.Count, length * 0.45, -length * 0.45));
                Vector fakeParentPos = direction * Range(i, LeftRibParts.Count, length * 0.45, -length * 0.45);
                AddPart(canvas, fakeParentPos, child, pos + childPos, creatureScale, visualScale, sizeMod, true);
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
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale, sizeMod);
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
                AddPart(canvas, pos, child, pos + (direction * length / 2).Rotate(rotateAngle), creatureScale, visualScale, sizeMod);
            }
            for (int i = 0; i < EyelidParts.Count; i++)
            {
                BodyPart child = EyelidParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(90);
                childPos *= Range(i, EyelidParts.Count, -length * 0.2, length * 0.2);
                AddPart(canvas, pos, child, pos + childPos + direction * length * 0.1, creatureScale, visualScale, sizeMod, true);
            }
            for (int i = 0; i < CheekParts.Count; i++)
            {
                BodyPart child = CheekParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(90);
                childPos *= Range(i, CheekParts.Count, -length * 0.2, length * 0.2);
                AddPart(canvas, pos, child, pos + childPos + direction * length * -0.2, creatureScale, visualScale, sizeMod, true);
            }
            for (int i = 0; i < EyeParts.Count; i++)
            {
                BodyPart child = EyeParts[i];
                double closeSet = 1;
                foreach (BodyPartMod mod in child.AppearanceMods)
                {
                    if (mod.Original.type == "CLOSE_SET")
                    {
                        closeSet = (mod.CurrentValue / 100.0) + 0.001;
                    }
                }
                Vector childPos = direction;
                childPos = childPos.Rotate(90);
                childPos *= Range(i, EyeParts.Count, -length * 0.1, length * 0.1);
                childPos *= closeSet;
                AddPart(canvas, pos, child, pos + childPos, creatureScale, visualScale, sizeMod, false);
            }
            for (int i = 0; i < MouthParts.Count; i++)
            {
                BodyPart child = MouthParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(180);
                childPos.Normalize();
                childPos *= length / 2;
                childPos *= Range(i, MouthParts.Count, 0.75, 0.5);
                AddPart(canvas, pos, child, pos + childPos, creatureScale, visualScale, sizeMod, true);
            }
            for (int i = 0; i < LipParts.Count; i++)
            {
                BodyPart child = LipParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(180);
                childPos.Normalize();
                childPos *= length / 2;
                childPos *= Range(i, LipParts.Count, 0.75, 0.6);
                AddPart(canvas, pos, child, pos + childPos, creatureScale, visualScale, sizeMod, true);
            }
            for (int i = 0; i < TuskParts.Count; i++)
            {
                BodyPart child = TuskParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(90);
                childPos *= Range(i, TuskParts.Count, -length * 0.2, length * 0.2);
                AddPart(canvas, pos, child, pos + childPos + direction * length * 0.5 * -0.75, creatureScale, visualScale, sizeMod, true);
            }
            for (int i = 0; i < NoseParts.Count; i++)
            {
                BodyPart child = NoseParts[i];
                Vector childPos = direction;
                childPos = childPos.Rotate(180);
                childPos.Normalize();
                childPos *= length / 2;
                childPos *= Range(i, NoseParts.Count, 0.1, 0.5);
                AddPart(canvas, pos, child, pos + childPos, creatureScale, visualScale, sizeMod, true);
            }
            #endregion
        }

        private static Shape MakeCylinder(BodyPartRaw originalPart, double ratio, double creatureScale, double visualScale, out double width, out double length)
        {
            width = GetRatioCylinderWidth(originalPart.relsize * creatureScale, ratio) * visualScale;
            length = width * ratio;
            return new Rectangle();
        }

        private static double Range(int i, int count, double min, double max)
        {
            if (count == 1)
                return (min + max) / 2;
            else
                return min + (i * (max - min) / (count - 1));
        }

        static readonly double cubeRootPi = Math.Pow(Math.PI, 1.0 / 3.0);
        static readonly double cubeRootSquare2 = Math.Pow(2.0, 2.0 / 3.0);

        static double GetRatioCylinderWidth(double volume, double ratio)
        {
            return (cubeRootSquare2 * Math.Pow(volume, 1.0 / 3.0)) / (cubeRootPi * Math.Pow(ratio, 1.0 / 3.0));
        }

        void FitCanvas(Canvas canvas)
        {
            if (canvas.Children.Count == 0)
                return;
            FrameworkElement firstChild = canvas.Children[0] as FrameworkElement;
            Rect totalBounds = new Rect();

            foreach (var child in canvas.Children)
            {
                FrameworkElement element = child as FrameworkElement;
                if (element == null)
                    continue;

                Rect bounds = new Rect(0, 0, element.ActualWidth, element.ActualHeight);

                bounds.Transform(element.RenderTransform.Value);

                bounds.X += Canvas.GetLeft(element);
                bounds.Y += Canvas.GetTop(element);


                totalBounds.Union(bounds);
            }

            canvas.Width = totalBounds.Width;
            canvas.Height = totalBounds.Height;
            Width = canvas.Width;
            Height = canvas.Height;
            MinWidth = canvas.Width;
            MinHeight = canvas.Height;

            foreach (var child in canvas.Children)
            {
                FrameworkElement element = child as FrameworkElement;
                if (element == null)
                    continue;

                Canvas.SetLeft(element, Canvas.GetLeft(element) - totalBounds.Left);
                Canvas.SetTop(element, Canvas.GetTop(element) - totalBounds.Top);
            }
        }
    }
}
