   M S B A M L   `   `   `  ����G  Cmdcheckerwpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:mdcheckerwpf.MVVM.Viewmdcheckerwpf.MVVM.View  5   	       mdcheckerwpf.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7vmView$clr-namespace:mdcheckerwpf.MVVM.View     5   	     � Title$  
MainWindow��5   	   $	��788��5	   	   $
��1044��6    � WindowStartupLocation$ CenterScreen=�5
   	    � WindowStyle$
 None=�5   	   ���	#FF1B2627 � AllowsTransparency .  � 
ResizeMode$ NoResize=�5   	   $��Script MT Bold)�5   	   ��5   	   Z� .��5      � .+�5   
   ��  �� CornerRadius$ 1��6   
������  �� Padding$ 0q�6;   $��5q�6G    J� Effect 5      3  -System.Windows.Media.Effects.DropShadowEffect  5         ShadowDepth$ 0Q�6#   	   
BlurRadius$	 5Q�63   
   Color$
 	#80000000��6B   ._�5      �� $ 1��6   
�������._�5      �  � ColumnDefinitions 5      �� 5      $	��150 �6+   �� 5      $��2 �6+   �� 5      $��* �6+   �� 5      $
��161* �6+    � RowDefinitions 5       �� 5!      $��80 �6(   �� 5"      $��* �6(   .+�5%      �� $��0��6!   $��0��61   $��2��6>    �� Orientation$ Vertical=�5&   !   $
��Left=�5'   !   $��Stretch=�5(   !   $��
10,10,0,10q�5)   !   .+�5+      �� $ 20��6!   $ 10q�63   $��0,0,0,40q�6@   $��40��6R   ��5,      �� 5-   "    �� EndPoint$	 1,1&�66    �� 
StartPoint$	 0,0&�6E   .��5.   &   �  � Color$ 	#FF3F7BE4��63    � Offset$ 0Q�6E   � 5/   &   $ 	#FF8AC3DB��63   $ 1Q�6E   50   #   ._�52      � .+�53   "   F� +����        р����р������    ���	6'   �� 54   "   $��Center=�6,   $��Center=�55   ,   $��Preview��56   ,   $��24(�57   ,   
�������$*��$/mdcheckerwpf;component/Fonts/#Inter)�59   ,    �� 
LineHeight$ 14��5:   ,   5;      5<      	� 5?      -   5         	�Name$ rbModel��5?   &   $��Модель��5@   &   
 � Tag$ 	ModelPage��5A   &    	� 	GroupName$ MenuButtons��5B   &      MenuButtonTheme#��[  5C   &   $��	-30,0,0,0q�5D   &    � Cursor$
 Hand��5E   &   $
��Left=�5F   &   $��ScriptC)�5G   &   
���   �5I   &   	� 5K      -   5        $ 
rbDrawings��5K   &   $��Чертежи��5L   &   $ DrawingsPage��5M   &   $ MenuButtons��5N   &   #��[  5O   &   $��	-30,0,0,0q�5P   &   $
 Hand��5Q   &   $
��Left=�5R   &   
���   �5T   &   �� 5V      $��Bottom=�6%   $��-30,450,0,10q�6@   .+�5W      	� -   5        $ 
rbSettings��5W   *   $��Настройки��5X   *   $ SettingsPage��5Y   *   $ MenuButtons��5Z   *   #��[  5[   *   $
 Hand��5\   *   $
��Left=�5]   *   
���   �5_   *   	� 5a      -   5        $ rbHelp��5a   *   $��Помощь��5b   *   $ HelpPage��5c   *   $ MenuButtons��5d   *   #��[  5e   *   $
 Hand��5f   *   $
��Left=�5g   *   
���   �5i   *   5j      5k      �� 5n      -   5         ��Name$ ContentArea��5n   %   $��1��5o   %   $��2��62   $��2��6B   $��Stretch=�5p   %   $��Stretch=�5q   %   $��0,0,-2,0q�5r   %   ��5s      �� 5t      $ 0.5,1&�62   $ 0.5,0&�6C   .��5u   "   � $ Black��6/   $ 0Q�6=   � 5v   "   $ White��6/   $ 1Q�6=   5w      5y      �� 5|      -   5         ��Name$ CloseButton��5|      $��3��5}      $��Right=�5~      $��	0,10,10,0q�5      $	��Top=�5�      $��40��5�      $��40��6(   $
 Hand��5�      ��5�      Z� ��5�      �� 5�       �� 
TargetType" ��6.   .R�5�   "   �� 
������ $ 20��6B   ._�5�   &   ��  ��Name$ CloseBorder��6-   
������ ._�5�   *   ��  ��Name$ 
CloseImage��60   $��Resources/closebutton.png��5�   0   5�   '   5�   #    �� Triggers 5�   "   P� 5�   &    P� Property �@J�IsMouseOver P� Value . . �5�   *   ��  �� 
TargetName$ CloseBorder��61     �� Property  � ��! �� Value! �	#FFE816166`   �� 5�   *   $ 
CloseImage��61     �@J�Opacity$	! 0.8Q�6\   5�   '   P� 5�   &    � �� . . �5�   *   �� $ 
CloseImage��61     �@J�Opacity$	! 0.6Q�6\   5�   '   5�      5�      5�      5�      5�      5�      5�      