@echo off

"C:\Program Files\7-Zip\7z.exe" x *.zip -aos -o".\betclic_test_api\" -r
".\betclic_test_api\betclic_test.exe"


set /p DUMMY=Hit ENTER to continue...