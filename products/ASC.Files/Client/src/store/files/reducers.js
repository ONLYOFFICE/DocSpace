import {
  SET_ACTION,
  SET_FILE,
  SET_FILES_FILTER,
  SET_FILES,
  SET_FILTER,
  SET_VIEW_AS,
  SET_FOLDER,
  SET_FOLDERS,
  SET_TREE_FOLDERS,
  SET_SELECTED_FOLDER,
  SET_SELECTED,
  SET_SELECTION,
  SELECT_FILE,
  DESELECT_FILE,
  SET_DRAGGING,
  SET_DRAG_ITEM,
  SET_MEDIA_VIEWER_VISIBLE,
  SET_PROGRESS_BAR_DATA,
  SET_CONVERT_DIALOG_VISIBLE,
  SET_NEW_TREE_FILES,
  SET_NEW_ROW_ITEMS
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
  viewAs: "row",
  selectedFolder: null,
  selection: [],
  dragging: false,
  dragItem: null,
  mediaViewerData: { visible: false, id: null },
  progressData: { percent: 0, label: "", visible: false },
  convertDialogVisible: false,
  updateTreeNew: false,
  newRowItems: []
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
    case SET_VIEW_AS:
      return Object.assign({}, state, {
        viewAs: action.viewAs
      });
    case SET_FILES_FILTER:
      return Object.assign({}, state, {
        filter: action.filter
      });
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
    case SET_DRAGGING: 
      return Object.assign({}, state, {
        dragging: action.dragging
      });
    case SET_DRAG_ITEM: 
      return Object.assign({}, state, {
        dragItem: action.dragItem
      });
    case SET_MEDIA_VIEWER_VISIBLE: 
      return Object.assign({}, state, {
        mediaViewerData: action.mediaViewerData
      });
    case SET_PROGRESS_BAR_DATA: 
      return Object.assign({}, state, {
        progressData: action.progressData
      });
    case SET_CONVERT_DIALOG_VISIBLE: 
      return Object.assign({}, state, {
        convertDialogVisible: action.convertDialogVisible
      });
    case SET_NEW_TREE_FILES: 
      return Object.assign({}, state, {
        updateTreeNew: action.updateTreeNew
      });
    case SET_NEW_ROW_ITEMS: 
      return Object.assign({}, state, {
        newRowItems: action.newRowItems
      });
    default:
      return state;
  }
};

export default filesReducer;
