set terminal png size 1200, 800 crop
set output "perceptron02.png"
set title "Learning rate = 0,2"
set xrange [-3:7]
set yrange [-7:7]
set key left
set pointsize 2
set grid
plot -(-0.4* x + -5.525)/-0.8 title "iteracja t = 1",-(-1.2* x + -16.575)/-2.4 title "iteracja t = 3",-(-1.99* x + -27.625)/-4 title "iteracja t = 5",-(-2.99* x + -38.675)/-5.7 title "iteracja t = 7",-(-3.64* x + -49.725)/-7.25 title "iteracja t = 9",-(-4.44* x + -60.775)/-8.85 title "iteracja t = 11",-(-1.69* x + -49.725)/-7.75 title "iteracja t = 13",-(1.01* x + -38.675)/-6.6 title "iteracja t = 15",-(3.76* x + -27.625)/-5.4 title "iteracja t = 17",-(6.71* x + -16.575)/-4.2 title "iteracja t = 19";

set terminal png
set output "perceptron02.png"
replot "class1.dat" title "class 1" with points


set terminal png
set output "perceptron02.png"
replot "class2.dat" title "class 2" with points
