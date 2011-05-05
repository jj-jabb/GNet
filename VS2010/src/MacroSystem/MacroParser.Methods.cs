using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using GOLDSimpleParserEngine;

namespace GOLDSimpleParserEngine
{
    // This partial implements the methods used by the Constants partial
    public partial class MacroParser
    {
        private const string CGTResourceName = "GNet.MacroSystem.Macro.cgt";
        
        #region Symbols

        // SYMBOL_EOF : (EOF)
        private object SYMBOL_EOF(Token token)
        {
            return null;
        }
        
        // SYMBOL_ERROR : (Error)
        private object SYMBOL_ERROR(Token token)
        {
            return null;
        }
        
        // SYMBOL_WHITESPACE : (Whitespace)
        private object SYMBOL_WHITESPACE(Token token)
        {
            return null;
        }
        
        // SYMBOL_COMMENTLINE : (Comment Line)
        private object SYMBOL_COMMENTLINE(Token token)
        {
            return null;
        }
        
        // SYMBOL_LPARAN : '('
        private object SYMBOL_LPARAN(Token token)
        {
            return null;
        }
        
        // SYMBOL_RPARAN : ')'
        private object SYMBOL_RPARAN(Token token)
        {
            return null;
        }
        
        // SYMBOL_COMMA : ','
        private object SYMBOL_COMMA(Token token)
        {
            return null;
        }
        
        // SYMBOL_LBRACKET : '['
        private object SYMBOL_LBRACKET(Token token)
        {
            return null;
        }
        
        // SYMBOL_RBRACKET : ']'
        private object SYMBOL_RBRACKET(Token token)
        {
            return null;
        }
        
        // SYMBOL_COOL : cool
        private object SYMBOL_COOL(Token token)
        {
            return null;
        }
        
        // SYMBOL_DECIMAL : Decimal
        private object SYMBOL_DECIMAL(Token token)
        {
            return null;
        }
        
        // SYMBOL_DELAY : delay
        private object SYMBOL_DELAY(Token token)
        {
            return null;
        }
        
        // SYMBOL_DOWN : down
        private object SYMBOL_DOWN(Token token)
        {
            return null;
        }
        
        // SYMBOL_FALSE : false
        private object SYMBOL_FALSE(Token token)
        {
            return null;
        }
        
        // SYMBOL_IDENTIFIER : Identifier
        private object SYMBOL_IDENTIFIER(Token token)
        {
            return null;
        }
        
        // SYMBOL_INTEGER : Integer
        private object SYMBOL_INTEGER(Token token)
        {
            return null;
        }
        
        // SYMBOL_M : m
        private object SYMBOL_M(Token token)
        {
            return null;
        }
        
        // SYMBOL_MACRO : macro
        private object SYMBOL_MACRO(Token token)
        {
            return null;
        }
        
        // SYMBOL_MIN : min
        private object SYMBOL_MIN(Token token)
        {
            return null;
        }
        
        // SYMBOL_MS : ms
        private object SYMBOL_MS(Token token)
        {
            return null;
        }
        
        // SYMBOL_MSEC : msec
        private object SYMBOL_MSEC(Token token)
        {
            return null;
        }
        
        // SYMBOL_NAME : name
        private object SYMBOL_NAME(Token token)
        {
            return null;
        }
        
        // SYMBOL_NULL : null
        private object SYMBOL_NULL(Token token)
        {
            return null;
        }
        
        // SYMBOL_S : s
        private object SYMBOL_S(Token token)
        {
            return null;
        }
        
        // SYMBOL_SEC : sec
        private object SYMBOL_SEC(Token token)
        {
            return null;
        }
        
        // SYMBOL_STEPS : steps
        private object SYMBOL_STEPS(Token token)
        {
            return null;
        }
        
        // SYMBOL_STRING : String
        private object SYMBOL_STRING(Token token)
        {
            return null;
        }
        
        // SYMBOL_TAP : tap
        private object SYMBOL_TAP(Token token)
        {
            return null;
        }
        
        // SYMBOL_TRUE : true
        private object SYMBOL_TRUE(Token token)
        {
            return null;
        }
        
        // SYMBOL_UP : up
        private object SYMBOL_UP(Token token)
        {
            return null;
        }
        
        // SYMBOL_DELAY2 : <Delay>
        private object SYMBOL_DELAY2(Token token)
        {
            return null;
        }
        
        // SYMBOL_DOWNSTEP : <DownStep>
        private object SYMBOL_DOWNSTEP(Token token)
        {
            return null;
        }
        
        // SYMBOL_FUNCTIONARG : <FunctionArg>
        private object SYMBOL_FUNCTIONARG(Token token)
        {
            return null;
        }
        
        // SYMBOL_FUNCTIONARGLIST : <FunctionArgList>
        private object SYMBOL_FUNCTIONARGLIST(Token token)
        {
            return null;
        }
        
