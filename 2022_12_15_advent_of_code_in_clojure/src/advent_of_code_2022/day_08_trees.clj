(ns advent-of-code-2022.day-08-trees
  (:require [clojure.string :as string])
  (:gen-class))


(defn load-data [text]
  (let [rows
        (->>
         text
         string/split-lines
         (map #(mapv (comp parse-long str) %))
         (into []))]
    {:rows rows :rows-count (count rows) :columns-count (count (rows 0))}))

(defn seq-of-interior-positions [{:keys [rows-count columns-count]}]
  (for [l (range 1 (dec rows-count))
        c (range 1 (dec columns-count))]
    [l c]))

(defn is-tree-visible [{:keys [rows rows-count columns-count]} [row column]]
  (let [value (get-in rows [row column])]
    (or
     (every? #(< (get-in rows [row %]) value) (range 0 column)) ; left
     (every? #(< (get-in rows [row %]) value) (range (inc column) columns-count)) ; right
     (every? #(< (get-in rows [% column]) value) (range 0 row)) ; up
     (every? #(< (get-in rows [% column]) value) (range (inc row) rows-count)) ; down
     )))


(defn count-visible-trees-in-line [value get-value-of-position positions]
  (reduce (fn [sum position]
            (let [value-of-position (get-value-of-position position)]
              (if (< value-of-position value)
                (inc sum)
                (reduced (inc sum)))))
          0
          positions))


(defn count-visible-trees [{:keys [rows rows-count columns-count]} [row column]]
  (let [value (get-in rows [row column])]
    (* (count-visible-trees-in-line value #(get-in rows [row %]) (range (dec column) -1 -1)) ; left
       (count-visible-trees-in-line value #(get-in rows [row %]) (range (inc column) columns-count 1)) ; right
       (count-visible-trees-in-line value #(get-in rows [% column]) (range (dec row) -1 -1)) ; up
       (count-visible-trees-in-line value #(get-in rows [% column]) (range (inc row) rows-count 1)) ; down
       )))


(defn puzzle-1 [text]
  (let [data (load-data text)
        visible-on-edges (+ (* 2 (:columns-count data)) (* 2 (- (:rows-count data) 2)))
        visible-interior (->>
                          (seq-of-interior-positions data)
                          (filter #(is-tree-visible data %))
                          count)]
    (+ visible-on-edges visible-interior)))


(defn puzzle-2 [text]
  (let [data (load-data text)]
    (->>
     data
     seq-of-interior-positions
     (map #(count-visible-trees data %))
     (apply max))))


(comment
  (def file-path "src/advent_of_code_2022/day_08.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)
