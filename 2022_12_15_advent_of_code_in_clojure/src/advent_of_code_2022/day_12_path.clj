(ns advent-of-code-2022.day-12-path
  (:require [clojure.string :as string])
  (:gen-class))


(defn all-positions [row-count column-count]
  (for [row (range 0 row-count)
        column (range 0 column-count)]
    [row column]))

(defn find-heights [heightmap row-count column-count height]
  (filter #(= (get-in heightmap %) height) (all-positions row-count column-count)))


(defn load-data [text]
  (let [heightmap (mapv #(mapv int %) (string/split-lines text))
        row-count (count heightmap)
        column-count (count (first heightmap))
        start (first (find-heights heightmap row-count column-count (int \S)))
        end (first (find-heights heightmap row-count column-count (int \E)))
        heightmap (->
                   heightmap
                   (assoc-in start (int \a))
                   (assoc-in end (int \z)))]
    {:heightmap heightmap :row-count row-count :column-count column-count :end end :start start}))



(defn neighbours [[row column] row-count column-count]
  (concat
   (when (> row 0) [[(dec row) column]])
   (when (> column 0) [[row (dec column)]])
   (let [next-row (inc row)]
     (when (< next-row row-count) [[next-row column]]))
   (let [next-column (inc column)]
     (when (< next-column column-count) [[row next-column]]))))


;; https://admay.github.io/queues-in-clojure/
(defn queue
  ([] (clojure.lang.PersistentQueue/EMPTY))
  ([coll]
   (reduce conj clojure.lang.PersistentQueue/EMPTY coll)))


(defn calculate-minimal-costs [{:keys [heightmap row-count column-count]} start move?]
  (loop [todo-queue (queue [start])
         costs {start 0}]
    (if (empty? todo-queue)
      costs
      (let [pos (peek todo-queue)
            pos-height (get-in heightmap pos)
            pos-cost (get costs pos)
            pos-cost-next (inc pos-cost)
            pos-neighbours
            (->>
             (neighbours pos row-count column-count)
             (filter  #(move? pos-height (get-in heightmap %)))
             (filter (fn [n]
                       (let [n-cost (get costs n)]
                         (or
                          (nil? n-cost)
                          (<  pos-cost-next n-cost))))))]
        (recur
         (into (pop todo-queue) pos-neighbours)
         (reduce  #(assoc %1 %2 pos-cost-next) costs  pos-neighbours))))))



(defn move-forward? [from-height to-height]
  (<= (- to-height from-height) 1))

(defn puzzle-1 [text]
  (let [{:keys [start end] :as data} (load-data text)]
    (get (calculate-minimal-costs data start move-forward?) end)))



(defn interior? [{:keys [heightmap row-count column-count]} pos]
  (and
   (= (get-in heightmap pos) (int \a))
   (every? #(= (get-in heightmap %) (int \a)) (neighbours pos row-count column-count))))


(defn puzzle-2 [text]
  (let [{:keys [end heightmap row-count column-count] :as data} (load-data text)
        starts (find-heights heightmap row-count column-count (int \a))]
    (->>
     starts
     (remove #(interior? data %))
     (map #(get (calculate-minimal-costs data % move-forward?) end))
     (remove nil?)
     (apply min))))






(comment
  (def file-path "src/advent_of_code_2022/day_12.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  (puzzle-2' text)

  :rfc)




(defn move-backward? [from-height to-height]
  (move-forward? to-height from-height))

(defn puzzle-2' [text]
  (let [{:keys [end heightmap row-count column-count] :as data} (load-data text)
        costs (calculate-minimal-costs data end move-backward?)]
    (->>
     (all-positions row-count column-count)
     (filter #(= (get-in heightmap %) (int \a)))
     (keep costs)
     (apply min))))



;; https://www.redblobgames.com/pathfinding/a-star/introduction.html
;; https://www.digitalocean.com/community/tutorials/breadth-first-search-depth-first-search-bfs-dfs
;; Breadth-First Search (BFS) - implementation above
;; Depth-First Search (DFS) - implementation below

(defn traverse [{:keys [heightmap row-count column-count end] :as data}  position track]
  (let [track (conj track position)
        value-of-postion (get-in heightmap position)]
    #_break
    (if
     (= position end)
      [(count track)]
      (->>
       (neighbours position row-count column-count)
       (remove track)
       (filter  #(<= (- (get-in heightmap %) value-of-postion) 1))
       (map #(traverse data % track))
       (apply concat)))
    ;
    ))

;; (apply min (traverse data position track))

