set loadpath '../palette/'
load 'set1.pal'
set encoding utf8

set terminal pdfcairo font "Helvetica,10.5" color size 3.5,2 enhanced
set output "./stepwise-yield.pdf"

bm = 0.20
tm = 0.86
lm = 0.1
rm = 0.96

set lmargin at screen lm
set rmargin at screen rm
set bmargin at screen bm
set tmargin at screen tm

set border 1+2
set xtics nomirror
set ytics nomirror

set key autotitle columnheader outside top Left reverse maxrows 1 offset 2,2.5 spacing 2 samplen 1.5

set ylabel "Proportion (%)" offset 1.7,-0.5
set yrange [0:100]
set ytics 20
set ytics out

set style data histograms
set style histogram rowstacked  # 堆叠柱状图
set style fill solid 0.5 border rgb "black"
set boxwidth 0.85 relative

# 设置x轴刻度
set xrange [-0.5:7.5]

unset colorbox

plot "./stepwise-yield.csv" using 2:xtic(1) ls 3, \
     "" using 0:(50):2 with labels font ",9" title "", \
     "" using 3 ls 1, \
     "" using 4 ls 2, \
     "" using 5 ls 4
