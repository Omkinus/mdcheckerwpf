using System;
using System.Windows;
using Tekla.Structures.Drawing;
using static Tekla.Structures.Drawing.StraightDimensionSet;
using tsm = Tekla.Structures.Model;

namespace mdcheckerwpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        tsm.Model _model = new tsm.Model();
        
        void drawingcheck()
        {
           
            
            DrawingHandler _drawinghandler = new DrawingHandler();

            try
            {
                if (!_model.GetConnectionStatus() || !_drawinghandler.GetConnectionStatus())
                    return;

                Tekla.Structures.Drawing.UI.DrawingSelector _currentdrawings = _drawinghandler.GetDrawingSelector();
                Tekla.Structures.Drawing.DrawingEnumerator _selecteddrawings = _currentdrawings.GetSelected();

                string listofnames = "";

                foreach (var _currentdrawing in _selecteddrawings)
                {

                    Tekla.Structures.Drawing.Drawing _currentdrawing1 = _currentdrawing as Tekla.Structures.Drawing.Drawing;

                    string drawingnumber = _currentdrawing1.Mark;

                    listofnames += drawingnumber;

                    //Вызываем поочередно функции проверок


                    check_precision(_currentdrawing1, out string message_checkprecision);


                    check_reflectedview(_currentdrawing1, out string message_reflectedview);

                    listofnames +=
                        message_reflectedview +
                        message_checkprecision +
                        "\n"
                        ;

                }

                MessageBox.Show(listofnames, "АЛЕРТ");

            }
            catch (Exception)
            {

                throw;
            }

            void check_precision(Tekla.Structures.Drawing.Drawing drawing, out string message_checkprecision)
            {
                try
                {
                    message_checkprecision = "";

                    if (drawing == null)
                    {
                        return;
                    }

                    ContainerView sheet = drawing.GetSheet();
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
                                    message_checkprecision += "Присутствует размер округленный не на 1/16 \n";
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
                                    message_checkprecision += "Присутствует угловой размер округленный не на 1/100 \n";
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

            void check_reflectedview(Tekla.Structures.Drawing.Drawing drawing, out string message_reflectedview)
            {
                try
                {
                    message_reflectedview = "";

                    if (drawing == null)
                    {
                        return;
                    }

                    ContainerView sheet = drawing.GetSheet();
                    DrawingObjectEnumerator allviews = sheet.GetAllViews();

                    string views = "";

                    foreach (var view1 in allviews)
                    {
                        Tekla.Structures.Drawing.View view2 = view1 as Tekla.Structures.Drawing.View;

                        if (view2.Attributes.ReflectedView == true)
                        {

                            Tekla.Structures.Drawing.ContainerElement view2name = view2.Attributes.TagsAttributes.TagA1.TagContent;

                            string viewname = "";
                            viewname += "(";

                            foreach (var textitems in view2name)
                            {
                                if (textitems is Tekla.Structures.Drawing.TextElement)
                                {
                                    Tekla.Structures.Drawing.TextElement item = textitems as Tekla.Structures.Drawing.TextElement;
                                    string itemstring = item.Value.ToString();

                                    viewname += itemstring;

                                };

                                if (textitems is Tekla.Structures.Drawing.PropertyElement)
                                {
                                    Tekla.Structures.Drawing.PropertyElement item = textitems as PropertyElement;
                                    string itemstring = item.Value.ToString();

                                    viewname += itemstring;

                                };
                            }

                            viewname += ")";

                            if (viewname == "()") 
                            {
                                viewname = "(MAIN VIEW)";
                            }

                            views += viewname;

                        }

                    }
                    if (views != "")
                    {

                        message_reflectedview = "Присутствует вид c включенным Reflected View (" + views + ")";

                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            
            void test() {
                
                
                Tekla.Structures.Drawing.Drawing curdraw = _drawinghandler.GetActiveDrawing();
                ContainerView sheet = curdraw.GetSheet();
                DrawingObjectEnumerator allviews = sheet.GetAllViews();

                foreach (var view in allviews)
                {
                    Tekla.Structures.Drawing.View view2 = view as View;
                    Tekla.Structures.Drawing.ContainerElement view2name = view2.Attributes.TagsAttributes.TagA1.TagContent;
                    MessageBox.Show(view2name.ToString(), "АЛЕРТ");
                }

            }
        }

       
    }

}
