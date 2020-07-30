@echo off
echo This may take a while...
pushd "C:\Program Files (x86)\gs\gs9.09\bin"
gswin32c -sDEVICE=pngalpha -r150 -o^
         "C:\Users\jopieter\Documents\Development\C#\MCDRI\Manual\V2\tmp\at least one correct-%%d.png"^
         "C:\Users\jopieter\Documents\Development\C#\MCDRI\Manual\V2\tmp\scan.pdf"
echo.
pause