(ns advent-of-code-2022.day-09-snake
  (:require [clojure.string :as string])
  (:gen-class))


(defn load-data [text]
  (->>
   text
   string/split-lines
   (map #(string/split % #"\s+"))
   (map (fn [[direction steps]] [direction (parse-long steps)]))))

(defn move-head [[xh yh] direction]
  (case direction
    "R" [(inc xh) yh]
    "L" [(dec xh) yh]
    "U" [xh (inc yh)]
    "D" [xh (dec yh)]))

(defn move-tail [[xt yt :as tail] [xh yh]]
  (let [y-delta (abs (- yh yt))
        x-delta (abs (- xh xt))]
    (if (or (= x-delta 2) (= y-delta 2))
      [(if (= x-delta 2) (/ (+ xh xt) 2) xh)
       (if (= y-delta 2) (/ (+ yh yt) 2) yh)]
      tail)))

(defn move-snake [snake direction]
  (let [snake' (update snake 0 move-head direction)]
    (reduce
     (fn [snake'' i]
       (let [head (get snake'' (dec i))
             tail (get snake'' i)
             new-tail (move-tail tail head)]
         (if (identical? tail new-tail)
           (reduced snake'')
           (assoc snake'' i new-tail)))
       #_(update snake'' i move-tail (get snake'' (dec i))))
     snake'
     (range 1 (count snake)))))


(defn puzzle [text length-of-snake]
  (->>
   text
   load-data
   (mapcat #(repeat (get % 1) (get % 0)))
   (reduce (fn [[snake trace] direction]
             (let [snake' (move-snake snake direction)]
               [snake' (conj trace (last snake'))]))
           [(into [] (repeat length-of-snake [0 0])) #{}])
   second
   count))

(defn puzzle-1 [text]
  (puzzle text 2))

(defn puzzle-2 [text]
  (puzzle text 10))


(comment
  (def file-path "src/advent_of_code_2022/day_09.txt")

  (def text (slurp file-path))

  (load-data text)

  ;; (puzzle-1 text)

  ;; (puzzle-2 text)

  :rfc)
