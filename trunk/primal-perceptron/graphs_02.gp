set terminal png
set output "perceptron02.png"
set title "Learning rate = 0,2"
set xrange [-3:0]
set yrange [-7:-5]
plot -(-0.4* x + -5.4)/-0.8 title "iteracja t = 1",-(-1.2* x + -16.2)/-2.4 title "iteracja t = 3",-(-2.05* x + -27)/-3.9 title "iteracja t = 5",-(-3.05* x + -37.8)/-5.6 title "iteracja t = 7",-(-3.8* x + -48.6)/-7.15 title "iteracja t = 9",-(-4.6* x + -59.4)/-9.05 title "iteracja t = 11",-(-1.8* x + -48.6)/-7.95 title "iteracja t = 13",-(0.75* x + -37.8)/-6.75 title "iteracja t = 15",-(3.55* x + -27)/-5.55 title "iteracja t = 17",-(6.6* x + -16.2)/-4.35 title "iteracja t = 19";