        // SYMBOL_FUNCTIONCALL : <FunctionCall>
        private object SYMBOL_FUNCTIONCALL(Token token)
        {
            return null;
        }
        
        // SYMBOL_KEYSTEP : <KeyStep>
        private object SYMBOL_KEYSTEP(Token token)
        {
            return null;
        }
        
        // SYMBOL_KEYSTEPLIST : <KeyStepList>
        private object SYMBOL_KEYSTEPLIST(Token token)
        {
            return null;
        }
        
        // SYMBOL_MACRO2 : <Macro>
        private object SYMBOL_MACRO2(Token token)
        {
            return null;
        }
        
        // SYMBOL_MACROLIST : <MacroList>
        private object SYMBOL_MACROLIST(Token token)
        {
            return null;
        }
        
        // SYMBOL_MACROPART : <MacroPart>
        private object SYMBOL_MACROPART(Token token)
        {
            return null;
        }
        
        // SYMBOL_MACROPARTS : <MacroParts>
        private object SYMBOL_MACROPARTS(Token token)
        {
            return null;
        }
        
        // SYMBOL_STEP : <Step>
        private object SYMBOL_STEP(Token token)
        {
            return null;
        }
        
        // SYMBOL_STEPLIST : <StepList>
        private object SYMBOL_STEPLIST(Token token)
        {
            return null;
        }
        
        // SYMBOL_TAPSTEP : <TapStep>
        private object SYMBOL_TAPSTEP(Token token)
        {
            return null;
        }
        
        // SYMBOL_TIMEUNIT : <TimeUnit>
        private object SYMBOL_TIMEUNIT(Token token)
        {
            return null;
        }
        
        // SYMBOL_UPSTEP : <UpStep>
        private object SYMBOL_UPSTEP(Token token)
        {
            return null;
        }
        

        #endregion

        #region Rules

        // RULE_MACROLIST : <MacroList> ::= <Macro>
        private object RULE_MACROLIST(Token token)
        {
            return null;
        }
        
        // RULE_MACROLIST2 : <MacroList> ::= <Macro> <MacroList>
        private object RULE_MACROLIST2(Token token)
        {
            return null;
        }
        
        // RULE_MACRO_MACRO_LBRACKET_RBRACKET : <Macro> ::= macro '[' <MacroParts> ']'
        private object RULE_MACRO_MACRO_LBRACKET_RBRACKET(Token token)
        {
            return null;
        }
        
        // RULE_MACROPARTS : <MacroParts> ::= <MacroPart>
        private object RULE_MACROPARTS(Token token)
        {
            return null;
        }
        
        // RULE_MACROPARTS2 : <MacroParts> ::= <MacroPart> <MacroParts>
        private object RULE_MACROPARTS2(Token token)
        {
            return null;
        }
        
        // RULE_MACROPART_NAME_STRING : <MacroPart> ::= name String
        private object RULE_MACROPART_NAME_STRING(Token token)
        {
            return null;
        }
        
        // RULE_MACROPART_STEPS : <MacroPart> ::= steps <StepList>
        private object RULE_MACROPART_STEPS(Token token)
        {
            return null;
        }
        
        // RULE_MACROPART_COOL_INTEGER : <MacroPart> ::= cool Integer <TimeUnit>
        private object RULE_MACROPART_COOL_INTEGER(Token token)
        {
            return null;
        }
        
        // RULE_TIMEUNIT_MS : <TimeUnit> ::= ms
        private object RULE_TIMEUNIT_MS(Token token)
        {
            return null;
        }
        
        // RULE_TIMEUNIT_S : <TimeUnit> ::= s
        private object RULE_TIMEUNIT_S(Token token)
        {
            return null;
        }
        
        // RULE_TIMEUNIT_M : <TimeUnit> ::= m
        private object RULE_TIMEUNIT_M(Token token)
        {
            return null;
        }
        
        // RULE_TIMEUNIT_MSEC : <TimeUnit> ::= msec
        private object RULE_TIMEUNIT_MSEC(Token token)
        {
            return null;
        }
        
        // RULE_TIMEUNIT_SEC : <TimeUnit> ::= sec
        private object RULE_TIMEUNIT_SEC(Token token)
        {
            return null;
        }
        
        // RULE_TIMEUNIT_MIN : <TimeUnit> ::= min
        private object RULE_TIMEUNIT_MIN(Token token)
        {
            return null;
        }
        
        // RULE_STEPLIST : <StepList> ::= <Step>
        private object RULE_STEPLIST(Token token)
        {
            return null;
        }
        
        // RULE_STEPLIST2 : <StepList> ::= <Step> <StepList>
        private object RULE_STEPLIST2(Token token)
        {
            return null;
        }
        
        // RULE_STEP : <Step> ::= <DownStep>
        private object RULE_STEP(Token token)
        {
            return null;
        }
        
