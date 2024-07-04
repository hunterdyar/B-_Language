// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// noinspection JSFileReferences
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
    onError: onError,
});

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);
exports.BMinusRuntime.Init();

document.getElementById('execute').onclick = ()=>{
    let p = editor.state.doc.toString();
    clearRegister();
    //todo: if already compiled, dont do this again (current state is ready)
    var success = exports.BMinusRuntime.Compile(p);
    
    if(success) {
        exports.BMinusRuntime.RunProgram(p);
        RenderAST();
    }
};

document.getElementById('compile').onclick = () => {
        clearOutput();
        let p = editor.state.doc.toString();
        var success = exports.BMinusRuntime.Compile(p);
        
        if(success) {
            RenderAST();
            GetAndRenderAllInstructions();
        }
};

document.getElementById('step').onclick = ()=>{
    //if state is ready, firstStep
    let s = exports.BMinusRuntime.GetState();
    if(s === 5 || s === 4 || s === 3){//uninitiazed, complete, error
        let p = editor.state.doc.toString();
        clearOutput();
        exports.BMinusRuntime.Compile(p);
        RenderAST();
        exports.BMinusRuntime.Step();

    }else if(s === 2 || s === 0){
    //if state is stepping, or compiled/ready. step
        exports.BMinusRuntime.Step();
    }else{
        //can't step, VM is
        console.log("can't step. VM is in state "+s);
    }
    //console.log(data);
};

const output = document.getElementById('out');
const outputContainer = output.parentElement.parentElement.parentElement;//the div s12
console.log(outputContainer);
const errorContainer = document.getElementById("errorContainer");
const errorMessage = document.getElementById("error");
errorContainer.parentElement.hidden = true;
function onOutput(outputText){
    outputText = outputText.replace(/(?:\r\n|\r|\n)/g, '<br>');
    output.hidden = false;
    output.innerHTML = outputText;
    errorContainer.parentElement.hidden = true;
}
function onError(eType, message){
    console.log(eType,message);
    errorContainer.parentElement.hidden = false;
    //use this section for different colors.
    if(eType === "lexer"){
        errorContainer.classList.add("tertiary");
    }else if(eType === "parser"){
        errorContainer.classList.add("tertiary");
    }else if(eType === "compiler"){
        errorContainer.classList.add("tertiary");
    }else if(eType === "vm"){
        errorContainer.classList.add("tertiary");
    }
    outputContainer.hidden = true;
    errorMessage.innerText = message;
}

