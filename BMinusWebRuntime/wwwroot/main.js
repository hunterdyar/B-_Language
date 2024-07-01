// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import {dotnet} from './_framework/dotnet.js'

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
    var textarea = document.getElementById('input')
    var p = textarea.value;
    console.log("Running Program...");
    exports.BMinusRuntime.RunProgram(p);

    var data = exports.BMinusRuntime.GetGlobals();
};

document.getElementById('step').onclick = ()=>{
    //if state is ready, firstStep
    var s = exports.BMinusRuntime.GetState();
    if(s == 5 || s == 4 || s == 3){//uninitiazed, complete, error
        var textarea = document.getElementById('input')
        var p = textarea.value;
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
        reg[r].innerText = data[r];
    }
}

const instructionOutput = [
    document.getElementById("insName"),
    document.getElementById("insOperandA"),
    document.getElementById("insOperandB"),
];

function onInstruction(ins, opCount){
    instructionOutput[0].innerText = ins[0];
    instructionOutput[1].innerText = ins[1];
    instructionOutput[2].innerText = ins[2];
    instructionOutput[1].hidden = opCount <= 0;
    instructionOutput[2].hidden = opCount <= 1;
}

const stacklist = document.getElementById("stackList");
const stacksize = document.getElementById("stackSize");

function onStack(ins, opCount){
    stacksize.innertext = opCount;
    stacklist.innerHTML = "";
    for(var i = 0;i<ins.length;i++){
        stacklist.innerHTML += "<li>"+ins[i]+"</li>";
    }
}


await dotnet.run();
