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
    console.log("Running Program...");
    clearRegister();
    exports.BMinusRuntime.RunProgram(p);

    var data = exports.BMinusRuntime.GetGlobals();
};

document.getElementById('step').onclick = ()=>{
    //if state is ready, firstStep
    var s = exports.BMinusRuntime.GetState();
    if(s == 5 || s == 4 || s == 3){//uninitiazed, complete, error
        var p = editor.state.doc.toString();
        clearRegister();
        exports.BMinusRuntime.CompileAndStep(p);
    }else if(s == 2){
    //if state is stepping, step
        exports.BMinusRuntime.Step();
    }else{
        //can't step, VM is
        console.log("can't step. VM is in state "+s);
    }
    //console.log(data);
};

function onOutput(output){
    output = output.replace(/(?:\r\n|\r|\n)/g, '<br>');
    document.getElementById('out').innerHTML = output;
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
    for (var r=0;r<reg.length;r++){
        reg[r].innerText = "0";
        if (reg[r].classList.contains('changed')) {
            reg[r].classList.remove('changed');
        }
    }
}

const instructionOutput = [
    document.getElementById("insName"),
    document.getElementById("insOperandA"),
    document.getElementById("insOperandB"),
    document.getElementById("insTooltip"),
];

function onInstruction(ins, opCount){
    instructionOutput[0].innerText = ins[0];
    instructionOutput[1].innerText = ins[1];
    instructionOutput[2].innerText = ins[2];
    instructionOutput[1].hidden = opCount <= 0;
    instructionOutput[2].hidden = opCount <= 1;
    instructionOutput[3].innerText = getTooltip(ins[0]);
}
function getTooltip(name)
{
    switch (name){
        case "SetReg":
            return "Sets Register Op2 to Value of Op1"
        case "Arithmetic":
            return "Does Operation Op1 on Registers A and B, puts result in X"
        case "SetGlobal":
            return "Sets global variable op1 to value of op2"
        
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


await dotnet.run();
