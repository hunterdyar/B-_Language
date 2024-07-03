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
    onState: onState,
    
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
        GetAndRenderAllInstructions();
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

const stateChip = document.getElementById('vmState');
onState(5);//assume uinitialized to start.
function onState(state){
    var text = "unknown";
    //see VMState.cs enum
    switch(state){
        case 0:
            text= "Ready";
            break;
        case 4:
            text = "Error";
            break;
        case 1:
            text = "Running";
            break;
        case 2:
            text = "Stepping (paused)";
            break;
        case 3:
            text = "Complete";
            break;
        case 5:
            text = "Uninitialized";
            break;
    }
    stateChip.innerText = text;
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
var inrow = null;
function onInstruction(ins){
    instructionOutput[0].innerText = ins[0];
    instructionOutput[1].innerText = ins[1];
    instructionOutput[2].innerText = ins[2];
    // instructionOutput[1].parentElement.hidden = ins[1].length===0;
    // instructionOutput[2].parentElement.hidden = ins[2].length===0;
    instructionOutput[3].innerText = getTooltip(ins[0]);
    let astID = Number.parseInt(ins[3]);
    let insLoc = ins[4];

    if(ast != null){
        if (ast.classList.contains('changed')) {
            ast.classList.remove('changed');
        }
    }
    ast = document.getElementById("ast-"+ins[3]);
    if (ast != null) {
        if (!ast.classList.contains('changed')) {
            ast.classList.add('changed');
        }
    }

    if (inrow != null) {
        if (inrow.classList.contains('changed')) {
            inrow.classList.remove('changed');
        }
    }
    inrow = document.getElementById("ins-" + insLoc);
    if (inrow != null) {
        if (!inrow.classList.contains('changed')) {
            inrow.classList.add('changed');
        }
        inrow.scrollIntoView({
            behavior: 'smooth',
            block: 'center'
        });
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

const fullInstructionList = document.getElementById("fullInstructions");
const instructionTabs = document.getElementById("tabLinks");
let currentActiveFrameLink = null;
let currentActiveFramePage = null;
let frames = [];

function setActiveInstructionList(index){
    currentActiveFrameLink?.classList.remove("active");
    currentActiveFrameLink = document.getElementById("frame-link-"+index.toString());
    currentActiveFrameLink.classList.add("active");

    currentActiveFramePage?.classList.remove("active");
    currentActiveFramePage = document.getElementById("frame-" + index.toString());
    currentActiveFramePage?.classList.add("active");
}
function GetAndRenderAllInstructions(){

    //clear existing
    instructionTabs.innerHTML = "";
    while (fullInstructionList.lastChild.id !== "tabLinks") {
        fullInstructionList.removeChild(fullInstructionList.lastChild);
    }
    var numberFrames = exports.BMinusRuntime.GetFrameCount();
    var firstlink;

    for (let curFrame = 0;curFrame<numberFrames;curFrame++) {

        var i = curFrame;
        var f = exports.BMinusRuntime.GetInstructions(curFrame);
        var link = document.createElement("a");
        link.id = "frame-link-" + i.toString();
        link.innerText = i === 0 ? "Instructions" : i.toString();
        if (curFrame === 0) {
            firstlink = link;
        }
        //todo: fix closure, make a function and make a closure that has the id.
        const id = i;
        link.addEventListener("click", () => setActiveInstructionList(id),false);
        
        instructionTabs.append(link);
        var pageContainer = document.createElement("div");
        pageContainer.classList.add("page");
        pageContainer.classList.add("scroll");
        pageContainer.classList.add("small-height");
        pageContainer.id = "frame-" + i.toString();
        fullInstructionList.append(pageContainer);

        var table = document.createElement("table");
        table.classList.add("border");
        table.classList.add("no-space");
        pageContainer.append(table);
        table.innerHTML = "<thead><th>#</th><th>Instruction</th ><th>Op A</th><th>Op B</th></thead>";
        var tableBody = document.createElement("tbody");
        table.append(tableBody);
        for (let j = 0; j < f.length; j += 5) {
            //ins.Op.ToString(), a, b, ins.ASTNodeID.ToString()
            var row = document.createElement("tr");
            row.classList.add("min");
            row.id = "ins-" + i.toString() + "-" + (j / 5).toString();
            row.innerHTML = "<td>"+(j/5)+" </td><td>" + f[j] + "</td><td>" + f[j + 1] + "</td><td>" + f[j + 2] + "</td>";
            tableBody.append(row);
        }

    }
    setActiveInstructionList(0);
}


await dotnet.run();
