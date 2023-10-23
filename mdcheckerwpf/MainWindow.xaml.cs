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
using Tekla.Structures.Drawing;
using tsm = Tekla.Structures.Model;
using Tekla.Structures.Drawing.UI;
using Tekla.Structures.Model;
using static Tekla.Structures.Model.ReferenceModel;
using static Tekla.Structures.Drawing.StraightDimensionSet;
using Tekla.Structures.Model.UI;
using System.Collections;

namespace mdcheckerwpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            testfunction();

            this.Close();

            InitializeComponent();
        }

        tsm.Model _model = new tsm.Model();
        DrawingHandler _drawinghandler = new DrawingHandler();
        

        void check_precision()
        {
            try
            {
                if (!_model.GetConnectionStatus() || !_drawinghandler.GetConnectionStatus())
                    return;

                Tekla.Structures.Drawing.Drawing _currentdrawing = _drawinghandler.GetActiveDrawing();
                Tekla.Structures.Drawing.UI.Picker picker = _drawinghandler.GetPicker();

                ContainerView sheet = _currentdrawing.GetSheet();
                DrawingObjectEnumerator allviews = sheet.GetAllViews();

                foreach (var view1 in allviews)
                {
                    Tekla.Structures.Drawing.ViewBase view3 = view1 as Tekla.Structures.Drawing.ViewBase;
                    DrawingObjectEnumerator viewallobjects = view3.GetAllObjects();

                    foreach (var obj in viewallobjects)
                    {
                        if (obj.GetType() == typeof(StraightDimensionSet))
                        {
                            StraightDimensionSet _dimensionforcheck = obj as Tekla.Structures.Drawing.StraightDimensionSet;
                            StraightDimensionSetAttributes _dimattributes = _dimensionforcheck.Attributes as StraightDimensionSetAttributes;
                            DimensionSetBaseAttributes.DimensionFormatAttributes _dimensionformat = _dimattributes.Format;
                            DimensionSetBaseAttributes.DimensionValuePrecisions _dimprecision = _dimensionformat.Precision;


                            if (_dimprecision.ToString() != "OnePerSixteen")
                            {
                                MessageBox.Show("Присутствует размер округленный не на 1/16", "АЛЕРТ");
                                break;
                            }

                        } //проверка обычных размеров на правильное округление 1/16

                        if (obj.GetType() == typeof(AngleDimension))
                        {
                            AngleDimension _angulardimensionforcheck = obj as Tekla.Structures.Drawing.AngleDimension;
                            AngleDimensionAttributes _angulardimattributes = _angulardimensionforcheck.Attributes as AngleDimensionAttributes;
                            AngleDimensionAttributes.DimensionFormatAttributes _angdimensionformat = _angulardimattributes.Format;
                            AngleDimensionAttributes.DimensionValuePrecisions _angledimprecision = _angdimensionformat.Precision;

                            if (_angledimprecision.ToString() != "OnePerHundred")
                            {
                                MessageBox.Show("Присутствует угловой размер округленный не на 1/100", "АЛЕРТ");
                                break;
                            }

                        } //проверка угловых размеров на правильное округление 1/100
                    }
                }

            }
            catch
            {
                throw;
            }
        }

        void check_reflectedview()
        {
            try
            {
                if (!_model.GetConnectionStatus() || !_drawinghandler.GetConnectionStatus())
                    return;

                Tekla.Structures.Drawing.Drawing _currentdrawing = _drawinghandler.GetActiveDrawing();
                Tekla.Structures.Drawing.UI.Picker picker = _drawinghandler.GetPicker();

                ContainerView sheet = _currentdrawing.GetSheet();
                DrawingObjectEnumerator allviews = sheet.GetAllViews();

                string views = "";

                foreach (var view1 in allviews)
                {
                    Tekla.Structures.Drawing.View view2 = view1 as Tekla.Structures.Drawing.View;

                    if (view2.Attributes.ReflectedView == true)
                    {

                        Tekla.Structures.Drawing.ContainerElement view2name = view2.Attributes.TagsAttributes.TagA1.TagContent;

                        views += "(";

                        foreach (var textitems in view2name)
                        {
                            if (textitems is Tekla.Structures.Drawing.TextElement)
                            {
                                Tekla.Structures.Drawing.TextElement item = textitems as Tekla.Structures.Drawing.TextElement;
                                string itemstring = item.Value.ToString();

                                views += itemstring;

                            };

                            if (textitems is Tekla.Structures.Drawing.PropertyElement)
                            {
                                Tekla.Structures.Drawing.PropertyElement item = textitems as PropertyElement;
                                string itemstring = item.Value.ToString();

                                views += itemstring;

                            };
                        }

                        views += ")";
                    }

                }
                if (views != "")
                {
                    MessageBox.Show("Присутствует вид c включенным Reflected View (" + views + ")", "АЛЕРТ");

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        void testfunction()
        {

            try
            {
                if (!_model.GetConnectionStatus() || !_drawinghandler.GetConnectionStatus())
                    return;

                Tekla.Structures.Drawing.UI.DrawingSelector _currentdrawings = _drawinghandler.GetDrawingSelector();
                Tekla.Structures.Drawing.DrawingEnumerator _selecteddrawings = _currentdrawings.GetSelected();

                string listofnames = null;

                foreach (var _currentdrawing in _selecteddrawings)
                {
                    check_precision();
                    Tekla.Structures.Drawing.Drawing _currentdrawing1 = _currentdrawing as Tekla.Structures.Drawing.Drawing;
                    string _currentdrawing1name = _currentdrawing1.Mark;
                    listofnames += _currentdrawing1name;
                    listofnames += "\n";
                }

                MessageBox.Show(listofnames, "АЛЕРТ");
            }
            catch (Exception)
            {

                throw;
            }



        }

    }
}
