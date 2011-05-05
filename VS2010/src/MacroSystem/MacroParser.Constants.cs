using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using GOLDSimpleParserEngine;

namespace GOLDSimpleParserEngine
{
    public partial class MacroParser
    {
        private object CreateObjectFromTerminal(Token token)
        {
            switch(token.Symbol.Id)
            {
                case (int)SymbolConstants.SYMBOL_EOF :
                    //(EOF)
                    return SYMBOL_EOF(token);

                case (int)SymbolConstants.SYMBOL_ERROR :
                    //(Error)
                    return SYMBOL_ERROR(token);

                case (int)SymbolConstants.SYMBOL_WHITESPACE :
                    //(Whitespace)
                    return SYMBOL_WHITESPACE(token);

                case (int)SymbolConstants.SYMBOL_COMMENTLINE :
                    //(Comment Line)
                    return SYMBOL_COMMENTLINE(token);

                case (int)SymbolConstants.SYMBOL_LPARAN :
                    //'('
                    return SYMBOL_LPARAN(token);

                case (int)SymbolConstants.SYMBOL_RPARAN :
                    //')'
                    return SYMBOL_RPARAN(token);

                case (int)SymbolConstants.SYMBOL_COMMA :
                    //','
                    return SYMBOL_COMMA(token);

                case (int)SymbolConstants.SYMBOL_LBRACKET :
                    //'['
                    return SYMBOL_LBRACKET(token);

                case (int)SymbolConstants.SYMBOL_RBRACKET :
                    //']'
                    return SYMBOL_RBRACKET(token);

                case (int)SymbolConstants.SYMBOL_COOL :
                    //cool
                    return SYMBOL_COOL(token);

                case (int)SymbolConstants.SYMBOL_DECIMAL :
                    //Decimal
                    return SYMBOL_DECIMAL(token);

                case (int)SymbolConstants.SYMBOL_DELAY :
                    //delay
                    return SYMBOL_DELAY(token);

                case (int)SymbolConstants.SYMBOL_DOWN :
                    //down
                    return SYMBOL_DOWN(token);

                case (int)SymbolConstants.SYMBOL_FALSE :
                    //false
                    return SYMBOL_FALSE(token);

                case (int)SymbolConstants.SYMBOL_IDENTIFIER :
                    //Identifier
                    return SYMBOL_IDENTIFIER(token);

                case (int)SymbolConstants.SYMBOL_INTEGER :
                    //Integer
                    return SYMBOL_INTEGER(token);

                case (int)SymbolConstants.SYMBOL_M :
                    //m
                    return SYMBOL_M(token);

                case (int)SymbolConstants.SYMBOL_MACRO :
                    //macro
                    return SYMBOL_MACRO(token);

                case (int)SymbolConstants.SYMBOL_MIN :
                    //min
                    return SYMBOL_MIN(token);

                case (int)SymbolConstants.SYMBOL_MS :
                    //ms
                    return SYMBOL_MS(token);

                case (int)SymbolConstants.SYMBOL_MSEC :
                    //msec
                    return SYMBOL_MSEC(token);

                case (int)SymbolConstants.SYMBOL_NAME :
                    //name
                    return SYMBOL_NAME(token);

                case (int)SymbolConstants.SYMBOL_NULL :
                    //null
                    return SYMBOL_NULL(token);

                case (int)SymbolConstants.SYMBOL_S :
                    //s
                    return SYMBOL_S(token);

                case (int)SymbolConstants.SYMBOL_SEC :
                    //sec
                    return SYMBOL_SEC(token);

                case (int)SymbolConstants.SYMBOL_STEPS :
                    //steps
                    return SYMBOL_STEPS(token);

                case (int)SymbolConstants.SYMBOL_STRING :
                    //String
                    return SYMBOL_STRING(token);

                case (int)SymbolConstants.SYMBOL_TAP :
                    //tap
                    return SYMBOL_TAP(token);

                case (int)SymbolConstants.SYMBOL_TRUE :
                    //true
                    return SYMBOL_TRUE(token);

                case (int)SymbolConstants.SYMBOL_UP :
                    //up
                    return SYMBOL_UP(token);

                case (int)SymbolConstants.SYMBOL_DELAY2 :
                    //<Delay>
                    return SYMBOL_DELAY2(token);

                case (int)SymbolConstants.SYMBOL_DOWNSTEP :
                    //<DownStep>
                    return SYMBOL_DOWNSTEP(token);

                case (int)SymbolConstants.SYMBOL_FUNCTIONARG :
                    //<FunctionArg>
                    return SYMBOL_FUNCTIONARG(token);

                case (int)SymbolConstants.SYMBOL_FUNCTIONARGLIST :
                    //<FunctionArgList>
                    return SYMBOL_FUNCTIONARGLIST(token);

                case (int)SymbolConstants.SYMBOL_FUNCTIONCALL :
                    //<FunctionCall>
                    return SYMBOL_FUNCTIONCALL(token);

                case (int)SymbolConstants.SYMBOL_KEYSTEP :
                    //<KeyStep>
                    return SYMBOL_KEYSTEP(token);

                case (int)SymbolConstants.SYMBOL_KEYSTEPLIST :
                    //<KeyStepList>
                    return SYMBOL_KEYSTEPLIST(token);

                case (int)SymbolConstants.SYMBOL_MACRO2 :
                    //<Macro>
                    return SYMBOL_MACRO2(token);

                case (int)SymbolConstants.SYMBOL_MACROLIST :
                    //<MacroList>
                    return SYMBOL_MACROLIST(token);

                case (int)SymbolConstants.SYMBOL_MACROPART :
                    //<MacroPart>
                    return SYMBOL_MACROPART(token);

                case (int)SymbolConstants.SYMBOL_MACROPARTS :
                    //<MacroParts>
                    return SYMBOL_MACROPARTS(token);

                case (int)SymbolConstants.SYMBOL_STEP :
                    //<Step>
                    return SYMBOL_STEP(token);

                case (int)SymbolConstants.SYMBOL_STEPLIST :
                    //<StepList>
                    return SYMBOL_STEPLIST(token);

                case (int)SymbolConstants.SYMBOL_TAPSTEP :
                    //<TapStep>
                    return SYMBOL_TAPSTEP(token);

                case (int)SymbolConstants.SYMBOL_TIMEUNIT :
                    //<TimeUnit>
                    return SYMBOL_TIMEUNIT(token);

                case (int)SymbolConstants.SYMBOL_UPSTEP :
                    //<UpStep>
                    return SYMBOL_UPSTEP(token);

            }
            
