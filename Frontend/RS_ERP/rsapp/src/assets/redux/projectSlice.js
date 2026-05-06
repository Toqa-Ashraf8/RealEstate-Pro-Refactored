import { createSlice } from "@reduxjs/toolkit";
import axios from 'axios'
import { variables } from "../variables";
import { toast } from 'react-toastify';
import { formatCurrency } from "../helpers";
import { 
    deleteProject, 
    fetchProjectsList, 
    fetchProjectUnits, 
    fetchUnitsDetails, 
    saveCompleteProject, 
    uploadProjectImage, 
    uploadUnitImage 
} from "../../services/projectService";
const initialState = {
    project: {
         ProjectCode: 0, ProjectName:"", ProjectType: "-1", 
         Location: "", TotalUnits: 0, ProjectStatus: "-1", ProjectImage: "" 
        },
    projectsList: [],
    selectedUnit: {},
    unitsList: [], 
    displayUnitsList: [],
    //set variables
    projectImageName: "", 
    unitImageName: "",
    calculatedUnitTotalPrice: "",
    //Modal state
    isUnitModalOpen: false,
    isDeleteUnitModalOpen: false,
    isDeleteProjectModalOpen: false,
    isSearchModalOpen: false,
    // status variables 
    unitFormMode: -1, 
    isDeleted:false, 
    saveErrorStatus: false,
    isUpdated:false,
    //set row indexs and row id 
    unitTableRowIndex:"",
    selectedProjectRowIndex: "",
    selectedProjectCode:"",
}

const projectSlice = createSlice({
    name: 'projects',
    initialState,
    reducers: {
        toggleUnitModal: (state, action) => {
            state.isUnitModalOpen = action.payload;
        },
        setProjectData: (state, action) => {
            state.project = { ...state.project, ...action.payload }
        },
        resetProjectForm: (state) => {
            state.project =initialState.project;;
            state.projectImageName = '';
            state.unitsList = [];
        },
        setUnitData: (state, action) => {
            state.selectedUnit = { ...state.selectedUnit, ...action.payload };
        },
        prepareUnitModal: (state, action) => {
            state.unitFormMode=-1
            state.unitImageName = "";
            state.selectedUnit = {};
            state.selectedUnit.serial = action.payload;
           
        },
       
        saveUnitToTable: (state, action) => {
            if (state.unitFormMode === -1) {
                state.unitsList = [...state.unitsList, state.selectedUnit];
            }
            else {
                state.unitsList[state.unitFormMode] = state.selectedUnit;
            }
            state.isUnitModalOpen = false;
        },
        showDeleteUnitModal: (state, action) => {
            state.unitTableRowIndex = action.payload;
            state.isDeleteUnitModalOpen = true;
        },
        deleteUnitFromList: (state, action) => {
            state.unitsList = 
            state.unitsList.filter((items, index) => index !== state.unitTableRowIndex);
            state.isDeleteUnitModalOpen = false;
        },
        hideDeleteUnitModal: (state) => {
            state.isDeleteUnitModalOpen = false;
        },
        calculateUnitTotalPrice: (state) => {
            if (state.selectedUnit.TotalArea && state.selectedUnit.MeterPrice) {
                state.calculatedUnitTotalPrice = 
                state.selectedUnit.TotalArea * state.selectedUnit.MeterPrice;
                state.selectedUnit.TotalPrice = state.calculatedUnitTotalPrice;
            } else {
                state.calculatedUnitTotalPrice = 0;
                state.selectedUnit.TotalPrice = 0;
            }
        },
        toggleDeleteProjectModal: (state, action) => {
            state.isDeleteProjectModalOpen= action.payload;
        },
        toggleSearchModal: (state, action) => {
            state.isSearchModalOpen = action.payload;
            state.selectedProjectRowIndex = "";
        },
         selectProjectFromSearch: (state, action) => {
            state.project = state.projectsList[action.payload]
            state.selectedProjectRowIndex = state.projectsList[action.payload].ProjectCode;

        }, 
        updateSelectedProjectCode:(state,action)=>{
            state.selectedProjectCode=state.projectsList[action.payload].ProjectCode;
        },
        SetRowIndexvalue:(state,action)=>{
            state.unitFormMode=action.payload;
            state.selectedUnit=state.unitsList[state.unitFormMode];
            state.unitImageName = state.unitsList[action.payload].unitImage;
        } 
       
    },
    extraReducers: (builder) => {
        builder
            .addCase(uploadProjectImage.fulfilled, (state, action) => {
             state.projectImageName = action.payload;  
            })

            .addCase(saveCompleteProject.fulfilled, (state, action) => {
                state.saveErrorStatus = action.payload.errorOccured;
                state.project.ProjectCode = action.payload.id;
            })

            .addCase(uploadUnitImage.fulfilled, (state, action) => {
                state.unitImageName = action.payload;
            })
            
            .addCase(deleteProject.fulfilled, (state, action) => {
                state.isDeleted = action.payload;
            })
         
            .addCase(fetchProjectsList.fulfilled, (state, action) => {
                state.projectsList = action.payload;
            })
        
            .addCase(fetchProjectUnits.fulfilled, (state, action) => {
                state.displayUnitsList = action.payload;
            }) 
            .addCase(fetchUnitsDetails.fulfilled, (state, action) => {
                state.unitsList = action.payload;
            })
            
    }
})
export const { 
    toggleUnitModal, 
    setProjectData, 
    resetProjectForm, 
    setUnitData, 
    prepareUnitModal,
    saveUnitToTable, 
    showDeleteUnitModal,
    deleteUnitFromList,
    hideDeleteUnitModal,
    calculateUnitTotalPrice,
    toggleDeleteProjectModal,
    toggleSearchModal, 
    selectProjectFromSearch,
    updateSelectedProjectCode,
    SetRowIndexvalue
} = projectSlice.actions;
const projReducer = projectSlice.reducer;
export default projReducer;