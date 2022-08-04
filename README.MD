# rEFInd AUTOMENU : rEFInd АВТОМЕНЮ
rEFInd AUTOMENU : Config generate and install                       
This program scans computer for the EFI boot loaders of systems, makes config on them and installing "rEFInd Boot Manager"

rEFInd АВТОМЕНЮ : Генерация конфига и установка                       
Эта программа сканирует компьютер на наличие EFI загрузчиков систем, делает по ним конфиг и устанавливает "rEFInd Менеджер загрузки".

# Notification : Предупреждение
If you have already installed rEFInd on a computer, it will need to be removed for installation !
Если у вас уже установлен rEFInd на компьютере, его необходимо будет удалить для установки !

# Command Prompt Arguments : Аргументы командной строки

  -i, --Install     Set install Mode
                    for example :
                    "refind -i" - rEFInd should be installed on current Computer,
                    "refind --install E:" - rEFInd should be installed on Flash Drive E:\,
                    "refind -i Desktop" - rEFInd should be unpacked on current User's Desktop

  -r, --Remove      Remove rEFInd from current Computer
                    for example :
                    "refind -r"
                    Can be Combined with other parametrs

  -s, --Scan        If rEFInd already installed on computer, Scan rEFInd folder on EFI Volume and write ressult to
                    "rEFInd Dir.txt" on your desktop
                    for example :
                    "refind -s"
                    Can be Combined with other parametrs

  -c, --Config      If rEFInd already installed on computer, this parametr programm will open "refind.conf"
                    for example :
                    "refind -c"
                    Can be Combined with other parametrs

  -d, --Download    Download latest version of rEFInd from SourceForge.com before installation
                    for example :
                    "refind -d -i" - rEFInd should be installed on current Computer with latest version of rEFInd
                    Can be Combined only with '--Install' parametr

  -t, --Theme       Set path to your theme folder (only with -i parametr)
                    for example :
                    "refind -t C:\Theme --install" - rEFInd should be installed on current Computer with theme in folder
                    C:\"Theme"
                    Can be Combined only with '--Install' parametr

  -a, --Arch        Force set installation arcitecture
                    Permissible values :
                    "AMD64, IA64, ARM64, x86"
                    for example :
                    "refind -a AMD64 -i
                    Can be Combined omly with '--Install' parametr

  -f, --Format      If drive has File System not FAT32, parametr allow to format him
                    for example :
                    "refind -f --install E:\" - rEFInd should be installed on flash drive E:\ with formating into FAT32
                    File system if nedded
                    Can be Combined only with '--Install' parametr

  --IgnoreCGL       Beta function !!!
                    Scan EFI System Partition for boot loaders and make Loader Config ignoring Config Generator List
                    for example :
                    "refind --IgnoreCGL --install" - rEFInd should be installed on current Computer ignoring Config
                    Generator List
                    Can be Combined only with '--Install' parametr

  --Wait

  --help            Display this help screen.

  --version         Display version information.

Powered by CommandLineParser

# Supported : Поддерживается
## Operating Systems : Операционные системы
1. Windows 7-11 or HackBGRT
2. PhoenixOS
3. Linux Ubuntu
4. Linux Debian
5. CentOS
6. Fedora Linux
7. Kali Linux

## Utilities : Утилиты
1. Grub File Manager


# Links : Ссылки
Group in VKontakte : Группа во Вконтакте (Only Russia / Только для русских)                                  
https://vk.com/refind_project

rEFInd++ Theme Pack                                                                  
https://drive.google.com/file/d/1wiWOfzluY3VQAoyCXsvDfJmdCxsCUNBw/view?usp=sharing

Original rEFInd project website : Сайт оригинального проекта rEFInd                      
https://www.rodsbooks.com/refind/

# License : Лицензия
GNU GPL v3
