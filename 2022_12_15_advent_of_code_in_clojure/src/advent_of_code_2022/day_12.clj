(ns advent-of-code-2022.day-12
  (:require [clojure.string :as string])
  (:gen-class))

#_(defn load-data [text]
    (->>
     text
     string/split-lines
     (mapv #(mapv int %))))

(defn find-height [heightmap height]
  (some
   (fn [[row-index row]]
     (let [column-index (first (keep-indexed #(when (= %2 height) %1) row))]
       (when-not (nil? column-index) [row-index column-index])))
   (map-indexed vector heightmap)))


(defn load-data [text]
  (let [heightmap (mapv #(mapv int %) (string/split-lines text))
        row-count (count heightmap)
        column-count (count (first heightmap))
        start (find-height heightmap (int \S))
        end (find-height heightmap (int \E))
        data {:heightmap heightmap :row-count row-count :column-count column-count :start start :end end}]

    (def track #{})
    (def position start)
    (def data data)
    (def heightmap heightmap)
    (def row-count row-count)
    (def column-count column-count)
    (def end end)
    (def start start)
    data))

;; (def heightmap (load-data text))

(defn neighbours [[row column] row-count column-count]
  (println "neighbours" row column row-count column-count)
  (concat
   (when (> row 0) [[(dec row) column]])
   (when (> column 0) [[row (dec column)]])
   (let [next-row (inc row)]
     (when (< next-row row-count) [[next-row column]]))
   (let [next-column (inc column)]
     (when (< next-column column-count) [[row next-column]]))))




;; (def start (find-height heightmap (int \S)))
;; (def finish (find-height heightmap (int \E)))


;; TODO
;; - zamiast board-size powinnt byc column-count row-count
;; - przemyslec jeszcze raz jak powinno dzialac zakonczenie wchodzenia na ostatni element
;; tzn trzeba isc chyba do konca gdy juz nie mozna dalej i wtedy sprawdzac czy w okolicy 
;; jest koniec

(defn traverse [{:keys [heightmap row-count column-count start end] :as data}  position track]
  (println "traverse: " position)

  (let [track (conj track position)
        value-of-postion (get-in heightmap position)]
    #_break
    (if
     (= position end)
      (do (println "KONIEC")
          [track])
      (->>
       (neighbours position row-count column-count)
       #_(remove (fn [x] true))
       (remove track)

       #_(map (fn [x]
                (println x)
                x))

       (filter #(or
                 (= position start)
                 (= % end)
                 (<= (- (get-in heightmap %) value-of-postion) 1)))

       #_(take 1)
      ;;  (map (fn [x]
      ;;         (println x)
      ;;         x))

       (map #(traverse data % track))
       (apply concat)))
    ;
    ))

(let [result (traverse data position track)
      _ (into [] result)]
  (apply min (map count result)))


;; (conj [] (traverse data position track))

;; (defn puzzle-2 [text]
;;   (let [heightmap (load-data text)
;;         row-count (count heightmap)
;;         column-count (count (first heightmap))
;;         start (find-height heightmap (int \S))
;;         end (find-height heightmap (int \E))
;;         data {:heightmap heightmap :row-count row-count :column-count column-count :start start :end end}]

;;     (def track #{})
;;     (def position start)
;;     (def data data)
;;     (def heightmap heightmap)
;;     (def row-count row-count)
;;     (def column-count column-count)
;;     (def end end)
;;     (def start start)



;;     (println start end)

;;     #_(traverse data start #{})
;;     ;
;;     ))

;; (defn puzzle [text]
;;   (let [heightmap (load-data text)
;;         row-count (count heightmap)
;;         column-count (count (first heightmap))
;;         start (find-height heightmap (int \S))
;;         end (find-height heightmap (int \E))
;;         data {:heightmap heightmap :row-count row-count :column-count column-count :start start :end end}]

;;     (def track #{})
;;     (def position start)
;;     (def data data)
;;     (def heightmap heightmap)
;;     (def row-count row-count)
;;     (def column-count column-count)
;;     (def end end)
;;     (def start start)



;;     (println start end)

;;     #_(traverse data start #{})
;;     ;
;;     ))

;; (find-height heightmap (int \E))

(comment
  (def file-path "src/advent_of_code_2022/day_12.txt")

  (def text (slurp file-path))

  (load-data text)

  (def data (load-data text))

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)





;; (defn puzzle [text]
;;   (let [monkeys (load-data text)
;;         monkeys-count (count monkeys)
;;         ;; monkeys-count 3
;;         ]
;;     ;; (def monkeys monkeys)
;;     ;; (def monkeys-count monkeys-count)

;;     (reduce
;;      (fn [monkeys index]
;;        (let [{:keys [worry-levels op-value op-op if-test if-true if-false]} (get monkeys index)
;;              monkeys (reduce
;;                       (fn [monkeys worry-level]
;;                         (let [worry-level (quot (op-op (or op-value worry-level) worry-level) 3)
;;                               monkey-index (if (= (mod worry-level if-test) 0)
;;                                              if-true
;;                                              if-false)]
;;                           (update-in monkeys [monkey-index :worry-levels] conj worry-level)))
;;                       monkeys
;;                       worry-levels)]

;;          (->
;;           monkeys
;;           (update-in [index :inspects-count] + (count worry-levels))
;;           (assoc-in [index :worry-levels] []))))
;;      monkeys
;;      (range 0 monkeys-count))

;;     ;
;;     ))
