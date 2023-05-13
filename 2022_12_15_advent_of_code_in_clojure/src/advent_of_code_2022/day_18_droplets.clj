(ns advent-of-code-2022.day-18-droplets
  (:require [clojure.string :as string])
  (:gen-class))

(defn load-data [text]
  (->>
   text
   string/split-lines
   (map #(into [] (map parse-long (re-seq #"\d+" %))))))


(defn sides-of-cube [[x y z]]
  [[:z z [x y]]
   [:z (inc z) [x y]]
   [:x x [z y]]
   [:x (inc x) [z y]]
   [:y y [x z]]
   [:y (inc y) [x z]]])

(defn puzzle-1 [text]
  (->>
   (load-data text)
   (mapcat sides-of-cube)
   (reduce #(update %1 %2 (fnil inc 0)) {})
   (filter #(= (second %) 1))
   count))




(defn calc-bounds [droplets]
  (reduce
   (fn [[mins maxs] cube]
     [(mapv min mins cube) (mapv max maxs cube)])
   [(vec (repeat 3 java.lang.Long/MAX_VALUE))
    (vec (repeat 3 java.lang.Long/MIN_VALUE))]
   droplets))


(defn expand-bounds-by-one [[mins maxs]]
  [(mapv dec mins) (mapv inc maxs)])

(defn calc-neighbours [[x y z]]
  [[(dec x) y z]
   [(inc x) y z]
   [x (dec y) z]
   [x (inc y) z]
   [x y (dec z)]
   [x y (inc z)]])

(defn cube-in-bounds? [[x y z] [[x-min y-min z-min] [x-max y-max z-max]]]
  (and (<= x-min x x-max) (<= y-min y y-max) (<= z-min z z-max)))

;; https://admay.github.io/queues-in-clojure/
(defn queue
  ([] (clojure.lang.PersistentQueue/EMPTY))
  ([coll]
   (reduce conj clojure.lang.PersistentQueue/EMPTY coll)))


(defn puzzle-2 [text]
  (let [droplets (load-data text)
        droplets-set (set droplets)
        [min-position _ :as bounds] (expand-bounds-by-one (calc-bounds droplets))]
    (loop [todo-queue (queue [min-position])
           visiting #{min-position}
           counter 0]
      (if
       (empty? todo-queue)
        counter
        (let [cube (peek todo-queue)
              {neighbours false neighbours-droplets true}
              (->>
               (calc-neighbours cube)
               (filter #(and (cube-in-bounds? % bounds) (not (contains? visiting %))))
               (group-by #(contains? droplets-set %)))

              todo-queue' (into (pop todo-queue) neighbours)
              visiting' (into visiting neighbours)
              counter' (+ counter (count neighbours-droplets))]
          (recur todo-queue' visiting' counter'))))))



(comment
  (def file-path "src/advent_of_code_2022/day_18.txt")

  (def text (slurp file-path))

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)



((defn tests []

   (assert (= (expand-bounds-by-one [[0 1 2] [10 11 12]]) [[-1 0 1] [11 12 13]]))

   (assert (= (calc-neighbours [1 2 3]) [[0 2 3] [2 2 3] [1 1 3] [1 3 3] [1 2 2] [1 2 4]]))

   (assert (cube-in-bounds? [0 1 2] [[0 1 2] [10 10 10]]))
   (assert (cube-in-bounds? [10 10 10] [[0 1 2] [10 10 10]]))
   (assert (cube-in-bounds? [5 5 5] [[0 1 2] [10 10 10]]))

   (assert (not (cube-in-bounds? [0 1 2] [[1 1 2] [10 10 10]])))
   (assert (not (cube-in-bounds? [10 10 11] [[0 1 2] [10 10 10]])))

   :rfc))
