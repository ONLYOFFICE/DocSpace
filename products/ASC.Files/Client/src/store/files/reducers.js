import { SET_FILES, SET_FOLDERS, SET_SELECTION, SET_SELECTED, SET_SELECTED_FOLDER } from "./actions";
const initialState = {
  files: [],
  folders: [],
  selection: [],
  selected: "none",
  selectedFolder: null
};

const filesReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_FOLDERS:
      return Object.assign({}, state, {
        groups: action.folders
      });
    case SET_FILES:
      return Object.assign({}, state, {
        users: action.files
      });
    case SET_SELECTION:
      return Object.assign({}, state, {
        selection: action.selection
      }); 
    case SET_SELECTED:
      return Object.assign({}, state, {
        selected: action.selected
      });
    case SET_SELECTED_FOLDER:
      return Object.assign({}, state, {
        selectedFolder: action.selectedFolder
      });
    default:
      return state;
  }
};

export default filesReducer;
