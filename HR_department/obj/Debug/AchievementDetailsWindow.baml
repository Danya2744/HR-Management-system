   M S B A M L   `   `   `  ����H  DHR_department, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:HR_departmentHR_department  5   	   ,    &HR_department.AchievementDetailsWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:HR_department     5   	     � Title$'  !Детали достижения��5   
   $	��700��5	   	   $	��600��6    � WindowStartupLocation$ CenterScreen=�5
   	    � 
ResizeMode$ NoResize=�5   	   ���	#FF2D2D30 � 	Resources 5         DetailButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType ��Name	 �� CornerRadius
 J� RenderTransformOrigin �� ScaleX �� ScaleY �� Triggers P� Property P� Value(  "System.Windows.Controls.DatePicker��%�  &	        '��     '�� �    '  �    �� 5   
   " ��6*   .�5      ��  � �� �	#FF007ACC6+   �� 5       � ��
 �����6+   �� 5       � �� �	#FF007ACC6,   �� 5       � ��$
 10,5q�6(   �� 5       � ��$ 14(�6)   �� 5       � ��$	 120��6&   �� 5       � ��$ 35��6'   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� $ border��6!   #��z��5   !   $	 5��5   !   #��z��5   !   $��1q�5   !   $
 0.5,0.5&�5    !   z�5!      �� 5"   "   $ 1Q�61   $ 1Q�6<   ._�5$      �� $��Center=�6/   $��Center=�6L   5%       5&      P� 5'       �@J�IsMouseOver . . �5(   "   ��  � �� �	#FF4BA4D96?   5)      5+      5-      5.      �� 50   
   " ��6   .�51      ��  � ��
 �����6+   �� 52       � ��$ 0,10,0,5q�6'   �� 53       � ��$ 14(�6)   54      �� 56   
   " ��6   .�57      ��  � �� �	#FF3E3E406+   �� 58       � ��
 �����6+   �� 59       � �� �	#FF3E3E406,   �� 5:       � ��$ 1q�60   �� 5;       � ��$ 5q�6(   �� 5<       � ��$ 0,0,0,10q�6'   5=      �� 5?   
   "  6   .�5@      ��  � �� �	#FF3E3E406+   �� 5A       � ��
 �����6+   �� 5B       � �� �	#FF3E3E406,   �� 5C       � ��$ 0,0,0,10q�6'   5D      5E      .��5G      � .+�5H   
   �� ���	#FF252526$	 10��6(   ���	#FF3E3E40$��1q�6R   $��Center=�5I      $��Center=�6,    �� Padding$ 20q�6I   $	��500��6V   $	��584��6b   ._�5J      �� .+�5K      �� $"��ФИО сотрудника:��6   $
��Bold"�63   �� 5L      $��14(�6A    �� 
IsReadOnly . ��6   �� *
FullName+ �� Mode$ OneWay=�6O   �� 5N      $$��Дата достижения:��6     5O      
���   �{�.     SelectedDate 6   �� *AchievementDate+$ OneWay=�6f   �� 5Q      $"��Тип достижения:��6   �� 5R       . ��6   �� *AchievementType+$ OneWay=�6H   �� 5T      $��Статус:��6   �� 5U      $
��Bold"�6?    . ��6   �� *Status+$ OneWay=�.��6c   ��5V      �� 5W      " ��6     �� BasedOn# [B��65    �� Triggers 5X      �� 5Y   "    �� Binding 6.   �� *Status+ �� Value Присуждена6I   .a�5Z   &   ��  � �� �	#FF1C4A2D6C   5[   #   �� 5\   "    6.   �� *Status+ Отменена6I   .a�5]   &   ��  � �� �	#FF4A1C1C6C   5^   #   5`      .��5a   %   5b      �� 5d      $��Описание:��6   �� 5e      $��80��6D    �� TextWrapping$
 Wrap=�6P    �� AcceptsReturn . " �� VerticalScrollBarVisibility$
 Auto=�5f   /    . ��5e      �� *Description+$ OneWay=�5f   R   �� 5h      $��Награда:��6   �� 5i       . ��6   �� *Reward+$ OneWay=�6?   �� 5k       �� Orientation$ 
Horizontal=�6   $��Right=�66   $��0,15,0,0q�6R   .+�5l      �� -   5        $��Закрыть��5l      #��[  6/   5m      5n      5o      5p      5q      5r      