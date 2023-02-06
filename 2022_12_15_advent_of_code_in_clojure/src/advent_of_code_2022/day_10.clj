(ns advent-of-code-2022.day-10
  (:require [clojure.string :as string])
  (:gen-class))

(defn load-data [text]
  (->>
   text
   string/split-lines
   (map #(if (= % "noop") nil (parse-long (subs % 5))))))


(defn to-sequence-of-registry-values [values]
  (->>
   values
   (mapcat #(if (nil? %) [0] [0 %]))
   (reductions
    (fn [[index before after] value]
      [(inc index) after (+ after value)])
    [0 0 1])
   (drop 1)))

(defn puzzle-1 [text]
  (->>
   text
   load-data
   to-sequence-of-registry-values
   (keep
    (fn [[index before after]]
      (when
       (= 0 (mod index 20))
        (let [q (quot index 20)]
          (when (odd? q) (* index before))))))
   (apply +)))

(defn puzzle-2 [text]
  (->>
   text
   load-data
   to-sequence-of-registry-values
   (map
    (fn [x [index before after]]
      (let [diff (abs (- x before))]
        (if (or (= diff 0) (= diff 1)) "#" ".")))
    (mapcat identity (repeat (range 0 40))))
   (partition 40)
   (map #(apply str %))
   (apply str)
   ;EZFCHJAB
   ))



(comment
  (def file-path "src/advent_of_code_2022/day_10.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)
