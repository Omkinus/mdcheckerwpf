   M S B A M L   `   `   `  ����   mdcheckerwpf&     mdcheckerwpf.MVVM.View.ModelPage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � 	Resources     GroupedDataView �� Source �� GroupDescriptions2  ,System.Windows.Data.PropertyGroupDescription   PropertyName&   System.Windows.Controls.DataGrid �� 
TargetType  ModernDataGridStyle �� Property �� Value �� 
TargetType �� CornerRadius	 ��Name
 � RowDefinitionsG  ASystem.Windows.Controls.Primitives.DataGridColumnHeadersPresenter=  7System.Windows.Controls.Primitives.DataGridColumnHeader)  #System.Windows.Controls.DataGridRow �� Triggers P� Property P� Value*  $System.Windows.Controls.DataGridCell��%�  &	        &	 5     '  �    '  �    '  '    ��  �� *	DataItems+   $ 	Errortype���� "  .���  � ��
 �������  �@ 	RowHeight$ 30Q���   �@ AlternatingRowBackground �#F7F7F7��  �@ GridLinesVisibility$
 None=���  �@ ColumnHeaderHeight$ 35Q���  � ��$ 14(���  � ��$ Segoe UI Semibold)���  � ��$ 0,10,0,0q���  � �� �� "  .R��� $ 1��#��z��._��� $	 DG_ScrollViewer��|�.  .��� 
 �� $
��Auto ��� $��* �.+��� $��1��  $��0���� "  .���  � �� �	#FFF3F3F3��  � �� �	#FF323232��  � ��$
 Bold"���  � ��$ Center=���  � ��$
 10,5q���  � ��$ 40���� "  .���  � ��$ 0q���  � ��$
 Left=���  � ��
 �   ���  � ��$ Center=���  � ��$ Segoe UI)���  � ��$ 13(���  � �� Z� ��  � ��$ 25����  �@�Cursor$
 Hand�� P�  �@J�IsMouseOver . . ���  � �� �	#FFEBEBEBP�  �@ 
IsSelected . . ���  � �� �	#FF4A90E2��  � ��
 ������� "  .���  � ��$ 0,0,1,0q���  � �� �#E0E0E0��  � ��$
 10,5q���  �@��TextAlignment$ Center=���  � ��$ Center=�.��� -    �Name$ 	ModelGrid��$	��874��$	��688�� � ColumnDefinitions �� $
��119* ��� $
��352* �
 �� $��64 ��� $	��560 ��� $��54 �.+��� $#��Проверка модели�����	#FF2B3460$��Center=�$	��Top=�$��32(�$��Yu Gothic UI Semibold)� �� TextAlignment$ Center=�$��
-220,0,0,0q�$��0��$��1��  -     Name$ ModelDataGrid��$��Center=�$��Center=�$	��842��$	��522��   AutoGenerateColumns .     CanUserAddRows .     CanUserResizeColumns .    HeadersVisibility$ Column=�#��[ $��1��$��2������  �� Source# [   �� 
GroupStyle (  "System.Windows.Controls.GroupStyle     HeaderTemplate �� .R���  �� Orientation$ 
Horizontal=�.+��� $
��Bold"�$��16(�
���OO/����� *Name+   ContainerStyle �� " ��.���  � �� �� " ��.R�9�  9� 
IsExpanded . ���� .����    Columns 0  *System.Windows.Controls.DataGridTextColumn  ,	  &System.Windows.Controls.DataGridColumn 	  Header$ 
Марка�� 	  Width5
  /System.Windows.Controls.DataGridLengthConverter$	 100
 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *ObjectNumber+  $ Имя��$	 150
  �� *
ObjectName+  $ Значение��$	 562
  �� *Description+� $��1��$��
0,551,0,-7q�$��2��$��2�� �� $
��263* ��� $
��254* ��� $
��357* �.+��� $ 
Horizontal=�$
��Left=�$��
10,-11,0,1q�$��0��$��3��.+��� -   $��Загрузить��  LoadButtonStyle#��[ $��7,9,0,9q��� -     ��Name$  SaveModelReportButton��$��Сохранить��  SaveButtonStyle#��[ $��10,0,0,0q��� $y�Hidden=�$��Clash check��  ClashCheckButtonStyle#��[ $��10,0,0,0q��� -   $!��Новая проверка��  NewCheckButtonStyle#��[ $��Right=�$��	0,3,16,15q�$��2������ ! �� EndPoint$	! 0,0&�" �� 
StartPoint$	" 0,1&�.��� # � Color$# 	#FF8AD855��$ � Offset$$ 1Q�� $# 	#FF43C127���� $��Имя модели:�����	#FF2B3460$��Center=�$	��Top=�$��20(�$��Yu Gothic UI Semilight)�$ Center=�$��
16,38,90,0q�$��0���� ���	#FF2B3460$
��Left=�$	��Top=�$��20(�$��Yu Gothic UI Semibold)�$ Center=�$��
140,38,0,0q�$��0��$��2��$��2��$	��356������ *	ModelName+� -   % �Name$% ProgressBar��$��40��$	��300��$y�	Collapsed=�& � Minimum$& 0Q�' � Maximum$	' 100Q�$��Center=�$��Center=�$��1��$��61,253,292,268q�$��1���� -   ( ��Name$( StatusMessage��$y�	Collapsed=�$��Загрузка...��$��Center=�$��Center=�$��171,219,398,318q�$��1��$��1��$��18(�$��84��$*��$/mdcheckerwpf;component/Fonts/#Inter)��� -   $  Button_SelectModel��$$��Выбрать в модели��$��1��$
��Left=�$��
469,35,0,0q�$	��Top=�$	��168��$��41��
�������$��2��$��	Helvetica)�$��16(���Z� ���� $	! 0,0&�) � MappingMode$) RelativeToBoundingBox=�$	" 0,1&�.��� $# 	#FFDEBA3F��$$ 0Q�� $# 	#FFF1CC50��$$ 1Q