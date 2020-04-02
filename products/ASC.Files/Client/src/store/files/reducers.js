import {
  SET_ACTION,
  SET_FILE,
  SET_FILES_FILTER,
  SET_FILES,
  SET_FILTER,
  SET_FOLDER,
  SET_FOLDERS,
  SET_TREE_FOLDERS,
  SET_SELECTED_FOLDER,
  SET_SELECTED,
  SET_SELECTION,
  SELECT_FILE,
  DESELECT_FILE,
  SET_SHARE_DATA
} from "./actions";
import { api } from "asc-web-common";
import { isFileSelected, skipFile, getFilesBySelected } from "./selectors";
const { FilesFilter } = api;

const initialState = {
  fileAction: {
    type: null
  },
  files: null,
  filter: FilesFilter.getDefault(),
  folders: null,
  treeFolders: [],
  selected: "none",
  selectedFolder: null,
  selection: [],
  shareData: []
};

const filesReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_FOLDER:
      return Object.assign({}, state, {
        folders: state.folders.map(folder =>
          folder.id === action.folder.id ? action.folder : folder
        )
      });
    case SET_FOLDERS:
      return Object.assign({}, state, {
        folders: action.folders
      });
    case SET_FILES:
      return Object.assign({}, state, {
        files: action.files
      });
    case SET_FILE:
      return Object.assign({}, state, {
        files: state.files.map(file =>
          file.id === action.file.id ? action.file : file
        )
      });
    case SET_SELECTION:
      return Object.assign({}, state, {
        selection: action.selection
      });
    case SET_SELECTED:
      return Object.assign({}, state, {
        selected: action.selected,
        selection: getFilesBySelected(state.files.concat(state.folders), action.selected)
      });
    case SET_SELECTED_FOLDER:
      return Object.assign({}, state, {
        selectedFolder: action.selectedFolder
      });
    case SET_TREE_FOLDERS:
      return Object.assign({}, state, {
        treeFolders: action.treeFolders
      });
    case SET_FILTER:
      return Object.assign({}, state, {
        filter: action.filter
      });
    case SET_FILES_FILTER:
      return Object.assign({}, state, {
        filter: action.filter
      });
    case SET_SHARE_DATA:
      return Object.assign({}, state, {
        shareData: action.shareData
      })
    case SELECT_FILE:
      if (!isFileSelected(state.selection, action.file.id, action.file.parentId)) {
        return Object.assign({}, state, {
          selection: [...state.selection, action.file]
        });
      } else return state;
    case DESELECT_FILE:
      if (isFileSelected(state.selection, action.file.id, action.file.parentId)) {
        return Object.assign({}, state, {
          selection: skipFile(state.selection, action.file.id)
        });
      } else return state;
    case SET_ACTION:
      return Object.assign({}, state, {
        fileAction: action.fileAction
      })
    default:
      return state;
  }
};

export default filesReducer;
