# rEFInd AUTOMENU : rEFInd АВТОМЕНЮ
rEFInd AUTOMENU : Config generate and install                       
This program scans computer for the EFI boot loaders of systems, makes config on them and installing "rEFInd Boot Manager"

rEFInd АВТОМЕНЮ : Генерация конфига и установка                       
Эта программа сканирует компьютер на наличие EFI загрузчиков систем, делает по ним конфиг и устанавливает "rEFInd Менеджер загрузки".

# Notification : Предупреждение
If you have already installed rEFInd on a computer, it will be deleted for installation!                      
Если у вас уже установлен rEFInd на компьютере он будет удалена для установки !

# Command Prompt Arguments : Аргументы командной строки
  -i, --Install    Set install Mode                                                                              
                   for example :                                       
                   "refind -i computer" - rEFInd should be installed on current Computer,                                       
                   "refind --install E:"- rEFInd should be installed on Flash Drive E:\                                       

  -t, --Theme      Set path to your theme folder (only with -i argument)                                       
                   for example :                                                                              
                   "refind --install Comp -t C:\Theme" rEFInd should be installed on current Computer with theme in folder C:\Theme\,
                   "refind -i E:\ --theme Theme" - rEFInd should be installed on Flash Drive E:\ with theme in folder near program

  -f, --Format     If drive has File System not FAT32, parametr allow to format him                                       
                   for example :                                                                              
                   "refind --install E:\ -f"                                                                              

  -r, --Remove     Remove rEFInd from current Computer                                       
                   for example :                                                                              
                   "refind -r"                                                                              

  -d, --Dir        If rEFInd already installed on computer, Scan rEFInd folder on EFI Volume and write ressult to
                   "rEFInd Dir.txt" on your desktop                                       
                   Can be Combined                                                                              

  -c, --Config     If rEFInd already installed on computer, this parametr programm will open "refind.conf"
                   Can be Combined                                       

  --help           Display this help screen.                                                                              

  --version        Display version information.                                                                              

Powered by CommandLineParser

# Supported OS : Поддерживаемые ОС
1. Windows 7-11 or HackBGRT
2. PhoenixOS
3. Linux Ubuntu
4. Linux Debian
5. CentOS
6. Fedora Linux
7. Kali Linux

# Links : Ссылки
Group in VKontakte : Группа во Вконтакте (Only Russia / Только для русских)                                  
https://vk.com/refind_project

rEFInd++ Constructor (Other Project / Другой проект)                         
https://github.com/Rikitav/rEFInd-Constructor

Original rEFInd project website : Сайт оригинального проекта rEFInd                      
https://www.rodsbooks.com/refind/

# License : Лицензия
GNU GPL v3
