set loadpath '../palette/'
load 'set1.pal'
set encoding utf8

set terminal pdfcairo font "Helvetica,10.5" color size 2,1.5 enhanced
set output "./figure4-sched-heater.pdf"

bm = 0.10
tm = 0.9
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

set key autotitle columnhead right Left outside reverse offset 2,0 horizontal spacing 1
set key maxrows 1

set ylabel "Instrument Utilization (%)"
set format y "%.0f"
set yrange [0:100]
set ytics 20
set ytics out

set style data histograms
set style fill solid 0.5 border rgb "black"
set boxwidth 1 relative

# 设置x轴刻度
set xrange [-0.5:1.5]

plot './figure4-sched.csv' using 3:xtic(1) with histogram ls 3 lw 0.5,\
    "" using 0:3:($3) notitle with labels offset -2,0.7,\
    '' using 4:xtic(1) with histogram ls 2 lw 0.5,\
    "" using 0:4:($4) notitle with labels offset 2,0.7,\

