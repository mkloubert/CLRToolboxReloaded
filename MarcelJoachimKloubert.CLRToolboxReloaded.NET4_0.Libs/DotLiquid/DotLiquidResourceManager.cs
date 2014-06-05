// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Libs.DotLiquid
{
    internal sealed class DotLiquidResourceManager
    {
        #region Methods (1)

        internal string GetString(string name)
        {
            switch (name)
            {
                case "AssignTagSyntaxException":
                    return "Syntax Error in 'assign' tag - Valid syntax: assign [var] = [source]";

                case "BlankFileSystemDoesNotAllowIncludesException":
                    return "Error - This liquid context does not allow includes";

                case "BlockTagAlreadyDefinedException":
                    return "Liquid Error - Block '{0}' already defined";

                case "BlockTagNoElseException":
                    return "{0} tag does not expect else tag";

                case "BlockTagNoEndException":
                    return "'end' is not a valid delimiter for {0} tags. Use {1}";

                case "BlockTagNotClosedException":
                    return "{0} tag was never closed";

                case "BlockTagNotTerminatedException":
                    return "Tag '{0}' was not properly terminated with regexp: {1}";

                case "BlockTagSyntaxException":
                    return "Syntax Error in 'block' tag - Valid syntax: block [name]";

                case "BlockUnknownTagException":
                    return "Unknown tag '{0}'";

                case "BlockVariableNotTerminatedException":
                    return "Variable '{0}' was not properly terminated with regexp: {1}";

                case "CapureTagSyntaxException":
                    return "Syntax Error in 'capture' tag - Valid syntax: capture [var]";

                case "CaseTagElseSyntaxException":
                    return "Syntax Error in 'case' tag - Valid else condition: {{% else %}} (no parameters)";

                case "CaseTagSyntaxException":
                    return "Syntax Error in 'case' tag - Valid syntax: case [condition]";

                case "CaseTagWhenSyntaxException":
                    return "Syntax Error in 'case' tag - Valid when condition: {{% when [condition] [or condition2...] %}}";

                case "ConditionUnknownOperatorException":
                    return "Unknown operator {0}";

                case "ContextLiquidError":
                    return "Liquid error: {0}";

                case "ContextLiquidSyntaxError":
                    return "Liquid syntax error: {0}";

                case "ContextObjectInvalidException":
                    return "Object '{0}' is invalid because it is neither a built-in type nor implements ILiquidizable";

                case "ContextStackException":
                    return "Nesting too deep";

                case "CycleTagSyntaxException":
                    return "Syntax Error in 'cycle' tag - Valid syntax: cycle [name :] var [, var2, var3 ...]";

                case "DropWrongNamingConventionMessage":
                    return "Missing property. Did you mean '{0}'?";

                case "ExtendsTagCanBeUsedOneException":
                    return "Liquid Error - 'extends' tag can be used only once";

                case "ExtendsTagMustBeFirstTagException":
                    return "Liquid Error - 'extends' must be the first tag in an extending template";

                case "ExtendsTagSyntaxException":
                    return "Syntax Error in 'extends' tag - Valid syntax: extends [template]";

                case "ExtendsTagUnallowedTagsException":
                    return "Liquid Error - Only 'comment' and 'block' tags are allowed in an extending template";

                case "ForTagSyntaxException":
                    return "Syntax Error in 'for' tag - Valid syntax: for [item] in [collection]";

                case "IfTagSyntaxException":
                    return "Syntax Error in 'if' tag - Valid syntax: if [expression]";

                case "IncludeTagSyntaxException":
                    return "Syntax Error in 'include' tag - Valid syntax: include [template]";

                case "LocalFileSystemIllegalTemplateNameException":
                    return "Error - Illegal template name '{0}'";

                case "LocalFileSystemIllegalTemplatePathException":
                    return "Error - Illegal template path '{0}'";

                case "LocalFileSystemTemplateNotFoundException":
                    return "Error - No such template '{0}'";

                case "StrainerFilterHasNoValueException":
                    return "Error - Filter '{0}' does not have a default value for '{1}' and no value was supplied";

                case "TableRowTagSyntaxException":
                    return "Syntax Error in 'tablerow' tag - Valid syntax: tablerow [item] in [collection] cols=[number]";

                case "VariableFilterNotFoundException":
                    return "Error - Filter '{0}' in '{1}' could not be found.";

                case "WeakTableKeyNotFoundException":
                    return "key could not be found";
            }

            return name;
        }

        #endregion Methods (1)
    }
}