module AlgorithmsAndDataStructures.AvlTree

open Utils

open System.Xml.Linq
open System.Xml
open System.Text
open System


// - jesli do zwyklego BST dodajemy 1, 2, 3, 4, 5 to drzewo staje sie lista i wyszukiwanie jest liniowe
// - wtedy pojawiaja algorytmy ktore po wstawieniu elementu balabsuja drzewo

// -- Red Black Trees
// --- czytalem o tym w ksiazce cormena "rozdzial 14 drzewa czerwono-czarne" ale algorytm jest dosyc zagmatwany,
// napisany jest imperatywnie, wygladalo jakby napisanie go dla struktur immutable bylo trudne albo nie mozliwe
// wiec sobie odpuscilem i patrzylem ba drzewa avl bo wydawalo sie to bardziej naturalne
// --- https://en.wikibooks.org/wiki/F_Sharp_Programming/Advanced_Data_Structures#Red_Black_Trees potem trafilem tutaj
//  i bardzo prosto napisanym byl kod immutable dla drzew czerwono czarnych :( czyli da sie

// -- AVL Trees
// --- tego nie bylo zdaje sie opisanego w ksiazce cormena wiec zeknalem na wiki i do google
// --- https://pl.wikipedia.org/wiki/Drzewo_AVL tutaj fajnie opisane o co mniej wiecej chodzi, ale dokladny algorytm nie jest opisany
// --- https://eduinf.waw.pl/inf/alg/001_search/0119.php tutaj na prawde bylo opisane tak ze zrozumialem nawet :) fajne obrazki
// --- generalnie w wezlach obok wartosci zapisujemy wysokosci liczone jako "wysokosc_lewego - wysokosc_prawego"
// (mozna zapisywac roznice wysokosci lub same wysokosci), wstawiamy normalnie node jako lisc do drzewa
// --- nastepnie sprawdzamy jak to wplynelo na zmiane wysokosci i jesli roznica jest inna jak "-1, 0, 1"
// (wiec moze byc jedynie 2 lub -2) to wykonywane sa  odpowidnie retacje, w sumie sa 4 mozliwosci, dla lewe i prawe strony
// dziala to analogicznie, ale w ramach jednej strony sa 2 mozliwosci, albo pojedyczna rotacja (np RR) abo podwojna rotacja (RL)
// --- fajne jest to ze faktycznie rotujac drzewo robi sie takie lokalne zmienu w strukturze drzewa ze czesc zostaje nieruszna,
// wydawalo sie ze ze implementacja bedzie do zrozueminia i nawet chcialem to na postawie tego opisu zaimplementowac ale w sumie
// dobrze chyba ze odpuscilem bo pewnie naleczylbym sie mimo wszystko :(
// --- co ciekawe standardowala Map<K,V> pod spodem wykorzystuje mniej wiecej drzewo AVL (let tolerance = 2)
// https://github.com/dotnet/fsharp/blob/main/src/FSharp.Core/map.fs
// --- https://en.wikibooks.org/wiki/F_Sharp_Programming/Advanced_Data_Structures#Red_Black_Trees tutaj zajebiscie prosto napisane


// https://en.wikibooks.org/wiki/F_Sharp_Programming/Advanced_Data_Structures#Red_Black_Trees
// - tutaj generalnie bardzo fajnie i prosto napisanych jest kilka struktur danych w F#
// 1. Stacks
// 2. Queues (Naive Queue, Queue From Two Stacks_
// 3. Binary Search Trees (Red Black Trees, AVL Trees, Heaps)
// 4. Lazy Data Structures
// 5. Additional Resources


type AvlTree<'a> =
    | Tip
    | Node of int * 'a * AvlTree<'a> * AvlTree<'a>
