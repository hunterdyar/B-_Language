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
    }
});

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

document.getElementById('execute').onclick = ()=>{
    var textarea = document.getElementById('input')
    var p = textarea.value;
    console.log("Running Program",p)
    var output = exports.BMinusRuntime.RunProgram(p);
        output = output.replace(/(?:\r\n|\r|\n)/g, '<br>');
    document.getElementById('out').innerHTML = output;
    var data = exports.BMinusRuntime.GetGlobals();
    console.log(data);
};


await dotnet.run();
