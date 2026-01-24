set loadpath '../palette/'
load 'set1.pal'
set encoding utf8

set terminal pdfcairo font "Helvetica,10.5" color size 1.5,1.5 enhanced
set output "./sequence-reads.pdf"

bm = 0.20
tm = 0.96
lm = 0.26
rm = 0.92

set lmargin at screen lm
set rmargin at screen rm
set bmargin at screen bm
set tmargin at screen tm

set border linewidth 0.5

set border 1+2
set xtics nomirror
set ytics nomirror

unset key

set ylabel "Bit recovery rate (%)" offset 1.7,-0.5
set yrange [0:100]
set ytics 20
set ytics out

set xlabel "Sequence depth" offset 0,0.5

set xrange [0:55]

plot "./sequence-reads.csv" u 1:($2) w lp ls 1 lw 1.5 pt 7 ps 0.5
