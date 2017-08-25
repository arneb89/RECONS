import numpy as np
import matplotlib.pyplot as plt

filename = "grid_5700_4700_M02.dat"

file = open(filename, 'rt')
line=file.readline()
line=file.readline()
str_mas = line.split()
n_lamb = int(str_mas[2])
line=file.readline()
line=file.readline()
str_mas = line.split()
n_mu = int(str_mas[0])
mus=[]
for i in range(0, n_mu):
    mus.append(float(str_mas[i+1]))

intes_phot_line=[]
intes_phot_cont=[]
intes_spot_line=[]
intes_spot_cont=[]

data = np.loadtxt(filename, skiprows=4)

lambdas=data[0:n_lamb,0]

for i in range(0, n_mu):
    intes_phot_cont.append(data[0:n_lamb, i+1])
    intes_phot_line.append(data[0:n_lamb, i+1+n_mu])
    intes_spot_cont.append(data[n_lamb:2*n_lamb, i+1])
    intes_spot_line.append(data[n_lamb:2*n_lamb, i+1+n_mu])
    
for i in range(0, n_mu):
    file = open("phot_mu_"+str(i)+".dat", "wt")
    for j in range(0, n_lamb):
        file.write(str(lambdas[j])+"\t"+str(intes_phot_line[i][j]/intes_phot_cont[i][j])+"\n")
    file.close()
    file = open("spot_mu_"+str(i)+".dat", "wt")
    for j in range(0, n_lamb):
        file.write(str(lambdas[j])+"\t"+str(intes_spot_line[i][j]/intes_spot_cont[i][j])+"\n")
    file.close()
    

aver_phot_cont = np.zeros(n_mu)
aver_spot_cont = np.zeros(n_mu)
for i in range(0, n_mu):
    aver_phot_cont[i]=np.mean(intes_phot_cont[i])
    aver_spot_cont[i]=np.mean(intes_spot_cont[i])
    

c_phot = np.polyfit(mus, aver_phot_cont, 1)
c_spot = np.polyfit(mus, aver_spot_cont, 1)
xP=1/((c_phot[1]/c_phot[0])+1)
xS=1/((c_spot[1]/c_spot[0])+1)
I0P=c_phot[0]/xP
I0S=c_spot[0]/xS
xx=[0,1]
yyp=[c_phot[1], c_phot[1]+c_phot[0]]
yys=[c_spot[1], c_spot[1]+c_spot[0]]
print('Mu = ', mus)
print('Phot: I0 = ', I0P, 'x = ', xP)
print('Spot: I0 = ', I0S, 'x = ', xS)
print('I0S/I0P = ', I0S/I0P)

plt.plot(mus, aver_phot_cont, '.')
plt.plot(mus, aver_spot_cont, '.')
plt.plot(xx, yyp, '-')
plt.plot(xx, yys, '-')

plt.show()
