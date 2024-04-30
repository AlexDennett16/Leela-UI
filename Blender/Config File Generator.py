import bpy
import bmesh
#Loop through selected items
#Check the name of the items that are selected


#Make functions for each type of boundary that can be generated, converting them into text and printing out

selection_items = []
try:
    selection_items = [obj for obj in bpy.context.selected_objects]
    for i in selection_items:
        print("Name: ", i.name)
    print("")
except Exception as error:
    print("An exception occurred:", error)
    
    
print("Format: X,Y,Z \n")  
try:
    for item in selection_items:
        if "boundary plane" in item.name:
            data = item.data
            bm = bmesh.from_edit_mesh(data)
            for f in bm.faces:
                if f.select:
                    index = 0
                    text = "Boundary plane_{"
                    for v in reversed(f.verts):
                        
                        if v.index != 1:#Skip unused point
                            index+=1
                            text += (" p" + str(index) + " " + str(int(v.co[0])) + " " + str(int(v.co[1])) + " " + str(int(v.co[2])) + " ")
                    text += "vel 0 0 0 }_plane"
                    print(text)
        elif "fluid block" in item.name:
            data = item.data
            bm = bmesh.from_edit_mesh(data)
            include = [0,3,6,2]
            text = "fluid block_{"
            for i in range(len(include)):
                index = 0
                for v in bm.verts:
                    #print(index , str(v.co))
                    index+=1
                    if v.index == include[i]:
                        text += (" p" + str(i+1) + " " + str(int(v.co[0])) + " " + str(int(v.co[1])) + " " + str(int(v.co[2])) + " ")
            text += "vel 0 0 0 vel_o 0 0 0 rho 1000 rho_o 1000 mu 0.001 gamma 0.07 }_block"
            print(text)
        elif "fluid plane" in item.name:
            data = item.data
            bm = bmesh.from_edit_mesh(data)
            for f in bm.faces:
                if f.select:
                    index = 0
                    text = "Fluid plane_{"
                    for v in reversed(f.verts):
                        
                        if v.index != 1:#Skip unused point, needs checking
                            index+=1
                            text += (" p" + str(index) + " " + str(int(v.co[0])) + " " + str(int(v.co[1])) + " " + str(int(v.co[2])) + " ")
                    text += "vel 0 0 0 }_plane"
                    print(text)
        elif "boundary line" in item.name:
            data = item.data
            bm = bmesh.from_edit_mesh(data)
            index = 0
            text = "boundary line_{"
            for v in bm.verts:
                index+=1
                text += (" p" + str(index) + " " + str(int(v.co[0])) + " " + str(int(v.co[1])) + " " + str(int(v.co[2])) + " ")
            text += "vel 0 0 0 }_line"
            print(text)
                

except Exception as error:
    print("An exception occurred:", error)

