(ns advent-of-code-2022.core
  (:require advent-of-code-2022.day-01-calories)
  (:require advent-of-code-2022.day-02-game)
  (:require advent-of-code-2022.day-03-chars)
  (:require advent-of-code-2022.day-04-ranges)
  (:require advent-of-code-2022.day-05-crates)
  (:require advent-of-code-2022.day-06-markers)
  (:require advent-of-code-2022.day-07-files)
  (:require advent-of-code-2022.day-08-trees)
  (:require advent-of-code-2022.day-09-snake)
  (:require advent-of-code-2022.day-10-cpu)
  (:require advent-of-code-2022.day-11-monkeys)
  (:require advent-of-code-2022.day-12-path)
  (:require advent-of-code-2022.day-13-packets)
  (:require advent-of-code-2022.day-14-sand)
  (:require advent-of-code-2022.day-15-scanners)
  (:require advent-of-code-2022.day-17-tetris)
  (:require advent-of-code-2022.day-18-droplets)
  (:gen-class))

(defn -main
  "I don't do a whole lot ... yet."
  [& args]
  (println "Hello, World!")

  (let [results
        [;
         (time (let [text (slurp "src/advent_of_code_2022/day_01.txt")] [(advent-of-code-2022.day-01-calories/puzzle-1 text) (advent-of-code-2022.day-01-calories/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_02.txt")] [(advent-of-code-2022.day-02-game/puzzle-1 text) (advent-of-code-2022.day-02-game/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_03.txt")] [(advent-of-code-2022.day-03-chars/puzzle-1 text) (advent-of-code-2022.day-03-chars/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_04.txt")] [(advent-of-code-2022.day-04-ranges/puzzle-1 text) (advent-of-code-2022.day-04-ranges/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_05.txt")] [(advent-of-code-2022.day-05-crates/puzzle-1 text) (advent-of-code-2022.day-05-crates/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_06.txt")] [(advent-of-code-2022.day-06-markers/puzzle-1 text) (advent-of-code-2022.day-06-markers/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_07.txt")] [(advent-of-code-2022.day-07-files/puzzle-1 text) (advent-of-code-2022.day-07-files/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_08.txt")] [(advent-of-code-2022.day-08-trees/puzzle-1 text) (advent-of-code-2022.day-08-trees/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_09.txt")] [(advent-of-code-2022.day-09-snake/puzzle-1 text) (advent-of-code-2022.day-09-snake/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_10.txt")] [(advent-of-code-2022.day-10-cpu/puzzle-1 text) (advent-of-code-2022.day-10-cpu/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_11.txt")] [(advent-of-code-2022.day-11-monkeys/puzzle-1 text) (advent-of-code-2022.day-11-monkeys/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_12.txt")] [(advent-of-code-2022.day-12-path/puzzle-1 text) (advent-of-code-2022.day-12-path/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_13.txt")] [(advent-of-code-2022.day-13-packets/puzzle-1 text) (advent-of-code-2022.day-13-packets/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_14.txt")] [(advent-of-code-2022.day-14-sand/puzzle-1 text) (advent-of-code-2022.day-14-sand/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_15.txt")] [(advent-of-code-2022.day-15-scanners/puzzle-1 text) (advent-of-code-2022.day-15-scanners/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_17.txt")] [(advent-of-code-2022.day-17-tetris/puzzle-1 text) (advent-of-code-2022.day-17-tetris/puzzle-2 text)]))
         (time (let [text (slurp "src/advent_of_code_2022/day_18.txt")] [(advent-of-code-2022.day-18-droplets/puzzle-1 text) (advent-of-code-2022.day-18-droplets/puzzle-2 text)]))
         ;
         ]]
    (println results)))
