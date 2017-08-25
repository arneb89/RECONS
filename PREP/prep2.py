import numpy as np
import matplotlib.pyplot as plt

#########################################################
xS=0.71
xP=0.62
infiles_phot=["phot_mu_0_comp1.lsd", "phot_mu_1_comp1.lsd", "phot_mu_2_comp1.lsd"]
infiles_spot=["spot_mu_0_comp1.lsd", "spot_mu_1_comp1.lsd", "spot_mu_2_comp1.lsd"]
mus=[0.8873, 0.5, 0.1127]
inten_rat=0.44
tsp=4700
tph=5700
logg=4.5
outfile=open("VelGrid.dat", "wt")
#########################################################

profs_phot=[]
profs_spot=[]
for i in range(0, len(infiles_phot)):
    profs_phot.append(np.loadtxt(infiles_phot[i]))
    
for i in range(0, len(infiles_spot)):
    profs_spot.append(np.loadtxt(infiles_spot[i]))
    
nvel=len(profs_phot[0][:,0])
vels=profs_phot[0][:,0]
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
        outfile.write(str(profs_phot[j][i,1]*cont)+'\t')
    outfile.write("\n")

for i in range(0, nvel):
    outfile.write(str(vels[i])+'\t')
    for j in range(0, len(mus)):
        cont=inten_rat*(1.0-xS*(1.0-mus[j]))
        outfile.write(str(cont)+'\t')
    for j in range(0, len(mus)):
        cont=inten_rat*(1.0-xS*(1.0-mus[j]))
        outfile.write(str(profs_spot[j][i,1]*cont)+'\t') 
    outfile.write("\n")

outfile.close()
