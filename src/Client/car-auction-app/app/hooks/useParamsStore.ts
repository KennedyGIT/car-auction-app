/*
The first line import { create } from "zustand" imports the create function from the “zustand” library. 
This function is used to create a state management store.
*/

import { create } from "zustand"


/* 
   Next, we define two TypeScript types: State and Actions.

    State represents the shape of our application state. It includes properties like pageNumber, pageSize, pageCount, and searchTerm.
    Actions defines the available actions that can modify the state. In this case, we have two actions: setParams and reset.

*/

type State = {
    pageNumber : number
    pageSize : number
    pageCount : number
    searchTerm : string
    searchValue : string
    orderBy: string
    filterBy: string
}



type Actions = {
    setParams: (params: Partial<State>) => void 
    reset: () => void 
    setSearchValue: (value: string) => void
}

/*
  We create an initialState object with default values for our state properties (pageNumber, pageSize, etc.).
*/

const initialState: State = {
    pageNumber: 1,
    pageSize: 12,
    pageCount: 1,
    searchTerm: '',
    searchValue: '',
    orderBy: 'make',
    filterBy: 'live'
}


/*
 The useParamsStore is created by calling create with a function that returns the initial state and the actions. 
 The set function provided by Zustand is used to update the state.
*/

export const useParamsStore = create<State & Actions >()((set) => ({
    ...initialState,

    /*
       The setParams action updates the state based on the newParams provided. 
       If pageNumber is included in newParams, it updates just the pageNumber. 
       Otherwise, it spreads the newParams into the state and resets pageNumber to 1.
       The reset action simply resets the state to the initialState.
    */

    setParams : (newParams: Partial<State>) => {
        set((state) => {
            if(newParams.pageNumber){
                return{...state, pageNumber: newParams.pageNumber}
            }else{
                return {...state, ...newParams, pageNumber: 1}
            }
        })
    },

    reset: () => set(initialState),

    setSearchValue: (value: string) => {
        set({searchValue:value})
    }
}))