﻿using Shuttle.Core.Contract;

namespace Shuttle.Core.Data;

public static class ScriptProviderExtensions
{
    public static string Get(this IScriptProvider scriptProvider, string connectionStringName, string scriptName, params object[] parameters)
    {
        return string.Format(Guard.AgainstNull(scriptProvider).Get(connectionStringName, scriptName), parameters);
    }
}