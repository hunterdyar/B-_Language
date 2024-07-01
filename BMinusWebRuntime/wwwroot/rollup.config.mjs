import {nodeResolve} from "@rollup/plugin-node-resolve"
export default {
    input: "./modules/editor.mjs",
    output: {
        file: "./js/editor.bundle.js",
        format: "es"
    },
    
    plugins: [nodeResolve()]
}

