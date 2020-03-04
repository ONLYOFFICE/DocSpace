import { SET_FILES, SET_FOLDERS, SET_SELECTION, SET_SELECTED, SET_SELECTED_FOLDER, SET_ROOT_FOLDERS, SET_FILES_FILTER, SET_FILTER } from "./actions";
import { api } from "asc-web-common";
const { FilesFilter } = api;

const initialState = {
  files: null,
  folders: null,
  selection: [],
  selected: "none",
  selectedFolder: null,
  rootFolders: [],
  filter: FilesFilter.getDefault()
};

const filesReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_FOLDERS:
      return Object.assign({}, state, {
        folders: action.folders
      });
    case SET_FILES:
      return Object.assign({}, state, {
        files: action.files
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
    case SET_ROOT_FOLDERS:
      return Object.assign({}, state, {
        rootFolders: action.rootFolders
      });
    case SET_FILTER:
      return Object.assign({}, state, {
        filter: action.filter
      });
	case SET_FILES_FILTER:
      return Object.assign({}, state, {
        filter: action.filter
      });    default:
      return state;
  }
};

export default filesReducer;
