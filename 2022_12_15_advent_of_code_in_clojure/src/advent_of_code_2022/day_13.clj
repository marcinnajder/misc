(ns advent-of-code-2022.day-13
  (:require [clojure.string :as string])
  (:gen-class))



(defn load-data [text]
  (->>
   (string/split-lines text)
   (partition 2 3)
   (map (partial mapv read-string))))


;; (defn puzzle-2 [text]
;;   (let [{:keys [end heightmap row-count column-count] :as data} (load-data text)
;;         starts (find-heights heightmap row-count column-count (int \a))]
;;     (->>
;;      starts
;;      (remove #(interior? data %))
;;      (map #(get (calculate-minimal-costs data % move-forward?) end))
;;      (remove nil?)
;;      (apply min))))






(comment
  (def file-path "src/advent_of_code_2022/day_13.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)
