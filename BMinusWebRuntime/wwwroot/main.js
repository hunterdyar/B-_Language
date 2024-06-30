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
    onOutput: onOutput
});

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

document.getElementById('execute').onclick = ()=>{
    var textarea = document.getElementById('input')
    var p = textarea.value;
    console.log("Running Program",p)
    exports.BMinusRuntime.RunProgram(p);

    var data = exports.BMinusRuntime.GetGlobals();
    console.log(data);
};

function onOutput(output){
    output = output.replace(/(?:\r\n|\r|\n)/g, '<br>');
    document.getElementById('out').innerHTML = output;
}

await dotnet.run();
