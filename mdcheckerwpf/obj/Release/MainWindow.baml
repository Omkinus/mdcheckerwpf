   M S B A M L   `   `   `  ���� G  Cmdcheckerwpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:mdcheckerwpfmdcheckerwpf      mdcheckerwpf.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:mdcheckerwpf       � Title$  
MainWindow��$	��788��$
��1044�� � WindowStartupLocation$ CenterScreen=� � WindowStyle$
 None=����	#FF1B2627 � AllowsTransparency .  � 
ResizeMode$ NoResize=�$��Script MT Bold)���Z� .��� .+���  �� CornerRadius$ 1��
������  �� Padding$ 0q�$��5q� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     ShadowDepth$ 0Q�	   
BlurRadius$	 5Q�
   Color$
 	#80000000��._��� $ 1��
�������._��  � ColumnDefinitions �� $	��150 ��� $��2 ��� $
��275* ��� $
��161* � � RowDefinitions �� $��80 ��� $��* �.+���  �� Orientation$ Vertical=�$
��Left=�$��Stretch=�$��0��$��0��$��2��$��
10,10,0,10q�.+��� $ 20��$ 10q�$��0,0,0,40q�$��40������  �� EndPoint$	 1,1&� �� 
StartPoint$	 0,0&�.���  � Color$ 	#FF3F7BE4�� � Offset$ 0Q�� $ 	#FF3F7BE4��$ 1Q�._�� .+�F� +����        р����р������    ���	�� $��Center=�$��Center=�$��Preview��$��24(�
�������$*��$/mdcheckerwpf;component/Fonts/#Inter)� �� 
LineHeight$ 14��	� -   $��Модель��
���   �   MenuButtonTheme#��[  $��	-30,0,0,0q� � Cursor$
 Hand�� 	� 	GroupName$ MenuButtons��
 � Tag$ 	ModelPage��$
��Left=�$��ScriptC)�	� -   $��Чертежи��
���   �#��[  $��	-30,0,0,0q�$
 Hand��$ MenuButtons��$ DrawingsPage��$
��Left=��� $��Bottom=�$��-30,450,0,10q�.+�	� -   $��Настройки��
���   �#��[  $��0,0,0,0q�$
 Hand��$ MenuButtons��$ SettingsPage��$
��Left=�	� -   $��Помощь��#��[  $��0,0,0,0q�$
 Hand��$ MenuButtons��$ HelpPage��$
��Left=�
���   ��� -    ��Name$ ContentArea��$��1��$��2��$��Stretch=�$��Bottom=�$	��874��$	��688��$��2��$��0,0,-2,0q����� *SelectedPage+���� $ 0.5,1&�$ 0.5,0&�.��� $ Black��$ 0Q�� $ White��$ 1Q��� -    ��Name$ CloseButton��$��Right=�$��	0,10,10,0q�$	��Top=�$��40��$��40��$
 Hand��$��3����Z� ����  �� 
TargetType" ��.R��� 
������ $ 20��._���  ��Name$ CloseBorder��
������ ._���  ��Name$ 
CloseImage��$��Resources/closebutton.png�� �� Triggers P�  P� Property �@J�IsMouseOver P� Value . . ���  �� 
TargetName$ CloseBorder�� �� Property � ��  �� Value  �	#FF8F0101�� $ 
CloseImage�� �@J�Opacity$	  0.8Q�P�  � �� . . ��� $ 
CloseImage�� �@J�Opacity$	  0.6Q