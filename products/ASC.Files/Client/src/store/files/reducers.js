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
  SET_PRIMARY_PROGRESS_BAR_DATA,
  SET_SECONDARY_PROGRESS_BAR_DATA,
  SET_CONVERT_DIALOG_VISIBLE,
  SET_SHARING_PANEL_VISIBLE,
  SET_UPDATE_TREE,
  SET_NEW_ROW_ITEMS,
  SET_SELECTED_NODE,
  SET_EXPAND_SETTINGS_TREE,
  SET_IS_LOADING,
  SET_THIRD_PARTY,
  SET_FILES_SETTINGS,
  SET_FILES_SETTING,
  SET_IS_ERROR_SETTINGS,
  SET_FIRST_LOAD,
  SET_UPLOAD_DATA,
} from "./actions";
import { api } from "asc-web-common";
import { isFileSelected, skipFile, getFilesBySelected } from "./selectors";
const { FilesFilter } = api;

const initialState = {
  fileAction: {
    type: null,
  },
  files: null,
  filter: FilesFilter.getDefault(),
  folders: null,
  treeFolders: [],
  selected: "close",
  viewAs: "row",
  selectedFolder: {},
  selection: [],
  dragging: false,
  dragItem: null,
  mediaViewerData: { visible: false, id: null },
  primaryProgressData: {
    percent: 0,
    label: "",
    visible: false,
    icon: "upload",
    alert: false,
  },
  secondaryProgressData: {
    percent: 0,
    label: "",
    visible: false,
    icon: "trash",
    alert: false,
  },
  convertDialogVisible: false,
  sharingPanelVisible: false,
  updateTree: false,
  newRowItems: [],
  selectedTreeNode: [],
  isLoading: false,
  firstLoad: true,
  settingsTree: {},
  uploadData: {
    files: [],
    filesSize: 0,
    convertFiles: [],
    convertFilesSize: 0,
    uploadStatus: null,
    uploadToFolder: null,
    uploadedFiles: 0,
    percent: 0,
    uploaded: true,
  },
  docservice: {
    coauthorDocs: [".pptx", ".ppsx", ".xlsx", ".csv", ".docx", ".txt"],
    commentedDocs: [".docx", ".xlsx", ".pptx"],
    convertDocs: [
      ".pptm",
      ".ppt",
      ".ppsm",
      ".pps",
      ".potx",
      ".potm",
      ".pot",
      ".odp",
      ".fodp",
      ".otp",
      ".xlsm",
      ".xls",
      ".xltx",
      ".xltm",
      ".xlt",
      ".ods",
      ".fods",
      ".ots",
      ".docm",
      ".doc",
      ".dotx",
      ".dotm",
      ".dot",
      ".odt",
      ".fodt",
      ".ott",
      ".rtf",
    ],
    editedDocs: [
      ".pptx",
      ".pptm",
      ".ppt",
      ".ppsx",
      ".ppsm",
      ".pps",
      ".potx",
      ".potm",
      ".pot",
      ".odp",
      ".fodp",
      ".otp",
      ".xlsx",
      ".xlsm",
      ".xls",
      ".xltx",
      ".xltm",
      ".xlt",
      ".ods",
      ".fods",
      ".ots",
      ".csv",
      ".docx",
      ".docm",
      ".doc",
      ".dotx",
      ".dotm",
      ".dot",
      ".odt",
      ".fodt",
      ".ott",
      ".txt",
      ".rtf",
      ".mht",
      ".html",
      ".htm",
    ],
    encryptedDocs: [".docx", ".xlsx", ".pptx"],
    formfillingDocs: [".docx"],
    customfilterDocs: [".xlsx"],
    reviewedDocs: [".docx"],
    viewedDocs: [
      ".pptx",
      ".pptm",
      ".ppt",
      ".ppsx",
      ".ppsm",
      ".pps",
      ".potx",
      ".potm",
      ".pot",
      ".odp",
      ".fodp",
      ".otp",
      ".gslides",
      ".xlsx",
      ".xlsm",
      ".xls",
      ".xltx",
      ".xltm",
      ".xlt",
      ".ods",
      ".fods",
      ".ots",
      ".gsheet",
      ".csv",
      ".docx",
      ".docm",
      ".doc",
      ".dotx",
      ".dotm",
      ".dot",
      ".odt",
      ".fodt",
      ".ott",
      ".gdoc",
      ".txt",
      ".rtf",
      ".mht",
      ".html",
      ".htm",
      ".epub",
      ".pdf",
      ".djvu",
      ".xps",
    ],
  },
  mediaViewerFormats: {
    images: [
      ".bmp",
      ".gif",
      ".jpeg",
      ".jpg",
      ".png",
      ".ico",
      ".tif",
      ".tiff",
      ".webp",
    ],
    media: [
      ".aac",
      ".flac",
      ".m4a",
      ".mp3",
      ".oga",
      ".ogg",
      ".wav",
      ".f4v",
      ".m4v",
      ".mov",
      ".mp4",
      ".ogv",
      ".webm",
      ".avi",
      ".mpg",
      ".mpeg",
      ".wmv",
    ],
  },
  formats: {
    archive: [
      ".zip",
      ".rar",
      ".ace",
      ".arc",
      ".arj",
      ".bh",
      ".cab",
      ".enc",
      ".gz",
      ".ha",
      ".jar",
      ".lha",
      ".lzh",
      ".pak",
      ".pk3",
      ".tar",
      ".tgz",
      ".gz",
      ".uu",
      ".uue",
      ".xxe",
      ".z",
      ".zoo",
    ],
    image: [
      ".bmp",
      ".cod",
      ".gif",
      ".ief",
      ".jpe",
      ".jpg",
      ".tif",
      ".cmx",
      ".ico",
      ".pnm",
      ".pbm",
      ".ppm",
      ".psd",
      ".rgb",
      ".xbm",
      ".xpm",
      ".xwd",
      ".png",
      ".ai",
      ".jpeg",
    ],
    sound: [
      ".aac",
      ".ac3",
      ".aiff",
      ".amr",
      ".ape",
      ".cda",
      ".flac",
      ".m4a",
      ".mid",
      ".mka",
      ".mp3",
      ".mpc",
      ".oga",
      ".ogg",
      ".pcm",
      ".ra",
      ".raw",
      ".wav",
      ".wma",
    ],
    video: [
      ".3gp",
      ".asf",
      ".avi",
      ".f4v",
      ".fla",
      ".flv",
      ".m2ts",
      ".m4v",
      ".mkv",
      ".mov",
      ".mp4",
      ".mpeg",
      ".mpg",
      ".mts",
      ".ogv",
      ".svi",
      ".vob",
      ".webm",
      ".wmv",
    ],
    html: [".htm", ".mht", ".html"],
    ebook: [".fb2", ".ibk", ".prc", ".epub"],
    document: [
      ".doc",
      ".docx",
      ".docm",
      ".dot",
      ".dotx",
      ".dotm",
      ".odt",
      ".fodt",
      ".ott",
      ".rtf",
      ".txt",
      ".html",
      ".htm",
      ".mht",
      ".pdf",
      ".djvu",
      ".fb2",
      ".epub",
      ".xps",
      ".doct",
      ".docy",
      ".gdoc",
    ],
    presentation: [
      ".pps",
      ".ppsx",
      ".ppsm",
      ".ppt",
      ".pptx",
      ".pptm",
      ".pot",
      ".potx",
      ".potm",
      ".odp",
      ".fodp",
      ".otp",
      ".pptt",
      ".ppty",
      ".gslides",
    ],
    spreadsheet: [
      ".xls",
      ".xlsx",
      ".xlsm",
      ".xlt",
      ".xltx",
      ".xltm",
      ".ods",
      ".fods",
      ".ots",
      ".csv",
      ".xlst",
      ".xlsy",
      ".xlsb",
      ".gsheet",
    ],
  },
  privacyInstructions: "https://www.onlyoffice.com/private-rooms.aspx",
};

const filesReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_FOLDER:
      return Object.assign({}, state, {
        folders: state.folders.map((folder) =>
          folder.id === action.folder.id ? action.folder : folder
        ),
      });
    case SET_FOLDERS:
      return Object.assign({}, state, {
        folders: action.folders,
      });
    case SET_FILES:
      return Object.assign({}, state, {
        files: action.files,
      });
    case SET_FILE:
      return Object.assign({}, state, {
        files: state.files.map((file) =>
          file.id === action.file.id ? action.file : file
        ),
      });
    case SET_SELECTION:
      return Object.assign({}, state, {
        selection: action.selection,
      });
    case SET_SELECTED:
      return Object.assign({}, state, {
        selected: action.selected,
        selection: getFilesBySelected(
          state.files.concat(state.folders),
          action.selected
        ),
      });
    case SET_SELECTED_FOLDER:
      return Object.assign({}, state, {
        selectedFolder: action.selectedFolder,
      });
    case SET_TREE_FOLDERS:
      return Object.assign({}, state, {
        treeFolders: action.treeFolders,
      });
    case SET_FILTER:
      return Object.assign({}, state, {
        filter: action.filter,
      });
    case SET_VIEW_AS:
      return Object.assign({}, state, {
        viewAs: action.viewAs,
      });
    case SET_FILES_FILTER:
      return Object.assign({}, state, {
        filter: action.filter,
      });
    case SELECT_FILE:
      if (
        !isFileSelected(state.selection, action.file.id, action.file.parentId)
      ) {
        return Object.assign({}, state, {
          selection: [...state.selection, action.file],
        });
      } else return state;
    case DESELECT_FILE:
      if (
        isFileSelected(state.selection, action.file.id, action.file.parentId)
      ) {
        return Object.assign({}, state, {
          selection: skipFile(state.selection, action.file.id),
        });
      } else return state;
    case SET_ACTION:
      return Object.assign({}, state, {
        fileAction: action.fileAction,
      });
    case SET_DRAGGING:
      return Object.assign({}, state, {
        dragging: action.dragging,
      });
    case SET_DRAG_ITEM:
      return Object.assign({}, state, {
        dragItem: action.dragItem,
      });
    case SET_MEDIA_VIEWER_VISIBLE:
      return Object.assign({}, state, {
        mediaViewerData: action.mediaViewerData,
      });
    case SET_PRIMARY_PROGRESS_BAR_DATA:
      return Object.assign({}, state, {
        primaryProgressData: action.primaryProgressData,
      });
    case SET_SECONDARY_PROGRESS_BAR_DATA:
      return Object.assign({}, state, {
        secondaryProgressData: action.secondaryProgressData,
      });
    case SET_CONVERT_DIALOG_VISIBLE:
      return Object.assign({}, state, {
        convertDialogVisible: action.convertDialogVisible,
      });
    case SET_SHARING_PANEL_VISIBLE:
      return Object.assign({}, state, {
        sharingPanelVisible: action.sharingPanelVisible,
      });
    case SET_UPDATE_TREE:
      return Object.assign({}, state, {
        updateTree: action.updateTree,
      });
    case SET_NEW_ROW_ITEMS:
      return Object.assign({}, state, {
        newRowItems: action.newRowItems,
      });
    case SET_SELECTED_NODE:
      if (action.node[0]) {
        return Object.assign({}, state, {
          selectedTreeNode: action.node,
        });
      } else {
        return state;
      }
    case SET_EXPAND_SETTINGS_TREE:
      return Object.assign({}, state, {
        settingsTree: {
          ...state.settingsTree,
          expandedSetting: action.setting,
        },
      });
    case SET_IS_LOADING:
      return Object.assign({}, state, {
        isLoading: action.isLoading,
      });
    case SET_THIRD_PARTY:
      return Object.assign({}, state, {
        settingsTree: { ...state.settingsTree, thirdParty: action.data },
      });
    case SET_FILES_SETTINGS:
      const {
        storeOriginalFiles,
        confirmDelete,
        updateIfExist,
        forcesave,
        storeForcesave,
        enableThirdParty,
      } = action.settings;
      return Object.assign({}, state, {
        settingsTree: {
          ...state.settingsTree,
          storeOriginalFiles,
          confirmDelete,
          updateIfExist,
          forceSave: forcesave,
          storeForceSave: storeForcesave,
          enableThirdParty,
        },
      });
    case SET_FILES_SETTING:
      const { setting, val } = action;
      return Object.assign({}, state, {
        settingsTree: {
          ...state.settingsTree,
          [setting]: val,
        },
      });
    case SET_IS_ERROR_SETTINGS:
      return Object.assign({}, state, {
        settingsTree: {
          ...state.settingsTree,
          isErrorSettings: action.isError,
        },
      });
    case SET_FIRST_LOAD:
      return Object.assign({}, state, {
        firstLoad: action.firstLoad,
      });
    case SET_UPLOAD_DATA:
      return Object.assign({}, state, {
        uploadData: action.uploadData,
      });
    default:
      return state;
  }
};

export default filesReducer;
