set loadpath '../palette/'
load 'set1.pal'
set encoding utf8

set terminal pdfcairo font "Helvetica,10.5" color size 2,1.5 enhanced
set output "./figure4-sched-time.pdf"

bm = 0.10
tm = 0.94
lm = 0.22
rm = 0.94 

set lmargin at screen lm
set rmargin at screen rm
set bmargin at screen bm
set tmargin at screen tm

set border linewidth 0.5
set border 1+2
set xtics nomirror
set ytics nomirror

unset key

set ylabel "Time (min)" offset 0,-0.5
set format y "%.0f"
set yrange [0:500]
set ytics 100
set ytics out

set style data histograms
set style fill solid 0.5 border rgb "black"
set boxwidth 1 relative

# 设置x轴刻度
set xrange [0.5:2.5]

plot './figure4-sched.csv' using 2:xtic(1) with histogram ls 1 lw 0.5,\
    "" using 0:2:($2) notitle with labels offset 0,0.7,\
