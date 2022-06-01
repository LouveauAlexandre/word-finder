use std::time::Instant;
use std::io;
use rand::{thread_rng, prelude::SliceRandom};
use rayon::iter::IntoParallelIterator;
use rayon::iter::ParallelIterator;

fn generate_word_list() -> Vec<String> {
    let mut res: Vec<String> = vec![];
    for c1 in 'A'..='Z' {
        for c2 in 'A'..='Z' {
            for c3 in 'A'..='Z' {
                for c4 in 'A'..='Z' {
                    res.push(format!("{}{}{}{}",c1,c2,c3,c4));
                }
            }
        }
    }
    res.shuffle(&mut thread_rng());
    res
}

fn is_correct_word(word : &str, pattern : &str) -> bool {
    let len = pattern.len();
    match word.get(..len) {
        Some(sub_word) => sub_word == pattern,
        _ => false,
    }
}

fn process_search_single_threaded<'a>(word_list: &'a Vec<String>, pattern: &'a str) -> Vec<&'a String> {
    let result: Vec<&String> = word_list.into_iter().filter(|word| is_correct_word(word, pattern)).collect();
    result
}

fn process_search_multi_threaded<'a>(word_list: &'a Vec<String>, pattern: &'a str) -> Vec<&'a String> {
    let result: Vec<&String> = word_list.into_par_iter().filter(|word| is_correct_word(word, pattern)).collect();
    result
}

fn main() {
    let word_list = generate_word_list();
    let mut input_pattern = String::new();

    while !match input_pattern.trim() {
        ":q" => true,
        _ => false,
    } {
        println!("Enter the pattern to search (enter \":q\" to quit)");
        input_pattern = String::new();
        io::stdin()
        .read_line(&mut input_pattern)
        .expect("Failed to read line");
        if !match input_pattern.trim() {
            ":q" => true,
            _ => false,
        } {
            input_pattern = input_pattern.trim().to_uppercase();
            let mut now = Instant::now();
            let single_threaded_result = process_search_single_threaded(&word_list, &input_pattern);
            let single_threaded_search_time = now.elapsed();

            now = Instant::now();
            let multi_threaded_result  = process_search_multi_threaded(&word_list, &input_pattern);
            let multi_threaded_search_time = now.elapsed();

            println!("\nMatching words:\n{:?}", multi_threaded_result);
            println!("\nSingle Threaded Search:");
            println!("Result count: {}", single_threaded_result.len());
            println!("Search time: {:.2?}", single_threaded_search_time);

            println!("\nMulti Threaded Search:");
            println!("Result count: {}", multi_threaded_result.len());
            println!("Search time: {:.2?}\n", multi_threaded_search_time);
        }
    }

}
