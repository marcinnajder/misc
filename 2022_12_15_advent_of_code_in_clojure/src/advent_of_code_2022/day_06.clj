(ns advent-of-code-2022.day-06
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


