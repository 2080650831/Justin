@echo off 
echo ������������ļ������Ե�һ��...... 

del *.pdb
del *.xml  
del/s/q log\*.* 
rd log
del DockPanel.config
del *.manifest
del *.vshost.exe*

echo �������! 
:echo. & pause