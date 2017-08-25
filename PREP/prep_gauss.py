import numpy as np
import matplotlib.pyplot as plt

#########################################################
xS=0.71
xP=0.62
width = 2
amp=0.78
mus=[0.8873, 0.5, 0.1127]
inten_rat=0.44
tsp=4700
tph=5700
logg=4.5
outfile=open("VelGrid.dat", "wt")
#########################################################
def Gaussian(amp, width, x):
    return 1.0-amp*np.exp(-(x**2)/(2*width**2))

profs_phot=[]
profs_spot=[]
vels=np.linspace(-70, 70, 4000)
nvel=len(vels)
for i in range(0, len(mus)):
    profs_phot.append(np.zeros(nvel))
    profs_spot.append(np.zeros(nvel))
for i in range(0, len(mus)):
    for j in range(0, nvel):
        profs_phot[i][j]=Gaussian(amp, width, vels[j])
        profs_spot[i][j]=Gaussian(amp, width, vels[j])
    

vel0=vels[0]
dvel=vels[1]-vels[0]

outfile.write(str(logg)+"\t"+"1.0\t1.0\n")
outfile.write(str(vel0)+"\t"+str(dvel)+"\t"+str(nvel)+"\n")
outfile.write(str(2)+"\t"+str(tph)+"\t"+str(tsp)+"\n")
outfile.write(str(len(mus))+"\t")
for i in range(0, len(mus)):
    outfile.write(str(mus[i])+"\t")
outfile.write("\n")

for i in range(0, nvel):
    outfile.write(str(vels[i])+'\t')
    for j in range(0, len(mus)):
        cont=1.0-xP*(1.0-mus[j])
        outfile.write(str(cont)+'\t')    
    for j in range(0, len(mus)):
        cont=1.0-xP*(1.0-mus[j])
        outfile.write(str(profs_phot[j][i]*cont)+'\t')
    outfile.write("\n")

for i in range(0, nvel):
    outfile.write(str(vels[i])+'\t')
    for j in range(0, len(mus)):
        cont=inten_rat*(1.0-xS*(1.0-mus[j]))
        outfile.write(str(cont)+'\t')
    for j in range(0, len(mus)):
        cont=inten_rat*(1.0-xS*(1.0-mus[j]))
        outfile.write(str(profs_spot[j][1]*cont)+'\t') 
    outfile.write("\n")
    
plt.plot(vels, profs_spot[2])
plt.show()

outfile.close()