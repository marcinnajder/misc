(ns advent-of-code-2022.core
  (:require advent-of-code-2022.day-01)
  (:require advent-of-code-2022.day-02)
  (:gen-class))

(defn -main
  "I don't do a whole lot ... yet."
  [& args]
  (println "Hello, World!")

  (let [results
        [(let [text (slurp "src/advent_of_code_2022/day_01.txt")] [(advent-of-code-2022.day-01/puzzle-1 text) (advent-of-code-2022.day-01/puzzle-2 text)])
         (let [text (slurp "src/advent_of_code_2022/day_02.txt")] [(advent-of-code-2022.day-02/puzzle-1 text) (advent-of-code-2022.day-02/puzzle-2 text)])]]
    (println results)))