            throw new SymbolException("Unknown symbol " + token.Symbol.Id);
        }
        
        private object CreateObjectFromNonterminal(Token token)
        {
            switch (token.Rule.Id)
            {
                case (int)RuleConstants.RULE_MACROLIST :
                    //<MacroList> ::= <Macro>
                    return RULE_MACROLIST(token);

                case (int)RuleConstants.RULE_MACROLIST2 :
                    //<MacroList> ::= <Macro> <MacroList>
                    return RULE_MACROLIST2(token);

                case (int)RuleConstants.RULE_MACRO_MACRO_LBRACKET_RBRACKET :
                    //<Macro> ::= macro '[' <MacroParts> ']'
                    return RULE_MACRO_MACRO_LBRACKET_RBRACKET(token);

                case (int)RuleConstants.RULE_MACROPARTS :
                    //<MacroParts> ::= <MacroPart>
                    return RULE_MACROPARTS(token);

                case (int)RuleConstants.RULE_MACROPARTS2 :
                    //<MacroParts> ::= <MacroPart> <MacroParts>
                    return RULE_MACROPARTS2(token);

                case (int)RuleConstants.RULE_MACROPART_NAME_STRING :
                    //<MacroPart> ::= name String
                    return RULE_MACROPART_NAME_STRING(token);

                case (int)RuleConstants.RULE_MACROPART_STEPS :
                    //<MacroPart> ::= steps <StepList>
                    return RULE_MACROPART_STEPS(token);

                case (int)RuleConstants.RULE_MACROPART_COOL_INTEGER :
                    //<MacroPart> ::= cool Integer <TimeUnit>
                    return RULE_MACROPART_COOL_INTEGER(token);

                case (int)RuleConstants.RULE_TIMEUNIT_MS :
                    //<TimeUnit> ::= ms
                    return RULE_TIMEUNIT_MS(token);

                case (int)RuleConstants.RULE_TIMEUNIT_S :
                    //<TimeUnit> ::= s
                    return RULE_TIMEUNIT_S(token);

                case (int)RuleConstants.RULE_TIMEUNIT_M :
                    //<TimeUnit> ::= m
                    return RULE_TIMEUNIT_M(token);

                case (int)RuleConstants.RULE_TIMEUNIT_MSEC :
                    //<TimeUnit> ::= msec
                    return RULE_TIMEUNIT_MSEC(token);

                case (int)RuleConstants.RULE_TIMEUNIT_SEC :
                    //<TimeUnit> ::= sec
                    return RULE_TIMEUNIT_SEC(token);

                case (int)RuleConstants.RULE_TIMEUNIT_MIN :
                    //<TimeUnit> ::= min
                    return RULE_TIMEUNIT_MIN(token);

                case (int)RuleConstants.RULE_STEPLIST :
                    //<StepList> ::= <Step>
                    return RULE_STEPLIST(token);

                case (int)RuleConstants.RULE_STEPLIST2 :
                    //<StepList> ::= <Step> <StepList>
                    return RULE_STEPLIST2(token);

                case (int)RuleConstants.RULE_STEP :
                    //<Step> ::= <DownStep>
                    return RULE_STEP(token);

                case (int)RuleConstants.RULE_STEP2 :
                    //<Step> ::= <UpStep>
                    return RULE_STEP2(token);

                case (int)RuleConstants.RULE_STEP3 :
                    //<Step> ::= <TapStep>
                    return RULE_STEP3(token);

                case (int)RuleConstants.RULE_STEP4 :
                    //<Step> ::= <Delay>
                    return RULE_STEP4(token);

                case (int)RuleConstants.RULE_STEP5 :
                    //<Step> ::= <FunctionCall>
                    return RULE_STEP5(token);

                case (int)RuleConstants.RULE_STEP6 :
                    //<Step> ::= <Macro>
                    return RULE_STEP6(token);

                case (int)RuleConstants.RULE_DOWNSTEP_DOWN :
                    //<DownStep> ::= down <KeyStepList>
                    return RULE_DOWNSTEP_DOWN(token);

                case (int)RuleConstants.RULE_UPSTEP_UP :
                    //<UpStep> ::= up <KeyStepList>
                    return RULE_UPSTEP_UP(token);

                case (int)RuleConstants.RULE_TAPSTEP_TAP :
                    //<TapStep> ::= tap <KeyStepList>
                    return RULE_TAPSTEP_TAP(token);

                case (int)RuleConstants.RULE_DELAY_DELAY_INTEGER :
                    //<Delay> ::= delay Integer <TimeUnit>
                    return RULE_DELAY_DELAY_INTEGER(token);

                case (int)RuleConstants.RULE_KEYSTEPLIST :
                    //<KeyStepList> ::= <KeyStep>
                    return RULE_KEYSTEPLIST(token);

                case (int)RuleConstants.RULE_KEYSTEPLIST_COMMA :
                    //<KeyStepList> ::= <KeyStep> ',' <KeyStepList>
                    return RULE_KEYSTEPLIST_COMMA(token);

                case (int)RuleConstants.RULE_KEYSTEP_IDENTIFIER :
                    //<KeyStep> ::= Identifier
                    return RULE_KEYSTEP_IDENTIFIER(token);

                case (int)RuleConstants.RULE_KEYSTEP_COOL_INTEGER :
                    //<KeyStep> ::= cool Integer <TimeUnit>
                    return RULE_KEYSTEP_COOL_INTEGER(token);

                case (int)RuleConstants.RULE_FUNCTIONCALL_IDENTIFIER_LPARAN_RPARAN :
                    //<FunctionCall> ::= Identifier '(' <FunctionArgList> ')'
                    return RULE_FUNCTIONCALL_IDENTIFIER_LPARAN_RPARAN(token);

                case (int)RuleConstants.RULE_FUNCTIONCALL_IDENTIFIER_LPARAN_RPARAN2 :
                    //<FunctionCall> ::= Identifier '(' ')'
                    return RULE_FUNCTIONCALL_IDENTIFIER_LPARAN_RPARAN2(token);

                case (int)RuleConstants.RULE_FUNCTIONARGLIST :
                    //<FunctionArgList> ::= <FunctionArg>
                    return RULE_FUNCTIONARGLIST(token);

                case (int)RuleConstants.RULE_FUNCTIONARGLIST_COMMA :
                    //<FunctionArgList> ::= <FunctionArg> ',' <FunctionArgList>
                    return RULE_FUNCTIONARGLIST_COMMA(token);

                case (int)RuleConstants.RULE_FUNCTIONARG :
                    //<FunctionArg> ::= <FunctionCall>
                    return RULE_FUNCTIONARG(token);

                case (int)RuleConstants.RULE_FUNCTIONARG_INTEGER :
                    //<FunctionArg> ::= Integer
                    return RULE_FUNCTIONARG_INTEGER(token);

                case (int)RuleConstants.RULE_FUNCTIONARG_DECIMAL :
                    //<FunctionArg> ::= Decimal
                    return RULE_FUNCTIONARG_DECIMAL(token);

                case (int)RuleConstants.RULE_FUNCTIONARG_STRING :
                    //<FunctionArg> ::= String
                    return RULE_FUNCTIONARG_STRING(token);

                case (int)RuleConstants.RULE_FUNCTIONARG_TRUE :
                    //<FunctionArg> ::= true
                    return RULE_FUNCTIONARG_TRUE(token);

                case (int)RuleConstants.RULE_FUNCTIONARG_FALSE :
                    //<FunctionArg> ::= false
                    return RULE_FUNCTIONARG_FALSE(token);

                case (int)RuleConstants.RULE_FUNCTIONARG_NULL :
                    //<FunctionArg> ::= null
                    return RULE_FUNCTIONARG_NULL(token);

                case (int)RuleConstants.RULE_FUNCTIONARG_IDENTIFIER :
                    //<FunctionArg> ::= Identifier
                    return RULE_FUNCTIONARG_IDENTIFIER(token);

            }
            
