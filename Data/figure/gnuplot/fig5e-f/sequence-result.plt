set loadpath '../palette/'
load 'set1.pal'
set encoding utf8

set terminal pdfcairo font "Helvetica,10.5" color size 2.7,2.5 enhanced
set output "./sequence-result.pdf"

bm = 0.15
tm = 0.94
lm = 0.12
rm = 0.88 

set lmargin at screen lm
set rmargin at screen rm
set bmargin at screen bm
set tmargin at screen tm

set border linewidth 0.5
set border 1+2+8
set xtics nomirror
set ytics nomirror

unset key

set ylabel "Proportion of Error" offset 2,-0.5
set yrange [0.001:1]
# set ytics 1
set ytics out offset 0.8,0
set logscale y

set y2label "Yield (%)" offset -2.8,-0.5
set y2range [0:100]
set y2tics 20
set y2tics out offset -0.8,0

set xrange [0.5:6.5]

set style fill solid 0.5 border -1
set style boxplot nooutliers
set linetype 1 lw 0.5
set linetype 2 lw 0.5
set linetype 3 lw 0.5

# set style boxplot outliers pointtype 7
set style data boxplot
set boxwidth  0.7
# set pointsize 0.5

set ytics ("10^{-3}" 0.001, "10^{-2}" 0.01, "10^{-1}" 0.1, "10^{0}" 1)
set xtics ("Deletion" 1, "Insertion" 2, "Substitution" 3, "Base-correct" 4, "Full-length" 5, "Stepwise" 6) rotate by 20 right offset 4,0

unset colorbox

set palette defined ( \
  0 '#3498db', 1 '#e74c3c', 2 '#2ecc71', \
  3 '#f39c12', 4 '#ECD089', 5 '#8DACCE', \
  6 '#9FDECB', 7 '#34495e' )
set cbrange [0:8] 

# set label "Error" at graph 0.20, graph 1 font "Helvetica Bold"
# set label "Yield" at graph 0.70, graph 1 font "Helvetica Bold"

set arrow from graph 0.5, graph 0 to graph 0.5, graph 1 nohead lc "black" lw 1 dashtype 2

# set label "6.63%" at 0.65,0.2769 front font ",9"
# set label "2.72%" at 1.65,0.0714 front font ",9"
# set label "2.48%" at 2.65,0.1243 front font ",9"


plot './sequence-ratio.csv' using (1):1:(0.8) with boxplot lt 1 lc 'gray' axis x1y1, \
     '' using (2):2:(0.8) with boxplot lt 2 lc 'gray' axis x1y1, \
     '' using (3):3:(0.8) with boxplot lt 3 lc 'gray' axis x1y1, \
     './sequence-yield.csv' using 0:2:(column(0)):xtic(1) with boxes lw 0.5 lc palette axis x1y2, \
     '' using ($0):2:3 with errorbars lw 0.5 lc "black" pt 0 axis x1y2
    #  , \
    #  '' using ($0):($2/2+50):(($0 > 3) ? sprintf("%.2f%", $2) : "") with labels font ",9" offset 0,1 axis x1y2

