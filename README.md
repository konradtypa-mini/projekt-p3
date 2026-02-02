Pliki csv z danymi były za duże, aby zapostować je na githubie, zostały więc skompresowane do formatu .zip.
Aby program działał poprawnie, trzeba je najpierw odpakować, do czego można użyć np tego polecenia (bash) w katalogu projektu (citybikesnyc):
find Data -name "*.zip" -execdir unzip -o -j {} \;
