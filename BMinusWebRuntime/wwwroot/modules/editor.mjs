export {editor}

import {basicSetup} from "codemirror"
import {EditorView, keymap} from "@codemirror/view"
import {cppLanguage} from "@codemirror/lang-cpp"
import {defaultKeymap} from "@codemirror/commands"
import {EditorState} from "@codemirror/state"

const inputDiv = document.getElementById("input");

let startState = EditorState.create({
	doc: "auto a;\n" +
		"a = 1 + 2;\n" +
		"putint(a);",
	extensions: 
		[	basicSetup,
			cppLanguage,
			keymap.of(defaultKeymap),
		],
	viewportMargin: Infinity
})


let editor = new EditorView(
{
	state: startState,
	parent: inputDiv,
})
