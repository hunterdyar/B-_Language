# B-

## About
What is this?
- A Recreation of the 'B' language (WIP) with custom parser and bytecode Virtual Machine, written in C#. It's like B, but a lil' worse (Slow and not fully featured).
- A custom C# engine that exposes the internals of the VM, the parser, instructions, register, stack, heap, etc.
- These C# Projects are compiled into WebAssembly and can be used with a serverless static webpage. Used by...
- A js project (website) that allows one to write and run, debug, and inspect code.
- The internals of the interpreter are visibly and clearly represented.
- The page should be an intuitive debugger that gives students a playground to explore, learn, and develop mental models of how programming languages work.

> This project is serving as design research for [scrub](https://github.com/hunterdyar/scrub-lang). This project is part of that effort. Letting me focus on the student-facing elements by implementing a much simpler, non-rewindable, language.
> I hope to gain insight by playtesting this project with students. I know that I want students to have a tool to develop their mental models by investigating the internals of an interpreter. 

## Future Objectives
Version 1 of this project means:
- A viewable AST that highlights the appropriate section as the program runs.
- A local frame viewer of some kind.
- Most of the important B features implemented.
  - Pointers/Dereferencing, Vectors, GoTo's, Labels, and out-of-order resolution of externs and function names are my next major milestones.
- An intuitive-enoug web UI that one can reverse-engineer some of the concepts at play.

Version 2 of this project means:
- Builtins that directly call javascript code. E.g. function registration for easily adding js functions to the runtime.
- A recreation of the [turtle](https://en.wikipedia.org/wiki/Turtle_(robot)) drawing program on the webpage, specifically modeled after the [python turtle](https://docs.python.org/3/library/turtle.html) project.

Version 3:
- Porting Scrub over (my reversible programming language/runtime), or adding record-rewind debugging abilities.
- Adding or switching between other languages that use the same interpreter. Something like [Postfix](https://cs.wellesley.edu/~cs251/s05/postfix.pdf), perhaps.

## Why B?

### Significant
[B (The Programming Language)[https://en.wikipedia.org/wiki/B_(programming_language)] came before C. It's an interesting artifact in being instrumental to modern languages, but (unlike fortran), not used today.

### Typeless
B is typeless. No types!
In my implementation, I use a 32-bit word. Everything is, basically, an int.
Types cause a lot of headaches and annoyances for students, and learning why they exist - by exploring the quirks of a languge where they *don't* exist - is, I believe, a worthwhile exercise.
Programming in B for a while
