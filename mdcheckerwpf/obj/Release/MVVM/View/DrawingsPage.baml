   M S B A M L   `   `   `  ����   mdcheckerwpf)    #mdcheckerwpf.MVVM.View.DrawingsPage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � 	Resources  &   System.Windows.Controls.DataGrid �� 
TargetType   ModernDataGridStyle �� Property �� Value �� 
TargetType �� CornerRadius ��Name � RowDefinitionsG  ASystem.Windows.Controls.Primitives.DataGridColumnHeadersPresenter=  7System.Windows.Controls.Primitives.DataGridColumnHeader)  #System.Windows.Controls.DataGridRow �� Triggers	 P� Property
 P� Value*  $System.Windows.Controls.DataGridCell��%�  &	        '  �    '  l    '      �� "  .���  � ��
 �������  �@ 	RowHeight$ 30Q���   �@ AlternatingRowBackground �#F7F7F7��  �@ GridLinesVisibility$
 None=���  �@ ColumnHeaderHeight$ 35Q���  � ��$ 14(���  � ��$ Segoe UI Semibold)���  � ��$ 0,10,0,0q���  � �� �� "  .R��� $ 1��#��z��._��� $ DG_ScrollViewer��|�.  .���  �� $
��Auto ��� $��* �.+��� $��1��  $��0���� "  .���  � �� �	#FFF3F3F3��  � �� �	#FF323232��  � ��$
 Bold"���  � ��$ Center=���  � ��$
 10,5q���  � ��$ 40���� "  .���  � ��$ 0q���  � ��$ 0q���  � ��$ Stretch=���  � ��
 �   ���  � ��$ Segoe UI)���  � ��$ 13(���  � ��
 �������  � ��$ 25����  �@�Cursor$
 Hand����  �@ 
IsSelected .   P� 	 �@J�IsMouseOver
 . . ���  � �� �	#FFEBEBEBP� 	 �@ 
IsSelected
 . . ���  � �� �	#FF4A90E2��  � ��
 ������� "  .���  � ��$ 0,0,1,0q���  � �� �#E0E0E0��  � ��$
 10,5q���  �@��TextAlignment$ Center=���  � ��$ Center=�.��� -    �Name$ DrawingGrid��$	��874��$	��688�� � ColumnDefinitions �� $
��119* ��� $
��352* � �� $��64 ��� $	��560 ��� $��54 �.+��� $'��!Проверка чертежей�����	#FF2B3460$��Center=�$	��Top=�$��32(�$��Yu Gothic UI Semibold)� �� TextAlignment$ Center=�$��
-220,0,0,0q�$��0��$��1���� $��Имя модели:�����	#FF2B3460$��Center=�$	��Top=�$��20(�$��Yu Gothic UI Semilight)�$ Center=�$��
16,38,90,0q�$��0���� ���	#FF2B3460$
��Left=�$	��Top=�$��20(�$��Yu Gothic UI Semibold)�$ Center=�$��136,38,623,0q�$��0��$��2������ *	ModelName+  $��Center=�$��Center=�$	��842��$	��522��   AutoGenerateColumns .     CanUserAddRows .     CanUserResizeColumns .    HeadersVisibility$ Column=�#��[  $��1��$��2������ *	DataItems+����  �� Color$ White�� �� Opacity$	 0.4Q�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ 
Номер��   Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding �� *DrawingMark+  $ Подробности��$	 442  �� *	Details+  $% Название чертежа��$	 300  �� *DrawingName+� -    �Name$ ProgressBar��$��40��$	��300��$y�	Collapsed=� � Minimum$ 0Q� � Maximum$	 100Q�$��Center=�$��Center=�$��1��$��61,253,292,268q�$��1���� -    ��Name$ ProgressText��$y�	Collapsed=�$��Загрузка...��$��Center=�$��Center=�$��61,210,164,329q�$��1��$��18(�$	��428��$*��$/mdcheckerwpf;component/Fonts/#Inter)�$��1��� $��2��$��0,5,0,0q�$��2�� �� $
��517* ��� $
��357* �.+���  �� Orientation$ 
Horizontal=�$
��Left=�$��
10,-19,0,0q�$��0��$��2��.+��� -   $��Загрузить��  LoadButtonStyle#��[ $��7,9,0,9q��� -   $��Сохранить��  SaveButtonStyle#��[ $��10,0,0,0q��� -   $!��Новая проверка��  NewCheckButtonStyle#��[ $��	327,0,0,0q�����  �� EndPoint$	 0,0&� �� 
StartPoint$	 0,1&�.���  � Color$ 	#FF8AD855��  � Offset$  1Q�� $ 	#FF43C127��