const stateChip = document.getElementById('vmState');
onState(5);//assume uinitialized to start.
function onState(state){
    let text = "unknown";
    var showError = false;
    //see VMState.cs enum
    switch(state){
        case 0:
            text= "Ready";
            break;
        case 4:
            text = "Error";
            showError = true;
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

    outputContainer.hidden = showError;
    errorContainer.parentElement.hidden = !showError;
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
    for (let r=0;r<reg.length;r++){
        // noinspection EqualityComparisonWithCoercionJS
        let changed = data[r] != reg[r].innerText;
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
    for (let r = 0; r < reg.length; r++) {
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
    //i hate that we parse this. maybe two function calls that get sent back to back. "on new instruction" (3 strings) and "on new instruction location" (3 ints)
   // let astID = Number.parseInt(ins[3]);
    let frameProto = Number.parseInt(ins[4]);
    //let insloc = Number.parseInt(ins[5]);
    
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
    
    if(frameProto !== currentVisibleInstructionList){
        console.log("need to change frame");
        setActiveInstructionList(frameProto);
    }
    
    //use ins[4] and ins[5] since they are strings, and we cast to ints, and now... back to strings!
    inrow = document.getElementById("ins-" +ins[4]+"-"+ ins[5]);
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
    for(let i = 0;i<ins.length;i++){
        stacklist.innerHTML += "<tr><td class=\"min\">"+i+"</td><td>"+ins[i]+"</td></tr>";
    }
}

const tree = document.getElementById("syntaxTree");
//todo: call from a "onCompiled"
function RenderAST(){
    let treeNode = exports.BMinusRuntime.GetAST();
    let n = JSON.parse(treeNode);
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
    let detailsNode = document.createElement("details");
    detailsNode.open = true;
    parentNode.append(detailsNode);
    
    name = document.createElement("summary");
    detailsNode.append(name);
    name.id = "ast-" + element["id"];

    let contentList = document.createElement("ul");
    detailsNode.append(contentList);
    
    //set name
    if(element["label"] !== "") {
        name.innerText = element["label"]+": "+element["name"];
    }else{
        name.innerText = element["name"];
    }
    
    //create children
    for(let i = 0;i<element.children.length;i++){
        let childItem = document.createElement("li");
        contentList.append(childItem);
        RenderTreeNode(childItem,element.children[i]);
    }
}

const fullInstructionList = document.getElementById("fullInstructions");
const instructionTabs = document.getElementById("tabLinks");
let currentActiveFrameLink = null;
let currentActiveFramePage = null;
let currentVisibleInstructionList = 0;
function setActiveInstructionList(index){
    if(index === undefined){
        console.log("instructions askes to update, but in bad state. ignoring. ")
        return;
    }
    currentActiveFrameLink?.classList.remove("active");
    currentActiveFrameLink = document.getElementById("frame-link-"+index.toString());
    if(currentActiveFrameLink != null) {
        currentActiveFrameLink.classList.add("active");
    }else{
        console.log("null frame link - ",index);
    }

    currentActiveFramePage?.classList.remove("active");
    currentActiveFramePage = document.getElementById("frame-" + index.toString());
    currentActiveFramePage?.classList.add("active");
    currentVisibleInstructionList = index;
}
function GetAndRenderAllInstructions(){
    //clear existing
    instructionTabs.innerHTML = "";
    while (fullInstructionList.lastChild.id !== "tabLinks") {
        fullInstructionList.removeChild(fullInstructionList.lastChild);
    }
    let numberFrames = exports.BMinusRuntime.GetFrameCount();
    let firstlink;

    for (let curFrame = 0;curFrame<numberFrames;curFrame++) {

        const i = curFrame;
        let f = exports.BMinusRuntime.GetInstructions(curFrame);
        let link = document.createElement("a");
        link.id = "frame-link-" + i.toString();
        link.innerText = i === 0 ? "Instructions" : i.toString();
        if (curFrame === 0) {
            firstlink = link;
        }
        //todo: fix closure, make a function and make a closure that has the id.
        link.addEventListener("click", () => setActiveInstructionList(i),false);
        
        instructionTabs.append(link);
        const pageContainer = document.createElement("div");
        pageContainer.classList.add("page");
        pageContainer.classList.add("scroll");
        pageContainer.classList.add("small-height");
        pageContainer.id = "frame-" + i.toString();
        fullInstructionList.append(pageContainer);

        const table = document.createElement("table");
        table.classList.add("border");
        table.classList.add("no-space");
        pageContainer.append(table);
        table.innerHTML = "<thead><th>#</th><th>Instruction</th ><th>Op A</th><th>Op B</th></thead>";
        const tableBody = document.createElement("tbody");
        table.append(tableBody);
        for (let j = 0; j < f.length; j += 5) {
            //ins.Op.ToString(), a, b, ins.ASTNodeID.ToString()
            const row = document.createElement("tr");
            row.classList.add("min");
            row.id = "ins-" + i.toString() + "-" + (j / 5).toString();
            row.innerHTML = "<td>"+(j/5)+" </td><td>" + f[j] + "</td><td>" + f[j + 1] + "</td><td>" + f[j + 2] + "</td>";
            tableBody.append(row);
        }

    }
    setActiveInstructionList(0);
}


await dotnet.run();
