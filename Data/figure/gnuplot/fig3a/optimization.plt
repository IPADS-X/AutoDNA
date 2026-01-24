set loadpath '../palette/'
load 'set1.pal'
set encoding utf8

set terminal pdfcairo font "Arial,10.5" color size 1.8,1.5 enhanced
set output "./optimization-combined-speed.pdf"

bm = 0.14
tm = 0.94
lm = 0.12
rm = 0.88

set lmargin at screen lm
set rmargin at screen rm
set bmargin at screen bm
set tmargin at screen tm

# set key autotitle columnhead outside bottom maxrows 1 Right offset 0,-3.5 spacing 2 
unset key

set border linewidth 0.5
set border 1+2+8

# First y-axis (left) - Time
set ylabel "Stepwise yield (%)" offset 3,0
set format y "%.s"
set yrange [90:100]
set ytics 5
set ytics out

# Second y-axis (right) - Yield
set y2label "Normalized speed (n.u.)" offset -3.7,-0.5
set y2range [0.4:1.6]  # Auto scale or set your own range
set y2tics 0.2
set y2tics out

# Common x-axis settings
set xtics 1 out offset 0,-1
set xrange [1.5:9.5]

set xtics nomirror rotate by 90

set ytics nomirror rotate by 90
set y2tics nomirror rotate by 90 offset -1,-0.6
set y2tics ("0.4" 0.4, "0.8" 0.8,"1.2" 1.2, "1.6" 1.6)

set style data histogram
set style fill solid border lc "black"
set boxwidth 0.8

# Plot both datasets
plot "./optimization-yield.csv" u 1:($2) w boxes ls 1 lw 0.5 axis x1y1, \
     '' using ($0+2):(91.3):(sprintf("%.2f", $2)) with labels font ", 9" rotate by 90 axis x1y1,\
     '' using ($0+2):2:3 with errorbars lw 1 lc "black" pt 0, \
     "./optimization-speed.csv" u 1:($2) w lp ls 4 lw 1 pt 7 ps 0.5 axis x1y2     