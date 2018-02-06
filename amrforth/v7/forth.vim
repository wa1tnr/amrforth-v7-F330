" Vim syntax file
" Language   : FORTH
" Maintainer : Christian V. J. Brüssow <cvjb@epost.de>
" Last change: 2002-04-28

" $Id: forth.vim,v 1.4 2002/04/28 18:47:28 bobby Exp bobby $

" The list of keywords is incomplete, compared with the offical ANS
" wordlist. If you use this language, please improve it, and send me
" the patches.

" Many Thanks to...
"
" 2002-04-22:
" Charles Shattuck <charley@forth.org> helped me to settle up with the
" binary and hex number highlighting.
"
" 2002-04-20:
" Charles Shattuck <charley@forth.org> send me some code for correctly
" highlighting char and [char] followed by an opening paren. He also added
" some words for operators, conditionals, and definitions; and added the
" highlighting for s" and c".
" 
" 2000-03-28:
" John Providenza <john@probo.com> made improvements for the
" highlighting of strings, and added the code for highlighting hex numbers.
"
" 2004-12-15:
" Almost completely changed by Charles Shattuck <charley@amresearch.com>
" in order to emulate colorforth.  Colon and code definition labels are
" red, the rest of the definition is green.  Outside a definition,
" including between [ and ] is black (on a white background).

" For version 5.x: Clear all syntax items
" For version 6.x: Quit when a syntax file was already loaded
if version < 600
	syntax clear
elseif exists("b:current_syntax")
	finish
endif

" Synchronization method
" syn sync ccomment maxlines=500
syntax sync fromstart

" I use gforth, so I set this to case ignore
syn case ignore

" Characters allowed in keywords
" I don't know if 128-255 are allowed in ANS-FORTH
if version >= 600
	setlocal iskeyword=!,@,33-35,%,$,38-64,A-Z,91-96,a-z,123-126,128-255
else
	set iskeyword=!,@,33-35,%,$,38-64,A-Z,91-96,a-z,123-126,128-255
endif

syntax region forthOutsideComment start=/\<(\>/ end=/)/
syntax region forthOutsideComment start='\\S\s' end='%$'
syntax match forthOutsideComment '\\\s.*$'

syntax region forthComment start=/\<(\>/ end=/)/ contained
syntax match forthComment '\\\s.*$' contained

syntax region forthInterpreted start=/\<\[\>/ end=/\<\]\>/ contained

syntax match forthColon '\<\S*:\S*\>\s*[^ \t]\+\>' nextgroup=forthWord skipwhite

syntax region forthWord start=/\>/ end=/\<\S*;\S*\>/ contained contains=forthComment,forthInterpreted,forthString

syntax match forthCode '\<code\>\s*[^ \t]\+\>' nextgroup=forthCodeWord skipwhite
syntax match forthCode '\<label\>\s*[^ \t]\+\>' nextgroup=forthCodeWord skipwhite

syntax region forthCodeWord start=/\>/ end=/;/ contained contains=forthComment,forthInterpreted,forthString

" recognize 'char (' or '[char] (' correctly, so it doesn't
" highlight everything after the paren as a comment till a closing ')'
syntax match forthInterpreted '\<\[char\]\s\S*\s' contained
syntax match forthInterpreted '\<postpone\s\S*\s' contained

" Strings
syntax region forthString start=+"+ end=+"+ end=+$+ contained

syntax match forthVariable /\<\S*variable\S*\>/
syntax match forthVariable /\<\S*constant\S*\>/
syntax match forthVariable /\<\S*create\S*\>/

" Define the default highlighting.
" For version 5.7 and earlier: only when not done already
" For version 5.8 and later: only when an item doesn't have highlighting yet
if version >= 508 || !exists("did_forth_syn_inits")
	if version < 508
		let did_forth_syn_inits = 1
		command -nargs=+ HiLink hi link <args>
	else
		command -nargs=+ HiLink hi def link <args>
	endif

" The default methods for highlighting. Can be overriden later.
" Chosen more for default colors than for meaning.
	HiLink forthString Type
"	HiLink forthInterpreted Black
	HiLink forthWord Type
	HiLink forthColon Constant
	HiLink forthComment Comment
	HiLink forthOutsideComment Comment
	HiLink forthCode Constant
"	HiLink forthCodeWord Black
	HiLink forthVariable Special
	delcommand HiLink
endif

let b:current_syntax = "forth"

" vim:ts=4:sw=4:nocindent:smartindent:
