# LKQ_INVENTORY
Jedná se o jednoduchou aplikaci, která provádí pravidelnou inventarizace 2x do týdne. Aplikace typu Server--Client.

# INVENTORY
Složka Inventory obsahuje Windows službu napsanout v jazyce C#, který představuje klientskou stranu připojení (Server--klient).

## Popis algoritmu:
1. Klient se pokusí připojit na server za pomocí IP adresy a PORTU, na kterém běží serverový program pro zpracování JSON souboru.
2. Po úspěšném připojení klient příjme zprávu identifikující, že se připojil ke správnému serveru.
3. Při přijetí zprávy indentifikující server aplikace zašle hostname pc a nasledně samotný file s vygenerovanými daty.
4. Po zaslaní souboru, klient řádně ukoční spojení a jde spát na definovaný čas.

Všechny potřebný informace jsou zapisovány do souboru log.txt ve složce INVETORY. Další dva soubory v této složce složí pouze pro klienta, aby bylo jednoznačné, kdy proběhla poslední inventarizace. V těchto souborech jsou zachyceny časy posledních akcí.

# inventory_server.py
Python script, který představuje serverouv část spojení Server--Client.

## Popis algoritmu:
1. Po zapnutí serveru se zablokuje příslušná IP adresa a PORT na kterém bude server poslouchat.
2. Následně server přechazí do hlavní smyčky, kde je využit nástroj <sub>Select</sub>, která při zaslaní serverového socketu přída příslušný identifikátor socketu clienta do seznamu připojených soketů.
4. Pokud příjde soket, který je již v seznamu tak nejdříve server příjme jméno souboru, který bude příjmat a následně příjme příslušný file.
5. Ten příslušný data naparsuje a uloží do vytvořený třídy představujíc počítač a monitor.
6. Přijatý soubor následně uluží do zvoelné složky pro zpětnou kontrolu.
