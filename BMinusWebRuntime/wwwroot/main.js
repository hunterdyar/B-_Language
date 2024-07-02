// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import {dotnet} from './_framework/dotnet.js'
import {editor} from "./js/editor.bundle.js"

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

setModuleImports('main.js', {
    window: {
        location: {
            href: () => globalThis.window.location.href
        }
    },
    onOutput: onOutput,
    onRegister: onRegister,
    onInstruction: onInstruction,
    onStack: onStack,
    
});

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);
exports.BMinusRuntime.Init();

document.getElementById('execute').onclick = ()=>{
    var p = editor.state.doc.toString();
    clearRegister();
    exports.BMinusRuntime.RunProgram(p);
    RenderAST();
    
    var data = exports.BMinusRuntime.GetGlobals();
};

document.getElementById('compile').onclick = () => {
        clearOutput();
        var p = editor.state.doc.toString();
        exports.BMinusRuntime.Compile(p);
        RenderAST();
};

document.getElementById('step').onclick = ()=>{
    //if state is ready, firstStep
    var s = exports.BMinusRuntime.GetState();
    if(s == 5 || s == 4 || s == 3){//uninitiazed, complete, error
        var p = editor.state.doc.toString();
        clearOutput();
        exports.BMinusRuntime.Compile(p);
        RenderAST();
        exports.BMinusRuntime.Step();

    }else if(s == 2 || s == 0){
    //if state is stepping, or compiled/ready. step
        exports.BMinusRuntime.Step();
    }else{
        //can't step, VM is
        console.log("can't step. VM is in state "+s);
    }
    //console.log(data);
};

const output = document.getElementById('out');
function onOutput(outputText){
    console.log("on output");
    outputText = outputText.replace(/(?:\r\n|\r|\n)/g, '<br>');
    output.innerHTML = outputText;
}

const reg = [
    document.getElementById("regx"),
    document.getElementById("rega"),
    document.getElementById("regb"),
    document.getElementById("regc"),
    document.getElementById("regd"),
];
function onRegister(data){
//highlight the registers that changed on that frame.
    for (var r=0;r<reg.length;r++){
        var changed = data[r] != reg[r].innerText;
        reg[r].innerText = data[r];
        if(!changed) {
            if (reg[r].classList.contains('changed')) {
                reg[r].classList.remove('changed');
            }
        }else{
            if (!reg[r].classList.contains('changed')) {
                reg[r].classList.add('changed');
            }
        }
    }
}

function clearRegister(){
    for (var r = 0; r < reg.length; r++) {
        reg[r].innerText = "0";
        if (reg[r].classList.contains('changed')) {
            reg[r].classList.remove('changed');
        }
    }
}
function clearOutput(){
    clearRegister();
    output.innerHTML = "";
}

const instructionOutput = [
    document.getElementById("insName"),
    document.getElementById("insOperandA"),
    document.getElementById("insOperandB"),
    document.getElementById("insDetails"),
];

var ast = null;
function onInstruction(ins, astID, opCount){
    instructionOutput[0].innerText = ins[0];
    instructionOutput[1].innerText = ins[1];
    instructionOutput[2].innerText = ins[2];
    instructionOutput[1].parentElement.hidden = opCount <= 0;
    instructionOutput[2].parentElement.hidden = opCount <= 1;
    instructionOutput[3].innerText = getTooltip(ins[0]);
    
    if(ast != null){
        if (ast.classList.contains('changed')) {
            ast.classList.remove('changed');
        }
    }
    ast = document.getElementById("ast-"+astID.toString());
    if (ast != null) {
        if (!ast.classList.contains('changed')) {
            ast.classList.add('changed');
        }
    }
}
function getTooltip(name)
{
    switch (name){
        case "SetReg":
            return "Sets Register OpB to Value of OpA"
        case "Arithmetic":
            return "Does Operation OpA on Registers A and B, puts result in X"
        case "SetGlobal":
            return "Sets global variable OpA to value of OpB"
        
    }
    return name;
}

const stacklist = document.getElementById("stackList");
const stacksize = document.getElementById("stackSize");

function onStack(ins, opCount){
    stacksize.innerText = opCount;
    stacklist.innerHTML = "";
    for(var i = 0;i<ins.length;i++){
        stacklist.innerHTML += "<tr><td class=\"min\">"+i+"</td><td>"+ins[i]+"</td></tr>";
    }
}

const tree = document.getElementById("syntaxTree");
//todo: call from a "onCompiled"
function RenderAST(){
    var treeNode = exports.BMinusRuntime.GetAST();
    var n = JSON.parse(treeNode);
    tree.innerHTML = "";
    RenderTreeNode(tree,n);
}
function RenderTreeNode(parentNode, element){
    if(element["children"].length === 0){
        //is a leaf node.
        var name = document.createElement("p");
        parentNode.append(name);
        name.id = "ast-" + element["id"];
        if (element["label"] !== "") {
            name.innerText = element["label"] + ": " + element["name"];
        } else {
            name.innerText = element["name"].toString();
        }
        return;
    }
    var detailsNode = document.createElement("details");
    detailsNode.open = true;
    parentNode.append(detailsNode);
    
    name = document.createElement("summary");
    detailsNode.append(name);
    name.id = "ast-" + element["id"];

    var contentList = document.createElement("ul");
    detailsNode.append(contentList);
    
    //set name
    if(element["label"] !== "") {
        name.innerText = element["label"]+": "+element["name"];
    }else{
        name.innerText = element["name"];
    }
    
    //create children
    for(var i = 0;i<element.children.length;i++){
        var childItem = document.createElement("li");
        contentList.append(childItem);
        RenderTreeNode(childItem,element.children[i]);
    }
    
}


await dotnet.run();
