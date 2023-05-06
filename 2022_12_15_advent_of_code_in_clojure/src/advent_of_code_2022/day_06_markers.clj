(ns advent-of-code-2022.day-06-markers
  (:gen-class))

(defn load-data [text]
  text)

(defn puzzle [text n]
  (let [[first-n other] (split-at n text)]
    (reduce
     (fn [last-n [index item]]
       (let [last-n' (assoc last-n (mod index n) item)]
         (if (apply distinct? (vals last-n'))
           (reduced (+ index n 1))
           last-n')))
     (into {} (map-indexed vector first-n))
     (map-indexed vector other))))

(defn puzzle-1 [text]
  (puzzle text 4))

(defn puzzle-2 [text]
  (puzzle text 14))


(comment
  (def file-path "src/advent_of_code_2022/day_06.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)



(defn inc-occurrs [occurrs c]
  (update occurrs c (fnil inc 0)))

(defn dec-occurrs [occurs c]
  (let [c-occur (get occurs c)]
    (if (= c-occur 1)
      (dissoc occurs c)
      (update occurs c dec))))

(defn update-state [{:keys [queue occurrs]} index c]
  (let [q-index (mod index (count queue))
        old-c (get queue q-index)]
    {:queue (assoc queue q-index c)
     :occurrs (-> occurrs (dec-occurrs old-c) (inc-occurrs c))}))

(defn puzzle' [text n]
  (let [[first-n other] (split-at n text)]
    (reduce
     (fn [state [index c]]
       (let [state (update-state state index c)]
         (if (= (count (:occurrs state)) n)
           (reduced (+ index n 1))
           state)))
     {:queue (into [] first-n) :occurrs (reduce inc-occurrs {} first-n)}
     (map-indexed vector other))))