            throw new RuleException("Unknown rule " + token.Rule.Id);
        }

        #region Support Classes

        enum SymbolConstants : int
        {
            SYMBOL_EOF             =  0, // (EOF)
            SYMBOL_ERROR           =  1, // (Error)
            SYMBOL_WHITESPACE      =  2, // (Whitespace)
            SYMBOL_COMMENTLINE     =  3, // (Comment Line)
            SYMBOL_LPARAN          =  4, // '('
            SYMBOL_RPARAN          =  5, // ')'
            SYMBOL_COMMA           =  6, // ','
            SYMBOL_LBRACKET        =  7, // '['
            SYMBOL_RBRACKET        =  8, // ']'
            SYMBOL_COOL            =  9, // cool
            SYMBOL_DECIMAL         = 10, // Decimal
            SYMBOL_DELAY           = 11, // delay
            SYMBOL_DOWN            = 12, // down
            SYMBOL_FALSE           = 13, // false
            SYMBOL_IDENTIFIER      = 14, // Identifier
            SYMBOL_INTEGER         = 15, // Integer
            SYMBOL_M               = 16, // m
            SYMBOL_MACRO           = 17, // macro
            SYMBOL_MIN             = 18, // min
            SYMBOL_MS              = 19, // ms
            SYMBOL_MSEC            = 20, // msec
            SYMBOL_NAME            = 21, // name
            SYMBOL_NULL            = 22, // null
            SYMBOL_S               = 23, // s
            SYMBOL_SEC             = 24, // sec
            SYMBOL_STEPS           = 25, // steps
            SYMBOL_STRING          = 26, // String
            SYMBOL_TAP             = 27, // tap
            SYMBOL_TRUE            = 28, // true
            SYMBOL_UP              = 29, // up
            SYMBOL_DELAY2          = 30, // <Delay>
            SYMBOL_DOWNSTEP        = 31, // <DownStep>
            SYMBOL_FUNCTIONARG     = 32, // <FunctionArg>
            SYMBOL_FUNCTIONARGLIST = 33, // <FunctionArgList>
            SYMBOL_FUNCTIONCALL    = 34, // <FunctionCall>
            SYMBOL_KEYSTEP         = 35, // <KeyStep>
            SYMBOL_KEYSTEPLIST     = 36, // <KeyStepList>
            SYMBOL_MACRO2          = 37, // <Macro>
            SYMBOL_MACROLIST       = 38, // <MacroList>
            SYMBOL_MACROPART       = 39, // <MacroPart>
            SYMBOL_MACROPARTS      = 40, // <MacroParts>
            SYMBOL_STEP            = 41, // <Step>
            SYMBOL_STEPLIST        = 42, // <StepList>
            SYMBOL_TAPSTEP         = 43, // <TapStep>
            SYMBOL_TIMEUNIT        = 44, // <TimeUnit>
            SYMBOL_UPSTEP          = 45  // <UpStep>
        };