        // RULE_STEP2 : <Step> ::= <UpStep>
        private object RULE_STEP2(Token token)
        {
            return null;
        }
        
        // RULE_STEP3 : <Step> ::= <TapStep>
        private object RULE_STEP3(Token token)
        {
            return null;
        }
        
        // RULE_STEP4 : <Step> ::= <Delay>
        private object RULE_STEP4(Token token)
        {
            return null;
        }
        
        // RULE_STEP5 : <Step> ::= <FunctionCall>
        private object RULE_STEP5(Token token)
        {
            return null;
        }
        
        // RULE_STEP6 : <Step> ::= <Macro>
        private object RULE_STEP6(Token token)
        {
            return null;
        }
        
        // RULE_DOWNSTEP_DOWN : <DownStep> ::= down <KeyStepList>
        private object RULE_DOWNSTEP_DOWN(Token token)
        {
            return null;
        }
        
        // RULE_UPSTEP_UP : <UpStep> ::= up <KeyStepList>
        private object RULE_UPSTEP_UP(Token token)
        {
            return null;
        }
        
        // RULE_TAPSTEP_TAP : <TapStep> ::= tap <KeyStepList>
        private object RULE_TAPSTEP_TAP(Token token)
        {
            return null;
        }
        
        // RULE_DELAY_DELAY_INTEGER : <Delay> ::= delay Integer <TimeUnit>
        private object RULE_DELAY_DELAY_INTEGER(Token token)
        {
            return null;
        }
        
        // RULE_KEYSTEPLIST : <KeyStepList> ::= <KeyStep>
        private object RULE_KEYSTEPLIST(Token token)
        {
            return null;
        }
        
        // RULE_KEYSTEPLIST_COMMA : <KeyStepList> ::= <KeyStep> ',' <KeyStepList>
        private object RULE_KEYSTEPLIST_COMMA(Token token)
        {
            return null;
        }
        
        // RULE_KEYSTEP_IDENTIFIER : <KeyStep> ::= Identifier
        private object RULE_KEYSTEP_IDENTIFIER(Token token)
        {
            return null;
        }
        
        // RULE_KEYSTEP_COOL_INTEGER : <KeyStep> ::= cool Integer <TimeUnit>
        private object RULE_KEYSTEP_COOL_INTEGER(Token token)
        {
            return null;
        }
        
        // RULE_FUNCTIONCALL_IDENTIFIER_LPARAN_RPARAN : <FunctionCall> ::= Identifier '(' <FunctionArgList> ')'
        private object RULE_FUNCTIONCALL_IDENTIFIER_LPARAN_RPARAN(Token token)
        {
            return null;
        }
        
        // RULE_FUNCTIONCALL_IDENTIFIER_LPARAN_RPARAN2 : <FunctionCall> ::= Identifier '(' ')'
        private object RULE_FUNCTIONCALL_IDENTIFIER_LPARAN_RPARAN2(Token token)
        {
            return null;
        }
        
        // RULE_FUNCTIONARGLIST : <FunctionArgList> ::= <FunctionArg>
        private object RULE_FUNCTIONARGLIST(Token token)
        {
            return null;
        }
        
        // RULE_FUNCTIONARGLIST_COMMA : <FunctionArgList> ::= <FunctionArg> ',' <FunctionArgList>
        private object RULE_FUNCTIONARGLIST_COMMA(Token token)
        {
            return null;
        }
        
        // RULE_FUNCTIONARG : <FunctionArg> ::= <FunctionCall>
        private object RULE_FUNCTIONARG(Token token)
        {
            return null;
        }
        
        // RULE_FUNCTIONARG_INTEGER : <FunctionArg> ::= Integer
        private object RULE_FUNCTIONARG_INTEGER(Token token)
        {
            return null;
        }
        
        // RULE_FUNCTIONARG_DECIMAL : <FunctionArg> ::= Decimal
        private object RULE_FUNCTIONARG_DECIMAL(Token token)
        {
            return null;
        }
        
        // RULE_FUNCTIONARG_STRING : <FunctionArg> ::= String
        private object RULE_FUNCTIONARG_STRING(Token token)
        {
            return null;
        }
        
        // RULE_FUNCTIONARG_TRUE : <FunctionArg> ::= true
        private object RULE_FUNCTIONARG_TRUE(Token token)
        {
            return null;
        }
        
        // RULE_FUNCTIONARG_FALSE : <FunctionArg> ::= false
        private object RULE_FUNCTIONARG_FALSE(Token token)
        {
            return null;
        }
        
        // RULE_FUNCTIONARG_NULL : <FunctionArg> ::= null
        private object RULE_FUNCTIONARG_NULL(Token token)
        {
            return null;
        }
        
        // RULE_FUNCTIONARG_IDENTIFIER : <FunctionArg> ::= Identifier
        private object RULE_FUNCTIONARG_IDENTIFIER(Token token)
        {
            return null;
        }
        

        #endregion
    }
}
