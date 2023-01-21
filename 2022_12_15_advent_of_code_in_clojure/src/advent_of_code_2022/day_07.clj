(ns advent-of-code-2022.day-07
  (:require [clojure.string :as string])
  (:gen-class))


(defn load-data [text]
  (string/split-lines text))

(defn path-to-str [path]
  (string/join "/"  path))

(defn lines-to-list-of-folders-with-sizes [lines]
  (:paths (reduce
           (fn [state line]
             (condp #(string/starts-with? %2 %1) line
               "$ cd " (let [dir-name (subs line (count "$ cd "))]
                         (if (= dir-name "..")
                           (update state :cwd pop)
                           (update state :cwd conj dir-name)))
               "dir" state
               "$ ls" (assoc state :cwd-as-str (path-to-str (:cwd state)))
               (let [file-size (parse-long (first (re-seq #"\d+" line)))]
                 (update-in state [:paths (:cwd-as-str state)] (fnil + 0) file-size))))
           {:cwd [] :cwd-as-str "" :paths {}}
           lines)))


(defn list-to-tree [paths]
  (reduce
   (fn [root [path size]]
     (let [path-segments
           (drop 2 (string/split path #"/"))]
       (assoc-in root (concat (interleave (repeat (count path-segments) :folders) path-segments) [:size]) size)))
   {}
   paths))

(defn tree-to-list-of-sizes [result folder]
  (let [[result' sum']
        (reduce (fn [[lst sum] subfolder]
                  (let [lst' (tree-to-list-of-sizes lst subfolder)]
                    [lst' (+ sum (first lst'))]))
                [result, 0]
                (vals (:folders folder)))]
    (cons (+ sum' (or (:size folder) 0)) result')))


(defn lines-to-list-of-sizes [lines]
  (->>
   lines
   lines-to-list-of-folders-with-sizes
   list-to-tree
   (tree-to-list-of-sizes '())))


(defn puzzle-1 [text]
  (->>
   text
   load-data
   lines-to-list-of-sizes
   (filter #(<= % 100000))
   (reduce +)))


(defn puzzle-2 [text]
  (let [sizes (lines-to-list-of-sizes (load-data text))
        unused-space (- 70000000 (first sizes))
        missing-free-space (- 30000000 unused-space)]
    (->>
     sizes
     sort
     (filter #(> % missing-free-space))
     first)))



(comment
  (def file-path "src/advent_of_code_2022/day_07.txt")
  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)







(defn sum-not-exceeding-threshold [value-1 value-2 threshold]
  (let [result (and value-1 value-2 (+ value-1 value-2))]
    (if (or (nil? result) (> result threshold))
      nil
      result)))

(defn sum-folder-sizes [folder threshold]
  (let [[size' result' :as r]
        (->>
         (vals (:folders folder))
         (map #(sum-folder-sizes % threshold))
         (concat [[(or (:size folder) 0) 0]])
         (reduce (fn [[size-sum result-sum] [size result]]
                   [(sum-not-exceeding-threshold size-sum size threshold) (+ result-sum result)])
                 [0 0]))]
    (if (nil? size')
      r
      [size' (+ size' result')])))

(defn puzzle-1' [text]
  (let [root (list-to-tree (lines-to-list-of-folders-with-sizes (load-data text)))]
    (sum-folder-sizes root 100000)))