        enum RuleConstants : int
        {
            RULE_MACROLIST                              =  0, // <MacroList> ::= <Macro>
            RULE_MACROLIST2                             =  1, // <MacroList> ::= <Macro> <MacroList>
            RULE_MACRO_MACRO_LBRACKET_RBRACKET          =  2, // <Macro> ::= macro '[' <MacroParts> ']'
            RULE_MACROPARTS                             =  3, // <MacroParts> ::= <MacroPart>
            RULE_MACROPARTS2                            =  4, // <MacroParts> ::= <MacroPart> <MacroParts>
            RULE_MACROPART_NAME_STRING                  =  5, // <MacroPart> ::= name String
            RULE_MACROPART_STEPS                        =  6, // <MacroPart> ::= steps <StepList>
            RULE_MACROPART_COOL_INTEGER                 =  7, // <MacroPart> ::= cool Integer <TimeUnit>
            RULE_TIMEUNIT_MS                            =  8, // <TimeUnit> ::= ms
            RULE_TIMEUNIT_S                             =  9, // <TimeUnit> ::= s
            RULE_TIMEUNIT_M                             = 10, // <TimeUnit> ::= m
            RULE_TIMEUNIT_MSEC                          = 11, // <TimeUnit> ::= msec
            RULE_TIMEUNIT_SEC                           = 12, // <TimeUnit> ::= sec
            RULE_TIMEUNIT_MIN                           = 13, // <TimeUnit> ::= min
            RULE_STEPLIST                               = 14, // <StepList> ::= <Step>
            RULE_STEPLIST2                              = 15, // <StepList> ::= <Step> <StepList>
            RULE_STEP                                   = 16, // <Step> ::= <DownStep>
            RULE_STEP2                                  = 17, // <Step> ::= <UpStep>
            RULE_STEP3                                  = 18, // <Step> ::= <TapStep>
            RULE_STEP4                                  = 19, // <Step> ::= <Delay>
            RULE_STEP5                                  = 20, // <Step> ::= <FunctionCall>
            RULE_STEP6                                  = 21, // <Step> ::= <Macro>
            RULE_DOWNSTEP_DOWN                          = 22, // <DownStep> ::= down <KeyStepList>
            RULE_UPSTEP_UP                              = 23, // <UpStep> ::= up <KeyStepList>
            RULE_TAPSTEP_TAP                            = 24, // <TapStep> ::= tap <KeyStepList>
            RULE_DELAY_DELAY_INTEGER                    = 25, // <Delay> ::= delay Integer <TimeUnit>
            RULE_KEYSTEPLIST                            = 26, // <KeyStepList> ::= <KeyStep>
            RULE_KEYSTEPLIST_COMMA                      = 27, // <KeyStepList> ::= <KeyStep> ',' <KeyStepList>
            RULE_KEYSTEP_IDENTIFIER                     = 28, // <KeyStep> ::= Identifier
            RULE_KEYSTEP_COOL_INTEGER                   = 29, // <KeyStep> ::= cool Integer <TimeUnit>
            RULE_FUNCTIONCALL_IDENTIFIER_LPARAN_RPARAN  = 30, // <FunctionCall> ::= Identifier '(' <FunctionArgList> ')'
            RULE_FUNCTIONCALL_IDENTIFIER_LPARAN_RPARAN2 = 31, // <FunctionCall> ::= Identifier '(' ')'
            RULE_FUNCTIONARGLIST                        = 32, // <FunctionArgList> ::= <FunctionArg>
            RULE_FUNCTIONARGLIST_COMMA                  = 33, // <FunctionArgList> ::= <FunctionArg> ',' <FunctionArgList>
            RULE_FUNCTIONARG                            = 34, // <FunctionArg> ::= <FunctionCall>
            RULE_FUNCTIONARG_INTEGER                    = 35, // <FunctionArg> ::= Integer
            RULE_FUNCTIONARG_DECIMAL                    = 36, // <FunctionArg> ::= Decimal
            RULE_FUNCTIONARG_STRING                     = 37, // <FunctionArg> ::= String
            RULE_FUNCTIONARG_TRUE                       = 38, // <FunctionArg> ::= true
            RULE_FUNCTIONARG_FALSE                      = 39, // <FunctionArg> ::= false
            RULE_FUNCTIONARG_NULL                       = 40, // <FunctionArg> ::= null
            RULE_FUNCTIONARG_IDENTIFIER                 = 41  // <FunctionArg> ::= Identifier
        };

        #endregion
    
    }
}
