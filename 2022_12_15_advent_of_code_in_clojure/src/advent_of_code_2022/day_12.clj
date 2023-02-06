(ns advent-of-code-2022.day-12
  (:require [clojure.string :as string])
  (:gen-class))

()

(defn load-data [text]
  (->>
   text
   string/split-lines
   (mapv #(mapv int %))))


(def heightmap (load-data text))

;; (concat nil [ 1 2 ] nil [10 4])

(def row 0)
(def column 2)
(def board-size 4)

(defn neighbours [[row column] board-size]
  (concat
   (when (> row 0) [[(dec row) column]])
   (when (> column 0) [[row (dec column)]])
   (let [next-row (inc row)]
     (when (< next-row board-size) [[next-row column]]))
   (let [next-column (inc column)]
     (when (< next-column board-size) [[row next-column]]))))

;; (def letter "E")
(defn move [board [row column] trace]
  
  )



(defn find-letter [heightmap letter]
  (some
   (fn [[row-index line]]
     (let [column-index (string/index-of line letter)]
       (when-not (nil? column-index) [row-index column-index])))
   (map-indexed vector heightmap)))


(comment
  (def file-path "src/advent_of_code_2022/day_12.txt")

  (def text (slurp file-path))

  (load-data text)

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
