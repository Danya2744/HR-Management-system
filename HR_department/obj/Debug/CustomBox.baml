   M S B A M L   `   `   `  ����H  DHR_department, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:HR_departmentHR_department  5   	       HR_department.CustomBox   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:HR_department     5   	     � Title$  CustomMessageBox��5   
   $��204.331��5	   	   $	��400��5
   	    � WindowStartupLocation$ CenterScreen=�5   	    � 
ResizeMode$ NoResize=�5   	   ���	#FF2D2D30���#323332$��4q�5   	    � WindowStyle$
 None=�5   	    � 	Resources 5         MessageBoxButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType	 ��Name
 �� CornerRadius �� Triggers P� Property P� Value��%�  &	        �� 5   
   " ��6.   .�5      ��  � �� �	#FF007ACC6+   �� 5       � ��
 �����6+   �� 5       � �� �	#FF007ACC6,   �� 5       � ��$
 10,5q�6(   �� 5       � ��$ 14(�6)   �� 5       � ��$ 80��6&   �� 5       � ��$ 30��6'   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� $	 border��6!   #��z��5    !   $
 5��5!   !   #��z��5"   !   $��1q�5#   !   ._�5$      �� $��Center=�6/   $��Center=�6L   5%       5&      P� 5'       �@J�IsMouseOver . . �5(   "   ��  � �� �	#FF4BA4D96?   5)      5+      5-      5.      5/      .��51      � .+�52   
   �� ���	#FF252526$
 10��6(   ���	#FF3E3E40$��1q�6R    �� Padding$ 20q�6f   $��	0,0,0,0.2q�6s   ._�53      �� $��0.2,0.2,0.2,-20.2q�6   .+�54      �� -   5         ��Name$ MessageText��54      $S��MВы уверены, что хотите удалить сотрудника?��61   
�������$��16(�6v   $
��Wrap=�6�   $��Center=�6�   $��0,0,0,20q�6�   �� 55      -   5         ��Name$ ButtonsPanel��55       �� Orientation$ 
Horizontal=�63   $��Center=�6L   .+�56      �� -   5         ��Name$ OkButton��56      $
��ОК��6/   #��[  6<   $��0,0,10,0q�6k   6}   �� 57      -   5        $ CancelButton��57      $��Отменить��63   #��[  6F   6u   58      59      5:      5;      5<      